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
    [Cmdlet(VerbsCommon.Set, "XenVMAppliance", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VM_appliance))]
    [OutputType(typeof(void))]
    public class SetXenVMAppliance : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM_appliance VMAppliance { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM_appliance> Ref { get; set; }

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

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm_appliance = ParseVMAppliance();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(vm_appliance);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(vm_appliance);

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

                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMAppliance()
        {
            string vm_appliance = null;

            if (VMAppliance != null)
                vm_appliance = (new XenRef<XenAPI.VM_appliance>(VMAppliance)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VM_appliance.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vm_appliance = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VM_appliance.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vm_appliance = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VM_appliance with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                vm_appliance = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMAppliance', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMAppliance));
            }

            return vm_appliance;
        }

        private void ProcessRecordNameLabel(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM_appliance.set_name_label(session, vm_appliance, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM_appliance.set_name_description(session, vm_appliance, NameDescription);

            });
        }

        #endregion
    }
}
