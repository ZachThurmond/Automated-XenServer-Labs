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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPoolUpdate", SupportsShouldProcess = true)]
    public class InvokeXenPoolUpdate : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool_update PoolUpdate { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool_update> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenPoolUpdateAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPoolUpdateAction.Introduce:
                    _context = new XenPoolUpdateActionIntroduceDynamicParameters();
                    return _context;
                case XenPoolUpdateAction.Precheck:
                    _context = new XenPoolUpdateActionPrecheckDynamicParameters();
                    return _context;
                case XenPoolUpdateAction.Apply:
                    _context = new XenPoolUpdateActionApplyDynamicParameters();
                    return _context;
                case XenPoolUpdateAction.PoolApply:
                    _context = new XenPoolUpdateActionPoolApplyDynamicParameters();
                    return _context;
                case XenPoolUpdateAction.PoolClean:
                    _context = new XenPoolUpdateActionPoolCleanDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool_update = ParsePoolUpdate();

            switch (XenAction)
            {
                case XenPoolUpdateAction.Introduce:
                    ProcessRecordIntroduce(pool_update);
                    break;
                case XenPoolUpdateAction.Precheck:
                    ProcessRecordPrecheck(pool_update);
                    break;
                case XenPoolUpdateAction.Apply:
                    ProcessRecordApply(pool_update);
                    break;
                case XenPoolUpdateAction.PoolApply:
                    ProcessRecordPoolApply(pool_update);
                    break;
                case XenPoolUpdateAction.PoolClean:
                    ProcessRecordPoolClean(pool_update);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePoolUpdate()
        {
            string pool_update = null;

            if (PoolUpdate != null)
                pool_update = (new XenRef<XenAPI.Pool_update>(PoolUpdate)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Pool_update.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pool_update = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.Pool_update.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    pool_update = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.Pool_update with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                pool_update = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PoolUpdate', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PoolUpdate));
            }

            return pool_update;
        }

        private void ProcessRecordIntroduce(string pool_update)
        {
            if (!ShouldProcess(pool_update, "Pool_update.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolUpdateActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_update.async_introduce(session, contxt.VDI);

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
                    string objRef = XenAPI.Pool_update.introduce(session, contxt.VDI);

                    if (PassThru)
                    {
                        XenAPI.Pool_update obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Pool_update.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPrecheck(string pool_update)
        {
            if (!ShouldProcess(pool_update, "Pool_update.precheck"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolUpdateActionPrecheckDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_update.async_precheck(session, pool_update, contxt.XenHost);

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
                    livepatch_status obj = XenAPI.Pool_update.precheck(session, pool_update, contxt.XenHost);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordApply(string pool_update)
        {
            if (!ShouldProcess(pool_update, "Pool_update.apply"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolUpdateActionApplyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_update.async_apply(session, pool_update, contxt.XenHost);

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
                    XenAPI.Pool_update.apply(session, pool_update, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_update.get_record(session, pool_update);
                        if (obj != null)
                            obj.opaque_ref = pool_update;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolApply(string pool_update)
        {
            if (!ShouldProcess(pool_update, "Pool_update.pool_apply"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolUpdateActionPoolApplyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_update.async_pool_apply(session, pool_update);

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
                    XenAPI.Pool_update.pool_apply(session, pool_update);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_update.get_record(session, pool_update);
                        if (obj != null)
                            obj.opaque_ref = pool_update;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolClean(string pool_update)
        {
            if (!ShouldProcess(pool_update, "Pool_update.pool_clean"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolUpdateActionPoolCleanDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_update.async_pool_clean(session, pool_update);

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
                    XenAPI.Pool_update.pool_clean(session, pool_update);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_update.get_record(session, pool_update);
                        if (obj != null)
                            obj.opaque_ref = pool_update;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPoolUpdateAction
    {
        Introduce,
        Precheck,
        Apply,
        PoolApply,
        PoolClean
    }

    public class XenPoolUpdateActionIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> VDI { get; set; }
 
    }

    public class XenPoolUpdateActionPrecheckDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolUpdateActionApplyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolUpdateActionPoolApplyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolUpdateActionPoolCleanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
