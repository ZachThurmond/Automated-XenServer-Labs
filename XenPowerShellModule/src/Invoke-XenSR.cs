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
    [Cmdlet(VerbsLifecycle.Invoke, "XenSR", SupportsShouldProcess = true)]
    public class InvokeXenSR : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SR SR { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SR> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenSRAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenSRAction.Introduce:
                    _context = new XenSRActionIntroduceDynamicParameters();
                    return _context;
                case XenSRAction.Make:
                    _context = new XenSRActionMakeDynamicParameters();
                    return _context;
                case XenSRAction.Forget:
                    _context = new XenSRActionForgetDynamicParameters();
                    return _context;
                case XenSRAction.Update:
                    _context = new XenSRActionUpdateDynamicParameters();
                    return _context;
                case XenSRAction.Scan:
                    _context = new XenSRActionScanDynamicParameters();
                    return _context;
                case XenSRAction.Probe:
                    _context = new XenSRActionProbeDynamicParameters();
                    return _context;
                case XenSRAction.CreateNewBlob:
                    _context = new XenSRActionCreateNewBlobDynamicParameters();
                    return _context;
                case XenSRAction.AssertCanHostHaStatefile:
                    _context = new XenSRActionAssertCanHostHaStatefileDynamicParameters();
                    return _context;
                case XenSRAction.AssertSupportsDatabaseReplication:
                    _context = new XenSRActionAssertSupportsDatabaseReplicationDynamicParameters();
                    return _context;
                case XenSRAction.EnableDatabaseReplication:
                    _context = new XenSRActionEnableDatabaseReplicationDynamicParameters();
                    return _context;
                case XenSRAction.DisableDatabaseReplication:
                    _context = new XenSRActionDisableDatabaseReplicationDynamicParameters();
                    return _context;
                case XenSRAction.RecordDataSource:
                    _context = new XenSRActionRecordDataSourceDynamicParameters();
                    return _context;
                case XenSRAction.QueryDataSource:
                    _context = new XenSRActionQueryDataSourceDynamicParameters();
                    return _context;
                case XenSRAction.ForgetDataSourceArchives:
                    _context = new XenSRActionForgetDataSourceArchivesDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sr = ParseSR();

            switch (XenAction)
            {
                case XenSRAction.Introduce:
                    ProcessRecordIntroduce(sr);
                    break;
                case XenSRAction.Make:
                    ProcessRecordMake(sr);
                    break;
                case XenSRAction.Forget:
                    ProcessRecordForget(sr);
                    break;
                case XenSRAction.Update:
                    ProcessRecordUpdate(sr);
                    break;
                case XenSRAction.Scan:
                    ProcessRecordScan(sr);
                    break;
                case XenSRAction.Probe:
                    ProcessRecordProbe(sr);
                    break;
                case XenSRAction.CreateNewBlob:
                    ProcessRecordCreateNewBlob(sr);
                    break;
                case XenSRAction.AssertCanHostHaStatefile:
                    ProcessRecordAssertCanHostHaStatefile(sr);
                    break;
                case XenSRAction.AssertSupportsDatabaseReplication:
                    ProcessRecordAssertSupportsDatabaseReplication(sr);
                    break;
                case XenSRAction.EnableDatabaseReplication:
                    ProcessRecordEnableDatabaseReplication(sr);
                    break;
                case XenSRAction.DisableDatabaseReplication:
                    ProcessRecordDisableDatabaseReplication(sr);
                    break;
                case XenSRAction.RecordDataSource:
                    ProcessRecordRecordDataSource(sr);
                    break;
                case XenSRAction.QueryDataSource:
                    ProcessRecordQueryDataSource(sr);
                    break;
                case XenSRAction.ForgetDataSourceArchives:
                    ProcessRecordForgetDataSourceArchives(sr);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSR()
        {
            string sr = null;

            if (SR != null)
                sr = (new XenRef<XenAPI.SR>(SR)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.SR.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    sr = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.SR.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    sr = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.SR with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                sr = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'SR', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    SR));
            }

            return sr;
        }

        private void ProcessRecordIntroduce(string sr)
        {
            if (!ShouldProcess(sr, "SR.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.Type, contxt.ContentType, contxt.Shared, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

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
                    string objRef = XenAPI.SR.introduce(session, contxt.UuidParam, contxt.NameLabel, contxt.NameDescription, contxt.Type, contxt.ContentType, contxt.Shared, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

                    if (PassThru)
                    {
                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordMake(string sr)
        {
            if (!ShouldProcess(sr, "SR.make"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionMakeDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_make(session, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DeviceConfig), contxt.PhysicalSize, contxt.NameLabel, contxt.NameDescription, contxt.Type, contxt.ContentType, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

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
                    string obj = XenAPI.SR.make(session, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DeviceConfig), contxt.PhysicalSize, contxt.NameLabel, contxt.NameDescription, contxt.Type, contxt.ContentType, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordForget(string sr)
        {
            if (!ShouldProcess(sr, "SR.forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_forget(session, sr);

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
                    XenAPI.SR.forget(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUpdate(string sr)
        {
            if (!ShouldProcess(sr, "SR.update"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionUpdateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_update(session, sr);

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
                    XenAPI.SR.update(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordScan(string sr)
        {
            if (!ShouldProcess(sr, "SR.scan"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionScanDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_scan(session, sr);

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
                    XenAPI.SR.scan(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordProbe(string sr)
        {
            if (!ShouldProcess(sr, "SR.probe"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionProbeDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_probe(session, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DeviceConfig), contxt.Type, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

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
                    string obj = XenAPI.SR.probe(session, contxt.XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.DeviceConfig), contxt.Type, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.SmConfig));

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCreateNewBlob(string sr)
        {
            if (!ShouldProcess(sr, "SR.create_new_blob"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionCreateNewBlobDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_create_new_blob(session, sr, contxt.NameParam, contxt.MimeType, contxt.Public);

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
                    string objRef = XenAPI.SR.create_new_blob(session, sr, contxt.NameParam, contxt.MimeType, contxt.Public);

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

        private void ProcessRecordAssertCanHostHaStatefile(string sr)
        {
            if (!ShouldProcess(sr, "SR.assert_can_host_ha_statefile"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionAssertCanHostHaStatefileDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_assert_can_host_ha_statefile(session, sr);

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
                    XenAPI.SR.assert_can_host_ha_statefile(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordAssertSupportsDatabaseReplication(string sr)
        {
            if (!ShouldProcess(sr, "SR.assert_supports_database_replication"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionAssertSupportsDatabaseReplicationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_assert_supports_database_replication(session, sr);

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
                    XenAPI.SR.assert_supports_database_replication(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordEnableDatabaseReplication(string sr)
        {
            if (!ShouldProcess(sr, "SR.enable_database_replication"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionEnableDatabaseReplicationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_enable_database_replication(session, sr);

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
                    XenAPI.SR.enable_database_replication(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableDatabaseReplication(string sr)
        {
            if (!ShouldProcess(sr, "SR.disable_database_replication"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSRActionDisableDatabaseReplicationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_disable_database_replication(session, sr);

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
                    XenAPI.SR.disable_database_replication(session, sr);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRecordDataSource(string sr)
        {
            if (!ShouldProcess(sr, "SR.record_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenSRActionRecordDataSourceDynamicParameters;

                    XenAPI.SR.record_data_source(session, sr, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordQueryDataSource(string sr)
        {
            if (!ShouldProcess(sr, "SR.query_data_source"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenSRActionQueryDataSourceDynamicParameters;

                    double obj = XenAPI.SR.query_data_source(session, sr, contxt.DataSource);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
            });
        }

        private void ProcessRecordForgetDataSourceArchives(string sr)
        {
            if (!ShouldProcess(sr, "SR.forget_data_source_archives"))
                return;

            RunApiCall(()=>
            {var contxt = _context as XenSRActionForgetDataSourceArchivesDynamicParameters;

                    XenAPI.SR.forget_data_source_archives(session, sr, contxt.DataSource);

                    if (PassThru)
                    {
                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);
                    }
            });
        }

        #endregion
    }

    public enum XenSRAction
    {
        Introduce,
        Make,
        Forget,
        Update,
        Scan,
        Probe,
        CreateNewBlob,
        AssertCanHostHaStatefile,
        AssertSupportsDatabaseReplication,
        EnableDatabaseReplication,
        DisableDatabaseReplication,
        RecordDataSource,
        QueryDataSource,
        ForgetDataSourceArchives
    }

    public class XenSRActionIntroduceDynamicParameters : IXenServerDynamicParameter
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
        public string Type { get; set; }

        [Parameter]
        public string ContentType { get; set; }

        [Parameter]
        public bool Shared { get; set; }

        [Parameter]
        public Hashtable SmConfig { get; set; }
       
    }

    public class XenSRActionMakeDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public Hashtable DeviceConfig { get; set; }

        [Parameter]
        public long PhysicalSize { get; set; }

        [Parameter]
        public string NameLabel { get; set; }

        [Parameter]
        public string NameDescription { get; set; }

        [Parameter]
        public string Type { get; set; }

        [Parameter]
        public string ContentType { get; set; }

        [Parameter]
        public Hashtable SmConfig { get; set; }
        
    }

    public class XenSRActionForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionUpdateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionScanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionProbeDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public Hashtable DeviceConfig { get; set; }

        [Parameter]
        public string Type { get; set; }

        [Parameter]
        public Hashtable SmConfig { get; set; }
    
    }

    public class XenSRActionCreateNewBlobDynamicParameters : IXenServerDynamicParameter
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

    public class XenSRActionAssertCanHostHaStatefileDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionAssertSupportsDatabaseReplicationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionEnableDatabaseReplicationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionDisableDatabaseReplicationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenSRActionRecordDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenSRActionQueryDataSourceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

    public class XenSRActionForgetDataSourceArchivesDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string DataSource { get; set; }
 
    }

}
