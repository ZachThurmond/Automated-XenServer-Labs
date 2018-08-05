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
    [Cmdlet(VerbsCommon.Set, "XenVBD", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VBD))]
    [OutputType(typeof(void))]
    public class SetXenVBD : XenServerCmdlet
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
        public string Userdevice
        {
            get { return _userdevice; }
            set
            {
                _userdevice = value;
                _userdeviceIsSpecified = true;
            }
        }
        private string _userdevice;
        private bool _userdeviceIsSpecified;

        [Parameter]
        public bool Bootable
        {
            get { return _bootable; }
            set
            {
                _bootable = value;
                _bootableIsSpecified = true;
            }
        }
        private bool _bootable;
        private bool _bootableIsSpecified;

        [Parameter]
        public vbd_mode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _modeIsSpecified = true;
            }
        }
        private vbd_mode _mode;
        private bool _modeIsSpecified;

        [Parameter]
        public vbd_type Type
        {
            get { return _type; }
            set
            {
                _type = value;
                _typeIsSpecified = true;
            }
        }
        private vbd_type _type;
        private bool _typeIsSpecified;

        [Parameter]
        public bool Unpluggable
        {
            get { return _unpluggable; }
            set
            {
                _unpluggable = value;
                _unpluggableIsSpecified = true;
            }
        }
        private bool _unpluggable;
        private bool _unpluggableIsSpecified;

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
        public string QosAlgorithmType
        {
            get { return _qosAlgorithmType; }
            set
            {
                _qosAlgorithmType = value;
                _qosAlgorithmTypeIsSpecified = true;
            }
        }
        private string _qosAlgorithmType;
        private bool _qosAlgorithmTypeIsSpecified;

        [Parameter]
        public Hashtable QosAlgorithmParams
        {
            get { return _qosAlgorithmParams; }
            set
            {
                _qosAlgorithmParams = value;
                _qosAlgorithmParamsIsSpecified = true;
            }
        }
        private Hashtable _qosAlgorithmParams;
        private bool _qosAlgorithmParamsIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vbd = ParseVBD();

            if (_userdeviceIsSpecified)
                ProcessRecordUserdevice(vbd);
            if (_bootableIsSpecified)
                ProcessRecordBootable(vbd);
            if (_modeIsSpecified)
                ProcessRecordMode(vbd);
            if (_typeIsSpecified)
                ProcessRecordType(vbd);
            if (_unpluggableIsSpecified)
                ProcessRecordUnpluggable(vbd);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(vbd);
            if (_qosAlgorithmTypeIsSpecified)
                ProcessRecordQosAlgorithmType(vbd);
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

        private void ProcessRecordUserdevice(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_userdevice"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_userdevice(session, vbd, Userdevice);

            });
        }

        private void ProcessRecordBootable(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_bootable"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_bootable(session, vbd, Bootable);

            });
        }

        private void ProcessRecordMode(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_mode"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_mode(session, vbd, Mode);

            });
        }

        private void ProcessRecordType(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_type"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_type(session, vbd, Type);

            });
        }

        private void ProcessRecordUnpluggable(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_unpluggable"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_unpluggable(session, vbd, Unpluggable);

            });
        }

        private void ProcessRecordOtherConfig(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_other_config(session, vbd, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordQosAlgorithmType(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_qos_algorithm_type"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_qos_algorithm_type(session, vbd, QosAlgorithmType);

            });
        }

        private void ProcessRecordQosAlgorithmParams(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.set_qos_algorithm_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VBD.set_qos_algorithm_params(session, vbd, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(QosAlgorithmParams));

            });
        }

        #endregion
    }
}
