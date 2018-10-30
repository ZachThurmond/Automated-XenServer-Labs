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
    [Cmdlet(VerbsCommon.Get, "XenTaskProperty", SupportsShouldProcess = false)]
    public class GetXenTaskProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Task Task { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Task> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenTaskProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string task = ParseTask();

            switch (XenProperty)
            {
                case XenTaskProperty.Uuid:
                    ProcessRecordUuid(task);
                    break;
                case XenTaskProperty.NameLabel:
                    ProcessRecordNameLabel(task);
                    break;
                case XenTaskProperty.NameDescription:
                    ProcessRecordNameDescription(task);
                    break;
                case XenTaskProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(task);
                    break;
                case XenTaskProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(task);
                    break;
                case XenTaskProperty.Created:
                    ProcessRecordCreated(task);
                    break;
                case XenTaskProperty.Finished:
                    ProcessRecordFinished(task);
                    break;
                case XenTaskProperty.Status:
                    ProcessRecordStatus(task);
                    break;
                case XenTaskProperty.ResidentOn:
                    ProcessRecordResidentOn(task);
                    break;
                case XenTaskProperty.Progress:
                    ProcessRecordProgress(task);
                    break;
                case XenTaskProperty.Type:
                    ProcessRecordType(task);
                    break;
                case XenTaskProperty.Result:
                    ProcessRecordResult(task);
                    break;
                case XenTaskProperty.ErrorInfo:
                    ProcessRecordErrorInfo(task);
                    break;
                case XenTaskProperty.OtherConfig:
                    ProcessRecordOtherConfig(task);
                    break;
                case XenTaskProperty.SubtaskOf:
                    ProcessRecordSubtaskOf(task);
                    break;
                case XenTaskProperty.Subtasks:
                    ProcessRecordSubtasks(task);
                    break;
                case XenTaskProperty.Backtrace:
                    ProcessRecordBacktrace(task);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseTask()
        {
            string task = null;

            if (Task != null)
                task = (new XenRef<XenAPI.Task>(Task)).opaque_ref;
            else if (Ref != null)
                task = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Task', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Task));
            }

            return task;
        }

        private void ProcessRecordUuid(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_uuid(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_name_label(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_name_description(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string task)
        {
            RunApiCall(()=>
            {
                    List<task_allowed_operations> obj = XenAPI.Task.get_allowed_operations(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string task)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Task.get_current_operations(session, task);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordCreated(string task)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Task.get_created(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFinished(string task)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Task.get_finished(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStatus(string task)
        {
            RunApiCall(()=>
            {
                    task_status_type obj = XenAPI.Task.get_status(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordResidentOn(string task)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Task.get_resident_on(session, task);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProgress(string task)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.Task.get_progress(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_type(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordResult(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_result(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordErrorInfo(string task)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Task.get_error_info(session, task);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string task)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Task.get_other_config(session, task);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSubtaskOf(string task)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Task.get_subtask_of(session, task);

                        XenAPI.Task obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Task.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSubtasks(string task)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Task.get_subtasks(session, task);

                        var records = new List<XenAPI.Task>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Task.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordBacktrace(string task)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Task.get_backtrace(session, task);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenTaskProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        AllowedOperations,
        CurrentOperations,
        Created,
        Finished,
        Status,
        ResidentOn,
        Progress,
        Type,
        Result,
        ErrorInfo,
        OtherConfig,
        SubtaskOf,
        Subtasks,
        Backtrace
    }

}
