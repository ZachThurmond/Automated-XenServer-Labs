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
    [Cmdlet(VerbsCommon.Get, "XenGPUGroupProperty", SupportsShouldProcess = false)]
    public class GetXenGPUGroupProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.GPU_group GPUGroup { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.GPU_group> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenGPUGroupProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenGPUGroupProperty.RemainingCapacity:
                    _context = new XenGPUGroupPropertyRemainingCapacityDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string gpu_group = ParseGPUGroup();

            switch (XenProperty)
            {
                case XenGPUGroupProperty.Uuid:
                    ProcessRecordUuid(gpu_group);
                    break;
                case XenGPUGroupProperty.NameLabel:
                    ProcessRecordNameLabel(gpu_group);
                    break;
                case XenGPUGroupProperty.NameDescription:
                    ProcessRecordNameDescription(gpu_group);
                    break;
                case XenGPUGroupProperty.PGPUs:
                    ProcessRecordPGPUs(gpu_group);
                    break;
                case XenGPUGroupProperty.VGPUs:
                    ProcessRecordVGPUs(gpu_group);
                    break;
                case XenGPUGroupProperty.GPUTypes:
                    ProcessRecordGPUTypes(gpu_group);
                    break;
                case XenGPUGroupProperty.OtherConfig:
                    ProcessRecordOtherConfig(gpu_group);
                    break;
                case XenGPUGroupProperty.AllocationAlgorithm:
                    ProcessRecordAllocationAlgorithm(gpu_group);
                    break;
                case XenGPUGroupProperty.SupportedVGPUTypes:
                    ProcessRecordSupportedVGPUTypes(gpu_group);
                    break;
                case XenGPUGroupProperty.EnabledVGPUTypes:
                    ProcessRecordEnabledVGPUTypes(gpu_group);
                    break;
                case XenGPUGroupProperty.RemainingCapacity:
                    ProcessRecordRemainingCapacity(gpu_group);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseGPUGroup()
        {
            string gpu_group = null;

            if (GPUGroup != null)
                gpu_group = (new XenRef<XenAPI.GPU_group>(GPUGroup)).opaque_ref;
            else if (Ref != null)
                gpu_group = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'GPUGroup', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    GPUGroup));
            }

            return gpu_group;
        }

        private void ProcessRecordUuid(string gpu_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.GPU_group.get_uuid(session, gpu_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string gpu_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.GPU_group.get_name_label(session, gpu_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string gpu_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.GPU_group.get_name_description(session, gpu_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPGPUs(string gpu_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.GPU_group.get_PGPUs(session, gpu_group);

                        var records = new List<XenAPI.PGPU>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PGPU.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVGPUs(string gpu_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.GPU_group.get_VGPUs(session, gpu_group);

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

        private void ProcessRecordGPUTypes(string gpu_group)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.GPU_group.get_GPU_types(session, gpu_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string gpu_group)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.GPU_group.get_other_config(session, gpu_group);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordAllocationAlgorithm(string gpu_group)
        {
            RunApiCall(()=>
            {
                    allocation_algorithm obj = XenAPI.GPU_group.get_allocation_algorithm(session, gpu_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSupportedVGPUTypes(string gpu_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.GPU_group.get_supported_VGPU_types(session, gpu_group);

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

        private void ProcessRecordEnabledVGPUTypes(string gpu_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.GPU_group.get_enabled_VGPU_types(session, gpu_group);

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

        private void ProcessRecordRemainingCapacity(string gpu_group)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenGPUGroupPropertyRemainingCapacityDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.GPU_group.async_get_remaining_capacity(session, gpu_group, contxt.VgpuType);

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
                    long obj = XenAPI.GPU_group.get_remaining_capacity(session, gpu_group, contxt.VgpuType);

                        WriteObject(obj, true);
                }

            });
        }

        #endregion
    }

    public enum XenGPUGroupProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        PGPUs,
        VGPUs,
        GPUTypes,
        OtherConfig,
        AllocationAlgorithm,
        SupportedVGPUTypes,
        EnabledVGPUTypes,
        RemainingCapacity
    }

    public class XenGPUGroupPropertyRemainingCapacityDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VGPU_type> VgpuType { get; set; }
 
    }

}
