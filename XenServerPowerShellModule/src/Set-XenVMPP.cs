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
    [Cmdlet(VerbsCommon.Set, "XenVMPP", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VMPP))]
    [OutputType(typeof(void))]
    public class SetXenVMPP : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VMPP VMPP { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VMPP> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


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
        public bool IsPolicyEnabled
        {
            get { return _isPolicyEnabled; }
            set
            {
                _isPolicyEnabled = value;
                _isPolicyEnabledIsSpecified = true;
            }
        }
        private bool _isPolicyEnabled;
        private bool _isPolicyEnabledIsSpecified;

        [Parameter]
        public vmpp_backup_type BackupType
        {
            get { return _backupType; }
            set
            {
                _backupType = value;
                _backupTypeIsSpecified = true;
            }
        }
        private vmpp_backup_type _backupType;
        private bool _backupTypeIsSpecified;

        [Parameter]
        public long BackupRetentionValue
        {
            get { return _backupRetentionValue; }
            set
            {
                _backupRetentionValue = value;
                _backupRetentionValueIsSpecified = true;
            }
        }
        private long _backupRetentionValue;
        private bool _backupRetentionValueIsSpecified;

        [Parameter]
        public vmpp_backup_frequency BackupFrequency
        {
            get { return _backupFrequency; }
            set
            {
                _backupFrequency = value;
                _backupFrequencyIsSpecified = true;
            }
        }
        private vmpp_backup_frequency _backupFrequency;
        private bool _backupFrequencyIsSpecified;

        [Parameter]
        public Hashtable BackupSchedule
        {
            get { return _backupSchedule; }
            set
            {
                _backupSchedule = value;
                _backupScheduleIsSpecified = true;
            }
        }
        private Hashtable _backupSchedule;
        private bool _backupScheduleIsSpecified;

        [Parameter]
        public vmpp_archive_frequency ArchiveFrequency
        {
            get { return _archiveFrequency; }
            set
            {
                _archiveFrequency = value;
                _archiveFrequencyIsSpecified = true;
            }
        }
        private vmpp_archive_frequency _archiveFrequency;
        private bool _archiveFrequencyIsSpecified;

        [Parameter]
        public Hashtable ArchiveSchedule
        {
            get { return _archiveSchedule; }
            set
            {
                _archiveSchedule = value;
                _archiveScheduleIsSpecified = true;
            }
        }
        private Hashtable _archiveSchedule;
        private bool _archiveScheduleIsSpecified;

        [Parameter]
        public vmpp_archive_target_type ArchiveTargetType
        {
            get { return _archiveTargetType; }
            set
            {
                _archiveTargetType = value;
                _archiveTargetTypeIsSpecified = true;
            }
        }
        private vmpp_archive_target_type _archiveTargetType;
        private bool _archiveTargetTypeIsSpecified;

        [Parameter]
        public Hashtable ArchiveTargetConfig
        {
            get { return _archiveTargetConfig; }
            set
            {
                _archiveTargetConfig = value;
                _archiveTargetConfigIsSpecified = true;
            }
        }
        private Hashtable _archiveTargetConfig;
        private bool _archiveTargetConfigIsSpecified;

        [Parameter]
        public bool IsAlarmEnabled
        {
            get { return _isAlarmEnabled; }
            set
            {
                _isAlarmEnabled = value;
                _isAlarmEnabledIsSpecified = true;
            }
        }
        private bool _isAlarmEnabled;
        private bool _isAlarmEnabledIsSpecified;

        [Parameter]
        public Hashtable AlarmConfig
        {
            get { return _alarmConfig; }
            set
            {
                _alarmConfig = value;
                _alarmConfigIsSpecified = true;
            }
        }
        private Hashtable _alarmConfig;
        private bool _alarmConfigIsSpecified;

        [Parameter]
        public DateTime BackupLastRunTime
        {
            get { return _backupLastRunTime; }
            set
            {
                _backupLastRunTime = value;
                _backupLastRunTimeIsSpecified = true;
            }
        }
        private DateTime _backupLastRunTime;
        private bool _backupLastRunTimeIsSpecified;

        [Parameter]
        public DateTime ArchiveLastRunTime
        {
            get { return _archiveLastRunTime; }
            set
            {
                _archiveLastRunTime = value;
                _archiveLastRunTimeIsSpecified = true;
            }
        }
        private DateTime _archiveLastRunTime;
        private bool _archiveLastRunTimeIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vmpp = ParseVMPP();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(vmpp);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(vmpp);
            if (_isPolicyEnabledIsSpecified)
                ProcessRecordIsPolicyEnabled(vmpp);
            if (_backupTypeIsSpecified)
                ProcessRecordBackupType(vmpp);
            if (_backupRetentionValueIsSpecified)
                ProcessRecordBackupRetentionValue(vmpp);
            if (_backupFrequencyIsSpecified)
                ProcessRecordBackupFrequency(vmpp);
            if (_backupScheduleIsSpecified)
                ProcessRecordBackupSchedule(vmpp);
            if (_archiveFrequencyIsSpecified)
                ProcessRecordArchiveFrequency(vmpp);
            if (_archiveScheduleIsSpecified)
                ProcessRecordArchiveSchedule(vmpp);
            if (_archiveTargetTypeIsSpecified)
                ProcessRecordArchiveTargetType(vmpp);
            if (_archiveTargetConfigIsSpecified)
                ProcessRecordArchiveTargetConfig(vmpp);
            if (_isAlarmEnabledIsSpecified)
                ProcessRecordIsAlarmEnabled(vmpp);
            if (_alarmConfigIsSpecified)
                ProcessRecordAlarmConfig(vmpp);
            if (_backupLastRunTimeIsSpecified)
                ProcessRecordBackupLastRunTime(vmpp);
            if (_archiveLastRunTimeIsSpecified)
                ProcessRecordArchiveLastRunTime(vmpp);

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

                        var obj = XenAPI.VMPP.get_record(session, vmpp);
                        if (obj != null)
                            obj.opaque_ref = vmpp;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMPP()
        {
            string vmpp = null;

            if (VMPP != null)
                vmpp = (new XenRef<XenAPI.VMPP>(VMPP)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VMPP.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vmpp = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VMPP.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vmpp = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VMPP with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
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

        private void ProcessRecordNameLabel(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_name_label(session, vmpp, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_name_description(session, vmpp, NameDescription);

            });
        }

        private void ProcessRecordIsPolicyEnabled(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_is_policy_enabled"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_is_policy_enabled(session, vmpp, IsPolicyEnabled);

            });
        }

        private void ProcessRecordBackupType(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_backup_type"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_backup_type(session, vmpp, BackupType);

            });
        }

        private void ProcessRecordBackupRetentionValue(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_backup_retention_value"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_backup_retention_value(session, vmpp, BackupRetentionValue);

            });
        }

        private void ProcessRecordBackupFrequency(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_backup_frequency"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_backup_frequency(session, vmpp, BackupFrequency);

            });
        }

        private void ProcessRecordBackupSchedule(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_backup_schedule"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_backup_schedule(session, vmpp, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(BackupSchedule));

            });
        }

        private void ProcessRecordArchiveFrequency(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_archive_frequency"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_archive_frequency(session, vmpp, ArchiveFrequency);

            });
        }

        private void ProcessRecordArchiveSchedule(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_archive_schedule"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_archive_schedule(session, vmpp, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(ArchiveSchedule));

            });
        }

        private void ProcessRecordArchiveTargetType(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_archive_target_type"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_archive_target_type(session, vmpp, ArchiveTargetType);

            });
        }

        private void ProcessRecordArchiveTargetConfig(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_archive_target_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_archive_target_config(session, vmpp, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(ArchiveTargetConfig));

            });
        }

        private void ProcessRecordIsAlarmEnabled(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_is_alarm_enabled"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_is_alarm_enabled(session, vmpp, IsAlarmEnabled);

            });
        }

        private void ProcessRecordAlarmConfig(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_alarm_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_alarm_config(session, vmpp, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(AlarmConfig));

            });
        }

        private void ProcessRecordBackupLastRunTime(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_backup_last_run_time"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_backup_last_run_time(session, vmpp, BackupLastRunTime);

            });
        }

        private void ProcessRecordArchiveLastRunTime(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.set_archive_last_run_time"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.set_archive_last_run_time(session, vmpp, ArchiveLastRunTime);

            });
        }

        #endregion
    }
}
