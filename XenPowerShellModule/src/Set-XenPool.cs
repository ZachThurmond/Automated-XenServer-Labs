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
    [Cmdlet(VerbsCommon.Set, "XenPool", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Pool))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenPool : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool Pool { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _haHostFailuresToTolerateIsSpecified
                       ^ _vswitchControllerIsSpecified
                       ^ _igmpSnoopingEnabledIsSpecified;
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
        public XenRef<XenAPI.SR> DefaultSR
        {
            get { return _defaultSR; }
            set
            {
                _defaultSR = value;
                _defaultSRIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _defaultSR;
        private bool _defaultSRIsSpecified;

        [Parameter]
        public XenRef<XenAPI.SR> SuspendImageSR
        {
            get { return _suspendImageSR; }
            set
            {
                _suspendImageSR = value;
                _suspendImageSRIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _suspendImageSR;
        private bool _suspendImageSRIsSpecified;

        [Parameter]
        public XenRef<XenAPI.SR> CrashDumpSR
        {
            get { return _crashDumpSR; }
            set
            {
                _crashDumpSR = value;
                _crashDumpSRIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _crashDumpSR;
        private bool _crashDumpSRIsSpecified;

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
        public bool HaAllowOvercommit
        {
            get { return _haAllowOvercommit; }
            set
            {
                _haAllowOvercommit = value;
                _haAllowOvercommitIsSpecified = true;
            }
        }
        private bool _haAllowOvercommit;
        private bool _haAllowOvercommitIsSpecified;

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
        public Hashtable GuiConfig
        {
            get { return _guiConfig; }
            set
            {
                _guiConfig = value;
                _guiConfigIsSpecified = true;
            }
        }
        private Hashtable _guiConfig;
        private bool _guiConfigIsSpecified;

        [Parameter]
        public Hashtable HealthCheckConfig
        {
            get { return _healthCheckConfig; }
            set
            {
                _healthCheckConfig = value;
                _healthCheckConfigIsSpecified = true;
            }
        }
        private Hashtable _healthCheckConfig;
        private bool _healthCheckConfigIsSpecified;

        [Parameter]
        public bool WlbEnabled
        {
            get { return _wlbEnabled; }
            set
            {
                _wlbEnabled = value;
                _wlbEnabledIsSpecified = true;
            }
        }
        private bool _wlbEnabled;
        private bool _wlbEnabledIsSpecified;

        [Parameter]
        public bool WlbVerifyCert
        {
            get { return _wlbVerifyCert; }
            set
            {
                _wlbVerifyCert = value;
                _wlbVerifyCertIsSpecified = true;
            }
        }
        private bool _wlbVerifyCert;
        private bool _wlbVerifyCertIsSpecified;

        [Parameter]
        public bool PolicyNoVendorDevice
        {
            get { return _policyNoVendorDevice; }
            set
            {
                _policyNoVendorDevice = value;
                _policyNoVendorDeviceIsSpecified = true;
            }
        }
        private bool _policyNoVendorDevice;
        private bool _policyNoVendorDeviceIsSpecified;

        [Parameter]
        public bool LivePatchingDisabled
        {
            get { return _livePatchingDisabled; }
            set
            {
                _livePatchingDisabled = value;
                _livePatchingDisabledIsSpecified = true;
            }
        }
        private bool _livePatchingDisabled;
        private bool _livePatchingDisabledIsSpecified;

        [Parameter]
        public long HaHostFailuresToTolerate
        {
            get { return _haHostFailuresToTolerate; }
            set
            {
                _haHostFailuresToTolerate = value;
                _haHostFailuresToTolerateIsSpecified = true;
            }
        }
        private long _haHostFailuresToTolerate;
        private bool _haHostFailuresToTolerateIsSpecified;

        [Parameter]
        public string VswitchController
        {
            get { return _vswitchController; }
            set
            {
                _vswitchController = value;
                _vswitchControllerIsSpecified = true;
            }
        }
        private string _vswitchController;
        private bool _vswitchControllerIsSpecified;

        [Parameter]
        public bool IgmpSnoopingEnabled
        {
            get { return _igmpSnoopingEnabled; }
            set
            {
                _igmpSnoopingEnabled = value;
                _igmpSnoopingEnabledIsSpecified = true;
            }
        }
        private bool _igmpSnoopingEnabled;
        private bool _igmpSnoopingEnabledIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool = ParsePool();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(pool);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(pool);
            if (_defaultSRIsSpecified)
                ProcessRecordDefaultSR(pool);
            if (_suspendImageSRIsSpecified)
                ProcessRecordSuspendImageSR(pool);
            if (_crashDumpSRIsSpecified)
                ProcessRecordCrashDumpSR(pool);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(pool);
            if (_haAllowOvercommitIsSpecified)
                ProcessRecordHaAllowOvercommit(pool);
            if (_tagsIsSpecified)
                ProcessRecordTags(pool);
            if (_guiConfigIsSpecified)
                ProcessRecordGuiConfig(pool);
            if (_healthCheckConfigIsSpecified)
                ProcessRecordHealthCheckConfig(pool);
            if (_wlbEnabledIsSpecified)
                ProcessRecordWlbEnabled(pool);
            if (_wlbVerifyCertIsSpecified)
                ProcessRecordWlbVerifyCert(pool);
            if (_policyNoVendorDeviceIsSpecified)
                ProcessRecordPolicyNoVendorDevice(pool);
            if (_livePatchingDisabledIsSpecified)
                ProcessRecordLivePatchingDisabled(pool);
            if (_haHostFailuresToTolerateIsSpecified)
                ProcessRecordHaHostFailuresToTolerate(pool);
            if (_vswitchControllerIsSpecified)
                ProcessRecordVswitchController(pool);
            if (_igmpSnoopingEnabledIsSpecified)
                ProcessRecordIgmpSnoopingEnabled(pool);

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

                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePool()
        {
            string pool = null;

            if (Pool != null)
                pool = (new XenRef<XenAPI.Pool>(Pool)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Pool.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pool = xenRef.opaque_ref;
            }
            else if (Ref != null)
                pool = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Pool', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Pool));
            }

            return pool;
        }

        private void ProcessRecordNameLabel(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_name_label(session, pool, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_name_description(session, pool, NameDescription);

            });
        }

        private void ProcessRecordDefaultSR(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_default_SR"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_default_SR(session, pool, DefaultSR);

            });
        }

        private void ProcessRecordSuspendImageSR(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_suspend_image_SR"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_suspend_image_SR(session, pool, SuspendImageSR);

            });
        }

        private void ProcessRecordCrashDumpSR(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_crash_dump_SR"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_crash_dump_SR(session, pool, CrashDumpSR);

            });
        }

        private void ProcessRecordOtherConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_other_config(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordHaAllowOvercommit(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_ha_allow_overcommit"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_ha_allow_overcommit(session, pool, HaAllowOvercommit);

            });
        }

        private void ProcessRecordTags(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_tags(session, pool, Tags);

            });
        }

        private void ProcessRecordGuiConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_gui_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_gui_config(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(GuiConfig));

            });
        }

        private void ProcessRecordHealthCheckConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_health_check_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_health_check_config(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(HealthCheckConfig));

            });
        }

        private void ProcessRecordWlbEnabled(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_wlb_enabled"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_wlb_enabled(session, pool, WlbEnabled);

            });
        }

        private void ProcessRecordWlbVerifyCert(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_wlb_verify_cert"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_wlb_verify_cert(session, pool, WlbVerifyCert);

            });
        }

        private void ProcessRecordPolicyNoVendorDevice(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_policy_no_vendor_device"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_policy_no_vendor_device(session, pool, PolicyNoVendorDevice);

            });
        }

        private void ProcessRecordLivePatchingDisabled(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_live_patching_disabled"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.set_live_patching_disabled(session, pool, LivePatchingDisabled);

            });
        }

        private void ProcessRecordHaHostFailuresToTolerate(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_ha_host_failures_to_tolerate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_set_ha_host_failures_to_tolerate(session, pool, HaHostFailuresToTolerate);

                }
                else
                {
                    XenAPI.Pool.set_ha_host_failures_to_tolerate(session, pool, HaHostFailuresToTolerate);

                }

            });
        }

        private void ProcessRecordVswitchController(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_vswitch_controller"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_set_vswitch_controller(session, VswitchController);

                }
                else
                {
                    XenAPI.Pool.set_vswitch_controller(session, VswitchController);

                }

            });
        }

        private void ProcessRecordIgmpSnoopingEnabled(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.set_igmp_snooping_enabled"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_set_igmp_snooping_enabled(session, pool, IgmpSnoopingEnabled);

                }
                else
                {
                    XenAPI.Pool.set_igmp_snooping_enabled(session, pool, IgmpSnoopingEnabled);

                }

            });
        }

        #endregion
    }
}
