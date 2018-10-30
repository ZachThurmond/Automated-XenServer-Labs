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
    [Cmdlet(VerbsCommon.Get, "XenPGPUProperty", SupportsShouldProcess = false)]
    public class GetXenPGPUProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PGPU PGPU { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PGPU> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPGPUProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenPGPUProperty.RemainingCapacity:
                    _context = new XenPGPUPropertyRemainingCapacityDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pgpu = ParsePGPU();

            switch (XenProperty)
            {
                case XenPGPUProperty.Uuid:
                    ProcessRecordUuid(pgpu);
                    break;
                case XenPGPUProperty.PCI:
                    ProcessRecordPCI(pgpu);
                    break;
                case XenPGPUProperty.GPUGroup:
                    ProcessRecordGPUGroup(pgpu);
                    break;
                case XenPGPUProperty.Host:
                    ProcessRecordHost(pgpu);
                    break;
                case XenPGPUProperty.OtherConfig:
                    ProcessRecordOtherConfig(pgpu);
                    break;
                case XenPGPUProperty.SupportedVGPUTypes:
                    ProcessRecordSupportedVGPUTypes(pgpu);
                    break;
                case XenPGPUProperty.EnabledVGPUTypes:
                    ProcessRecordEnabledVGPUTypes(pgpu);
                    break;
                case XenPGPUProperty.ResidentVGPUs:
                    ProcessRecordResidentVGPUs(pgpu);
                    break;
                case XenPGPUProperty.SupportedVGPUMaxCapacities:
                    ProcessRecordSupportedVGPUMaxCapacities(pgpu);
                    break;
                case XenPGPUProperty.Dom0Access:
                    ProcessRecordDom0Access(pgpu);
                    break;
                case XenPGPUProperty.IsSystemDisplayDevice:
                    ProcessRecordIsSystemDisplayDevice(pgpu);
                    break;
                case XenPGPUProperty.CompatibilityMetadata:
                    ProcessRecordCompatibilityMetadata(pgpu);
                    break;
                case XenPGPUProperty.RemainingCapacity:
                    ProcessRecordRemainingCapacity(pgpu);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePGPU()
        {
            string pgpu = null;

            if (PGPU != null)
                pgpu = (new XenRef<XenAPI.PGPU>(PGPU)).opaque_ref;
            else if (Ref != null)
                pgpu = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PGPU', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PGPU));
            }

            return pgpu;
        }

        private void ProcessRecordUuid(string pgpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PGPU.get_uuid(session, pgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPCI(string pgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PGPU.get_PCI(session, pgpu);

                        XenAPI.PCI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PCI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGPUGroup(string pgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PGPU.get_GPU_group(session, pgpu);

                        XenAPI.GPU_group obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.GPU_group.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string pgpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PGPU.get_host(session, pgpu);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string pgpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PGPU.get_other_config(session, pgpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSupportedVGPUTypes(string pgpu)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PGPU.get_supported_VGPU_types(session, pgpu);

                        var records = new List<XenAPI.VGPU_type>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VGPU_type.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordEnabledVGPUTypes(string pgpu)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PGPU.get_enabled_VGPU_types(session, pgpu);

                        var records = new List<XenAPI.VGPU_type>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VGPU_type.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordResidentVGPUs(string pgpu)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PGPU.get_resident_VGPUs(session, pgpu);

                        var records = new List<XenAPI.VGPU>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VGPU.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSupportedVGPUMaxCapacities(string pgpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PGPU.get_supported_VGPU_max_capacities(session, pgpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDom0Access(string pgpu)
        {
            RunApiCall(()=>
            {
                    pgpu_dom0_access obj = XenAPI.PGPU.get_dom0_access(session, pgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsSystemDisplayDevice(string pgpu)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PGPU.get_is_system_display_device(session, pgpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCompatibilityMetadata(string pgpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PGPU.get_compatibility_metadata(session, pgpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordRemainingCapacity(string pgpu)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenPGPUPropertyRemainingCapacityDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PGPU.async_get_remaining_capacity(session, pgpu, contxt.VgpuType);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    long obj = XenAPI.PGPU.get_remaining_capacity(session, pgpu, contxt.VgpuType);

                        WriteObject(obj, true);
                }

            });
        }

        #endregion
    }

    public enum XenPGPUProperty
    {
        Uuid,
        PCI,
        GPUGroup,
        Host,
        OtherConfig,
        SupportedVGPUTypes,
        EnabledVGPUTypes,
        ResidentVGPUs,
        SupportedVGPUMaxCapacities,
        Dom0Access,
        IsSystemDisplayDevice,
        CompatibilityMetadata,
        RemainingCapacity
    }

    public class XenPGPUPropertyRemainingCapacityDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VGPU_type> VgpuType { get; set; }
 
    }

}
