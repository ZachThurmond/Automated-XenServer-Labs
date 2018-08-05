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
    [Cmdlet(VerbsLifecycle.Invoke, "XenHost", SupportsShouldProcess = true)]
    public class InvokeXenHost : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host XenHost { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenHostAction.Disable:
                    _context = new XenHostActionDisableDynamicParameters();
                    return _context;
                case XenHostAction.Enable:
                    _context = new XenHostActionEnableDynamicParameters();
                    return _context;
                case XenHostAction.Shutdown:
                    _context = new XenHostActionShutdownDynamicParameters();
                    return _context;
                case XenHostAction.Reboot:
                    _context = new XenHostActionRebootDynamicParameters();
                    return _context;
                case XenHostAction.Dmesg:
                    _context = new XenHostActionDmesgDynamicParameters();
                    return _context;
                case XenHostAction.DmesgClear:
                    _context = new XenHostActionDmesgClearDynamicParameters();
                    return _context;
                case XenHostAction.SendDebugKeys:
                    _context = new XenHostActionSendDebugKeysDynamicParameters();
                    return _context;
                case XenHostAction.BugreportUpload:
                    _context = new XenHostActionBugreportUploadDynamicParameters();
                    return _context;
                case XenHostAction.LicenseApply:
                    _context = new XenHostActionLicenseApplyDynamicParameters();
                    return _context;
                case XenHostAction.LicenseAdd:
                    _context = new XenHostActionLicenseAddDynamicParameters();
                    return _context;
                case XenHostAction.LicenseRemove:
                    _context = new XenHostActionLicenseRemoveDynamicParameters();
                    return _context;
                case XenHostAction.PowerOn:
                    _context = new XenHostActionPowerOnDynamicParameters();
                    return _context;
                case XenHostAction.EmergencyHaDisable:
                    _context = new XenHostActionEmergencyHaDisableDynamicParameters();
                    return _context;
                case XenHostAction.RecordDataSource:
                    _context = new XenHostActionRecordDataSourceDynamicParameters();
                    return _context;
                case XenHostAction.QueryDataSource:
                    _context = new XenHostActionQueryDataSourceDynamicParameters();
                    return _context;
                case XenHostAction.ForgetDataSourceArchives:
                    _context = new XenHostActionForgetDataSourceArchivesDynamicParameters();
                    return _context;
                case XenHostAction.AssertCanEvacuate:
                    _context = new XenHostActionAssertCanEvacuateDynamicParameters();
                    return _context;
                case XenHostAction.Evacuate:
                    _context = new XenHostActionEvacuateDynamicParameters();
                    return _context;
                case XenHostAction.SyslogReconfigure:
                    _context = new XenHostActionSyslogReconfigureDynamicParameters();
                    return _context;
                case XenHostAction.ManagementReconfigure:
                    _context = new XenHostActionManagementReconfigureDynamicParameters();
                    return _context;
                case XenHostAction.LocalManagementReconfigure:
                    _context = new XenHostActionLocalManagementReconfigureDynamicParameters();
                    return _context;
                case XenHostAction.RestartAgent:
                    _context = new XenHostActionRestartAgentDynamicParameters();
                    return _context;
                case XenHostAction.ComputeFreeMemory:
                    _context = new XenHostActionComputeFreeMemoryDynamicParameters();
                    return _context;
                case XenHostAction.ComputeMemoryOverhead:
                    _context = new XenHostActionComputeMemoryOverheadDynamicParameters();
                    return _context;
                case XenHostAction.BackupRrds:
                    _context = new XenHostActionBackupRrdsDynamicParameters();
                    return _context;
                case XenHostAction.CreateNewBlob:
                    _context = new XenHostActionCreateNewBlobDynamicParameters();
                    return _context;
                case XenHostAction.CallPlugin:
                    _context = new XenHostActionCallPluginDynamicParameters();
                    return _context;
                case XenHostAction.HasExtension:
                    _context = new XenHostActionHasExtensionDynamicParameters();
                    return _context;
                case XenHostAction.CallExtension:
                    _context = new XenHostActionCallExtensionDynamicParameters();
                    return _context;
                case XenHostAction.EnableExternalAuth:
                    _context = new XenHostActionEnableExternalAuthDynamicParameters();
                    return _context;
                case XenHostAction.DisableExternalAuth:
                    _context = new XenHostActionDisableExternalAuthDynamicParameters();
                    return _context;
                case XenHostAction.RetrieveWlbEvacuateRecommendations:
                    _context = new XenHostActionRetrieveWlbEvacuateRecommendationsDynamicParameters();
                    return _context;
                case XenHostAction.ApplyEdition:
                    _context = new XenHostActionApplyEditionDynamicParameters();
                    return _context;
                case XenHostAction.RefreshPackInfo:
                    _context = new XenHostActionRefreshPackInfoDynamicParameters();
                    return _context;
                case XenHostAction.EnableLocalStorageCaching:
                    _context = new XenHostActionEnableLocalStorageCachingDynamicParameters();
                    return _context;
                case XenHostAction.MigrateReceive:
                    _context = new XenHostActionMigrateReceiveDynamicParameters();
                    return _context;
                case XenHostAction.DeclareDead:
                    _context = new XenHostActionDeclareDeadDynamicParameters();
                    return _context;
                case XenHostAction.EnableDisplay:
                    _context = new XenHostActionEnableDisplayDynamicParameters();
                    return _context;
                case XenHostAction.DisableDisplay:
                    _context = new XenHostActionDisableDisplayDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host = ParseXenHost();

            switch (XenAction)
            {
                case XenHostAction.Disable:
                    ProcessRecordDisable(host);
                    break;
                case XenHostAction.Enable:
                    ProcessRecordEnable(host);
                    break;
                case XenHostAction.Shutdown:
                    ProcessRecordShutdown(host);
                    break;
                case XenHostAction.Reboot:
                    ProcessRecordReboot(host);
                    break;
                case XenHostAction.Dmesg:
                    ProcessRecordDmesg(host);
                    break;
                case XenHostAction.DmesgClear:
                    ProcessRecordDmesgClear(host);
                    break;
                case XenHostAction.SendDebugKeys:
                    ProcessRecordSendDebugKeys(host);
                    break;
                case XenHostAction.BugreportUpload:
                    ProcessRecordBugreportUpload(host);
                    break;
                case XenHostAction.ListMethods:
                    ProcessRecordListMethods(host);
                    break;
                case XenHostAction.LicenseApply:
                    ProcessRecordLicenseApply(host);
                    break;
                case XenHostAction.LicenseAdd:
                    ProcessRecordLicenseAdd(host);
                    break;
                case XenHostAction.LicenseRemove:
                    ProcessRecordLicenseRemove(host);
                    break;
                case XenHostAction.PowerOn:
                    ProcessRecordPowerOn(host);
                    break;
                case XenHostAction.EmergencyHaDisable:
                    ProcessRecordEmergencyHaDisable(host);
                    break;
                case XenHostAction.RecordDataSource:
                    ProcessRecordRecordDataSource(host);
                    break;
                case XenHostAction.QueryDataSource:
                    ProcessRecordQueryDataSource(host);
                    break;
                case XenHostAction.ForgetDataSourceArchives:
                    ProcessRecordForgetDataSourceArchives(host);
                    break;
                case XenHostAction.AssertCanEvacuate:
                    ProcessRecordAssertCanEvacuate(host);
                    break;
                case XenHostAction.Evacuate:
                    ProcessRecordEvacuate(host);
                    break;
                case XenHostAction.SyslogReconfigure:
                    ProcessRecordSyslogReconfigure(host);
                    break;
                case XenHostAction.ManagementReconfigure:
                    ProcessRecordManagementReconfigure(host);
                    break;
                case XenHostAction.LocalManagementReconfigure:
                    ProcessRecordLocalManagementReconfigure(host);
                    break;
                case XenHostAction.ManagementDisable:
                    ProcessRecordManagementDisable(host);
                    break;
                case XenHostAction.RestartAgent:
                    ProcessRecordRestartAgent(host);
                    break;
                case XenHostAction.ShutdownAgent:
                    ProcessRecordShutdownAgent(host);
                    break;
                case XenHostAction.ComputeFreeMemory:
                    ProcessRecordComputeFreeMemory(host);
                    break;
                case XenHostAction.ComputeMemoryOverhead:
                    ProcessRecordComputeMemoryOverhead(host);
                    break;
                case XenHostAction.SyncData:
                    ProcessRecordSyncData(host);
                    break;
                case XenHostAction.BackupRrds:
                    ProcessRecordBackupRrds(host);
                    break;
                case XenHostAction.CreateNewBlob:
                    ProcessRecordCreateNewBlob(host);
                    break;
                case XenHostAction.CallPlugin:
                    ProcessRecordCallPlugin(host);
                    break;
                case XenHostAction.HasExtension:
                    ProcessRecordHasExtension(host);
                    break;
                case XenHostAction.CallExtension:
                    ProcessRecordCallExtension(host);
                    break;
                case XenHostAction.EnableExternalAuth:
                    ProcessRecordEnableExternalAuth(host);
                    break;
                case XenHostAction.DisableExternalAuth:
                    ProcessRecordDisableExternalAuth(host);
                    break;
                case XenHostAction.RetrieveWlbEvacuateRecommendations:
                    ProcessRecordRetrieveWlbEvacuateRecommendations(host);
                    break;
                case XenHostAction.ApplyEdition:
                    ProcessRecordApplyEdition(host);
                    break;
                case XenHostAction.RefreshPackInfo:
                    ProcessRecordRefreshPackInfo(host);
                    break;
                case XenHostAction.ResetCpuFeatures:
                    ProcessRecordResetCpuFeatures(host);
                    break;
                case XenHostAction.EnableLocalStorageCaching:
                    ProcessRecordEnableLocalStorageCaching(host);
                    break;
                case XenHostAction.DisableLocalStorageCaching:
                    ProcessRecordDisableLocalStorageCaching(host);
                    break;
                case XenHostAction.MigrateReceive:
                    ProcessRecordMigrateReceive(host);
                    break;
                case XenHostAction.DeclareDead:
                    ProcessRecordDeclareDead(host);
                    break;
                case XenHostAction.EnableDisplay:
                    ProcessRecordEnableDisplay(host);
                    break;
                case XenHostAction.DisableDisplay:
                    ProcessRecordDisableDisplay(host);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseXenHost()
        {
            string host = null;

            if (XenHost != null)
                host = (new XenRef<XenAPI.Host>(XenHost)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Host.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    host = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.Host.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    host = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.Host with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                host = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'XenHost', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    XenHost));
            }

            return host;
        }

        private void ProcessRecordDisable(string host)
        {
            if (!ShouldProcess(host, "Host.disable"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionDisableDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_disable(session, host);

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
                    XenAPI.Host.disable(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnable(string host)
        {
            if (!ShouldProcess(host, "Host.enable"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionEnableDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_enable(session, host);

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
                    XenAPI.Host.enable(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordShutdown(string host)
        {
            if (!ShouldProcess(host, "Host.shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_shutdown(session, host);

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
                    XenAPI.Host.shutdown(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordReboot(string host)
        {
            if (!ShouldProcess(host, "Host.reboot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionRebootDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_reboot(session, host);

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
                    XenAPI.Host.reboot(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDmesg(string host)
        {
            if (!ShouldProcess(host, "Host.dmesg"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionDmesgDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_dmesg(session, host);

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
                    string obj = XenAPI.Host.dmesg(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDmesgClear(string host)
        {
            if (!ShouldProcess(host, "Host.dmesg_clear"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionDmesgClearDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_dmesg_clear(session, host);

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
                    string obj = XenAPI.Host.dmesg_clear(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSendDebugKeys(string host)
        {
            if (!ShouldProcess(host, "Host.send_debug_keys"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionSendDebugKeysDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_send_debug_keys(session, host, contxt.Keys);

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
                    XenAPI.Host.send_debug_keys(session, host, contxt.Keys);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordBugreportUpload(string host)
        {
            if (!ShouldProcess(host, "Host.bugreport_upload"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionBugreportUploadDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_bugreport_upload(session, host, contxt.Url_, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

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
                    XenAPI.Host.bugreport_upload(session, host, contxt.Url_, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordListMethods(string host)
        {
            if (!ShouldProcess(host, "Host.list_methods"))
                return;

            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.list_methods(session);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordLicenseApply(string host)
        {
            if (!ShouldProcess(host, "Host.license_apply"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionLicenseApplyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_license_apply(session, host, contxt.Contents);

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
                    XenAPI.Host.license_apply(session, host, contxt.Contents);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordLicenseAdd(string host)
        {
            if (!ShouldProcess(host, "Host.license_add"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionLicenseAddDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_license_add(session, host, contxt.Contents);

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
                    XenAPI.Host.license_add(session, host, contxt.Contents);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordLicenseRemove(string host)
        {
            if (!ShouldProcess(host, "Host.license_remove"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionLicenseRemoveDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_license_remove(session, host);

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
                    XenAPI.Host.license_remove(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPowerOn(string host)
        {
            if (!ShouldProcess(host, "Host.power_on"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionPowerOnDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_power_on(session, host);

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
                    XenAPI.Host.power_on(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEmergencyHaDisable(string host)
        {
            if (!ShouldProcess(host, "Host.emergency_ha_disable"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionEmergencyHaDisableDynamicParameters;

                    XenAPI.Host.emergency_ha_disable(session, contxt.Soft);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordRecordDataSource(string host)
        {
            if (!ShouldProcess(host, "Host.record_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionRecordDataSourceDynamicParameters;

                    XenAPI.Host.record_data_source(session, host, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordQueryDataSource(string host)
        {
            if (!ShouldProcess(host, "Host.query_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionQueryDataSourceDynamicParameters;

                    double obj = XenAPI.Host.query_data_source(session, host, contxt.DataSource);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordForgetDataSourceArchives(string host)
        {
            if (!ShouldProcess(host, "Host.forget_data_source_archives"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionForgetDataSourceArchivesDynamicParameters;

                    XenAPI.Host.forget_data_source_archives(session, host, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordAssertCanEvacuate(string host)
        {
            if (!ShouldProcess(host, "Host.assert_can_evacuate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionAssertCanEvacuateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_assert_can_evacuate(session, host);

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
                    XenAPI.Host.assert_can_evacuate(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEvacuate(string host)
        {
            if (!ShouldProcess(host, "Host.evacuate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionEvacuateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_evacuate(session, host);

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
                    XenAPI.Host.evacuate(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSyslogReconfigure(string host)
        {
            if (!ShouldProcess(host, "Host.syslog_reconfigure"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionSyslogReconfigureDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_syslog_reconfigure(session, host);

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
                    XenAPI.Host.syslog_reconfigure(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordManagementReconfigure(string host)
        {
            if (!ShouldProcess(host, "Host.management_reconfigure"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionManagementReconfigureDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_management_reconfigure(session, contxt.PIF);

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
                    XenAPI.Host.management_reconfigure(session, contxt.PIF);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordLocalManagementReconfigure(string host)
        {
            if (!ShouldProcess(host, "Host.local_management_reconfigure"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionLocalManagementReconfigureDynamicParameters;

                    XenAPI.Host.local_management_reconfigure(session, contxt.Interface);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordManagementDisable(string host)
        {
            if (!ShouldProcess(host, "Host.management_disable"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.management_disable(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordRestartAgent(string host)
        {
            if (!ShouldProcess(host, "Host.restart_agent"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionRestartAgentDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_restart_agent(session, host);

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
                    XenAPI.Host.restart_agent(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordShutdownAgent(string host)
        {
            if (!ShouldProcess(host, "Host.shutdown_agent"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.shutdown_agent(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordComputeFreeMemory(string host)
        {
            if (!ShouldProcess(host, "Host.compute_free_memory"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionComputeFreeMemoryDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_compute_free_memory(session, host);

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
                    long obj = XenAPI.Host.compute_free_memory(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordComputeMemoryOverhead(string host)
        {
            if (!ShouldProcess(host, "Host.compute_memory_overhead"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionComputeMemoryOverheadDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_compute_memory_overhead(session, host);

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
                    long obj = XenAPI.Host.compute_memory_overhead(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSyncData(string host)
        {
            if (!ShouldProcess(host, "Host.sync_data"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.sync_data(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordBackupRrds(string host)
        {
            if (!ShouldProcess(host, "Host.backup_rrds"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionBackupRrdsDynamicParameters;

                    XenAPI.Host.backup_rrds(session, host, contxt.Delay);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordCreateNewBlob(string host)
        {
            if (!ShouldProcess(host, "Host.create_new_blob"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionCreateNewBlobDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_create_new_blob(session, host, contxt.NameParam, contxt.MimeType, contxt.Public);

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
                    string objRef = XenAPI.Host.create_new_blob(session, host, contxt.NameParam, contxt.MimeType, contxt.Public);

                    if (PassThru)
                    {
                        XenAPI.Blob obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Blob.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCallPlugin(string host)
        {
            if (!ShouldProcess(host, "Host.call_plugin"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionCallPluginDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_call_plugin(session, host, contxt.Plugin, contxt.Fn, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Args));

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
                    string obj = XenAPI.Host.call_plugin(session, host, contxt.Plugin, contxt.Fn, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Args));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHasExtension(string host)
        {
            if (!ShouldProcess(host, "Host.has_extension"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionHasExtensionDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_has_extension(session, host, contxt.NameParam);

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
                    bool obj = XenAPI.Host.has_extension(session, host, contxt.NameParam);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCallExtension(string host)
        {
            if (!ShouldProcess(host, "Host.call_extension"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionCallExtensionDynamicParameters;

                    string obj = XenAPI.Host.call_extension(session, host, contxt.Call);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordEnableExternalAuth(string host)
        {
            if (!ShouldProcess(host, "Host.enable_external_auth"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionEnableExternalAuthDynamicParameters;

                    XenAPI.Host.enable_external_auth(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config), contxt.ServiceName, contxt.AuthType);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordDisableExternalAuth(string host)
        {
            if (!ShouldProcess(host, "Host.disable_external_auth"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionDisableExternalAuthDynamicParameters;

                    XenAPI.Host.disable_external_auth(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config));

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordRetrieveWlbEvacuateRecommendations(string host)
        {
            if (!ShouldProcess(host, "Host.retrieve_wlb_evacuate_recommendations"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionRetrieveWlbEvacuateRecommendationsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_retrieve_wlb_evacuate_recommendations(session, host);

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
                    var dict = XenAPI.Host.retrieve_wlb_evacuate_recommendations(session, host);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordApplyEdition(string host)
        {
            if (!ShouldProcess(host, "Host.apply_edition"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionApplyEditionDynamicParameters;

                    XenAPI.Host.apply_edition(session, host, contxt.Edition, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordRefreshPackInfo(string host)
        {
            if (!ShouldProcess(host, "Host.refresh_pack_info"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionRefreshPackInfoDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_refresh_pack_info(session, host);

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
                    XenAPI.Host.refresh_pack_info(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordResetCpuFeatures(string host)
        {
            if (!ShouldProcess(host, "Host.reset_cpu_features"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.reset_cpu_features(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordEnableLocalStorageCaching(string host)
        {
            if (!ShouldProcess(host, "Host.enable_local_storage_caching"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenHostActionEnableLocalStorageCachingDynamicParameters;

                    XenAPI.Host.enable_local_storage_caching(session, host, contxt.SR);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordDisableLocalStorageCaching(string host)
        {
            if (!ShouldProcess(host, "Host.disable_local_storage_caching"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.disable_local_storage_caching(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordMigrateReceive(string host)
        {
            if (!ShouldProcess(host, "Host.migrate_receive"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionMigrateReceiveDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_migrate_receive(session, host, contxt.Network, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

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
                    var dict = XenAPI.Host.migrate_receive(session, host, contxt.Network, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordDeclareDead(string host)
        {
            if (!ShouldProcess(host, "Host.declare_dead"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionDeclareDeadDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_declare_dead(session, host);

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
                    XenAPI.Host.declare_dead(session, host);

                    if (PassThru)
                    {
                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableDisplay(string host)
        {
            if (!ShouldProcess(host, "Host.enable_display"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionEnableDisplayDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_enable_display(session, host);

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
                    host_display obj = XenAPI.Host.enable_display(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableDisplay(string host)
        {
            if (!ShouldProcess(host, "Host.disable_display"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenHostActionDisableDisplayDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_disable_display(session, host);

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
                    host_display obj = XenAPI.Host.disable_display(session, host);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenHostAction
    {
        Disable,
        Enable,
        Shutdown,
        Reboot,
        Dmesg,
        DmesgClear,
        SendDebugKeys,
        BugreportUpload,
        ListMethods,
        LicenseApply,
        LicenseAdd,
        LicenseRemove,
        PowerOn,
        EmergencyHaDisable,
        RecordDataSource,
        QueryDataSource,
        ForgetDataSourceArchives,
        AssertCanEvacuate,
        Evacuate,
        SyslogReconfigure,
        ManagementReconfigure,
        LocalManagementReconfigure,
        ManagementDisable,
        RestartAgent,
        ShutdownAgent,
        ComputeFreeMemory,
        ComputeMemoryOverhead,
        SyncData,
        BackupRrds,
        CreateNewBlob,
        CallPlugin,
        HasExtension,
        CallExtension,
        EnableExternalAuth,
        DisableExternalAuth,
        RetrieveWlbEvacuateRecommendations,
        ApplyEdition,
        RefreshPackInfo,
        ResetCpuFeatures,
        EnableLocalStorageCaching,
        DisableLocalStorageCaching,
        MigrateReceive,
        DeclareDead,
        EnableDisplay,
        DisableDisplay
    }

    public class XenHostActionDisableDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionEnableDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionRebootDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionDmesgDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionDmesgClearDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionSendDebugKeysDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Keys { get; set; }
 
    }

    public class XenHostActionBugreportUploadDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Url_ { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }
  
    }

    public class XenHostActionLicenseApplyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Contents { get; set; }
 
    }

    public class XenHostActionLicenseAddDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Contents { get; set; }
 
    }

    public class XenHostActionLicenseRemoveDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionPowerOnDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionEmergencyHaDisableDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public bool Soft { get; set; }
 
    }

    public class XenHostActionRecordDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenHostActionQueryDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenHostActionForgetDataSourceArchivesDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenHostActionAssertCanEvacuateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionEvacuateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionSyslogReconfigureDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionManagementReconfigureDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.PIF> PIF { get; set; }
 
    }

    public class XenHostActionLocalManagementReconfigureDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string Interface { get; set; }
 
    }

    public class XenHostActionRestartAgentDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionComputeFreeMemoryDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionComputeMemoryOverheadDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionBackupRrdsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public double Delay { get; set; }
 
    }

    public class XenHostActionCreateNewBlobDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }

        [Parameter]
        public string MimeType { get; set; }

        [Parameter]
        public bool Public { get; set; }
   
    }

    public class XenHostActionCallPluginDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Plugin { get; set; }

        [Parameter]
        public string Fn { get; set; }

        [Parameter]
        public Hashtable Args { get; set; }
   
    }

    public class XenHostActionHasExtensionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }
 
    }

    public class XenHostActionCallExtensionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string Call { get; set; }
 
    }

    public class XenHostActionEnableExternalAuthDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Config { get; set; }

        [Parameter]
        public string ServiceName { get; set; }

        [Parameter]
        public string AuthType { get; set; }
   
    }

    public class XenHostActionDisableExternalAuthDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Config { get; set; }
 
    }

    public class XenHostActionRetrieveWlbEvacuateRecommendationsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionApplyEditionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string Edition { get; set; }

        [Parameter]
        public bool Force { get; set; }
  
    }

    public class XenHostActionRefreshPackInfoDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionEnableLocalStorageCachingDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }
 
    }

    public class XenHostActionMigrateReceiveDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }
  
    }

    public class XenHostActionDeclareDeadDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionEnableDisplayDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostActionDisableDisplayDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
