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
    [Cmdlet(VerbsCommon.Get, "XenVGPUTypeProperty", SupportsShouldProcess = false)]
    public class GetXenVGPUTypeProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VGPU_type VGPUType { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VGPU_type> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVGPUTypeProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vgpu_type = ParseVGPUType();

            switch (XenProperty)
            {
                case XenVGPUTypeProperty.Uuid:
                    ProcessRecordUuid(vgpu_type);
                    break;
                case XenVGPUTypeProperty.VendorName:
                    ProcessRecordVendorName(vgpu_type);
                    break;
                case XenVGPUTypeProperty.ModelName:
                    ProcessRecordModelName(vgpu_type);
                    break;
                case XenVGPUTypeProperty.FramebufferSize:
                    ProcessRecordFramebufferSize(vgpu_type);
                    break;
                case XenVGPUTypeProperty.MaxHeads:
                    ProcessRecordMaxHeads(vgpu_type);
                    break;
                case XenVGPUTypeProperty.MaxResolutionX:
                    ProcessRecordMaxResolutionX(vgpu_type);
                    break;
                case XenVGPUTypeProperty.MaxResolutionY:
                    ProcessRecordMaxResolutionY(vgpu_type);
                    break;
                case XenVGPUTypeProperty.SupportedOnPGPUs:
                    ProcessRecordSupportedOnPGPUs(vgpu_type);
                    break;
                case XenVGPUTypeProperty.EnabledOnPGPUs:
                    ProcessRecordEnabledOnPGPUs(vgpu_type);
                    break;
                case XenVGPUTypeProperty.VGPUs:
                    ProcessRecordVGPUs(vgpu_type);
                    break;
                case XenVGPUTypeProperty.SupportedOnGPUGroups:
                    ProcessRecordSupportedOnGPUGroups(vgpu_type);
                    break;
                case XenVGPUTypeProperty.EnabledOnGPUGroups:
                    ProcessRecordEnabledOnGPUGroups(vgpu_type);
                    break;
                case XenVGPUTypeProperty.Implementation:
                    ProcessRecordImplementation(vgpu_type);
                    break;
                case XenVGPUTypeProperty.Identifier:
                    ProcessRecordIdentifier(vgpu_type);
                    break;
                case XenVGPUTypeProperty.Experimental:
                    ProcessRecordExperimental(vgpu_type);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVGPUType()
        {
            string vgpu_type = null;

            if (VGPUType != null)
                vgpu_type = (new XenRef<XenAPI.VGPU_type>(VGPUType)).opaque_ref;
            else if (Ref != null)
                vgpu_type = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VGPUType', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VGPUType));
            }

            return vgpu_type;
        }

        private void ProcessRecordUuid(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU_type.get_uuid(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorName(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU_type.get_vendor_name(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordModelName(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU_type.get_model_name(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFramebufferSize(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VGPU_type.get_framebuffer_size(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMaxHeads(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VGPU_type.get_max_heads(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMaxResolutionX(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VGPU_type.get_max_resolution_x(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMaxResolutionY(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VGPU_type.get_max_resolution_y(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSupportedOnPGPUs(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VGPU_type.get_supported_on_PGPUs(session, vgpu_type);

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

        private void ProcessRecordEnabledOnPGPUs(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VGPU_type.get_enabled_on_PGPUs(session, vgpu_type);

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

        private void ProcessRecordVGPUs(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VGPU_type.get_VGPUs(session, vgpu_type);

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

        private void ProcessRecordSupportedOnGPUGroups(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VGPU_type.get_supported_on_GPU_groups(session, vgpu_type);

                        var records = new List<XenAPI.GPU_group>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.GPU_group.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordEnabledOnGPUGroups(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VGPU_type.get_enabled_on_GPU_groups(session, vgpu_type);

                        var records = new List<XenAPI.GPU_group>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.GPU_group.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordImplementation(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    vgpu_type_implementation obj = XenAPI.VGPU_type.get_implementation(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIdentifier(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VGPU_type.get_identifier(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordExperimental(string vgpu_type)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VGPU_type.get_experimental(session, vgpu_type);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVGPUTypeProperty
    {
        Uuid,
        VendorName,
        ModelName,
        FramebufferSize,
        MaxHeads,
        MaxResolutionX,
        MaxResolutionY,
        SupportedOnPGPUs,
        EnabledOnPGPUs,
        VGPUs,
        SupportedOnGPUGroups,
        EnabledOnGPUGroups,
        Implementation,
        Identifier,
        Experimental
    }

}
