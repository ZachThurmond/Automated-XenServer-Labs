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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPoolPatch", SupportsShouldProcess = true)]
    public class InvokeXenPoolPatch : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool_patch PoolPatch { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool_patch> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenPoolPatchAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPoolPatchAction.Apply:
                    _context = new XenPoolPatchActionApplyDynamicParameters();
                    return _context;
                case XenPoolPatchAction.PoolApply:
                    _context = new XenPoolPatchActionPoolApplyDynamicParameters();
                    return _context;
                case XenPoolPatchAction.Precheck:
                    _context = new XenPoolPatchActionPrecheckDynamicParameters();
                    return _context;
                case XenPoolPatchAction.Clean:
                    _context = new XenPoolPatchActionCleanDynamicParameters();
                    return _context;
                case XenPoolPatchAction.PoolClean:
                    _context = new XenPoolPatchActionPoolCleanDynamicParameters();
                    return _context;
                case XenPoolPatchAction.CleanOnHost:
                    _context = new XenPoolPatchActionCleanOnHostDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool_patch = ParsePoolPatch();

            switch (XenAction)
            {
                case XenPoolPatchAction.Apply:
                    ProcessRecordApply(pool_patch);
                    break;
                case XenPoolPatchAction.PoolApply:
                    ProcessRecordPoolApply(pool_patch);
                    break;
                case XenPoolPatchAction.Precheck:
                    ProcessRecordPrecheck(pool_patch);
                    break;
                case XenPoolPatchAction.Clean:
                    ProcessRecordClean(pool_patch);
                    break;
                case XenPoolPatchAction.PoolClean:
                    ProcessRecordPoolClean(pool_patch);
                    break;
                case XenPoolPatchAction.CleanOnHost:
                    ProcessRecordCleanOnHost(pool_patch);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePoolPatch()
        {
            string pool_patch = null;

            if (PoolPatch != null)
                pool_patch = (new XenRef<XenAPI.Pool_patch>(PoolPatch)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Pool_patch.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pool_patch = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.Pool_patch.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    pool_patch = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.Pool_patch with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                pool_patch = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PoolPatch', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PoolPatch));
            }

            return pool_patch;
        }

        private void ProcessRecordApply(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.apply"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionApplyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_apply(session, pool_patch, contxt.XenHost);

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
                    string obj = XenAPI.Pool_patch.apply(session, pool_patch, contxt.XenHost);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolApply(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.pool_apply"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionPoolApplyDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_pool_apply(session, pool_patch);

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
                    XenAPI.Pool_patch.pool_apply(session, pool_patch);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_patch.get_record(session, pool_patch);
                        if (obj != null)
                            obj.opaque_ref = pool_patch;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPrecheck(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.precheck"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionPrecheckDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_precheck(session, pool_patch, contxt.XenHost);

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
                    string obj = XenAPI.Pool_patch.precheck(session, pool_patch, contxt.XenHost);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordClean(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.clean"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionCleanDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_clean(session, pool_patch);

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
                    XenAPI.Pool_patch.clean(session, pool_patch);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_patch.get_record(session, pool_patch);
                        if (obj != null)
                            obj.opaque_ref = pool_patch;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPoolClean(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.pool_clean"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionPoolCleanDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_pool_clean(session, pool_patch);

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
                    XenAPI.Pool_patch.pool_clean(session, pool_patch);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_patch.get_record(session, pool_patch);
                        if (obj != null)
                            obj.opaque_ref = pool_patch;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCleanOnHost(string pool_patch)
        {
            if (!ShouldProcess(pool_patch, "Pool_patch.clean_on_host"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPoolPatchActionCleanOnHostDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool_patch.async_clean_on_host(session, pool_patch, contxt.XenHost);

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
                    XenAPI.Pool_patch.clean_on_host(session, pool_patch, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.Pool_patch.get_record(session, pool_patch);
                        if (obj != null)
                            obj.opaque_ref = pool_patch;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPoolPatchAction
    {
        Apply,
        PoolApply,
        Precheck,
        Clean,
        PoolClean,
        CleanOnHost
    }

    public class XenPoolPatchActionApplyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolPatchActionPoolApplyDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolPatchActionPrecheckDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPoolPatchActionCleanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolPatchActionPoolCleanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPoolPatchActionCleanOnHostDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

}
