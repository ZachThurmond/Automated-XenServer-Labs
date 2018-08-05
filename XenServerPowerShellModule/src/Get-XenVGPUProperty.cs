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
    [Cmdlet(VerbsCommon.Get, "XenVGPUProperty", SupportsShouldProcess = false)]
    public class GetXenVGPUProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VGPU VGPU { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VGPU> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVGPUProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vgpu = ParseVGPU();

            switch (XenProperty)
            {
                case XenVGPUProperty.Uuid:
                    ProcessRecordUuid(vgpu);
                    break;
                case XenVGPUProperty.VM:
                    ProcessRecordVM(vgpu);
                    break;
                case XenVGPUProperty.GPUGroup:
                    ProcessRecordGPUGroup(vgpu);
                    break;
                case XenVGPUProperty.Device:
                    ProcessRecordDevice(vgpu);
                    break;
                case XenVGPUProperty.CurrentlyAttached:
                    ProcessRecordCurrentlyAttached(vgpu);
                    break;
                case XenVGPUProperty.OtherConfig:
                    ProcessRecordOtherConfig(vgpu);
                    break;
                case XenVGPUProperty.Type:
                    ProcessRecordType(vgpu);
                    break;
                case XenVGPUProperty.ResidentOn:
                    ProcessRecordResidentOn(vgpu);
                    break;
                case XenVGPUProperty.ScheduledToBeResidentOn:
                    ProcessRecordScheduledToBeResidentOn(vgpu);
                    break;
                case XenVGPUProperty.CompatibilityMetadata:
                    ProcessRecordCompatibilityMetadata(vgpu);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVGPU()
        {
            string vgpu = null;

            if (VGPU != null)
                vgpu = (new XenRef<XenAPI.VGPU>(VGPU)).opaque_ref;
            else if (Ref != null)
                vgpu = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VGPU', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VGPU));
            }

            return vgpu;
        }

        private void ProcessRecordUuid(string vgpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU.get_uuid(session, vgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVM(string vgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VGPU.get_VM(session, vgpu);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGPUGroup(string vgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VGPU.get_GPU_group(session, vgpu);

                        XenAPI.GPU_group obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.GPU_group.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDevice(string vgpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU.get_device(session, vgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentlyAttached(string vgpu)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VGPU.get_currently_attached(session, vgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vgpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VGPU.get_other_config(session, vgpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordType(string vgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VGPU.get_type(session, vgpu);

                        XenAPI.VGPU_type obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VGPU_type.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordResidentOn(string vgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VGPU.get_resident_on(session, vgpu);

                        XenAPI.PGPU obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PGPU.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordScheduledToBeResidentOn(string vgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VGPU.get_scheduled_to_be_resident_on(session, vgpu);

                        XenAPI.PGPU obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PGPU.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCompatibilityMetadata(string vgpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VGPU.get_compatibility_metadata(session, vgpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenVGPUProperty
    {
        Uuid,
        VM,
        GPUGroup,
        Device,
        CurrentlyAttached,
        OtherConfig,
        Type,
        ResidentOn,
        ScheduledToBeResidentOn,
        CompatibilityMetadata
    }

}
