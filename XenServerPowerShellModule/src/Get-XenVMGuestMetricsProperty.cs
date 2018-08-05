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
    [Cmdlet(VerbsCommon.Get, "XenVMGuestMetricsProperty", SupportsShouldProcess = false)]
    public class GetXenVMGuestMetricsProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM_guest_metrics VMGuestMetrics { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM_guest_metrics> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMGuestMetricsProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm_guest_metrics = ParseVMGuestMetrics();

            switch (XenProperty)
            {
                case XenVMGuestMetricsProperty.Uuid:
                    ProcessRecordUuid(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.OsVersion:
                    ProcessRecordOsVersion(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.PVDriversVersion:
                    ProcessRecordPVDriversVersion(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.PVDriversUpToDate:
                    ProcessRecordPVDriversUpToDate(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.Memory:
                    ProcessRecordMemory(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.Disks:
                    ProcessRecordDisks(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.Networks:
                    ProcessRecordNetworks(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.Other:
                    ProcessRecordOther(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.LastUpdated:
                    ProcessRecordLastUpdated(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.OtherConfig:
                    ProcessRecordOtherConfig(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.Live:
                    ProcessRecordLive(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.CanUseHotplugVbd:
                    ProcessRecordCanUseHotplugVbd(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.CanUseHotplugVif:
                    ProcessRecordCanUseHotplugVif(vm_guest_metrics);
                    break;
                case XenVMGuestMetricsProperty.PVDriversDetected:
                    ProcessRecordPVDriversDetected(vm_guest_metrics);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMGuestMetrics()
        {
            string vm_guest_metrics = null;

            if (VMGuestMetrics != null)
                vm_guest_metrics = (new XenRef<XenAPI.VM_guest_metrics>(VMGuestMetrics)).opaque_ref;
            else if (Ref != null)
                vm_guest_metrics = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMGuestMetrics', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMGuestMetrics));
            }

            return vm_guest_metrics;
        }

        private void ProcessRecordUuid(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM_guest_metrics.get_uuid(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOsVersion(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_os_version(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPVDriversVersion(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_PV_drivers_version(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPVDriversUpToDate(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_guest_metrics.get_PV_drivers_up_to_date(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemory(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_memory(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDisks(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_disks(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordNetworks(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_networks(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordOther(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_other(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLastUpdated(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VM_guest_metrics.get_last_updated(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM_guest_metrics.get_other_config(session, vm_guest_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLive(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_guest_metrics.get_live(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCanUseHotplugVbd(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    tristate_type obj = XenAPI.VM_guest_metrics.get_can_use_hotplug_vbd(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCanUseHotplugVif(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    tristate_type obj = XenAPI.VM_guest_metrics.get_can_use_hotplug_vif(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVDriversDetected(string vm_guest_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM_guest_metrics.get_PV_drivers_detected(session, vm_guest_metrics);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVMGuestMetricsProperty
    {
        Uuid,
        OsVersion,
        PVDriversVersion,
        PVDriversUpToDate,
        Memory,
        Disks,
        Networks,
        Other,
        LastUpdated,
        OtherConfig,
        Live,
        CanUseHotplugVbd,
        CanUseHotplugVif,
        PVDriversDetected
    }

}
