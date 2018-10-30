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
    [Cmdlet(VerbsLifecycle.Invoke, "XenVDI", SupportsShouldProcess = true)]
    public class InvokeXenVDI : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VDI VDI { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VDI> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenVDIAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenVDIAction.Snapshot:
                    _context = new XenVDIActionSnapshotDynamicParameters();
                    return _context;
                case XenVDIAction.Clone:
                    _context = new XenVDIActionCloneDynamicParameters();
                    return _context;
                case XenVDIAction.Resize:
                    _context = new XenVDIActionResizeDynamicParameters();
                    return _context;
                case XenVDIAction.ResizeOnline:
                    _context = new XenVDIActionResizeOnlineDynamicParameters();
                    return _context;
                case XenVDIAction.Introduce:
                    _context = new XenVDIActionIntroduceDynamicParameters();
                    return _context;
                case XenVDIAction.DbIntroduce:
                    _context = new XenVDIActionDbIntroduceDynamicParameters();
                    return _context;
                case XenVDIAction.DbForget:
                    _context = new XenVDIActionDbForgetDynamicParameters();
                    return _context;
                case XenVDIAction.Update:
                    _context = new XenVDIActionUpdateDynamicParameters();
                    return _context;
                case XenVDIAction.Copy:
                    _context = new XenVDIActionCopyDynamicParameters();
                    return _context;
                case XenVDIAction.Forget:
                    _context = new XenVDIActionForgetDynamicParameters();
                    return _context;
                case XenVDIAction.OpenDatabase:
                    _context = new XenVDIActionOpenDatabaseDynamicParameters();
                    return _context;
                case XenVDIAction.ReadDatabasePoolUuid:
                    _context = new XenVDIActionReadDatabasePoolUuidDynamicParameters();
                    return _context;
                case XenVDIAction.PoolMigrate:
                    _context = new XenVDIActionPoolMigrateDynamicParameters();
                    return _context;
                case XenVDIAction.EnableCbt:
                    _context = new XenVDIActionEnableCbtDynamicParameters();
                    return _context;
                case XenVDIAction.DisableCbt:
                    _context = new XenVDIActionDisableCbtDynamicParameters();
                    return _context;
                case XenVDIAction.DataDestroy:
                    _context = new XenVDIActionDataDestroyDynamicParameters();
                    return _context;
                case XenVDIAction.ListChangedBlocks:
                    _context = new XenVDIActionListChangedBlocksDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vdi = ParseVDI();

            switch (XenAction)
            {
                case XenVDIAction.Snapshot:
                    ProcessRecordSnapshot(vdi);
                    break;
                case XenVDIAction.Clone:
                    ProcessRecordClone(vdi);
                    break;
                case XenVDIAction.Resize:
                    ProcessRecordResize(vdi);
                    break;
                case XenVDIAction.ResizeOnline:
                    ProcessRecordResizeOnline(vdi);
                    break;
                case XenVDIAction.Introduce:
                    ProcessRecordIntroduce(vdi);
                    break;
                case XenVDIAction.DbIntroduce:
                    ProcessRecordDbIntroduce(vdi);
                    break;
                case XenVDIAction.DbForget:
                    ProcessRecordDbForget(vdi);
                    break;
                case XenVDIAction.Update:
                    ProcessRecordUpdate(vdi);
                    break;
                case XenVDIAction.Copy:
                    ProcessRecordCopy(vdi);
                    break;
                case XenVDIAction.Forget:
                    ProcessRecordForget(vdi);
                    break;
                case XenVDIAction.OpenDatabase:
                    ProcessRecordOpenDatabase(vdi);
                    break;
                case XenVDIAction.ReadDatabasePoolUuid:
                    ProcessRecordReadDatabasePoolUuid(vdi);
                    break;
                case XenVDIAction.PoolMigrate:
                    ProcessRecordPoolMigrate(vdi);
                    break;
                case XenVDIAction.EnableCbt:
                    ProcessRecordEnableCbt(vdi);
                    break;
                case XenVDIAction.DisableCbt:
                    ProcessRecordDisableCbt(vdi);
                    break;
                case XenVDIAction.DataDestroy:
                    ProcessRecordDataDestroy(vdi);
                    break;
                case XenVDIAction.ListChangedBlocks:
                    ProcessRecordListChangedBlocks(vdi);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVDI()
        {
            string vdi = null;

            if (VDI != null)
                vdi = (new XenRef<XenAPI.VDI>(VDI)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VDI.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vdi = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VDI.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vdi = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VDI with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                vdi = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VDI', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VDI));
            }

            return vdi;
        }

        private void ProcessRecordSnapshot(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.snapshot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionSnapshotDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_snapshot(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DriverParams));

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
                    string objRef = XenAPI.VDI.snapshot(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DriverParams));

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordClone(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.clone"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionCloneDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_clone(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DriverParams));

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
                    string objRef = XenAPI.VDI.clone(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DriverParams));

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordResize(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.resize"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionResizeDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_resize(session, vdi, contxt.Size);

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
                    XenAPI.VDI.resize(session, vdi, contxt.Size);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordResizeOnline(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.resize_online"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionResizeOnlineDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_resize_online(session, vdi, contxt.Size);

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
                    XenAPI.VDI.resize_online(session, vdi, contxt.Size);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordIntroduce(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.SR, contxt.Type, contxt.Sharable, contxt.ReadOnly, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.Location, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.XenstoreData), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig), contxt.Managed, contxt.VirtualSize, contxt.PhysicalUtilisation, contxt.MetadataOfPool, contxt.IsASnapshot, contxt.SnapshotTime, contxt.SnapshotOf);

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
                    string objRef = XenAPI.VDI.introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.SR, contxt.Type, contxt.Sharable, contxt.ReadOnly, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.Location, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.XenstoreData), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig), contxt.Managed, contxt.VirtualSize, contxt.PhysicalUtilisation, contxt.MetadataOfPool, contxt.IsASnapshot, contxt.SnapshotTime, contxt.SnapshotOf);

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDbIntroduce(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.db_introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionDbIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_db_introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.SR, contxt.Type, contxt.Sharable, contxt.ReadOnly, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.Location, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.XenstoreData), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig), contxt.Managed, contxt.VirtualSize, contxt.PhysicalUtilisation, contxt.MetadataOfPool, contxt.IsASnapshot, contxt.SnapshotTime, contxt.SnapshotOf, contxt.CbtEnabled);

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
                    string objRef = XenAPI.VDI.db_introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.SR, contxt.Type, contxt.Sharable, contxt.ReadOnly, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.Location, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.XenstoreData), CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig), contxt.Managed, contxt.VirtualSize, contxt.PhysicalUtilisation, contxt.MetadataOfPool, contxt.IsASnapshot, contxt.SnapshotTime, contxt.SnapshotOf, contxt.CbtEnabled);

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDbForget(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.db_forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionDbForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_db_forget(session, vdi);

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
                    XenAPI.VDI.db_forget(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUpdate(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.update"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionUpdateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_update(session, vdi);

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
                    XenAPI.VDI.update(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCopy(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.copy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionCopyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_copy(session, vdi, contxt.SR, contxt.BaseVdi, contxt.IntoVdi);

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
                    string objRef = XenAPI.VDI.copy(session, vdi, contxt.SR, contxt.BaseVdi, contxt.IntoVdi);

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordForget(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_forget(session, vdi);

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
                    XenAPI.VDI.forget(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordOpenDatabase(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.open_database"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionOpenDatabaseDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_open_database(session, vdi);

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
                    string objRef = XenAPI.VDI.open_database(session, vdi);

                    if (PassThru)
                    {
                        XenAPI.Session obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Session.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordReadDatabasePoolUuid(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.read_database_pool_uuid"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionReadDatabasePoolUuidDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_read_database_pool_uuid(session, vdi);

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
                    string obj = XenAPI.VDI.read_database_pool_uuid(session, vdi);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolMigrate(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.pool_migrate"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionPoolMigrateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_pool_migrate(session, vdi, contxt.SR, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

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
                    string objRef = XenAPI.VDI.pool_migrate(session, vdi, contxt.SR, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Options));

                    if (PassThru)
                    {
                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableCbt(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.enable_cbt"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionEnableCbtDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_enable_cbt(session, vdi);

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
                    XenAPI.VDI.enable_cbt(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableCbt(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.disable_cbt"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionDisableCbtDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_disable_cbt(session, vdi);

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
                    XenAPI.VDI.disable_cbt(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDataDestroy(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.data_destroy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionDataDestroyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_data_destroy(session, vdi);

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
                    XenAPI.VDI.data_destroy(session, vdi);

                    if (PassThru)
                    {
                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordListChangedBlocks(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.list_changed_blocks"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVDIActionListChangedBlocksDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_list_changed_blocks(session, contxt.VdiFrom, contxt.VdiTo);

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
                    string obj = XenAPI.VDI.list_changed_blocks(session, contxt.VdiFrom, contxt.VdiTo);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenVDIAction
    {
        Snapshot,
        Clone,
        Resize,
        ResizeOnline,
        Introduce,
        DbIntroduce,
        DbForget,
        Update,
        Copy,
        Forget,
        OpenDatabase,
        ReadDatabasePoolUuid,
        PoolMigrate,
        EnableCbt,
        DisableCbt,
        DataDestroy,
        ListChangedBlocks
    }

    public class XenVDIActionSnapshotDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public Hashtable DriverParams { get; set; }
 
    }

    public class XenVDIActionCloneDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public Hashtable DriverParams { get; set; }
 
    }

    public class XenVDIActionResizeDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public long Size { get; set; }
 
    }

    public class XenVDIActionResizeOnlineDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public long Size { get; set; }
 
    }

    public class XenVDIActionIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string UuidParam { get; set; }

        [Parameter]
        public string NameLabel { get; set; }

        [Parameter]
        public string NameDescription { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public vdi_type Type { get; set; }

        [Parameter]
        public bool Sharable { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public Hashtable OtherConfig { get; set; }

        [Parameter]
        public string Location { get; set; }

        [Parameter]
        public Hashtable XenstoreData { get; set; }

        [Parameter]
        public Hashtable SmConfig { get; set; }

        [Parameter]
        public bool Managed { get; set; }

        [Parameter]
        public long VirtualSize { get; set; }

        [Parameter]
        public long PhysicalUtilisation { get; set; }

        [Parameter]
        public XenRef<XenAPI.Pool> MetadataOfPool { get; set; }

        [Parameter]
        public bool IsASnapshot { get; set; }

        [Parameter]
        public DateTime SnapshotTime { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> SnapshotOf { get; set; }
                  
    }

    public class XenVDIActionDbIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string UuidParam { get; set; }

        [Parameter]
        public string NameLabel { get; set; }

        [Parameter]
        public string NameDescription { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public vdi_type Type { get; set; }

        [Parameter]
        public bool Sharable { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public Hashtable OtherConfig { get; set; }

        [Parameter]
        public string Location { get; set; }

        [Parameter]
        public Hashtable XenstoreData { get; set; }

        [Parameter]
        public Hashtable SmConfig { get; set; }

        [Parameter]
        public bool Managed { get; set; }

        [Parameter]
        public long VirtualSize { get; set; }

        [Parameter]
        public long PhysicalUtilisation { get; set; }

        [Parameter]
        public XenRef<XenAPI.Pool> MetadataOfPool { get; set; }

        [Parameter]
        public bool IsASnapshot { get; set; }

        [Parameter]
        public DateTime SnapshotTime { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> SnapshotOf { get; set; }

        [Parameter]
        public bool CbtEnabled { get; set; }
                   
    }

    public class XenVDIActionDbForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionUpdateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionCopyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> BaseVdi { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> IntoVdi { get; set; }
   
    }

    public class XenVDIActionForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionOpenDatabaseDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionReadDatabasePoolUuidDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionPoolMigrateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public Hashtable Options { get; set; }
  
    }

    public class XenVDIActionEnableCbtDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionDisableCbtDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionDataDestroyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVDIActionListChangedBlocksDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> VdiFrom { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> VdiTo { get; set; }
  
    }

}
