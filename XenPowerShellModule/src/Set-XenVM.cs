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
    [Cmdlet(VerbsCommon.Set, "XenVM", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VM))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenVM : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM VM { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _vCPUsNumberLiveIsSpecified
                       ^ _memoryDynamicRangeIsSpecified
                       ^ _memoryStaticRangeIsSpecified
                       ^ _memoryLimitsIsSpecified
                       ^ _memoryIsSpecified
                       ^ _memoryTargetLiveIsSpecified
                       ^ _shadowMultiplierLiveIsSpecified
                       ^ _biosStringsIsSpecified
                       ^ _startDelayIsSpecified
                       ^ _shutdownDelayIsSpecified
                       ^ _orderIsSpecified
                       ^ _suspendVDIIsSpecified
                       ^ _applianceIsSpecified
                       ^ _hasVendorDeviceIsSpecified;
            }
        }

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
        public long UserVersion
        {
            get { return _userVersion; }
            set
            {
                _userVersion = value;
                _userVersionIsSpecified = true;
            }
        }
        private long _userVersion;
        private bool _userVersionIsSpecified;

        [Parameter]
        public bool IsATemplate
        {
            get { return _isATemplate; }
            set
            {
                _isATemplate = value;
                _isATemplateIsSpecified = true;
            }
        }
        private bool _isATemplate;
        private bool _isATemplateIsSpecified;

        [Parameter]
        public XenRef<XenAPI.Host> Affinity
        {
            get { return _affinity; }
            set
            {
                _affinity = value;
                _affinityIsSpecified = true;
            }
        }
        private XenRef<XenAPI.Host> _affinity;
        private bool _affinityIsSpecified;

        [Parameter]
        public Hashtable VCPUsParams
        {
            get { return _vCPUsParams; }
            set
            {
                _vCPUsParams = value;
                _vCPUsParamsIsSpecified = true;
            }
        }
        private Hashtable _vCPUsParams;
        private bool _vCPUsParamsIsSpecified;

        [Parameter]
        public on_normal_exit ActionsAfterShutdown
        {
            get { return _actionsAfterShutdown; }
            set
            {
                _actionsAfterShutdown = value;
                _actionsAfterShutdownIsSpecified = true;
            }
        }
        private on_normal_exit _actionsAfterShutdown;
        private bool _actionsAfterShutdownIsSpecified;

        [Parameter]
        public on_normal_exit ActionsAfterReboot
        {
            get { return _actionsAfterReboot; }
            set
            {
                _actionsAfterReboot = value;
                _actionsAfterRebootIsSpecified = true;
            }
        }
        private on_normal_exit _actionsAfterReboot;
        private bool _actionsAfterRebootIsSpecified;

        [Parameter]
        public on_crash_behaviour ActionsAfterCrash
        {
            get { return _actionsAfterCrash; }
            set
            {
                _actionsAfterCrash = value;
                _actionsAfterCrashIsSpecified = true;
            }
        }
        private on_crash_behaviour _actionsAfterCrash;
        private bool _actionsAfterCrashIsSpecified;

        [Parameter]
        public string PVBootloader
        {
            get { return _pVBootloader; }
            set
            {
                _pVBootloader = value;
                _pVBootloaderIsSpecified = true;
            }
        }
        private string _pVBootloader;
        private bool _pVBootloaderIsSpecified;

        [Parameter]
        public string PVKernel
        {
            get { return _pVKernel; }
            set
            {
                _pVKernel = value;
                _pVKernelIsSpecified = true;
            }
        }
        private string _pVKernel;
        private bool _pVKernelIsSpecified;

        [Parameter]
        public string PVRamdisk
        {
            get { return _pVRamdisk; }
            set
            {
                _pVRamdisk = value;
                _pVRamdiskIsSpecified = true;
            }
        }
        private string _pVRamdisk;
        private bool _pVRamdiskIsSpecified;

        [Parameter]
        public string PVArgs
        {
            get { return _pVArgs; }
            set
            {
                _pVArgs = value;
                _pVArgsIsSpecified = true;
            }
        }
        private string _pVArgs;
        private bool _pVArgsIsSpecified;

        [Parameter]
        public string PVBootloaderArgs
        {
            get { return _pVBootloaderArgs; }
            set
            {
                _pVBootloaderArgs = value;
                _pVBootloaderArgsIsSpecified = true;
            }
        }
        private string _pVBootloaderArgs;
        private bool _pVBootloaderArgsIsSpecified;

        [Parameter]
        public string PVLegacyArgs
        {
            get { return _pVLegacyArgs; }
            set
            {
                _pVLegacyArgs = value;
                _pVLegacyArgsIsSpecified = true;
            }
        }
        private string _pVLegacyArgs;
        private bool _pVLegacyArgsIsSpecified;

        [Parameter]
        public string HVMBootPolicy
        {
            get { return _hVMBootPolicy; }
            set
            {
                _hVMBootPolicy = value;
                _hVMBootPolicyIsSpecified = true;
            }
        }
        private string _hVMBootPolicy;
        private bool _hVMBootPolicyIsSpecified;

        [Parameter]
        public Hashtable HVMBootParams
        {
            get { return _hVMBootParams; }
            set
            {
                _hVMBootParams = value;
                _hVMBootParamsIsSpecified = true;
            }
        }
        private Hashtable _hVMBootParams;
        private bool _hVMBootParamsIsSpecified;

        [Parameter]
        public Hashtable Platform
        {
            get { return _platform; }
            set
            {
                _platform = value;
                _platformIsSpecified = true;
            }
        }
        private Hashtable _platform;
        private bool _platformIsSpecified;

        [Parameter]
        public string PCIBus
        {
            get { return _pCIBus; }
            set
            {
                _pCIBus = value;
                _pCIBusIsSpecified = true;
            }
        }
        private string _pCIBus;
        private bool _pCIBusIsSpecified;

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
        public string Recommendations
        {
            get { return _recommendations; }
            set
            {
                _recommendations = value;
                _recommendationsIsSpecified = true;
            }
        }
        private string _recommendations;
        private bool _recommendationsIsSpecified;

        [Parameter]
        public Hashtable XenstoreData
        {
            get { return _xenstoreData; }
            set
            {
                _xenstoreData = value;
                _xenstoreDataIsSpecified = true;
            }
        }
        private Hashtable _xenstoreData;
        private bool _xenstoreDataIsSpecified;

        [Parameter]
        public string[] Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                _tagsIsSpecified = true;
            }
        }
        private string[] _tags;
        private bool _tagsIsSpecified;

        [Parameter]
        public Hashtable BlockedOperations
        {
            get { return _blockedOperations; }
            set
            {
                _blockedOperations = value;
                _blockedOperationsIsSpecified = true;
            }
        }
        private Hashtable _blockedOperations;
        private bool _blockedOperationsIsSpecified;

        [Parameter]
        public XenRef<XenAPI.SR> SuspendSR
        {
            get { return _suspendSR; }
            set
            {
                _suspendSR = value;
                _suspendSRIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _suspendSR;
        private bool _suspendSRIsSpecified;

        [Parameter]
        public long HardwarePlatformVersion
        {
            get { return _hardwarePlatformVersion; }
            set
            {
                _hardwarePlatformVersion = value;
                _hardwarePlatformVersionIsSpecified = true;
            }
        }
        private long _hardwarePlatformVersion;
        private bool _hardwarePlatformVersionIsSpecified;

        [Parameter]
        public long VCPUsNumberLive
        {
            get { return _vCPUsNumberLive; }
            set
            {
                _vCPUsNumberLive = value;
                _vCPUsNumberLiveIsSpecified = true;
            }
        }
        private long _vCPUsNumberLive;
        private bool _vCPUsNumberLiveIsSpecified;

        [Parameter]
        public string HaRestartPriority
        {
            get { return _haRestartPriority; }
            set
            {
                _haRestartPriority = value;
                _haRestartPriorityIsSpecified = true;
            }
        }
        private string _haRestartPriority;
        private bool _haRestartPriorityIsSpecified;

        [Parameter]
        public bool HaAlwaysRun
        {
            get { return _haAlwaysRun; }
            set
            {
                _haAlwaysRun = value;
                _haAlwaysRunIsSpecified = true;
            }
        }
        private bool _haAlwaysRun;
        private bool _haAlwaysRunIsSpecified;

        [Parameter]
        public long MemoryDynamicMax
        {
            get { return _memoryDynamicMax; }
            set
            {
                _memoryDynamicMax = value;
                _memoryDynamicMaxIsSpecified = true;
            }
        }
        private long _memoryDynamicMax;
        private bool _memoryDynamicMaxIsSpecified;

        [Parameter]
        public long MemoryDynamicMin
        {
            get { return _memoryDynamicMin; }
            set
            {
                _memoryDynamicMin = value;
                _memoryDynamicMinIsSpecified = true;
            }
        }
        private long _memoryDynamicMin;
        private bool _memoryDynamicMinIsSpecified;

        [Parameter]
        public long[] MemoryDynamicRange
        {
            get { return _memoryDynamicRange; }
            set
            {
                _memoryDynamicRange = value;
                _memoryDynamicRangeIsSpecified = true;
            }
        }
        private long[] _memoryDynamicRange;
        private bool _memoryDynamicRangeIsSpecified;

        [Parameter]
        public long MemoryStaticMax
        {
            get { return _memoryStaticMax; }
            set
            {
                _memoryStaticMax = value;
                _memoryStaticMaxIsSpecified = true;
            }
        }
        private long _memoryStaticMax;
        private bool _memoryStaticMaxIsSpecified;

        [Parameter]
        public long MemoryStaticMin
        {
            get { return _memoryStaticMin; }
            set
            {
                _memoryStaticMin = value;
                _memoryStaticMinIsSpecified = true;
            }
        }
        private long _memoryStaticMin;
        private bool _memoryStaticMinIsSpecified;

        [Parameter]
        public long[] MemoryStaticRange
        {
            get { return _memoryStaticRange; }
            set
            {
                _memoryStaticRange = value;
                _memoryStaticRangeIsSpecified = true;
            }
        }
        private long[] _memoryStaticRange;
        private bool _memoryStaticRangeIsSpecified;

        [Parameter]
        public long[] MemoryLimits
        {
            get { return _memoryLimits; }
            set
            {
                _memoryLimits = value;
                _memoryLimitsIsSpecified = true;
            }
        }
        private long[] _memoryLimits;
        private bool _memoryLimitsIsSpecified;

        [Parameter]
        public long Memory
        {
            get { return _memory; }
            set
            {
                _memory = value;
                _memoryIsSpecified = true;
            }
        }
        private long _memory;
        private bool _memoryIsSpecified;

        [Parameter]
        public long MemoryTargetLive
        {
            get { return _memoryTargetLive; }
            set
            {
                _memoryTargetLive = value;
                _memoryTargetLiveIsSpecified = true;
            }
        }
        private long _memoryTargetLive;
        private bool _memoryTargetLiveIsSpecified;

        [Parameter]
        public double HVMShadowMultiplier
        {
            get { return _hVMShadowMultiplier; }
            set
            {
                _hVMShadowMultiplier = value;
                _hVMShadowMultiplierIsSpecified = true;
            }
        }
        private double _hVMShadowMultiplier;
        private bool _hVMShadowMultiplierIsSpecified;

        [Parameter]
        public double ShadowMultiplierLive
        {
            get { return _shadowMultiplierLive; }
            set
            {
                _shadowMultiplierLive = value;
                _shadowMultiplierLiveIsSpecified = true;
            }
        }
        private double _shadowMultiplierLive;
        private bool _shadowMultiplierLiveIsSpecified;

        [Parameter]
        public long VCPUsMax
        {
            get { return _vCPUsMax; }
            set
            {
                _vCPUsMax = value;
                _vCPUsMaxIsSpecified = true;
            }
        }
        private long _vCPUsMax;
        private bool _vCPUsMaxIsSpecified;

        [Parameter]
        public long VCPUsAtStartup
        {
            get { return _vCPUsAtStartup; }
            set
            {
                _vCPUsAtStartup = value;
                _vCPUsAtStartupIsSpecified = true;
            }
        }
        private long _vCPUsAtStartup;
        private bool _vCPUsAtStartupIsSpecified;

        [Parameter]
        public Hashtable BiosStrings
        {
            get { return _biosStrings; }
            set
            {
                _biosStrings = value;
                _biosStringsIsSpecified = true;
            }
        }
        private Hashtable _biosStrings;
        private bool _biosStringsIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VMPP> ProtectionPolicy
        {
            get { return _protectionPolicy; }
            set
            {
                _protectionPolicy = value;
                _protectionPolicyIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VMPP> _protectionPolicy;
        private bool _protectionPolicyIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VMSS> SnapshotSchedule
        {
            get { return _snapshotSchedule; }
            set
            {
                _snapshotSchedule = value;
                _snapshotScheduleIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VMSS> _snapshotSchedule;
        private bool _snapshotScheduleIsSpecified;

        [Parameter]
        public long StartDelay
        {
            get { return _startDelay; }
            set
            {
                _startDelay = value;
                _startDelayIsSpecified = true;
            }
        }
        private long _startDelay;
        private bool _startDelayIsSpecified;

        [Parameter]
        public long ShutdownDelay
        {
            get { return _shutdownDelay; }
            set
            {
                _shutdownDelay = value;
                _shutdownDelayIsSpecified = true;
            }
        }
        private long _shutdownDelay;
        private bool _shutdownDelayIsSpecified;

        [Parameter]
        public long Order
        {
            get { return _order; }
            set
            {
                _order = value;
                _orderIsSpecified = true;
            }
        }
        private long _order;
        private bool _orderIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VDI> SuspendVDI
        {
            get { return _suspendVDI; }
            set
            {
                _suspendVDI = value;
                _suspendVDIIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VDI> _suspendVDI;
        private bool _suspendVDIIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VM_appliance> Appliance
        {
            get { return _appliance; }
            set
            {
                _appliance = value;
                _applianceIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VM_appliance> _appliance;
        private bool _applianceIsSpecified;

        [Parameter]
        public bool HasVendorDevice
        {
            get { return _hasVendorDevice; }
            set
            {
                _hasVendorDevice = value;
                _hasVendorDeviceIsSpecified = true;
            }
        }
        private bool _hasVendorDevice;
        private bool _hasVendorDeviceIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm = ParseVM();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(vm);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(vm);
            if (_userVersionIsSpecified)
                ProcessRecordUserVersion(vm);
            if (_isATemplateIsSpecified)
                ProcessRecordIsATemplate(vm);
            if (_affinityIsSpecified)
                ProcessRecordAffinity(vm);
            if (_vCPUsParamsIsSpecified)
                ProcessRecordVCPUsParams(vm);
            if (_actionsAfterShutdownIsSpecified)
                ProcessRecordActionsAfterShutdown(vm);
            if (_actionsAfterRebootIsSpecified)
                ProcessRecordActionsAfterReboot(vm);
            if (_actionsAfterCrashIsSpecified)
                ProcessRecordActionsAfterCrash(vm);
            if (_pVBootloaderIsSpecified)
                ProcessRecordPVBootloader(vm);
            if (_pVKernelIsSpecified)
                ProcessRecordPVKernel(vm);
            if (_pVRamdiskIsSpecified)
                ProcessRecordPVRamdisk(vm);
            if (_pVArgsIsSpecified)
                ProcessRecordPVArgs(vm);
            if (_pVBootloaderArgsIsSpecified)
                ProcessRecordPVBootloaderArgs(vm);
            if (_pVLegacyArgsIsSpecified)
                ProcessRecordPVLegacyArgs(vm);
            if (_hVMBootPolicyIsSpecified)
                ProcessRecordHVMBootPolicy(vm);
            if (_hVMBootParamsIsSpecified)
                ProcessRecordHVMBootParams(vm);
            if (_platformIsSpecified)
                ProcessRecordPlatform(vm);
            if (_pCIBusIsSpecified)
                ProcessRecordPCIBus(vm);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(vm);
            if (_recommendationsIsSpecified)
                ProcessRecordRecommendations(vm);
            if (_xenstoreDataIsSpecified)
                ProcessRecordXenstoreData(vm);
            if (_tagsIsSpecified)
                ProcessRecordTags(vm);
            if (_blockedOperationsIsSpecified)
                ProcessRecordBlockedOperations(vm);
            if (_suspendSRIsSpecified)
                ProcessRecordSuspendSR(vm);
            if (_hardwarePlatformVersionIsSpecified)
                ProcessRecordHardwarePlatformVersion(vm);
            if (_vCPUsNumberLiveIsSpecified)
                ProcessRecordVCPUsNumberLive(vm);
            if (_haRestartPriorityIsSpecified)
                ProcessRecordHaRestartPriority(vm);
            if (_haAlwaysRunIsSpecified)
                ProcessRecordHaAlwaysRun(vm);
            if (_memoryDynamicMaxIsSpecified)
                ProcessRecordMemoryDynamicMax(vm);
            if (_memoryDynamicMinIsSpecified)
                ProcessRecordMemoryDynamicMin(vm);
            if (_memoryDynamicRangeIsSpecified)
                ProcessRecordMemoryDynamicRange(vm);
            if (_memoryStaticMaxIsSpecified)
                ProcessRecordMemoryStaticMax(vm);
            if (_memoryStaticMinIsSpecified)
                ProcessRecordMemoryStaticMin(vm);
            if (_memoryStaticRangeIsSpecified)
                ProcessRecordMemoryStaticRange(vm);
            if (_memoryLimitsIsSpecified)
                ProcessRecordMemoryLimits(vm);
            if (_memoryIsSpecified)
                ProcessRecordMemory(vm);
            if (_memoryTargetLiveIsSpecified)
                ProcessRecordMemoryTargetLive(vm);
            if (_hVMShadowMultiplierIsSpecified)
                ProcessRecordHVMShadowMultiplier(vm);
            if (_shadowMultiplierLiveIsSpecified)
                ProcessRecordShadowMultiplierLive(vm);
            if (_vCPUsMaxIsSpecified)
                ProcessRecordVCPUsMax(vm);
            if (_vCPUsAtStartupIsSpecified)
                ProcessRecordVCPUsAtStartup(vm);
            if (_biosStringsIsSpecified)
                ProcessRecordBiosStrings(vm);
            if (_protectionPolicyIsSpecified)
                ProcessRecordProtectionPolicy(vm);
            if (_snapshotScheduleIsSpecified)
                ProcessRecordSnapshotSchedule(vm);
            if (_startDelayIsSpecified)
                ProcessRecordStartDelay(vm);
            if (_shutdownDelayIsSpecified)
                ProcessRecordShutdownDelay(vm);
            if (_orderIsSpecified)
                ProcessRecordOrder(vm);
            if (_suspendVDIIsSpecified)
                ProcessRecordSuspendVDI(vm);
            if (_applianceIsSpecified)
                ProcessRecordAppliance(vm);
            if (_hasVendorDeviceIsSpecified)
                ProcessRecordHasVendorDevice(vm);

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

                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVM()
        {
            string vm = null;

            if (VM != null)
                vm = (new XenRef<XenAPI.VM>(VM)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VM.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vm = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VM.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vm = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VM with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
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

        private void ProcessRecordNameLabel(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_name_label(session, vm, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_name_description(session, vm, NameDescription);

            });
        }

        private void ProcessRecordUserVersion(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_user_version"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_user_version(session, vm, UserVersion);

            });
        }

        private void ProcessRecordIsATemplate(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_is_a_template"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_is_a_template(session, vm, IsATemplate);

            });
        }

        private void ProcessRecordAffinity(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_affinity"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_affinity(session, vm, Affinity);

            });
        }

        private void ProcessRecordVCPUsParams(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_VCPUs_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_VCPUs_params(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(VCPUsParams));

            });
        }

        private void ProcessRecordActionsAfterShutdown(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_actions_after_shutdown"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_actions_after_shutdown(session, vm, ActionsAfterShutdown);

            });
        }

        private void ProcessRecordActionsAfterReboot(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_actions_after_reboot"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_actions_after_reboot(session, vm, ActionsAfterReboot);

            });
        }

        private void ProcessRecordActionsAfterCrash(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_actions_after_crash"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_actions_after_crash(session, vm, ActionsAfterCrash);

            });
        }

        private void ProcessRecordPVBootloader(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_bootloader"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_bootloader(session, vm, PVBootloader);

            });
        }

        private void ProcessRecordPVKernel(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_kernel"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_kernel(session, vm, PVKernel);

            });
        }

        private void ProcessRecordPVRamdisk(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_ramdisk"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_ramdisk(session, vm, PVRamdisk);

            });
        }

        private void ProcessRecordPVArgs(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_args"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_args(session, vm, PVArgs);

            });
        }

        private void ProcessRecordPVBootloaderArgs(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_bootloader_args"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_bootloader_args(session, vm, PVBootloaderArgs);

            });
        }

        private void ProcessRecordPVLegacyArgs(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PV_legacy_args"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PV_legacy_args(session, vm, PVLegacyArgs);

            });
        }

        private void ProcessRecordHVMBootPolicy(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_HVM_boot_policy"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_HVM_boot_policy(session, vm, HVMBootPolicy);

            });
        }

        private void ProcessRecordHVMBootParams(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_HVM_boot_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_HVM_boot_params(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(HVMBootParams));

            });
        }

        private void ProcessRecordPlatform(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_platform"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_platform(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(Platform));

            });
        }

        private void ProcessRecordPCIBus(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_PCI_bus"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_PCI_bus(session, vm, PCIBus);

            });
        }

        private void ProcessRecordOtherConfig(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_other_config(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordRecommendations(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_recommendations"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_recommendations(session, vm, Recommendations);

            });
        }

        private void ProcessRecordXenstoreData(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_xenstore_data"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_xenstore_data(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(XenstoreData));

            });
        }

        private void ProcessRecordTags(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_tags(session, vm, Tags);

            });
        }

        private void ProcessRecordBlockedOperations(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_blocked_operations"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_blocked_operations(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<vm_operations, string>(BlockedOperations));

            });
        }

        private void ProcessRecordSuspendSR(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_suspend_SR"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_suspend_SR(session, vm, SuspendSR);

            });
        }

        private void ProcessRecordHardwarePlatformVersion(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_hardware_platform_version"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_hardware_platform_version(session, vm, HardwarePlatformVersion);

            });
        }

        private void ProcessRecordVCPUsNumberLive(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_VCPUs_number_live"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_VCPUs_number_live(session, vm, VCPUsNumberLive);

                }
                else
                {
                    XenAPI.VM.set_VCPUs_number_live(session, vm, VCPUsNumberLive);

                }

            });
        }

        private void ProcessRecordHaRestartPriority(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_ha_restart_priority"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_ha_restart_priority(session, vm, HaRestartPriority);

            });
        }

        private void ProcessRecordHaAlwaysRun(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_ha_always_run"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_ha_always_run(session, vm, HaAlwaysRun);

            });
        }

        private void ProcessRecordMemoryDynamicMax(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_dynamic_max"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_memory_dynamic_max(session, vm, MemoryDynamicMax);

            });
        }

        private void ProcessRecordMemoryDynamicMin(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_dynamic_min"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_memory_dynamic_min(session, vm, MemoryDynamicMin);

            });
        }

        private void ProcessRecordMemoryDynamicRange(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_dynamic_range"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_memory_dynamic_range(session, vm, MemoryDynamicRange[0], MemoryDynamicRange[1]);

                }
                else
                {
                    XenAPI.VM.set_memory_dynamic_range(session, vm, MemoryDynamicRange[0], MemoryDynamicRange[1]);

                }

            });
        }

        private void ProcessRecordMemoryStaticMax(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_static_max"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_memory_static_max(session, vm, MemoryStaticMax);

            });
        }

        private void ProcessRecordMemoryStaticMin(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_static_min"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_memory_static_min(session, vm, MemoryStaticMin);

            });
        }

        private void ProcessRecordMemoryStaticRange(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_static_range"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_memory_static_range(session, vm, MemoryStaticRange[0], MemoryStaticRange[1]);

                }
                else
                {
                    XenAPI.VM.set_memory_static_range(session, vm, MemoryStaticRange[0], MemoryStaticRange[1]);

                }

            });
        }

        private void ProcessRecordMemoryLimits(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_limits"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_memory_limits(session, vm, MemoryLimits[0], MemoryLimits[1], MemoryLimits[2], MemoryLimits[3]);

                }
                else
                {
                    XenAPI.VM.set_memory_limits(session, vm, MemoryLimits[0], MemoryLimits[1], MemoryLimits[2], MemoryLimits[3]);

                }

            });
        }

        private void ProcessRecordMemory(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_memory(session, vm, Memory);

                }
                else
                {
                    XenAPI.VM.set_memory(session, vm, Memory);

                }

            });
        }

        private void ProcessRecordMemoryTargetLive(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_memory_target_live"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_memory_target_live(session, vm, MemoryTargetLive);

                }
                else
                {
                    XenAPI.VM.set_memory_target_live(session, vm, MemoryTargetLive);

                }

            });
        }

        private void ProcessRecordHVMShadowMultiplier(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_HVM_shadow_multiplier"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_HVM_shadow_multiplier(session, vm, HVMShadowMultiplier);

            });
        }

        private void ProcessRecordShadowMultiplierLive(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_shadow_multiplier_live"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_shadow_multiplier_live(session, vm, ShadowMultiplierLive);

                }
                else
                {
                    XenAPI.VM.set_shadow_multiplier_live(session, vm, ShadowMultiplierLive);

                }

            });
        }

        private void ProcessRecordVCPUsMax(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_VCPUs_max"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_VCPUs_max(session, vm, VCPUsMax);

            });
        }

        private void ProcessRecordVCPUsAtStartup(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_VCPUs_at_startup"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_VCPUs_at_startup(session, vm, VCPUsAtStartup);

            });
        }

        private void ProcessRecordBiosStrings(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_bios_strings"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_bios_strings(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(BiosStrings));

                }
                else
                {
                    XenAPI.VM.set_bios_strings(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(BiosStrings));

                }

            });
        }

        private void ProcessRecordProtectionPolicy(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_protection_policy"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_protection_policy(session, vm, ProtectionPolicy);

            });
        }

        private void ProcessRecordSnapshotSchedule(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_snapshot_schedule"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VM.set_snapshot_schedule(session, vm, SnapshotSchedule);

            });
        }

        private void ProcessRecordStartDelay(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_start_delay"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_start_delay(session, vm, StartDelay);

                }
                else
                {
                    XenAPI.VM.set_start_delay(session, vm, StartDelay);

                }

            });
        }

        private void ProcessRecordShutdownDelay(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_shutdown_delay"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_shutdown_delay(session, vm, ShutdownDelay);

                }
                else
                {
                    XenAPI.VM.set_shutdown_delay(session, vm, ShutdownDelay);

                }

            });
        }

        private void ProcessRecordOrder(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_order"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_order(session, vm, Order);

                }
                else
                {
                    XenAPI.VM.set_order(session, vm, Order);

                }

            });
        }

        private void ProcessRecordSuspendVDI(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_suspend_VDI"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_suspend_VDI(session, vm, SuspendVDI);

                }
                else
                {
                    XenAPI.VM.set_suspend_VDI(session, vm, SuspendVDI);

                }

            });
        }

        private void ProcessRecordAppliance(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_appliance"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_appliance(session, vm, Appliance);

                }
                else
                {
                    XenAPI.VM.set_appliance(session, vm, Appliance);

                }

            });
        }

        private void ProcessRecordHasVendorDevice(string vm)
        {
            if (!ShouldProcess(vm, "VM.set_has_vendor_device"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_set_has_vendor_device(session, vm, HasVendorDevice);

                }
                else
                {
                    XenAPI.VM.set_has_vendor_device(session, vm, HasVendorDevice);

                }

            });
        }

        #endregion
    }
}
