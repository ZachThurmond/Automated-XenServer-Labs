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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPGPU", SupportsShouldProcess = true)]
    public class InvokeXenPGPU : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PGPU PGPU { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PGPU> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenPGPUAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPGPUAction.EnableDom0Access:
                    _context = new XenPGPUActionEnableDom0AccessDynamicParameters();
                    return _context;
                case XenPGPUAction.DisableDom0Access:
                    _context = new XenPGPUActionDisableDom0AccessDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pgpu = ParsePGPU();

            switch (XenAction)
            {
                case XenPGPUAction.EnableDom0Access:
                    ProcessRecordEnableDom0Access(pgpu);
                    break;
                case XenPGPUAction.DisableDom0Access:
                    ProcessRecordDisableDom0Access(pgpu);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePGPU()
        {
            string pgpu = null;

            if (PGPU != null)
                pgpu = (new XenRef<XenAPI.PGPU>(PGPU)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.PGPU.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pgpu = xenRef.opaque_ref;
            }
            else if (Ref != null)
                pgpu = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PGPU', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PGPU));
            }

            return pgpu;
        }

        private void ProcessRecordEnableDom0Access(string pgpu)
        {
            if (!ShouldProcess(pgpu, "PGPU.enable_dom0_access"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPGPUActionEnableDom0AccessDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PGPU.async_enable_dom0_access(session, pgpu);

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
                    pgpu_dom0_access obj = XenAPI.PGPU.enable_dom0_access(session, pgpu);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDisableDom0Access(string pgpu)
        {
            if (!ShouldProcess(pgpu, "PGPU.disable_dom0_access"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPGPUActionDisableDom0AccessDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PGPU.async_disable_dom0_access(session, pgpu);

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
                    pgpu_dom0_access obj = XenAPI.PGPU.disable_dom0_access(session, pgpu);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPGPUAction
    {
        EnableDom0Access,
        DisableDom0Access
    }

    public class XenPGPUActionEnableDom0AccessDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPGPUActionDisableDom0AccessDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
