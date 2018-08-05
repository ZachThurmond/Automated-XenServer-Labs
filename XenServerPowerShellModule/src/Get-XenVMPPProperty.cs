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
    [Cmdlet(VerbsCommon.Get, "XenVMPPProperty", SupportsShouldProcess = false)]
    public class GetXenVMPPProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VMPP VMPP { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VMPP> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMPPProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenVMPPProperty.Alerts:
                    _context = new XenVMPPPropertyAlertsDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vmpp = ParseVMPP();

            switch (XenProperty)
            {
                case XenVMPPProperty.Uuid:
                    ProcessRecordUuid(vmpp);
                    break;
                case XenVMPPProperty.NameLabel:
                    ProcessRecordNameLabel(vmpp);
                    break;
                case XenVMPPProperty.NameDescription:
                    ProcessRecordNameDescription(vmpp);
                    break;
                case XenVMPPProperty.IsPolicyEnabled:
                    ProcessRecordIsPolicyEnabled(vmpp);
                    break;
                case XenVMPPProperty.BackupType:
                    ProcessRecordBackupType(vmpp);
                    break;
                case XenVMPPProperty.BackupRetentionValue:
                    ProcessRecordBackupRetentionValue(vmpp);
                    break;
                case XenVMPPProperty.BackupFrequency:
                    ProcessRecordBackupFrequency(vmpp);
                    break;
                case XenVMPPProperty.BackupSchedule:
                    ProcessRecordBackupSchedule(vmpp);
                    break;
                case XenVMPPProperty.IsBackupRunning:
                    ProcessRecordIsBackupRunning(vmpp);
                    break;
                case XenVMPPProperty.BackupLastRunTime:
                    ProcessRecordBackupLastRunTime(vmpp);
                    break;
                case XenVMPPProperty.ArchiveTargetType:
                    ProcessRecordArchiveTargetType(vmpp);
                    break;
                case XenVMPPProperty.ArchiveTargetConfig:
                    ProcessRecordArchiveTargetConfig(vmpp);
                    break;
                case XenVMPPProperty.ArchiveFrequency:
                    ProcessRecordArchiveFrequency(vmpp);
                    break;
                case XenVMPPProperty.ArchiveSchedule:
                    ProcessRecordArchiveSchedule(vmpp);
                    break;
                case XenVMPPProperty.IsArchiveRunning:
                    ProcessRecordIsArchiveRunning(vmpp);
                    break;
                case XenVMPPProperty.ArchiveLastRunTime:
                    ProcessRecordArchiveLastRunTime(vmpp);
                    break;
                case XenVMPPProperty.VMs:
                    ProcessRecordVMs(vmpp);
                    break;
                case XenVMPPProperty.IsAlarmEnabled:
                    ProcessRecordIsAlarmEnabled(vmpp);
                    break;
                case XenVMPPProperty.AlarmConfig:
                    ProcessRecordAlarmConfig(vmpp);
                    break;
                case XenVMPPProperty.RecentAlerts:
                    ProcessRecordRecentAlerts(vmpp);
                    break;
                case XenVMPPProperty.Alerts:
                    ProcessRecordAlerts(vmpp);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMPP()
        {
            string vmpp = null;

            if (VMPP != null)
                vmpp = (new XenRef<XenAPI.VMPP>(VMPP)).opaque_ref;
            else if (Ref != null)
                vmpp = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMPP', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMPP));
            }

            return vmpp;
        }

        private void ProcessRecordUuid(string vmpp)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMPP.get_uuid(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string vmpp)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMPP.get_name_label(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string vmpp)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMPP.get_name_description(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsPolicyEnabled(string vmpp)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VMPP.get_is_policy_enabled(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackupType(string vmpp)
        {
            RunApiCall(()=>
            {
                    vmpp_backup_type obj = XenAPI.VMPP.get_backup_type(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackupRetentionValue(string vmpp)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VMPP.get_backup_retention_value(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackupFrequency(string vmpp)
        {
            RunApiCall(()=>
            {
                    vmpp_backup_frequency obj = XenAPI.VMPP.get_backup_frequency(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackupSchedule(string vmpp)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VMPP.get_backup_schedule(session, vmpp);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordIsBackupRunning(string vmpp)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VMPP.get_is_backup_running(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackupLastRunTime(string vmpp)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VMPP.get_backup_last_run_time(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordArchiveTargetType(string vmpp)
        {
            RunApiCall(()=>
            {
                    vmpp_archive_target_type obj = XenAPI.VMPP.get_archive_target_type(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordArchiveTargetConfig(string vmpp)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VMPP.get_archive_target_config(session, vmpp);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordArchiveFrequency(string vmpp)
        {
            RunApiCall(()=>
            {
                    vmpp_archive_frequency obj = XenAPI.VMPP.get_archive_frequency(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordArchiveSchedule(string vmpp)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VMPP.get_archive_schedule(session, vmpp);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordIsArchiveRunning(string vmpp)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VMPP.get_is_archive_running(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordArchiveLastRunTime(string vmpp)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VMPP.get_archive_last_run_time(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVMs(string vmpp)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VMPP.get_VMs(session, vmpp);

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

        private void ProcessRecordIsAlarmEnabled(string vmpp)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VMPP.get_is_alarm_enabled(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAlarmConfig(string vmpp)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VMPP.get_alarm_config(session, vmpp);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordRecentAlerts(string vmpp)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VMPP.get_recent_alerts(session, vmpp);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAlerts(string vmpp)
        {
            RunApiCall(()=>
            {var contxt = _context as XenVMPPPropertyAlertsDynamicParameters;

                    string[] obj = XenAPI.VMPP.get_alerts(session, vmpp, contxt.HoursFromNow);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVMPPProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        IsPolicyEnabled,
        BackupType,
        BackupRetentionValue,
        BackupFrequency,
        BackupSchedule,
        IsBackupRunning,
        BackupLastRunTime,
        ArchiveTargetType,
        ArchiveTargetConfig,
        ArchiveFrequency,
        ArchiveSchedule,
        IsArchiveRunning,
        ArchiveLastRunTime,
        VMs,
        IsAlarmEnabled,
        AlarmConfig,
        RecentAlerts,
        Alerts
    }

    public class XenVMPPPropertyAlertsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public long HoursFromNow { get; set; }
 
    }

}
