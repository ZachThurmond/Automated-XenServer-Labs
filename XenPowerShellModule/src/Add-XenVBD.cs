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
    [Cmdlet(VerbsCommon.Add, "XenVBD", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VBD))]
    [OutputType(typeof(void))]
    public class AddXenVBD : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VBD VBD { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VBD> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


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

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vbd = ParseVBD();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(vbd);
            if (_qosAlgorithmParamsIsSpecified)
                ProcessRecordQosAlgorithmParams(vbd);

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

                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVBD()
        {
            string vbd = null;

            if (VBD != null)
                vbd = (new XenRef<XenAPI.VBD>(VBD)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VBD.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vbd = xenRef.opaque_ref;
            }
            else if (Ref != null)
                vbd = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VBD', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VBD));
            }

            return vbd;
        }

        private void ProcessRecordOtherConfig(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.add_to_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.add_to_other_config(session, vbd, OtherConfig.Key, OtherConfig.Value);

            });
        }

        private void ProcessRecordQosAlgorithmParams(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.add_to_qos_algorithm_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.add_to_qos_algorithm_params(session, vbd, QosAlgorithmParams.Key, QosAlgorithmParams.Value);

            });
        }

        #endregion
    }
}
