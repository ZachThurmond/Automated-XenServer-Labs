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
    [Cmdlet(VerbsCommon.New, "XenVM", DefaultParameterSetName = "Hashtable", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VM))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class NewXenVMCommand : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "Hashtable", Mandatory = true)]
        public Hashtable HashTable { get; set; }

        [Parameter(ParameterSetName = "Record", Mandatory = true)]
        public XenAPI.VM Record { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string NameLabel { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string NameDescription { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long UserVersion { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool IsATemplate { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.Host> Affinity { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long MemoryTarget { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long MemoryStaticMax { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long MemoryDynamicMax { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long MemoryDynamicMin { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long MemoryStaticMin { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable VCPUsParams { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long VCPUsMax { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long VCPUsAtStartup { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public on_normal_exit ActionsAfterShutdown { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public on_normal_exit ActionsAfterReboot { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public on_crash_behaviour ActionsAfterCrash { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVBootloader { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVKernel { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVRamdisk { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVArgs { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVBootloaderArgs { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PVLegacyArgs { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string HVMBootPolicy { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable HVMBootParams { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public double HVMShadowMultiplier { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable Platform { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string PCIBus { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable OtherConfig { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string Recommendations { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable XenstoreData { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool HaAlwaysRun { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string HaRestartPriority { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string[] Tags { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable BlockedOperations { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.VMPP> ProtectionPolicy { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool IsSnapshotFromVmpp { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.VMSS> SnapshotSchedule { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool IsVmssSnapshot { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.VM_appliance> Appliance { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long StartDelay { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long ShutdownDelay { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long Order { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.SR> SuspendSR { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long Version { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string GenerationId { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long HardwarePlatformVersion { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool HasVendorDevice { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string ReferenceLabel { get; set; }

        protected override bool GenerateAsyncParam
        {
            get { return true; }
        }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();
            if (Record == null && HashTable == null)
            {
                Record = new XenAPI.VM();
                Record.name_label = NameLabel;
                Record.name_description = NameDescription;
                Record.user_version = UserVersion;
                Record.is_a_template = IsATemplate;
                Record.affinity = Affinity == null ? null : new XenRef<XenAPI.Host>(Affinity.opaque_ref);
                Record.memory_target = MemoryTarget;
                Record.memory_static_max = MemoryStaticMax;
                Record.memory_dynamic_max = MemoryDynamicMax;
                Record.memory_dynamic_min = MemoryDynamicMin;
                Record.memory_static_min = MemoryStaticMin;
                Record.VCPUs_params = CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(VCPUsParams);
                Record.VCPUs_max = VCPUsMax;
                Record.VCPUs_at_startup = VCPUsAtStartup;
                Record.actions_after_shutdown = ActionsAfterShutdown;
                Record.actions_after_reboot = ActionsAfterReboot;
                Record.actions_after_crash = ActionsAfterCrash;
                Record.PV_bootloader = PVBootloader;
                Record.PV_kernel = PVKernel;
                Record.PV_ramdisk = PVRamdisk;
                Record.PV_args = PVArgs;
                Record.PV_bootloader_args = PVBootloaderArgs;
                Record.PV_legacy_args = PVLegacyArgs;
                Record.HVM_boot_policy = HVMBootPolicy;
                Record.HVM_boot_params = CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(HVMBootParams);
                Record.HVM_shadow_multiplier = HVMShadowMultiplier;
                Record.platform = CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(Platform);
                Record.PCI_bus = PCIBus;
                Record.other_config = CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig);
                Record.recommendations = Recommendations;
                Record.xenstore_data = CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(XenstoreData);
                Record.ha_always_run = HaAlwaysRun;
                Record.ha_restart_priority = HaRestartPriority;
                Record.tags = Tags;
                Record.blocked_operations = CommonCmdletFunctions.ConvertHashTableToDictionary<vm_operations, string>(BlockedOperations);
                Record.protection_policy = ProtectionPolicy == null ? null : new XenRef<XenAPI.VMPP>(ProtectionPolicy.opaque_ref);
                Record.is_snapshot_from_vmpp = IsSnapshotFromVmpp;
                Record.snapshot_schedule = SnapshotSchedule == null ? null : new XenRef<XenAPI.VMSS>(SnapshotSchedule.opaque_ref);
                Record.is_vmss_snapshot = IsVmssSnapshot;
                Record.appliance = Appliance == null ? null : new XenRef<XenAPI.VM_appliance>(Appliance.opaque_ref);
                Record.start_delay = StartDelay;
                Record.shutdown_delay = ShutdownDelay;
                Record.order = Order;
                Record.suspend_SR = SuspendSR == null ? null : new XenRef<XenAPI.SR>(SuspendSR.opaque_ref);
                Record.version = Version;
                Record.generation_id = GenerationId;
                Record.hardware_platform_version = HardwarePlatformVersion;
                Record.has_vendor_device = HasVendorDevice;
                Record.reference_label = ReferenceLabel;
            }
            else if (Record == null)
            {
                Record = new XenAPI.VM(HashTable);
            }

            if (!ShouldProcess(session.Url, "VM.create"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_create(session, Record);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    string objRef = XenAPI.VM.create(session, Record);

                    if (PassThru)
                    {
                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });

            UpdateSessions();
        }

        #endregion
   }
}
