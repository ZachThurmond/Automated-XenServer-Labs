/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

using XenAPI;

namespace Citrix.XenServer.Commands
{
    [Cmdlet(VerbsCommon.Set, "XenPIF", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.PIF))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenPIF : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PIF PIF { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PIF> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _primaryAddressTypeIsSpecified
                       ^ _propertyIsSpecified;
            }
        }

        [Parameter]
        public Hashtable OtherConfig
        {
            get { return _otherConfig; }
            set
            {
                _otherConfig = value;
                _otherConfigIsSpecified = true;
            }
        }
        private Hashtable _otherConfig;
        private bool _otherConfigIsSpecified;

        [Parameter]
        public bool DisallowUnplug
        {
            get { return _disallowUnplug; }
            set
            {
                _disallowUnplug = value;
                _disallowUnplugIsSpecified = true;
            }
        }
        private bool _disallowUnplug;
        private bool _disallowUnplugIsSpecified;

        [Parameter]
        public primary_address_type PrimaryAddressType
        {
            get { return _primaryAddressType; }
            set
            {
                _primaryAddressType = value;
                _primaryAddressTypeIsSpecified = true;
            }
        }
        private primary_address_type _primaryAddressType;
        private bool _primaryAddressTypeIsSpecified;

        [Parameter]
        public string[] Property
        {
            get { return _property; }
            set
            {
                _property = value;
                _propertyIsSpecified = true;
            }
        }
        private string[] _property;
        private bool _propertyIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pif = ParsePIF();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(pif);
            if (_disallowUnplugIsSpecified)
                ProcessRecordDisallowUnplug(pif);
            if (_primaryAddressTypeIsSpecified)
                ProcessRecordPrimaryAddressType(pif);
            if (_propertyIsSpecified)
                ProcessRecordProperty(pif);

            if (!PassThru)
                return;

            RunApiCall(() =>
                {
                    var contxt = _context as XenServerCmdletDynamicParameters;

                    if (contxt != null && contxt.Async)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != null && taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                    else
                    {

                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePIF()
        {
            string pif = null;

            if (PIF != null)
                pif = (new XenRef<XenAPI.PIF>(PIF)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.PIF.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pif = xenRef.opaque_ref;
            }
            else if (Ref != null)
                pif = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PIF', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PIF));
            }

            return pif;
        }

        private void ProcessRecordOtherConfig(string pif)
        {
            if (!ShouldProcess(pif, "PIF.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.PIF.set_other_config(session, pif, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordDisallowUnplug(string pif)
        {
            if (!ShouldProcess(pif, "PIF.set_disallow_unplug"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.PIF.set_disallow_unplug(session, pif, DisallowUnplug);

            });
        }

        private void ProcessRecordPrimaryAddressType(string pif)
        {
            if (!ShouldProcess(pif, "PIF.set_primary_address_type"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_set_primary_address_type(session, pif, PrimaryAddressType);

                }
                else
                {
                    XenAPI.PIF.set_primary_address_type(session, pif, PrimaryAddressType);

                }

            });
        }

        private void ProcessRecordProperty(string pif)
        {
            if (!ShouldProcess(pif, "PIF.set_property"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_set_property(session, pif, Property[0], Property[1]);

                }
                else
                {
                    XenAPI.PIF.set_property(session, pif, Property[0], Property[1]);

                }

            });
        }

        #endregion
    }
}
