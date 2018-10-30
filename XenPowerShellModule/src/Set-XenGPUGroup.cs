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
    [Cmdlet(VerbsCommon.Set, "XenGPUGroup", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.GPU_group))]
    [OutputType(typeof(void))]
    public class SetXenGPUGroup : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.GPU_group GPUGroup { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.GPU_group> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter]
        public string NameLabel
        {
            get { return _nameLabel; }
            set
            {
                _nameLabel = value;
                _nameLabelIsSpecified = true;
            }
        }
        private string _nameLabel;
        private bool _nameLabelIsSpecified;

        [Parameter]
        public string NameDescription
        {
            get { return _nameDescription; }
            set
            {
                _nameDescription = value;
                _nameDescriptionIsSpecified = true;
            }
        }
        private string _nameDescription;
        private bool _nameDescriptionIsSpecified;

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
        public allocation_algorithm AllocationAlgorithm
        {
            get { return _allocationAlgorithm; }
            set
            {
                _allocationAlgorithm = value;
                _allocationAlgorithmIsSpecified = true;
            }
        }
        private allocation_algorithm _allocationAlgorithm;
        private bool _allocationAlgorithmIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string gpu_group = ParseGPUGroup();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(gpu_group);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(gpu_group);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(gpu_group);
            if (_allocationAlgorithmIsSpecified)
                ProcessRecordAllocationAlgorithm(gpu_group);

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

                        var obj = XenAPI.GPU_group.get_record(session, gpu_group);
                        if (obj != null)
                            obj.opaque_ref = gpu_group;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseGPUGroup()
        {
            string gpu_group = null;

            if (GPUGroup != null)
                gpu_group = (new XenRef<XenAPI.GPU_group>(GPUGroup)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.GPU_group.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    gpu_group = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.GPU_group.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    gpu_group = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.GPU_group with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
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

        private void ProcessRecordNameLabel(string gpu_group)
        {
            if (!ShouldProcess(gpu_group, "GPU_group.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.GPU_group.set_name_label(session, gpu_group, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string gpu_group)
        {
            if (!ShouldProcess(gpu_group, "GPU_group.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.GPU_group.set_name_description(session, gpu_group, NameDescription);

            });
        }

        private void ProcessRecordOtherConfig(string gpu_group)
        {
            if (!ShouldProcess(gpu_group, "GPU_group.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.GPU_group.set_other_config(session, gpu_group, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordAllocationAlgorithm(string gpu_group)
        {
            if (!ShouldProcess(gpu_group, "GPU_group.set_allocation_algorithm"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.GPU_group.set_allocation_algorithm(session, gpu_group, AllocationAlgorithm);

            });
        }

        #endregion
    }
}
