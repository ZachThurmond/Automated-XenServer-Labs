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
    [Cmdlet(VerbsLifecycle.Invoke, "XenVM", SupportsShouldProcess = true)]
    public class InvokeXenVM : XenServerCmdlet
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


        [Parameter(Mandatory = true)]
        public XenVMAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenVMAction.Snapshot:
                    _context = new XenVMActionSnapshotDynamicParameters();
                    return _context;
                case XenVMAction.SnapshotWithQuiesce:
                    _context = new XenVMActionSnapshotWithQuiesceDynamicParameters();
                    return _context;
                case XenVMAction.Clone:
                    _context = new XenVMActionCloneDynamicParameters();
                    return _context;
                case XenVMAction.Copy:
                    _context = new XenVMActionCopyDynamicParameters();
                    return _context;
                case XenVMAction.Revert:
                    _context = new XenVMActionRevertDynamicParameters();
                    return _context;
                case XenVMAction.Checkpoint:
                    _context = new XenVMActionCheckpointDynamicParameters();
                    return _context;
                case XenVMAction.Provision:
                    _context = new XenVMActionProvisionDynamicParameters();
                    return _context;
                case XenVMAction.Start:
                    _context = new XenVMActionStartDynamicParameters();
                    return _context;
                case XenVMAction.StartOn:
                    _context = new XenVMActionStartOnDynamicParameters();
                    return _context;
                case XenVMAction.Pause:
                    _context = new XenVMActionPauseDynamicParameters();
                    return _context;
                case XenVMAction.Unpause:
                    _context = new XenVMActionUnpauseDynamicParameters();
                    return _context;
                case XenVMAction.CleanShutdown:
                    _context = new XenVMActionCleanShutdownDynamicParameters();
                    return _context;
                case XenVMAction.Shutdown:
                    _context = new XenVMActionShutdownDynamicParameters();
                    return _context;
                case XenVMAction.CleanReboot:
                    _context = new XenVMActionCleanRebootDynamicParameters();
                    return _context;
                case XenVMAction.HardShutdown:
                    _context = new XenVMActionHardShutdownDynamicParameters();
                    return _context;
                case XenVMAction.PowerStateReset:
                    _context = new XenVMActionPowerStateResetDynamicParameters();
                    return _context;
                case XenVMAction.HardReboot:
                    _context = new XenVMActionHardRebootDynamicParameters();
                    return _context;
                case XenVMAction.Suspend:
                    _context = new XenVMActionSuspendDynamicParameters();
                    return _context;
                case XenVMAction.Resume:
                    _context = new XenVMActionResumeDynamicParameters();
                    return _context;
                case XenVMAction.ResumeOn:
                    _context = new XenVMActionResumeOnDynamicParameters();
                    return _context;
                case XenVMAction.PoolMigrate:
                    _context = new XenVMActionPoolMigrateDynamicParameters();
                    return _context;
                case XenVMAction.ComputeMemoryOverhead:
                    _context = new XenVMActionComputeMemoryOverheadDynamicParameters();
                    return _context;
                case XenVMAction.WaitMemoryTargetLive:
                    _context = new XenVMActionWaitMemoryTargetLiveDynamicParameters();
                    return _context;
                case XenVMAction.SendSysrq:
                    _context = new XenVMActionSendSysrqDynamicParameters();
                    return _context;
                case XenVMAction.SendTrigger:
                    _context = new XenVMActionSendTriggerDynamicParameters();
                    return _context;
                case XenVMAction.MaximiseMemory:
                    _context = new XenVMActionMaximiseMemoryDynamicParameters();
                    return _context;
                case XenVMAction.MigrateSend:
                    _context = new XenVMActionMigrateSendDynamicParameters();
                    return _context;
                case XenVMAction.AssertCanMigrate:
                    _context = new XenVMActionAssertCanMigrateDynamicParameters();
                    return _context;
                case XenVMAction.RecordDataSource:
                    _context = new XenVMActionRecordDataSourceDynamicParameters();
                    return _context;
                case XenVMAction.QueryDataSource:
                    _context = new XenVMActionQueryDataSourceDynamicParameters();
                    return _context;
                case XenVMAction.ForgetDataSourceArchives:
                    _context = new XenVMActionForgetDataSourceArchivesDynamicParameters();
                    return _context;
                case XenVMAction.AssertOperationValid:
                    _context = new XenVMActionAssertOperationValidDynamicParameters();
                    return _context;
                case XenVMAction.UpdateAllowedOperations:
                    _context = new XenVMActionUpdateAllowedOperationsDynamicParameters();
                    return _context;
                case XenVMAction.AssertCanBootHere:
                    _context = new XenVMActionAssertCanBootHereDynamicParameters();
                    return _context;
                case XenVMAction.CreateNewBlob:
                    _context = new XenVMActionCreateNewBlobDynamicParameters();
                    return _context;
                case XenVMAction.AssertAgile:
                    _context = new XenVMActionAssertAgileDynamicParameters();
                    return _context;
                case XenVMAction.RetrieveWlbRecommendations:
                    _context = new XenVMActionRetrieveWlbRecommendationsDynamicParameters();
                    return _context;
                case XenVMAction.CopyBiosStrings:
                    _context = new XenVMActionCopyBiosStringsDynamicParameters();
                    return _context;
                case XenVMAction.AssertCanBeRecovered:
                    _context = new XenVMActionAssertCanBeRecoveredDynamicParameters();
                    return _context;
                case XenVMAction.Recover:
                    _context = new XenVMActionRecoverDynamicParameters();
                    return _context;
                case XenVMAction.ImportConvert:
                    _context = new XenVMActionImportConvertDynamicParameters();
                    return _context;
                case XenVMAction.QueryServices:
                    _context = new XenVMActionQueryServicesDynamicParameters();
                    return _context;
                case XenVMAction.CallPlugin:
                    _context = new XenVMActionCallPluginDynamicParameters();
                    return _context;
                case XenVMAction.Import:
                    _context = new XenVMActionImportDynamicParameters();
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

            switch (XenAction)
            {
                case XenVMAction.Snapshot:
                    ProcessRecordSnapshot(vm);
                    break;
                case XenVMAction.SnapshotWithQuiesce:
                    ProcessRecordSnapshotWithQuiesce(vm);
                    break;
                case XenVMAction.Clone:
                    ProcessRecordClone(vm);
                    break;
                case XenVMAction.Copy:
                    ProcessRecordCopy(vm);
                    break;
                case XenVMAction.Revert:
                    ProcessRecordRevert(vm);
                    break;
                case XenVMAction.Checkpoint:
                    ProcessRecordCheckpoint(vm);
                    break;
                case XenVMAction.Provision:
                    ProcessRecordProvision(vm);
                    break;
                case XenVMAction.Start:
                    ProcessRecordStart(vm);
                    break;
                case XenVMAction.StartOn:
                    ProcessRecordStartOn(vm);
                    break;
                case XenVMAction.Pause:
                    ProcessRecordPause(vm);
                    break;
                case XenVMAction.Unpause:
                    ProcessRecordUnpause(vm);
                    break;
                case XenVMAction.CleanShutdown:
                    ProcessRecordCleanShutdown(vm);
                    break;
                case XenVMAction.Shutdown:
                    ProcessRecordShutdown(vm);
                    break;
                case XenVMAction.CleanReboot:
                    ProcessRecordCleanReboot(vm);
                    break;
                case XenVMAction.HardShutdown:
                    ProcessRecordHardShutdown(vm);
                    break;
                case XenVMAction.PowerStateReset:
                    ProcessRecordPowerStateReset(vm);
                    break;
                case XenVMAction.HardReboot:
                    ProcessRecordHardReboot(vm);
                    break;
                case XenVMAction.Suspend:
                    ProcessRecordSuspend(vm);
                    break;
                case XenVMAction.Resume:
                    ProcessRecordResume(vm);
                    break;
                case XenVMAction.ResumeOn:
                    ProcessRecordResumeOn(vm);
                    break;
                case XenVMAction.PoolMigrate:
                    ProcessRecordPoolMigrate(vm);
                    break;
                case XenVMAction.ComputeMemoryOverhead:
                    ProcessRecordComputeMemoryOverhead(vm);
                    break;
                case XenVMAction.WaitMemoryTargetLive:
                    ProcessRecordWaitMemoryTargetLive(vm);
                    break;
                case XenVMAction.SendSysrq:
                    ProcessRecordSendSysrq(vm);
                    break;
                case XenVMAction.SendTrigger:
                    ProcessRecordSendTrigger(vm);
                    break;
                case XenVMAction.MaximiseMemory:
                    ProcessRecordMaximiseMemory(vm);
                    break;
                case XenVMAction.MigrateSend:
                    ProcessRecordMigrateSend(vm);
                    break;
                case XenVMAction.AssertCanMigrate:
                    ProcessRecordAssertCanMigrate(vm);
                    break;
                case XenVMAction.RecordDataSource:
                    ProcessRecordRecordDataSource(vm);
                    break;
                case XenVMAction.QueryDataSource:
                    ProcessRecordQueryDataSource(vm);
                    break;
                case XenVMAction.ForgetDataSourceArchives:
                    ProcessRecordForgetDataSourceArchives(vm);
                    break;
                case XenVMAction.AssertOperationValid:
                    ProcessRecordAssertOperationValid(vm);
                    break;
                case XenVMAction.UpdateAllowedOperations:
                    ProcessRecordUpdateAllowedOperations(vm);
                    break;
                case XenVMAction.AssertCanBootHere:
                    ProcessRecordAssertCanBootHere(vm);
                    break;
                case XenVMAction.CreateNewBlob:
                    ProcessRecordCreateNewBlob(vm);
                    break;
                case XenVMAction.AssertAgile:
                    ProcessRecordAssertAgile(vm);
                    break;
                case XenVMAction.RetrieveWlbRecommendations:
                    ProcessRecordRetrieveWlbRecommendations(vm);
                    break;
                case XenVMAction.CopyBiosStrings:
                    ProcessRecordCopyBiosStrings(vm);
                    break;
                case XenVMAction.AssertCanBeRecovered:
                    ProcessRecordAssertCanBeRecovered(vm);
                    break;
                case XenVMAction.Recover:
                    ProcessRecordRecover(vm);
                    break;
                case XenVMAction.ImportConvert:
                    ProcessRecordImportConvert(vm);
                    break;
                case XenVMAction.QueryServices:
                    ProcessRecordQueryServices(vm);
                    break;
                case XenVMAction.CallPlugin:
                    ProcessRecordCallPlugin(vm);
                    break;
                case XenVMAction.Import:
                    ProcessRecordImport(vm);
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

        private void ProcessRecordSnapshot(string vm)
        {
            if (!ShouldProcess(vm, "VM.snapshot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionSnapshotDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_snapshot(session, vm, contxt.NewName);

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
                    string objRef = XenAPI.VM.snapshot(session, vm, contxt.NewName);

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
        }

        private void ProcessRecordSnapshotWithQuiesce(string vm)
        {
            if (!ShouldProcess(vm, "VM.snapshot_with_quiesce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionSnapshotWithQuiesceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_snapshot_with_quiesce(session, vm, contxt.NewName);

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
                    string objRef = XenAPI.VM.snapshot_with_quiesce(session, vm, contxt.NewName);

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
        }

        private void ProcessRecordClone(string vm)
        {
            if (!ShouldProcess(vm, "VM.clone"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCloneDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_clone(session, vm, contxt.NewName);

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
                    string objRef = XenAPI.VM.clone(session, vm, contxt.NewName);

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
        }

        private void ProcessRecordCopy(string vm)
        {
            if (!ShouldProcess(vm, "VM.copy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCopyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_copy(session, vm, contxt.NewName, contxt.SR);

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
                    string objRef = XenAPI.VM.copy(session, vm, contxt.NewName, contxt.SR);

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
        }

        private void ProcessRecordRevert(string vm)
        {
            if (!ShouldProcess(vm, "VM.revert"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionRevertDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_revert(session, contxt.Snapshot);

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
                    XenAPI.VM.revert(session, contxt.Snapshot);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCheckpoint(string vm)
        {
            if (!ShouldProcess(vm, "VM.checkpoint"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCheckpointDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_checkpoint(session, vm, contxt.NewName);

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
                    string objRef = XenAPI.VM.checkpoint(session, vm, contxt.NewName);

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
        }

        private void ProcessRecordProvision(string vm)
        {
            if (!ShouldProcess(vm, "VM.provision"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionProvisionDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_provision(session, vm);

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
                    XenAPI.VM.provision(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordStart(string vm)
        {
            if (!ShouldProcess(vm, "VM.start"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionStartDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_start(session, vm, contxt.StartPaused, contxt.Force);

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
                    XenAPI.VM.start(session, vm, contxt.StartPaused, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordStartOn(string vm)
        {
            if (!ShouldProcess(vm, "VM.start_on"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionStartOnDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_start_on(session, vm, contxt.XenHost, contxt.StartPaused, contxt.Force);

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
                    XenAPI.VM.start_on(session, vm, contxt.XenHost, contxt.StartPaused, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPause(string vm)
        {
            if (!ShouldProcess(vm, "VM.pause"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionPauseDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_pause(session, vm);

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
                    XenAPI.VM.pause(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnpause(string vm)
        {
            if (!ShouldProcess(vm, "VM.unpause"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionUnpauseDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_unpause(session, vm);

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
                    XenAPI.VM.unpause(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCleanShutdown(string vm)
        {
            if (!ShouldProcess(vm, "VM.clean_shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCleanShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_clean_shutdown(session, vm);

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
                    XenAPI.VM.clean_shutdown(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordShutdown(string vm)
        {
            if (!ShouldProcess(vm, "VM.shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_shutdown(session, vm);

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
                    XenAPI.VM.shutdown(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCleanReboot(string vm)
        {
            if (!ShouldProcess(vm, "VM.clean_reboot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCleanRebootDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_clean_reboot(session, vm);

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
                    XenAPI.VM.clean_reboot(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHardShutdown(string vm)
        {
            if (!ShouldProcess(vm, "VM.hard_shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionHardShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_hard_shutdown(session, vm);

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
                    XenAPI.VM.hard_shutdown(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPowerStateReset(string vm)
        {
            if (!ShouldProcess(vm, "VM.power_state_reset"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionPowerStateResetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_power_state_reset(session, vm);

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
                    XenAPI.VM.power_state_reset(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHardReboot(string vm)
        {
            if (!ShouldProcess(vm, "VM.hard_reboot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionHardRebootDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_hard_reboot(session, vm);

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
                    XenAPI.VM.hard_reboot(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSuspend(string vm)
        {
            if (!ShouldProcess(vm, "VM.suspend"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionSuspendDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_suspend(session, vm);

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
                    XenAPI.VM.suspend(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordResume(string vm)
        {
            if (!ShouldProcess(vm, "VM.resume"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionResumeDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_resume(session, vm, contxt.StartPaused, contxt.Force);

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
                    XenAPI.VM.resume(session, vm, contxt.StartPaused, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordResumeOn(string vm)
        {
            if (!ShouldProcess(vm, "VM.resume_on"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionResumeOnDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_resume_on(session, vm, contxt.XenHost, contxt.StartPaused, contxt.Force);

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
                    XenAPI.VM.resume_on(session, vm, contxt.XenHost, contxt.StartPaused, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolMigrate(string vm)
        {
            if (!ShouldProcess(vm, "VM.pool_migrate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionPoolMigrateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_pool_migrate(session, vm, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

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
                    XenAPI.VM.pool_migrate(session, vm, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordComputeMemoryOverhead(string vm)
        {
            if (!ShouldProcess(vm, "VM.compute_memory_overhead"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionComputeMemoryOverheadDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_compute_memory_overhead(session, vm);

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
                    long obj = XenAPI.VM.compute_memory_overhead(session, vm);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordWaitMemoryTargetLive(string vm)
        {
            if (!ShouldProcess(vm, "VM.wait_memory_target_live"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionWaitMemoryTargetLiveDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_wait_memory_target_live(session, vm);

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
                    XenAPI.VM.wait_memory_target_live(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSendSysrq(string vm)
        {
            if (!ShouldProcess(vm, "VM.send_sysrq"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionSendSysrqDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_send_sysrq(session, vm, contxt.Key);

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
                    XenAPI.VM.send_sysrq(session, vm, contxt.Key);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordSendTrigger(string vm)
        {
            if (!ShouldProcess(vm, "VM.send_trigger"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionSendTriggerDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_send_trigger(session, vm, contxt.Trigger);

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
                    XenAPI.VM.send_trigger(session, vm, contxt.Trigger);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordMaximiseMemory(string vm)
        {
            if (!ShouldProcess(vm, "VM.maximise_memory"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionMaximiseMemoryDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_maximise_memory(session, vm, contxt.Total, contxt.Approximate);

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
                    long obj = XenAPI.VM.maximise_memory(session, vm, contxt.Total, contxt.Approximate);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordMigrateSend(string vm)
        {
            if (!ShouldProcess(vm, "VM.migrate_send"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionMigrateSendDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_migrate_send(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Dest), contxt.Live, CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VDI>, XenRef<XenAPI.SR>>(contxt.VdiMap), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VIF>, XenRef<XenAPI.Network>>(contxt.VifMap), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VGPU>, XenRef<XenAPI.GPU_group>>(contxt.VgpuMap));

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
                    string objRef = XenAPI.VM.migrate_send(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Dest), contxt.Live, CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VDI>, XenRef<XenAPI.SR>>(contxt.VdiMap), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VIF>, XenRef<XenAPI.Network>>(contxt.VifMap), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VGPU>, XenRef<XenAPI.GPU_group>>(contxt.VgpuMap));

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
        }

        private void ProcessRecordAssertCanMigrate(string vm)
        {
            if (!ShouldProcess(vm, "VM.assert_can_migrate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionAssertCanMigrateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_assert_can_migrate(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Dest), contxt.Live, CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VDI>, XenRef<XenAPI.SR>>(contxt.VdiMap), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VIF>, XenRef<XenAPI.Network>>(contxt.VifMap), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VGPU>, XenRef<XenAPI.GPU_group>>(contxt.VgpuMap));

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
                    XenAPI.VM.assert_can_migrate(session, vm, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Dest), contxt.Live, CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VDI>, XenRef<XenAPI.SR>>(contxt.VdiMap), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VIF>, XenRef<XenAPI.Network>>(contxt.VifMap), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options), CommonCmdletFunctions.ConvertHashTableToDictionary<XenRef<XenAPI.VGPU>, XenRef<XenAPI.GPU_group>>(contxt.VgpuMap));

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRecordDataSource(string vm)
        {
            if (!ShouldProcess(vm, "VM.record_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenVMActionRecordDataSourceDynamicParameters;

                    XenAPI.VM.record_data_source(session, vm, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordQueryDataSource(string vm)
        {
            if (!ShouldProcess(vm, "VM.query_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenVMActionQueryDataSourceDynamicParameters;

                    double obj = XenAPI.VM.query_data_source(session, vm, contxt.DataSource);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordForgetDataSourceArchives(string vm)
        {
            if (!ShouldProcess(vm, "VM.forget_data_source_archives"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenVMActionForgetDataSourceArchivesDynamicParameters;

                    XenAPI.VM.forget_data_source_archives(session, vm, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordAssertOperationValid(string vm)
        {
            if (!ShouldProcess(vm, "VM.assert_operation_valid"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionAssertOperationValidDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_assert_operation_valid(session, vm, contxt.Op);

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
                    XenAPI.VM.assert_operation_valid(session, vm, contxt.Op);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUpdateAllowedOperations(string vm)
        {
            if (!ShouldProcess(vm, "VM.update_allowed_operations"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionUpdateAllowedOperationsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_update_allowed_operations(session, vm);

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
                    XenAPI.VM.update_allowed_operations(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordAssertCanBootHere(string vm)
        {
            if (!ShouldProcess(vm, "VM.assert_can_boot_here"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionAssertCanBootHereDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_assert_can_boot_here(session, vm, contxt.XenHost);

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
                    XenAPI.VM.assert_can_boot_here(session, vm, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCreateNewBlob(string vm)
        {
            if (!ShouldProcess(vm, "VM.create_new_blob"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCreateNewBlobDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_create_new_blob(session, vm, contxt.NameParam, contxt.MimeType, contxt.Public);

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
                    string objRef = XenAPI.VM.create_new_blob(session, vm, contxt.NameParam, contxt.MimeType, contxt.Public);

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

        private void ProcessRecordAssertAgile(string vm)
        {
            if (!ShouldProcess(vm, "VM.assert_agile"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionAssertAgileDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_assert_agile(session, vm);

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
                    XenAPI.VM.assert_agile(session, vm);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRetrieveWlbRecommendations(string vm)
        {
            if (!ShouldProcess(vm, "VM.retrieve_wlb_recommendations"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionRetrieveWlbRecommendationsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_retrieve_wlb_recommendations(session, vm);

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
                    var dict = XenAPI.VM.retrieve_wlb_recommendations(session, vm);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordCopyBiosStrings(string vm)
        {
            if (!ShouldProcess(vm, "VM.copy_bios_strings"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCopyBiosStringsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_copy_bios_strings(session, vm, contxt.XenHost);

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
                    XenAPI.VM.copy_bios_strings(session, vm, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordAssertCanBeRecovered(string vm)
        {
            if (!ShouldProcess(vm, "VM.assert_can_be_recovered"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionAssertCanBeRecoveredDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_assert_can_be_recovered(session, vm, contxt.SessionTo);

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
                    XenAPI.VM.assert_can_be_recovered(session, vm, contxt.SessionTo);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRecover(string vm)
        {
            if (!ShouldProcess(vm, "VM.recover"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionRecoverDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_recover(session, vm, contxt.SessionTo, contxt.Force);

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
                    XenAPI.VM.recover(session, vm, contxt.SessionTo, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordImportConvert(string vm)
        {
            if (!ShouldProcess(vm, "VM.import_convert"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionImportConvertDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_import_convert(session, contxt.Type, contxt.Username, contxt.Password, contxt.SR, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.RemoteConfig));

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
                    XenAPI.VM.import_convert(session, contxt.Type, contxt.Username, contxt.Password, contxt.SR, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.RemoteConfig));

                    if (PassThru)
                    {
                        var obj = XenAPI.VM.get_record(session, vm);
                        if (obj != null)
                            obj.opaque_ref = vm;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordQueryServices(string vm)
        {
            if (!ShouldProcess(vm, "VM.query_services"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionQueryServicesDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_query_services(session, vm);

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
                    var dict = XenAPI.VM.query_services(session, vm);

                    if (PassThru)
                    {
                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                    }
                }

            });
        }

        private void ProcessRecordCallPlugin(string vm)
        {
            if (!ShouldProcess(vm, "VM.call_plugin"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionCallPluginDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_call_plugin(session, vm, contxt.Plugin, contxt.Fn, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Args));

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
                    string obj = XenAPI.VM.call_plugin(session, vm, contxt.Plugin, contxt.Fn, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Args));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordImport(string vm)
        {
            if (!ShouldProcess(vm, "VM.import"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMActionImportDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM.async_import(session, contxt.Url_, contxt.SR, contxt.FullRestore, contxt.Force);

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
                    var refs = XenAPI.VM.import(session, contxt.Url_, contxt.SR, contxt.FullRestore, contxt.Force);

                    if (PassThru)
                    {
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
                    }
                }

            });
        }

        #endregion
    }

    public enum XenVMAction
    {
        Snapshot,
        SnapshotWithQuiesce,
        Clone,
        Copy,
        Revert,
        Checkpoint,
        Provision,
        Start,
        StartOn,
        Pause,
        Unpause,
        CleanShutdown,
        Shutdown,
        CleanReboot,
        HardShutdown,
        PowerStateReset,
        HardReboot,
        Suspend,
        Resume,
        ResumeOn,
        PoolMigrate,
        ComputeMemoryOverhead,
        WaitMemoryTargetLive,
        SendSysrq,
        SendTrigger,
        MaximiseMemory,
        MigrateSend,
        AssertCanMigrate,
        RecordDataSource,
        QueryDataSource,
        ForgetDataSourceArchives,
        AssertOperationValid,
        UpdateAllowedOperations,
        AssertCanBootHere,
        CreateNewBlob,
        AssertAgile,
        RetrieveWlbRecommendations,
        CopyBiosStrings,
        AssertCanBeRecovered,
        Recover,
        ImportConvert,
        QueryServices,
        CallPlugin,
        Import
    }

    public class XenVMActionSnapshotDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NewName { get; set; }
 
    }

    public class XenVMActionSnapshotWithQuiesceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NewName { get; set; }
 
    }

    public class XenVMActionCloneDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NewName { get; set; }
 
    }

    public class XenVMActionCopyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NewName { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }
  
    }

    public class XenVMActionRevertDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VM> Snapshot { get; set; }
 
    }

    public class XenVMActionCheckpointDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string NewName { get; set; }
 
    }

    public class XenVMActionProvisionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionStartDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public bool StartPaused { get; set; }

        [Parameter]
        public bool Force { get; set; }
  
    }

    public class XenVMActionStartOnDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public bool StartPaused { get; set; }

        [Parameter]
        public bool Force { get; set; }
   
    }

    public class XenVMActionPauseDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionUnpauseDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionCleanShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionCleanRebootDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionHardShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionPowerStateResetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionHardRebootDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionSuspendDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionResumeDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public bool StartPaused { get; set; }

        [Parameter]
        public bool Force { get; set; }
  
    }

    public class XenVMActionResumeOnDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public bool StartPaused { get; set; }

        [Parameter]
        public bool Force { get; set; }
   
    }

    public class XenVMActionPoolMigrateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }
  
    }

    public class XenVMActionComputeMemoryOverheadDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionWaitMemoryTargetLiveDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionSendSysrqDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Key { get; set; }
 
    }

    public class XenVMActionSendTriggerDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Trigger { get; set; }
 
    }

    public class XenVMActionMaximiseMemoryDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public long Total { get; set; }

        [Parameter]
        public bool Approximate { get; set; }
  
    }

    public class XenVMActionMigrateSendDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public Hashtable Dest { get; set; }

        [Parameter]
        public bool Live { get; set; }

        [Parameter]
        public Hashtable VdiMap { get; set; }

        [Parameter]
        public Hashtable VifMap { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }

        [Parameter]
        public Hashtable VgpuMap { get; set; }
      
    }

    public class XenVMActionAssertCanMigrateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public Hashtable Dest { get; set; }

        [Parameter]
        public bool Live { get; set; }

        [Parameter]
        public Hashtable VdiMap { get; set; }

        [Parameter]
        public Hashtable VifMap { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }

        [Parameter]
        public Hashtable VgpuMap { get; set; }
      
    }

    public class XenVMActionRecordDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenVMActionQueryDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenVMActionForgetDataSourceArchivesDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenVMActionAssertOperationValidDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public vm_operations Op { get; set; }
 
    }

    public class XenVMActionUpdateAllowedOperationsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionAssertCanBootHereDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenVMActionCreateNewBlobDynamicParameters : IXenServerDynamicParameter
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

    public class XenVMActionAssertAgileDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionRetrieveWlbRecommendationsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionCopyBiosStringsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenVMActionAssertCanBeRecoveredDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }
 
    }

    public class XenVMActionRecoverDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }

        [Parameter]
        public bool Force { get; set; }
  
    }

    public class XenVMActionImportConvertDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Type { get; set; }

        [Parameter]
        public string Username { get; set; }

        [Parameter]
        public string Password { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public Hashtable RemoteConfig { get; set; }
     
    }

    public class XenVMActionQueryServicesDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMActionCallPluginDynamicParameters : IXenServerDynamicParameter
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

    public class XenVMActionImportDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Url_ { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public bool FullRestore { get; set; }

        [Parameter]
        public bool Force { get; set; }
    
    }

}
