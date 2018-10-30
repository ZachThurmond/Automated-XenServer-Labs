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
    [Cmdlet(VerbsCommon.Get, "XenVBDProperty", SupportsShouldProcess = false)]
    public class GetXenVBDProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VBD VBD { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VBD> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVBDProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vbd = ParseVBD();

            switch (XenProperty)
            {
                case XenVBDProperty.Uuid:
                    ProcessRecordUuid(vbd);
                    break;
                case XenVBDProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(vbd);
                    break;
                case XenVBDProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(vbd);
                    break;
                case XenVBDProperty.VM:
                    ProcessRecordVM(vbd);
                    break;
                case XenVBDProperty.VDI:
                    ProcessRecordVDI(vbd);
                    break;
                case XenVBDProperty.Device:
                    ProcessRecordDevice(vbd);
                    break;
                case XenVBDProperty.Userdevice:
                    ProcessRecordUserdevice(vbd);
                    break;
                case XenVBDProperty.Bootable:
                    ProcessRecordBootable(vbd);
                    break;
                case XenVBDProperty.Mode:
                    ProcessRecordMode(vbd);
                    break;
                case XenVBDProperty.Type:
                    ProcessRecordType(vbd);
                    break;
                case XenVBDProperty.Unpluggable:
                    ProcessRecordUnpluggable(vbd);
                    break;
                case XenVBDProperty.StorageLock:
                    ProcessRecordStorageLock(vbd);
                    break;
                case XenVBDProperty.Empty:
                    ProcessRecordEmpty(vbd);
                    break;
                case XenVBDProperty.OtherConfig:
                    ProcessRecordOtherConfig(vbd);
                    break;
                case XenVBDProperty.CurrentlyAttached:
                    ProcessRecordCurrentlyAttached(vbd);
                    break;
                case XenVBDProperty.StatusCode:
                    ProcessRecordStatusCode(vbd);
                    break;
                case XenVBDProperty.StatusDetail:
                    ProcessRecordStatusDetail(vbd);
                    break;
                case XenVBDProperty.RuntimeProperties:
                    ProcessRecordRuntimeProperties(vbd);
                    break;
                case XenVBDProperty.QosAlgorithmType:
                    ProcessRecordQosAlgorithmType(vbd);
                    break;
                case XenVBDProperty.QosAlgorithmParams:
                    ProcessRecordQosAlgorithmParams(vbd);
                    break;
                case XenVBDProperty.QosSupportedAlgorithms:
                    ProcessRecordQosSupportedAlgorithms(vbd);
                    break;
                case XenVBDProperty.Metrics:
                    ProcessRecordMetrics(vbd);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVBD()
        {
            string vbd = null;

            if (VBD != null)
                vbd = (new XenRef<XenAPI.VBD>(VBD)).opaque_ref;
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

        private void ProcessRecordUuid(string vbd)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VBD.get_uuid(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string vbd)
        {
            RunApiCall(()=>
            {
                    List<vbd_operations> obj = XenAPI.VBD.get_allowed_operations(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string vbd)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VBD.get_current_operations(session, vbd);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVM(string vbd)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VBD.get_VM(session, vbd);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVDI(string vbd)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VBD.get_VDI(session, vbd);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDevice(string vbd)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VBD.get_device(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUserdevice(string vbd)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VBD.get_userdevice(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBootable(string vbd)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VBD.get_bootable(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMode(string vbd)
        {
            RunApiCall(()=>
            {
                    vbd_mode obj = XenAPI.VBD.get_mode(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string vbd)
        {
            RunApiCall(()=>
            {
                    vbd_type obj = XenAPI.VBD.get_type(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUnpluggable(string vbd)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VBD.get_unpluggable(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStorageLock(string vbd)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VBD.get_storage_lock(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordEmpty(string vbd)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VBD.get_empty(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vbd)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VBD.get_other_config(session, vbd);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordCurrentlyAttached(string vbd)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VBD.get_currently_attached(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStatusCode(string vbd)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VBD.get_status_code(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStatusDetail(string vbd)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VBD.get_status_detail(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRuntimeProperties(string vbd)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VBD.get_runtime_properties(session, vbd);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordQosAlgorithmType(string vbd)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VBD.get_qos_algorithm_type(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordQosAlgorithmParams(string vbd)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VBD.get_qos_algorithm_params(session, vbd);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordQosSupportedAlgorithms(string vbd)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VBD.get_qos_supported_algorithms(session, vbd);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetrics(string vbd)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VBD.get_metrics(session, vbd);

                        XenAPI.VBD_metrics obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VBD_metrics.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVBDProperty
    {
        Uuid,
        AllowedOperations,
        CurrentOperations,
        VM,
        VDI,
        Device,
        Userdevice,
        Bootable,
        Mode,
        Type,
        Unpluggable,
        StorageLock,
        Empty,
        OtherConfig,
        CurrentlyAttached,
        StatusCode,
        StatusDetail,
        RuntimeProperties,
        QosAlgorithmType,
        QosAlgorithmParams,
        QosSupportedAlgorithms,
        Metrics
    }

}
