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
    [Cmdlet(VerbsCommon.Get, "XenVMProperty", SupportsShouldProcess = false)]
    public class GetXenVMProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM VM { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenVMProperty.Cooperative:
                    _context = new XenVMPropertyCooperativeDynamicParameters();
                    return _context;
                case XenVMProperty.PossibleHosts:
                    _context = new XenVMPropertyPossibleHostsDynamicParameters();
                    return _context;
                case XenVMProperty.SRsRequiredForRecovery:
                    _context = new XenVMPropertySRsRequiredForRecoveryDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm = ParseVM();

            switch (XenProperty)
            {
                case XenVMProperty.Uuid:
                    ProcessRecordUuid(vm);
                    break;
                case XenVMProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(vm);
                    break;
                case XenVMProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(vm);
                    break;
                case XenVMProperty.PowerState:
                    ProcessRecordPowerState(vm);
                    break;
                case XenVMProperty.NameLabel:
                    ProcessRecordNameLabel(vm);
                    break;
                case XenVMProperty.NameDescription:
                    ProcessRecordNameDescription(vm);
                    break;
                case XenVMProperty.UserVersion:
                    ProcessRecordUserVersion(vm);
                    break;
                case XenVMProperty.IsATemplate:
                    ProcessRecordIsATemplate(vm);
                    break;
                case XenVMProperty.IsDefaultTemplate:
                    ProcessRecordIsDefaultTemplate(vm);
                    break;
                case XenVMProperty.SuspendVDI:
                    ProcessRecordSuspendVDI(vm);
                    break;
                case XenVMProperty.ResidentOn:
                    ProcessRecordResidentOn(vm);
                    break;
                case XenVMProperty.Affinity:
                    ProcessRecordAffinity(vm);
                    break;
                case XenVMProperty.MemoryOverhead:
                    ProcessRecordMemoryOverhead(vm);
                    break;
                case XenVMProperty.MemoryTarget:
                    ProcessRecordMemoryTarget(vm);
                    break;
                case XenVMProperty.MemoryStaticMax:
                    ProcessRecordMemoryStaticMax(vm);
                    break;
                case XenVMProperty.MemoryDynamicMax:
                    ProcessRecordMemoryDynamicMax(vm);
                    break;
                case XenVMProperty.MemoryDynamicMin:
                    ProcessRecordMemoryDynamicMin(vm);
                    break;
                case XenVMProperty.MemoryStaticMin:
                    ProcessRecordMemoryStaticMin(vm);
                    break;
                case XenVMProperty.VCPUsParams:
                    ProcessRecordVCPUsParams(vm);
                    break;
                case XenVMProperty.VCPUsMax:
                    ProcessRecordVCPUsMax(vm);
                    break;
                case XenVMProperty.VCPUsAtStartup:
                    ProcessRecordVCPUsAtStartup(vm);
                    break;
                case XenVMProperty.ActionsAfterShutdown:
                    ProcessRecordActionsAfterShutdown(vm);
                    break;
                case XenVMProperty.ActionsAfterReboot:
                    ProcessRecordActionsAfterReboot(vm);
                    break;
                case XenVMProperty.ActionsAfterCrash:
                    ProcessRecordActionsAfterCrash(vm);
                    break;
                case XenVMProperty.Consoles:
                    ProcessRecordConsoles(vm);
                    break;
                case XenVMProperty.VIFs:
                    ProcessRecordVIFs(vm);
                    break;
                case XenVMProperty.VBDs:
                    ProcessRecordVBDs(vm);
                    break;
                case XenVMProperty.VUSBs:
                    ProcessRecordVUSBs(vm);
                    break;
                case XenVMProperty.CrashDumps:
                    ProcessRecordCrashDumps(vm);
                    break;
                case XenVMProperty.VTPMs:
                    ProcessRecordVTPMs(vm);
                    break;
                case XenVMProperty.PVBootloader:
                    ProcessRecordPVBootloader(vm);
                    break;
                case XenVMProperty.PVKernel:
                    ProcessRecordPVKernel(vm);
                    break;
                case XenVMProperty.PVRamdisk:
                    ProcessRecordPVRamdisk(vm);
                    break;
                case XenVMProperty.PVArgs:
                    ProcessRecordPVArgs(vm);
                    break;
                case XenVMProperty.PVBootloaderArgs:
                    ProcessRecordPVBootloaderArgs(vm);
                    break;
                case XenVMProperty.PVLegacyArgs:
                    ProcessRecordPVLegacyArgs(vm);
                    break;
                case XenVMProperty.HVMBootPolicy:
                    ProcessRecordHVMBootPolicy(vm);
                    break;
                case XenVMProperty.HVMBootParams:
                    ProcessRecordHVMBootParams(vm);
                    break;
                case XenVMProperty.HVMShadowMultiplier:
                    ProcessRecordHVMShadowMultiplier(vm);
                    break;
                case XenVMProperty.Platform:
                    ProcessRecordPlatform(vm);
                    break;
                case XenVMProperty.PCIBus:
                    ProcessRecordPCIBus(vm);
                    break;
                case XenVMProperty.OtherConfig:
                    ProcessRecordOtherConfig(vm);
                    break;
                case XenVMProperty.Domid:
                    ProcessRecordDomid(vm);
                    break;
                case XenVMProperty.Domarch:
                    ProcessRecordDomarch(vm);
                    break;
                case XenVMProperty.LastBootCPUFlags:
                    ProcessRecordLastBootCPUFlags(vm);
                    break;
                case XenVMProperty.IsControlDomain:
                    ProcessRecordIsControlDomain(vm);
                    break;
                case XenVMProperty.Metrics:
                    ProcessRecordMetrics(vm);
                    break;
                case XenVMProperty.GuestMetrics:
                    ProcessRecordGuestMetrics(vm);
                    break;
                case XenVMProperty.LastBootedRecord:
                    ProcessRecordLastBootedRecord(vm);
                    break;
                case XenVMProperty.Recommendations:
                    ProcessRecordRecommendations(vm);
                    break;
                case XenVMProperty.XenstoreData:
                    ProcessRecordXenstoreData(vm);
                    break;
                case XenVMProperty.HaAlwaysRun:
                    ProcessRecordHaAlwaysRun(vm);
                    break;
                case XenVMProperty.HaRestartPriority:
                    ProcessRecordHaRestartPriority(vm);
                    break;
                case XenVMProperty.IsASnapshot:
                    ProcessRecordIsASnapshot(vm);
                    break;
                case XenVMProperty.SnapshotOf:
                    ProcessRecordSnapshotOf(vm);
                    break;
                case XenVMProperty.Snapshots:
                    ProcessRecordSnapshots(vm);
                    break;
                case XenVMProperty.SnapshotTime:
                    ProcessRecordSnapshotTime(vm);
                    break;
                case XenVMProperty.TransportableSnapshotId:
                    ProcessRecordTransportableSnapshotId(vm);
                    break;
                case XenVMProperty.Blobs:
                    ProcessRecordBlobs(vm);
                    break;
                case XenVMProperty.Tags:
                    ProcessRecordTags(vm);
                    break;
                case XenVMProperty.BlockedOperations:
                    ProcessRecordBlockedOperations(vm);
                    break;
                case XenVMProperty.SnapshotInfo:
                    ProcessRecordSnapshotInfo(vm);
                    break;
                case XenVMProperty.SnapshotMetadata:
                    ProcessRecordSnapshotMetadata(vm);
                    break;
                case XenVMProperty.Parent:
                    ProcessRecordParent(vm);
                    break;
                case XenVMProperty.Children:
                    ProcessRecordChildren(vm);
                    break;
                case XenVMProperty.BiosStrings:
                    ProcessRecordBiosStrings(vm);
                    break;
                case XenVMProperty.ProtectionPolicy:
                    ProcessRecordProtectionPolicy(vm);
                    break;
                case XenVMProperty.IsSnapshotFromVmpp:
                    ProcessRecordIsSnapshotFromVmpp(vm);
                    break;
                case XenVMProperty.SnapshotSchedule:
                    ProcessRecordSnapshotSchedule(vm);
                    break;
                case XenVMProperty.IsVmssSnapshot:
                    ProcessRecordIsVmssSnapshot(vm);
                    break;
                case XenVMProperty.Appliance:
                    ProcessRecordAppliance(vm);
                    break;
                case XenVMProperty.StartDelay:
                    ProcessRecordStartDelay(vm);
                    break;
                case XenVMProperty.ShutdownDelay:
                    ProcessRecordShutdownDelay(vm);
                    break;
                case XenVMProperty.Order:
                    ProcessRecordOrder(vm);
                    break;
                case XenVMProperty.VGPUs:
                    ProcessRecordVGPUs(vm);
                    break;
                case XenVMProperty.AttachedPCIs:
                    ProcessRecordAttachedPCIs(vm);
                    break;
                case XenVMProperty.SuspendSR:
                    ProcessRecordSuspendSR(vm);
                    break;
                case XenVMProperty.Version:
                    ProcessRecordVersion(vm);
                    break;
                case XenVMProperty.GenerationId:
                    ProcessRecordGenerationId(vm);
                    break;
                case XenVMProperty.HardwarePlatformVersion:
                    ProcessRecordHardwarePlatformVersion(vm);
                    break;
                case XenVMProperty.HasVendorDevice:
                    ProcessRecordHasVendorDevice(vm);
                    break;
                case XenVMProperty.RequiresReboot:
                    ProcessRecordRequiresReboot(vm);
                    break;
                case XenVMProperty.ReferenceLabel:
                    ProcessRecordReferenceLabel(vm);
                    break;
                case XenVMProperty.Cooperative:
                    ProcessRecordCooperative(vm);
                    break;
                case XenVMProperty.BootRecord:
                    ProcessRecordBootRecord(vm);
                    break;
                case XenVMProperty.DataSources:
                    ProcessRecordDataSources(vm);
                    break;
                case XenVMProperty.AllowedVBDDevices:
                    ProcessRecordAllowedVBDDevices(vm);
                    break;
                case XenVMProperty.AllowedVIFDevices:
                    ProcessRecordAllowedVIFDevices(vm);
                    break;
                case XenVMProperty.PossibleHosts:
                    ProcessRecordPossibleHosts(vm);
                    break;
                case XenVMProperty.SRsRequiredForRecovery:
                    ProcessRecordSRsRequiredForRecovery(vm);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVM()
        {
            string vm = null;

            if (VM != null)
                vm = (new XenRef<XenAPI.VM>(VM)).opaque_ref;
            else if (Ref != null)
                vm = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VM', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VM));
            }

            return vm;
        }

        private void ProcessRecordUuid(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_uuid(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string vm)
        {
            RunApiCall(()=>
            {
                    List<vm_operations> obj = XenAPI.VM.get_allowed_operations(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_current_operations(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPowerState(string vm)
        {
            RunApiCall(()=>
            {
                    vm_power_state obj = XenAPI.VM.get_power_state(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_name_label(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_name_description(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUserVersion(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_user_version(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsATemplate(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_a_template(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsDefaultTemplate(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_default_template(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSuspendVDI(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_suspend_VDI(session, vm);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordResidentOn(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_resident_on(session, vm);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAffinity(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_affinity(session, vm);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryOverhead(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_overhead(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryTarget(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_target(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryStaticMax(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_static_max(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryDynamicMax(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_dynamic_max(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryDynamicMin(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_dynamic_min(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryStaticMin(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_memory_static_min(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVCPUsParams(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_VCPUs_params(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVCPUsMax(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_VCPUs_max(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVCPUsAtStartup(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_VCPUs_at_startup(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordActionsAfterShutdown(string vm)
        {
            RunApiCall(()=>
            {
                    on_normal_exit obj = XenAPI.VM.get_actions_after_shutdown(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordActionsAfterReboot(string vm)
        {
            RunApiCall(()=>
            {
                    on_normal_exit obj = XenAPI.VM.get_actions_after_reboot(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordActionsAfterCrash(string vm)
        {
            RunApiCall(()=>
            {
                    on_crash_behaviour obj = XenAPI.VM.get_actions_after_crash(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordConsoles(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_consoles(session, vm);

                        var records = new List<XenAPI.Console>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Console.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVIFs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_VIFs(session, vm);

                        var records = new List<XenAPI.VIF>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VIF.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVBDs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_VBDs(session, vm);

                        var records = new List<XenAPI.VBD>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VBD.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVUSBs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_VUSBs(session, vm);

                        var records = new List<XenAPI.VUSB>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VUSB.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordCrashDumps(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_crash_dumps(session, vm);

                        var records = new List<XenAPI.Crashdump>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Crashdump.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVTPMs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_VTPMs(session, vm);

                        var records = new List<XenAPI.VTPM>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VTPM.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPVBootloader(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_bootloader(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVKernel(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_kernel(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVRamdisk(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_ramdisk(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVArgs(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_args(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVBootloaderArgs(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_bootloader_args(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVLegacyArgs(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PV_legacy_args(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHVMBootPolicy(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_HVM_boot_policy(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHVMBootParams(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_HVM_boot_params(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordHVMShadowMultiplier(string vm)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.VM.get_HVM_shadow_multiplier(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPlatform(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_platform(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPCIBus(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_PCI_bus(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_other_config(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDomid(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_domid(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDomarch(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_domarch(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastBootCPUFlags(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_last_boot_CPU_flags(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordIsControlDomain(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_control_domain(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetrics(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_metrics(session, vm);

                        XenAPI.VM_metrics obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM_metrics.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGuestMetrics(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_guest_metrics(session, vm);

                        XenAPI.VM_guest_metrics obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM_guest_metrics.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastBootedRecord(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_last_booted_record(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRecommendations(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_recommendations(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordXenstoreData(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_xenstore_data(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordHaAlwaysRun(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_ha_always_run(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHaRestartPriority(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_ha_restart_priority(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsASnapshot(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_a_snapshot(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSnapshotOf(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_snapshot_of(session, vm);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSnapshots(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_snapshots(session, vm);

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

        private void ProcessRecordSnapshotTime(string vm)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VM.get_snapshot_time(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTransportableSnapshotId(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_transportable_snapshot_id(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBlobs(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_blobs(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordTags(string vm)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VM.get_tags(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBlockedOperations(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_blocked_operations(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSnapshotInfo(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_snapshot_info(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSnapshotMetadata(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_snapshot_metadata(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordParent(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_parent(session, vm);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordChildren(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_children(session, vm);

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

        private void ProcessRecordBiosStrings(string vm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VM.get_bios_strings(session, vm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordProtectionPolicy(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_protection_policy(session, vm);

                        XenAPI.VMPP obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VMPP.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsSnapshotFromVmpp(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_snapshot_from_vmpp(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSnapshotSchedule(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_snapshot_schedule(session, vm);

                        XenAPI.VMSS obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VMSS.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsVmssSnapshot(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_is_vmss_snapshot(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAppliance(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_appliance(session, vm);

                        XenAPI.VM_appliance obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM_appliance.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStartDelay(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_start_delay(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordShutdownDelay(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_shutdown_delay(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOrder(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_order(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVGPUs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_VGPUs(session, vm);

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

        private void ProcessRecordAttachedPCIs(string vm)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VM.get_attached_PCIs(session, vm);

                        var records = new List<XenAPI.PCI>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PCI.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSuspendSR(string vm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VM.get_suspend_SR(session, vm);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_version(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGenerationId(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_generation_id(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHardwarePlatformVersion(string vm)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VM.get_hardware_platform_version(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHasVendorDevice(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_has_vendor_device(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRequiresReboot(string vm)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VM.get_requires_reboot(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordReferenceLabel(string vm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VM.get_reference_label(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCooperative(string vm)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenVMPropertyCooperativeDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_get_cooperative(session, vm);

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
                    bool obj = XenAPI.VM.get_cooperative(session, vm);

                        WriteObject(obj, true);
                }

            });
        }

        private void ProcessRecordBootRecord(string vm)
        {
            RunApiCall(()=>
            {
                    XenAPI.VM obj = XenAPI.VM.get_boot_record(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDataSources(string vm)
        {
            RunApiCall(()=>
            {
                    List<XenAPI.Data_source> obj = XenAPI.VM.get_data_sources(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedVBDDevices(string vm)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VM.get_allowed_VBD_devices(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedVIFDevices(string vm)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VM.get_allowed_VIF_devices(session, vm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPossibleHosts(string vm)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenVMPropertyPossibleHostsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_get_possible_hosts(session, vm);

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
                    var refs = XenAPI.VM.get_possible_hosts(session, vm);

                        var records = new List<XenAPI.Host>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
                }

            });
        }

        private void ProcessRecordSRsRequiredForRecovery(string vm)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenVMPropertySRsRequiredForRecoveryDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_get_SRs_required_for_recovery(session, vm, contxt.SessionTo);

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
                    var refs = XenAPI.VM.get_SRs_required_for_recovery(session, vm, contxt.SessionTo);

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

    public enum XenVMProperty
    {
        Uuid,
        AllowedOperations,
        CurrentOperations,
        PowerState,
        NameLabel,
        NameDescription,
        UserVersion,
        IsATemplate,
        IsDefaultTemplate,
        SuspendVDI,
        ResidentOn,
        Affinity,
        MemoryOverhead,
        MemoryTarget,
        MemoryStaticMax,
        MemoryDynamicMax,
        MemoryDynamicMin,
        MemoryStaticMin,
        VCPUsParams,
        VCPUsMax,
        VCPUsAtStartup,
        ActionsAfterShutdown,
        ActionsAfterReboot,
        ActionsAfterCrash,
        Consoles,
        VIFs,
        VBDs,
        VUSBs,
        CrashDumps,
        VTPMs,
        PVBootloader,
        PVKernel,
        PVRamdisk,
        PVArgs,
        PVBootloaderArgs,
        PVLegacyArgs,
        HVMBootPolicy,
        HVMBootParams,
        HVMShadowMultiplier,
        Platform,
        PCIBus,
        OtherConfig,
        Domid,
        Domarch,
        LastBootCPUFlags,
        IsControlDomain,
        Metrics,
        GuestMetrics,
        LastBootedRecord,
        Recommendations,
        XenstoreData,
        HaAlwaysRun,
        HaRestartPriority,
        IsASnapshot,
        SnapshotOf,
        Snapshots,
        SnapshotTime,
        TransportableSnapshotId,
        Blobs,
        Tags,
        BlockedOperations,
        SnapshotInfo,
        SnapshotMetadata,
        Parent,
        Children,
        BiosStrings,
        ProtectionPolicy,
        IsSnapshotFromVmpp,
        SnapshotSchedule,
        IsVmssSnapshot,
        Appliance,
        StartDelay,
        ShutdownDelay,
        Order,
        VGPUs,
        AttachedPCIs,
        SuspendSR,
        Version,
        GenerationId,
        HardwarePlatformVersion,
        HasVendorDevice,
        RequiresReboot,
        ReferenceLabel,
        Cooperative,
        BootRecord,
        DataSources,
        AllowedVBDDevices,
        AllowedVIFDevices,
        PossibleHosts,
        SRsRequiredForRecovery
    }

    public class XenVMPropertyCooperativeDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMPropertyPossibleHostsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMPropertySRsRequiredForRecoveryDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }
 
    }

}
