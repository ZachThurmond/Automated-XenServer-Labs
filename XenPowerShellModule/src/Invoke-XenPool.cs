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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPool", SupportsShouldProcess = true)]
    public class InvokeXenPool : XenServerCmdlet
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


        [Parameter(Mandatory = true)]
        public XenPoolAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPoolAction.Join:
                    _context = new XenPoolActionJoinDynamicParameters();
                    return _context;
                case XenPoolAction.JoinForce:
                    _context = new XenPoolActionJoinForceDynamicParameters();
                    return _context;
                case XenPoolAction.Eject:
                    _context = new XenPoolActionEjectDynamicParameters();
                    return _context;
                case XenPoolAction.EmergencyResetMaster:
                    _context = new XenPoolActionEmergencyResetMasterDynamicParameters();
                    return _context;
                case XenPoolAction.RecoverSlaves:
                    _context = new XenPoolActionRecoverSlavesDynamicParameters();
                    return _context;
                case XenPoolAction.CreateVLAN:
                    _context = new XenPoolActionCreateVLANDynamicParameters();
                    return _context;
                case XenPoolAction.ManagementReconfigure:
                    _context = new XenPoolActionManagementReconfigureDynamicParameters();
                    return _context;
                case XenPoolAction.CreateVLANFromPIF:
                    _context = new XenPoolActionCreateVLANFromPIFDynamicParameters();
                    return _context;
                case XenPoolAction.EnableHa:
                    _context = new XenPoolActionEnableHaDynamicParameters();
                    return _context;
                case XenPoolAction.DisableHa:
                    _context = new XenPoolActionDisableHaDynamicParameters();
                    return _context;
                case XenPoolAction.SyncDatabase:
                    _context = new XenPoolActionSyncDatabaseDynamicParameters();
                    return _context;
                case XenPoolAction.DesignateNewMaster:
                    _context = new XenPoolActionDesignateNewMasterDynamicParameters();
                    return _context;
                case XenPoolAction.HaPreventRestartsFor:
                    _context = new XenPoolActionHaPreventRestartsForDynamicParameters();
                    return _context;
                case XenPoolAction.HaFailoverPlanExists:
                    _context = new XenPoolActionHaFailoverPlanExistsDynamicParameters();
                    return _context;
                case XenPoolAction.HaComputeHypotheticalMaxHostFailuresToTolerate:
                    _context = new XenPoolActionHaComputeHypotheticalMaxHostFailuresToTolerateDynamicParameters();
                    return _context;
                case XenPoolAction.HaComputeVmFailoverPlan:
                    _context = new XenPoolActionHaComputeVmFailoverPlanDynamicParameters();
                    return _context;
                case XenPoolAction.CreateNewBlob:
                    _context = new XenPoolActionCreateNewBlobDynamicParameters();
                    return _context;
                case XenPoolAction.EnableExternalAuth:
                    _context = new XenPoolActionEnableExternalAuthDynamicParameters();
                    return _context;
                case XenPoolAction.DisableExternalAuth:
                    _context = new XenPoolActionDisableExternalAuthDynamicParameters();
                    return _context;
                case XenPoolAction.InitializeWlb:
                    _context = new XenPoolActionInitializeWlbDynamicParameters();
                    return _context;
                case XenPoolAction.DeconfigureWlb:
                    _context = new XenPoolActionDeconfigureWlbDynamicParameters();
                    return _context;
                case XenPoolAction.SendWlbConfiguration:
                    _context = new XenPoolActionSendWlbConfigurationDynamicParameters();
                    return _context;
                case XenPoolAction.RetrieveWlbConfiguration:
                    _context = new XenPoolActionRetrieveWlbConfigurationDynamicParameters();
                    return _context;
                case XenPoolAction.RetrieveWlbRecommendations:
                    _context = new XenPoolActionRetrieveWlbRecommendationsDynamicParameters();
                    return _context;
                case XenPoolAction.SendTestPost:
                    _context = new XenPoolActionSendTestPostDynamicParameters();
                    return _context;
                case XenPoolAction.CertificateInstall:
                    _context = new XenPoolActionCertificateInstallDynamicParameters();
                    return _context;
                case XenPoolAction.CertificateUninstall:
                    _context = new XenPoolActionCertificateUninstallDynamicParameters();
                    return _context;
                case XenPoolAction.CertificateList:
                    _context = new XenPoolActionCertificateListDynamicParameters();
                    return _context;
                case XenPoolAction.CrlInstall:
                    _context = new XenPoolActionCrlInstallDynamicParameters();
                    return _context;
                case XenPoolAction.CrlUninstall:
                    _context = new XenPoolActionCrlUninstallDynamicParameters();
                    return _context;
                case XenPoolAction.CrlList:
                    _context = new XenPoolActionCrlListDynamicParameters();
                    return _context;
                case XenPoolAction.CertificateSync:
                    _context = new XenPoolActionCertificateSyncDynamicParameters();
                    return _context;
                case XenPoolAction.EnableRedoLog:
                    _context = new XenPoolActionEnableRedoLogDynamicParameters();
                    return _context;
                case XenPoolAction.DisableRedoLog:
                    _context = new XenPoolActionDisableRedoLogDynamicParameters();
                    return _context;
                case XenPoolAction.TestArchiveTarget:
                    _context = new XenPoolActionTestArchiveTargetDynamicParameters();
                    return _context;
                case XenPoolAction.EnableLocalStorageCaching:
                    _context = new XenPoolActionEnableLocalStorageCachingDynamicParameters();
                    return _context;
                case XenPoolAction.DisableLocalStorageCaching:
                    _context = new XenPoolActionDisableLocalStorageCachingDynamicParameters();
                    return _context;
                case XenPoolAction.ApplyEdition:
                    _context = new XenPoolActionApplyEditionDynamicParameters();
                    return _context;
                case XenPoolAction.EnableSslLegacy:
                    _context = new XenPoolActionEnableSslLegacyDynamicParameters();
                    return _context;
                case XenPoolAction.DisableSslLegacy:
                    _context = new XenPoolActionDisableSslLegacyDynamicParameters();
                    return _context;
                case XenPoolAction.HasExtension:
                    _context = new XenPoolActionHasExtensionDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool = ParsePool();

            switch (XenAction)
            {
                case XenPoolAction.Join:
                    ProcessRecordJoin(pool);
                    break;
                case XenPoolAction.JoinForce:
                    ProcessRecordJoinForce(pool);
                    break;
                case XenPoolAction.Eject:
                    ProcessRecordEject(pool);
                    break;
                case XenPoolAction.EmergencyTransitionToMaster:
                    ProcessRecordEmergencyTransitionToMaster(pool);
                    break;
                case XenPoolAction.EmergencyResetMaster:
                    ProcessRecordEmergencyResetMaster(pool);
                    break;
                case XenPoolAction.RecoverSlaves:
                    ProcessRecordRecoverSlaves(pool);
                    break;
                case XenPoolAction.CreateVLAN:
                    ProcessRecordCreateVLAN(pool);
                    break;
                case XenPoolAction.ManagementReconfigure:
                    ProcessRecordManagementReconfigure(pool);
                    break;
                case XenPoolAction.CreateVLANFromPIF:
                    ProcessRecordCreateVLANFromPIF(pool);
                    break;
                case XenPoolAction.EnableHa:
                    ProcessRecordEnableHa(pool);
                    break;
                case XenPoolAction.DisableHa:
                    ProcessRecordDisableHa(pool);
                    break;
                case XenPoolAction.SyncDatabase:
                    ProcessRecordSyncDatabase(pool);
                    break;
                case XenPoolAction.DesignateNewMaster:
                    ProcessRecordDesignateNewMaster(pool);
                    break;
                case XenPoolAction.HaPreventRestartsFor:
                    ProcessRecordHaPreventRestartsFor(pool);
                    break;
                case XenPoolAction.HaFailoverPlanExists:
                    ProcessRecordHaFailoverPlanExists(pool);
                    break;
                case XenPoolAction.HaComputeMaxHostFailuresToTolerate:
                    ProcessRecordHaComputeMaxHostFailuresToTolerate(pool);
                    break;
                case XenPoolAction.HaComputeHypotheticalMaxHostFailuresToTolerate:
                    ProcessRecordHaComputeHypotheticalMaxHostFailuresToTolerate(pool);
                    break;
                case XenPoolAction.HaComputeVmFailoverPlan:
                    ProcessRecordHaComputeVmFailoverPlan(pool);
                    break;
                case XenPoolAction.CreateNewBlob:
                    ProcessRecordCreateNewBlob(pool);
                    break;
                case XenPoolAction.EnableExternalAuth:
                    ProcessRecordEnableExternalAuth(pool);
                    break;
                case XenPoolAction.DisableExternalAuth:
                    ProcessRecordDisableExternalAuth(pool);
                    break;
                case XenPoolAction.DetectNonhomogeneousExternalAuth:
                    ProcessRecordDetectNonhomogeneousExternalAuth(pool);
                    break;
                case XenPoolAction.InitializeWlb:
                    ProcessRecordInitializeWlb(pool);
                    break;
                case XenPoolAction.DeconfigureWlb:
                    ProcessRecordDeconfigureWlb(pool);
                    break;
                case XenPoolAction.SendWlbConfiguration:
                    ProcessRecordSendWlbConfiguration(pool);
                    break;
                case XenPoolAction.RetrieveWlbConfiguration:
                    ProcessRecordRetrieveWlbConfiguration(pool);
                    break;
                case XenPoolAction.RetrieveWlbRecommendations:
                    ProcessRecordRetrieveWlbRecommendations(pool);
                    break;
                case XenPoolAction.SendTestPost:
                    ProcessRecordSendTestPost(pool);
                    break;
                case XenPoolAction.CertificateInstall:
                    ProcessRecordCertificateInstall(pool);
                    break;
                case XenPoolAction.CertificateUninstall:
                    ProcessRecordCertificateUninstall(pool);
                    break;
                case XenPoolAction.CertificateList:
                    ProcessRecordCertificateList(pool);
                    break;
                case XenPoolAction.CrlInstall:
                    ProcessRecordCrlInstall(pool);
                    break;
                case XenPoolAction.CrlUninstall:
                    ProcessRecordCrlUninstall(pool);
                    break;
                case XenPoolAction.CrlList:
                    ProcessRecordCrlList(pool);
                    break;
                case XenPoolAction.CertificateSync:
                    ProcessRecordCertificateSync(pool);
                    break;
                case XenPoolAction.EnableRedoLog:
                    ProcessRecordEnableRedoLog(pool);
                    break;
                case XenPoolAction.DisableRedoLog:
                    ProcessRecordDisableRedoLog(pool);
                    break;
                case XenPoolAction.TestArchiveTarget:
                    ProcessRecordTestArchiveTarget(pool);
                    break;
                case XenPoolAction.EnableLocalStorageCaching:
                    ProcessRecordEnableLocalStorageCaching(pool);
                    break;
                case XenPoolAction.DisableLocalStorageCaching:
                    ProcessRecordDisableLocalStorageCaching(pool);
                    break;
                case XenPoolAction.ApplyEdition:
                    ProcessRecordApplyEdition(pool);
                    break;
                case XenPoolAction.EnableSslLegacy:
                    ProcessRecordEnableSslLegacy(pool);
                    break;
                case XenPoolAction.DisableSslLegacy:
                    ProcessRecordDisableSslLegacy(pool);
                    break;
                case XenPoolAction.HasExtension:
                    ProcessRecordHasExtension(pool);
                    break;
            }

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

        private void ProcessRecordJoin(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.join"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionJoinDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_join(session, contxt.MasterAddress, contxt.MasterUsername, contxt.MasterPassword);

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
                    XenAPI.Pool.join(session, contxt.MasterAddress, contxt.MasterUsername, contxt.MasterPassword);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordJoinForce(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.join_force"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionJoinForceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_join_force(session, contxt.MasterAddress, contxt.MasterUsername, contxt.MasterPassword);

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
                    XenAPI.Pool.join_force(session, contxt.MasterAddress, contxt.MasterUsername, contxt.MasterPassword);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEject(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.eject"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionEjectDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_eject(session, contxt.XenHost);

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
                    XenAPI.Pool.eject(session, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEmergencyTransitionToMaster(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.emergency_transition_to_master"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.emergency_transition_to_master(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordEmergencyResetMaster(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.emergency_reset_master"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionEmergencyResetMasterDynamicParameters;

                    XenAPI.Pool.emergency_reset_master(session, contxt.MasterAddress);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordRecoverSlaves(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.recover_slaves"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionRecoverSlavesDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_recover_slaves(session);

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
                    var refs = XenAPI.Pool.recover_slaves(session);

                    if (PassThru)
                    {
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
                }

            });
        }

        private void ProcessRecordCreateVLAN(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.create_VLAN"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCreateVLANDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_create_VLAN(session, contxt.Device, contxt.Network, contxt.VLAN);

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
                    var refs = XenAPI.Pool.create_VLAN(session, contxt.Device, contxt.Network, contxt.VLAN);

                    if (PassThru)
                    {
                        var records = new List<XenAPI.PIF>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PIF.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
                    }
                }

            });
        }

        private void ProcessRecordManagementReconfigure(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.management_reconfigure"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionManagementReconfigureDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_management_reconfigure(session, contxt.Network);

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
                    XenAPI.Pool.management_reconfigure(session, contxt.Network);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCreateVLANFromPIF(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.create_VLAN_from_PIF"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCreateVLANFromPIFDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_create_VLAN_from_PIF(session, contxt.PIF, contxt.Network, contxt.VLAN);

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
                    var refs = XenAPI.Pool.create_VLAN_from_PIF(session, contxt.PIF, contxt.Network, contxt.VLAN);

                    if (PassThru)
                    {
                        var records = new List<XenAPI.PIF>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PIF.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableHa(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.enable_ha"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionEnableHaDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_enable_ha(session, contxt.HeartbeatSrs, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Configuration));

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
                    XenAPI.Pool.enable_ha(session, contxt.HeartbeatSrs, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Configuration));

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableHa(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.disable_ha"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDisableHaDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_disable_ha(session);

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
                    XenAPI.Pool.disable_ha(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSyncDatabase(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.sync_database"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionSyncDatabaseDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_sync_database(session);

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
                    XenAPI.Pool.sync_database(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDesignateNewMaster(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.designate_new_master"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDesignateNewMasterDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_designate_new_master(session, contxt.XenHost);

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
                    XenAPI.Pool.designate_new_master(session, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHaPreventRestartsFor(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.ha_prevent_restarts_for"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionHaPreventRestartsForDynamicParameters;

                    XenAPI.Pool.ha_prevent_restarts_for(session, contxt.Seconds);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordHaFailoverPlanExists(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.ha_failover_plan_exists"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionHaFailoverPlanExistsDynamicParameters;

                    bool obj = XenAPI.Pool.ha_failover_plan_exists(session, contxt.N);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordHaComputeMaxHostFailuresToTolerate(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.ha_compute_max_host_failures_to_tolerate"))
                return;

            RunApiCall(()=>
            {
                    long obj = XenAPI.Pool.ha_compute_max_host_failures_to_tolerate(session);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordHaComputeHypotheticalMaxHostFailuresToTolerate(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.ha_compute_hypothetical_max_host_failures_to_tolerate"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionHaComputeHypotheticalMaxHostFailuresToTolerateDynamicParameters;

                    long obj = XenAPI.Pool.ha_compute_hypothetical_max_host_failures_to_tolerate(session, CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VM>, string>(contxt.Configuration));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordHaComputeVmFailoverPlan(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.ha_compute_vm_failover_plan"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionHaComputeVmFailoverPlanDynamicParameters;

                    var dict = XenAPI.Pool.ha_compute_vm_failover_plan(session, contxt.FailedHosts, contxt.FailedVms);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
            });
        }

        private void ProcessRecordCreateNewBlob(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.create_new_blob"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCreateNewBlobDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_create_new_blob(session, pool, contxt.NameParam, contxt.MimeType, contxt.Public);

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
                    string objRef = XenAPI.Pool.create_new_blob(session, pool, contxt.NameParam, contxt.MimeType, contxt.Public);

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

        private void ProcessRecordEnableExternalAuth(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.enable_external_auth"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionEnableExternalAuthDynamicParameters;

                    XenAPI.Pool.enable_external_auth(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config), contxt.ServiceName, contxt.AuthType);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordDisableExternalAuth(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.disable_external_auth"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionDisableExternalAuthDynamicParameters;

                    XenAPI.Pool.disable_external_auth(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config));

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordDetectNonhomogeneousExternalAuth(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.detect_nonhomogeneous_external_auth"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.detect_nonhomogeneous_external_auth(session, pool);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordInitializeWlb(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.initialize_wlb"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionInitializeWlbDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_initialize_wlb(session, contxt.WlbUrl, contxt.WlbUsername, contxt.WlbPassword, contxt.XenserverUsername, contxt.XenserverPassword);

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
                    XenAPI.Pool.initialize_wlb(session, contxt.WlbUrl, contxt.WlbUsername, contxt.WlbPassword, contxt.XenserverUsername, contxt.XenserverPassword);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDeconfigureWlb(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.deconfigure_wlb"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDeconfigureWlbDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_deconfigure_wlb(session);

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
                    XenAPI.Pool.deconfigure_wlb(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSendWlbConfiguration(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.send_wlb_configuration"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionSendWlbConfigurationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_send_wlb_configuration(session, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config));

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
                    XenAPI.Pool.send_wlb_configuration(session, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config));

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRetrieveWlbConfiguration(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.retrieve_wlb_configuration"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionRetrieveWlbConfigurationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_retrieve_wlb_configuration(session);

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
                    var dict = XenAPI.Pool.retrieve_wlb_configuration(session);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordRetrieveWlbRecommendations(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.retrieve_wlb_recommendations"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionRetrieveWlbRecommendationsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_retrieve_wlb_recommendations(session);

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
                    var dict = XenAPI.Pool.retrieve_wlb_recommendations(session);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordSendTestPost(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.send_test_post"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionSendTestPostDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_send_test_post(session, contxt.XenHost, contxt.Port, contxt.Body);

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
                    string obj = XenAPI.Pool.send_test_post(session, contxt.XenHost, contxt.Port, contxt.Body);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCertificateInstall(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.certificate_install"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCertificateInstallDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_certificate_install(session, contxt.NameParam, contxt.Cert);

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
                    XenAPI.Pool.certificate_install(session, contxt.NameParam, contxt.Cert);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCertificateUninstall(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.certificate_uninstall"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCertificateUninstallDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_certificate_uninstall(session, contxt.NameParam);

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
                    XenAPI.Pool.certificate_uninstall(session, contxt.NameParam);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCertificateList(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.certificate_list"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCertificateListDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_certificate_list(session);

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
                    string[] obj = XenAPI.Pool.certificate_list(session);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCrlInstall(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.crl_install"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCrlInstallDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_crl_install(session, contxt.NameParam, contxt.Cert);

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
                    XenAPI.Pool.crl_install(session, contxt.NameParam, contxt.Cert);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCrlUninstall(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.crl_uninstall"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCrlUninstallDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_crl_uninstall(session, contxt.NameParam);

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
                    XenAPI.Pool.crl_uninstall(session, contxt.NameParam);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCrlList(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.crl_list"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCrlListDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_crl_list(session);

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
                    string[] obj = XenAPI.Pool.crl_list(session);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCertificateSync(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.certificate_sync"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionCertificateSyncDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_certificate_sync(session);

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
                    XenAPI.Pool.certificate_sync(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableRedoLog(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.enable_redo_log"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionEnableRedoLogDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_enable_redo_log(session, contxt.SR);

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
                    XenAPI.Pool.enable_redo_log(session, contxt.SR);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableRedoLog(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.disable_redo_log"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDisableRedoLogDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_disable_redo_log(session);

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
                    XenAPI.Pool.disable_redo_log(session);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordTestArchiveTarget(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.test_archive_target"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenPoolActionTestArchiveTargetDynamicParameters;

                    string obj = XenAPI.Pool.test_archive_target(session, pool, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Config));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordEnableLocalStorageCaching(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.enable_local_storage_caching"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionEnableLocalStorageCachingDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_enable_local_storage_caching(session, pool);

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
                    XenAPI.Pool.enable_local_storage_caching(session, pool);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableLocalStorageCaching(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.disable_local_storage_caching"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDisableLocalStorageCachingDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_disable_local_storage_caching(session, pool);

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
                    XenAPI.Pool.disable_local_storage_caching(session, pool);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordApplyEdition(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.apply_edition"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionApplyEditionDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_apply_edition(session, pool, contxt.Edition);

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
                    XenAPI.Pool.apply_edition(session, pool, contxt.Edition);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableSslLegacy(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.enable_ssl_legacy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionEnableSslLegacyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_enable_ssl_legacy(session, pool);

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
                    XenAPI.Pool.enable_ssl_legacy(session, pool);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableSslLegacy(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.disable_ssl_legacy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionDisableSslLegacyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_disable_ssl_legacy(session, pool);

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
                    XenAPI.Pool.disable_ssl_legacy(session, pool);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHasExtension(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.has_extension"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolActionHasExtensionDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_has_extension(session, pool, contxt.NameParam);

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
                    bool obj = XenAPI.Pool.has_extension(session, pool, contxt.NameParam);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPoolAction
    {
        Join,
        JoinForce,
        Eject,
        EmergencyTransitionToMaster,
        EmergencyResetMaster,
        RecoverSlaves,
        CreateVLAN,
        ManagementReconfigure,
        CreateVLANFromPIF,
        EnableHa,
        DisableHa,
        SyncDatabase,
        DesignateNewMaster,
        HaPreventRestartsFor,
        HaFailoverPlanExists,
        HaComputeMaxHostFailuresToTolerate,
        HaComputeHypotheticalMaxHostFailuresToTolerate,
        HaComputeVmFailoverPlan,
        CreateNewBlob,
        EnableExternalAuth,
        DisableExternalAuth,
        DetectNonhomogeneousExternalAuth,
        InitializeWlb,
        DeconfigureWlb,
        SendWlbConfiguration,
        RetrieveWlbConfiguration,
        RetrieveWlbRecommendations,
        SendTestPost,
        CertificateInstall,
        CertificateUninstall,
        CertificateList,
        CrlInstall,
        CrlUninstall,
        CrlList,
        CertificateSync,
        EnableRedoLog,
        DisableRedoLog,
        TestArchiveTarget,
        EnableLocalStorageCaching,
        DisableLocalStorageCaching,
        ApplyEdition,
        EnableSslLegacy,
        DisableSslLegacy,
        HasExtension
    }

    public class XenPoolActionJoinDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string MasterAddress { get; set; }

        [Parameter]
        public string MasterUsername { get; set; }

        [Parameter]
        public string MasterPassword { get; set; }
   
    }

    public class XenPoolActionJoinForceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string MasterAddress { get; set; }

        [Parameter]
        public string MasterUsername { get; set; }

        [Parameter]
        public string MasterPassword { get; set; }
   
    }

    public class XenPoolActionEjectDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolActionEmergencyResetMasterDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string MasterAddress { get; set; }
 
    }

    public class XenPoolActionRecoverSlavesDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionCreateVLANDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Device { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }

        [Parameter]
        public long VLAN { get; set; }
   
    }

    public class XenPoolActionManagementReconfigureDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }
 
    }

    public class XenPoolActionCreateVLANFromPIFDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.PIF> PIF { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }

        [Parameter]
        public long VLAN { get; set; }
   
    }

    public class XenPoolActionEnableHaDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public List<XenRef<XenAPI.SR>> HeartbeatSrs { get; set; }

        [Parameter]
        public Hashtable Configuration { get; set; }
  
    }

    public class XenPoolActionDisableHaDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionSyncDatabaseDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionDesignateNewMasterDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolActionHaPreventRestartsForDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public long Seconds { get; set; }
 
    }

    public class XenPoolActionHaFailoverPlanExistsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public long N { get; set; }
 
    }

    public class XenPoolActionHaComputeHypotheticalMaxHostFailuresToTolerateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Configuration { get; set; }
 
    }

    public class XenPoolActionHaComputeVmFailoverPlanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public List<XenRef<XenAPI.Host>> FailedHosts { get; set; }

        [Parameter]
        public List<XenRef<XenAPI.VM>> FailedVms { get; set; }
  
    }

    public class XenPoolActionCreateNewBlobDynamicParameters : IXenServerDynamicParameter
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

    public class XenPoolActionEnableExternalAuthDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Config { get; set; }

        [Parameter]
        public string ServiceName { get; set; }

        [Parameter]
        public string AuthType { get; set; }
   
    }

    public class XenPoolActionDisableExternalAuthDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Config { get; set; }
 
    }

    public class XenPoolActionInitializeWlbDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string WlbUrl { get; set; }

        [Parameter]
        public string WlbUsername { get; set; }

        [Parameter]
        public string WlbPassword { get; set; }

        [Parameter]
        public string XenserverUsername { get; set; }

        [Parameter]
        public string XenserverPassword { get; set; }
     
    }

    public class XenPoolActionDeconfigureWlbDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionSendWlbConfigurationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public Hashtable Config { get; set; }
 
    }

    public class XenPoolActionRetrieveWlbConfigurationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionRetrieveWlbRecommendationsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionSendTestPostDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string XenHost { get; set; }

        [Parameter]
        public long Port { get; set; }

        [Parameter]
        public string Body { get; set; }
   
    }

    public class XenPoolActionCertificateInstallDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }

        [Parameter]
        public string Cert { get; set; }
  
    }

    public class XenPoolActionCertificateUninstallDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }
 
    }

    public class XenPoolActionCertificateListDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionCrlInstallDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }

        [Parameter]
        public string Cert { get; set; }
  
    }

    public class XenPoolActionCrlUninstallDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }
 
    }

    public class XenPoolActionCrlListDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionCertificateSyncDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionEnableRedoLogDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }
 
    }

    public class XenPoolActionDisableRedoLogDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionTestArchiveTargetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public Hashtable Config { get; set; }
 
    }

    public class XenPoolActionEnableLocalStorageCachingDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionDisableLocalStorageCachingDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionApplyEditionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Edition { get; set; }
 
    }

    public class XenPoolActionEnableSslLegacyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionDisableSslLegacyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolActionHasExtensionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NameParam { get; set; }
 
    }

}
