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
    [Cmdlet(VerbsCommon.Add, "XenVIF", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VIF))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class AddXenVIF : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VIF VIF { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VIF> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _ipv4AllowedIsSpecified
                       ^ _ipv6AllowedIsSpecified;
            }
        }

        [Parameter]
        public KeyValuePair<string, string> OtherConfig
        {
            get { return _otherConfig; }
            set
            {
                _otherConfig = value;
                _otherConfigIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _otherConfig;
        private bool _otherConfigIsSpecified;

        [Parameter]
        public KeyValuePair<string, string> QosAlgorithmParams
        {
            get { return _qosAlgorithmParams; }
            set
            {
                _qosAlgorithmParams = value;
                _qosAlgorithmParamsIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _qosAlgorithmParams;
        private bool _qosAlgorithmParamsIsSpecified;

        [Parameter]
        public string Ipv4Allowed
        {
            get { return _ipv4Allowed; }
            set
            {
                _ipv4Allowed = value;
                _ipv4AllowedIsSpecified = true;
            }
        }
        private string _ipv4Allowed;
        private bool _ipv4AllowedIsSpecified;

        [Parameter]
        public string Ipv6Allowed
        {
            get { return _ipv6Allowed; }
            set
            {
                _ipv6Allowed = value;
                _ipv6AllowedIsSpecified = true;
            }
        }
        private string _ipv6Allowed;
        private bool _ipv6AllowedIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vif = ParseVIF();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(vif);
            if (_qosAlgorithmParamsIsSpecified)
                ProcessRecordQosAlgorithmParams(vif);
            if (_ipv4AllowedIsSpecified)
                ProcessRecordIpv4Allowed(vif);
            if (_ipv6AllowedIsSpecified)
                ProcessRecordIpv6Allowed(vif);

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

                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVIF()
        {
            string vif = null;

            if (VIF != null)
                vif = (new XenRef<XenAPI.VIF>(VIF)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VIF.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vif = xenRef.opaque_ref;
            }
            else if (Ref != null)
                vif = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VIF', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VIF));
            }

            return vif;
        }

        private void ProcessRecordOtherConfig(string vif)
        {
            if (!ShouldProcess(vif, "VIF.add_to_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VIF.add_to_other_config(session, vif, OtherConfig.Key, OtherConfig.Value);

            });
        }

        private void ProcessRecordQosAlgorithmParams(string vif)
        {
            if (!ShouldProcess(vif, "VIF.add_to_qos_algorithm_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VIF.add_to_qos_algorithm_params(session, vif, QosAlgorithmParams.Key, QosAlgorithmParams.Value);

            });
        }

        private void ProcessRecordIpv4Allowed(string vif)
        {
            if (!ShouldProcess(vif, "VIF.add_ipv4_allowed"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_add_ipv4_allowed(session, vif, Ipv4Allowed);

                }
                else
                {
                    XenAPI.VIF.add_ipv4_allowed(session, vif, Ipv4Allowed);

                }

            });
        }

        private void ProcessRecordIpv6Allowed(string vif)
        {
            if (!ShouldProcess(vif, "VIF.add_ipv6_allowed"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_add_ipv6_allowed(session, vif, Ipv6Allowed);

                }
                else
                {
                    XenAPI.VIF.add_ipv6_allowed(session, vif, Ipv6Allowed);

                }

            });
        }

        #endregion
    }
}
