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
    [Cmdlet(VerbsCommon.Get, "XenVMApplianceProperty", SupportsShouldProcess = false)]
    public class GetXenVMApplianceProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM_appliance VMAppliance { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM_appliance> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMApplianceProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenVMApplianceProperty.SRsRequiredForRecovery:
                    _context = new XenVMAppliancePropertySRsRequiredForRecoveryDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm_appliance = ParseVMAppliance();

            switch (XenProperty)
            {
                case XenVMApplianceProperty.Uuid:
                    ProcessRecordUuid(vm_appliance);
                    break;
                case XenVMApplianceProperty.NameLabel:
                    ProcessRecordNameLabel(vm_appliance);
                    break;
                case XenVMApplianceProperty.NameDescription:
                    ProcessRecordNameDescription(vm_appliance);
                    break;
                case XenVMApplianceProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(vm_appliance);
                    break;
                case XenVMApplianceProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(vm_appliance);
                    break;
                case XenVMApplianceProperty.VMs:
                    ProcessRecordVMs(vm_appliance);
                    break;
                case XenVMApplianceProperty.SRsRequiredForRecovery:
                    ProcessRecordSRsRequiredForRecovery(vm_appliance);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMAppliance()
        {
            string vm_appliance = null;

            if (VMAppliance != null)
                vm_appliance = (new XenRef<XenAPI.VM_appliance>(VMAppliance)).opaque_ref;
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

        private void ProcessRecordUuid(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM_appliance.get_uuid(session, vm_appliance);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM_appliance.get_name_label(session, vm_appliance);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM_appliance.get_name_description(session, vm_appliance);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    List<vm_appliance_operation> obj = XenAPI.VM_appliance.get_allowed_operations(session, vm_appliance);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_appliance.get_current_operations(session, vm_appliance);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVMs(string vm_appliance)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM_appliance.get_VMs(session, vm_appliance);

                        var records = new List<XenAPI.VM>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VM.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSRsRequiredForRecovery(string vm_appliance)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenVMAppliancePropertySRsRequiredForRecoveryDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_get_SRs_required_for_recovery(session, vm_appliance, contxt.SessionTo);

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
                    var refs = XenAPI.VM_appliance.get_SRs_required_for_recovery(session, vm_appliance, contxt.SessionTo);

                        var records = new List<XenAPI.SR>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.SR.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
                }

            });
        }

        #endregion
    }

    public enum XenVMApplianceProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        AllowedOperations,
        CurrentOperations,
        VMs,
        SRsRequiredForRecovery
    }

    public class XenVMAppliancePropertySRsRequiredForRecoveryDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }
 
    }

}
