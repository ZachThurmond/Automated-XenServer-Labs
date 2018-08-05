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
    [Cmdlet(VerbsCommon.Get, "XenVMMetricsProperty", SupportsShouldProcess = false)]
    public class GetXenVMMetricsProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM_metrics VMMetrics { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM_metrics> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMMetricsProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm_metrics = ParseVMMetrics();

            switch (XenProperty)
            {
                case XenVMMetricsProperty.Uuid:
                    ProcessRecordUuid(vm_metrics);
                    break;
                case XenVMMetricsProperty.MemoryActual:
                    ProcessRecordMemoryActual(vm_metrics);
                    break;
                case XenVMMetricsProperty.VCPUsNumber:
                    ProcessRecordVCPUsNumber(vm_metrics);
                    break;
                case XenVMMetricsProperty.VCPUsUtilisation:
                    ProcessRecordVCPUsUtilisation(vm_metrics);
                    break;
                case XenVMMetricsProperty.VCPUsCPU:
                    ProcessRecordVCPUsCPU(vm_metrics);
                    break;
                case XenVMMetricsProperty.VCPUsParams:
                    ProcessRecordVCPUsParams(vm_metrics);
                    break;
                case XenVMMetricsProperty.VCPUsFlags:
                    ProcessRecordVCPUsFlags(vm_metrics);
                    break;
                case XenVMMetricsProperty.State:
                    ProcessRecordState(vm_metrics);
                    break;
                case XenVMMetricsProperty.StartTime:
                    ProcessRecordStartTime(vm_metrics);
                    break;
                case XenVMMetricsProperty.InstallTime:
                    ProcessRecordInstallTime(vm_metrics);
                    break;
                case XenVMMetricsProperty.LastUpdated:
                    ProcessRecordLastUpdated(vm_metrics);
                    break;
                case XenVMMetricsProperty.OtherConfig:
                    ProcessRecordOtherConfig(vm_metrics);
                    break;
                case XenVMMetricsProperty.Hvm:
                    ProcessRecordHvm(vm_metrics);
                    break;
                case XenVMMetricsProperty.NestedVirt:
                    ProcessRecordNestedVirt(vm_metrics);
                    break;
                case XenVMMetricsProperty.Nomigrate:
                    ProcessRecordNomigrate(vm_metrics);
                    break;
                case XenVMMetricsProperty.CurrentDomainType:
                    ProcessRecordCurrentDomainType(vm_metrics);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMMetrics()
        {
            string vm_metrics = null;

            if (VMMetrics != null)
                vm_metrics = (new XenRef<XenAPI.VM_metrics>(VMMetrics)).opaque_ref;
            else if (Ref != null)
                vm_metrics = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMMetrics', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMMetrics));
            }

            return vm_metrics;
        }

        private void ProcessRecordUuid(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM_metrics.get_uuid(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryActual(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM_metrics.get_memory_actual(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVCPUsNumber(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM_metrics.get_VCPUs_number(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVCPUsUtilisation(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_metrics.get_VCPUs_utilisation(session, vm_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVCPUsCPU(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_metrics.get_VCPUs_CPU(session, vm_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVCPUsParams(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_metrics.get_VCPUs_params(session, vm_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVCPUsFlags(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_metrics.get_VCPUs_flags(session, vm_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordState(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VM_metrics.get_state(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStartTime(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VM_metrics.get_start_time(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordInstallTime(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VM_metrics.get_install_time(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastUpdated(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VM_metrics.get_last_updated(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_metrics.get_other_config(session, vm_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordHvm(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_metrics.get_hvm(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNestedVirt(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_metrics.get_nested_virt(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNomigrate(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_metrics.get_nomigrate(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentDomainType(string vm_metrics)
        {
            RunApiCall(()=>
            {
                    domain_type obj = XenAPI.VM_metrics.get_current_domain_type(session, vm_metrics);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVMMetricsProperty
    {
        Uuid,
        MemoryActual,
        VCPUsNumber,
        VCPUsUtilisation,
        VCPUsCPU,
        VCPUsParams,
        VCPUsFlags,
        State,
        StartTime,
        InstallTime,
        LastUpdated,
        OtherConfig,
        Hvm,
        NestedVirt,
        Nomigrate,
        CurrentDomainType
    }

}
