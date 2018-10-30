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
    [Cmdlet(VerbsCommon.Set, "XenBond", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Bond))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenBond : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Bond Bond { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Bond> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _modeIsSpecified
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
        public bond_mode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _modeIsSpecified = true;
            }
        }
        private bond_mode _mode;
        private bool _modeIsSpecified;

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

            string bond = ParseBond();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(bond);
            if (_modeIsSpecified)
                ProcessRecordMode(bond);
            if (_propertyIsSpecified)
                ProcessRecordProperty(bond);

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

                        var obj = XenAPI.Bond.get_record(session, bond);
                        if (obj != null)
                            obj.opaque_ref = bond;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseBond()
        {
            string bond = null;

            if (Bond != null)
                bond = (new XenRef<XenAPI.Bond>(Bond)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Bond.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    bond = xenRef.opaque_ref;
            }
            else if (Ref != null)
                bond = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Bond', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Bond));
            }

            return bond;
        }

        private void ProcessRecordOtherConfig(string bond)
        {
            if (!ShouldProcess(bond, "Bond.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Bond.set_other_config(session, bond, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordMode(string bond)
        {
            if (!ShouldProcess(bond, "Bond.set_mode"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Bond.async_set_mode(session, bond, Mode);

                }
                else
                {
                    XenAPI.Bond.set_mode(session, bond, Mode);

                }

            });
        }

        private void ProcessRecordProperty(string bond)
        {
            if (!ShouldProcess(bond, "Bond.set_property"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Bond.async_set_property(session, bond, Property[0], Property[1]);

                }
                else
                {
                    XenAPI.Bond.set_property(session, bond, Property[0], Property[1]);

                }

            });
        }

        #endregion
    }
}
