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
    [Cmdlet(VerbsCommon.Remove, "XenVMPPProperty", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VMPP))]
    public class RemoveXenVMPPProperty : XenServerCmdlet
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
        public string BackupSchedule
        {
            get { return _backupSchedule; }
            set
            {
                _backupSchedule = value;
                _backupScheduleIsSpecified = true;
            }
        }
        private string _backupSchedule;
        private bool _backupScheduleIsSpecified;

        [Parameter]
        public string ArchiveTargetConfig
        {
            get { return _archiveTargetConfig; }
            set
            {
                _archiveTargetConfig = value;
                _archiveTargetConfigIsSpecified = true;
            }
        }
        private string _archiveTargetConfig;
        private bool _archiveTargetConfigIsSpecified;

        [Parameter]
        public string ArchiveSchedule
        {
            get { return _archiveSchedule; }
            set
            {
                _archiveSchedule = value;
                _archiveScheduleIsSpecified = true;
            }
        }
        private string _archiveSchedule;
        private bool _archiveScheduleIsSpecified;

        [Parameter]
        public string AlarmConfig
        {
            get { return _alarmConfig; }
            set
            {
                _alarmConfig = value;
                _alarmConfigIsSpecified = true;
            }
        }
        private string _alarmConfig;
        private bool _alarmConfigIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vmpp = ParseVMPP();

            if (_backupScheduleIsSpecified)
                ProcessRecordBackupSchedule(vmpp);
            if (_archiveTargetConfigIsSpecified)
                ProcessRecordArchiveTargetConfig(vmpp);
            if (_archiveScheduleIsSpecified)
                ProcessRecordArchiveSchedule(vmpp);
            if (_alarmConfigIsSpecified)
                ProcessRecordAlarmConfig(vmpp);

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

        private void ProcessRecordBackupSchedule(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.remove_from_backup_schedule"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.remove_from_backup_schedule(session, vmpp, BackupSchedule);

            });
        }

        private void ProcessRecordArchiveTargetConfig(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.remove_from_archive_target_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.remove_from_archive_target_config(session, vmpp, ArchiveTargetConfig);

            });
        }

        private void ProcessRecordArchiveSchedule(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.remove_from_archive_schedule"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.remove_from_archive_schedule(session, vmpp, ArchiveSchedule);

            });
        }

        private void ProcessRecordAlarmConfig(string vmpp)
        {
            if (!ShouldProcess(vmpp, "VMPP.remove_from_alarm_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VMPP.remove_from_alarm_config(session, vmpp, AlarmConfig);

            });
        }

        #endregion
    }
}
