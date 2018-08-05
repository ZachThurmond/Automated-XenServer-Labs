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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPVSServer", SupportsShouldProcess = true)]
    public class InvokeXenPVSServer : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PVS_server PVSServer { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PVS_server> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenPVSServerAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPVSServerAction.Introduce:
                    _context = new XenPVSServerActionIntroduceDynamicParameters();
                    return _context;
                case XenPVSServerAction.Forget:
                    _context = new XenPVSServerActionForgetDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pvs_server = ParsePVSServer();

            switch (XenAction)
            {
                case XenPVSServerAction.Introduce:
                    ProcessRecordIntroduce(pvs_server);
                    break;
                case XenPVSServerAction.Forget:
                    ProcessRecordForget(pvs_server);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePVSServer()
        {
            string pvs_server = null;

            if (PVSServer != null)
                pvs_server = (new XenRef<XenAPI.PVS_server>(PVSServer)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.PVS_server.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pvs_server = xenRef.opaque_ref;
            }
            else if (Ref != null)
                pvs_server = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PVSServer', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PVSServer));
            }

            return pvs_server;
        }

        private void ProcessRecordIntroduce(string pvs_server)
        {
            if (!ShouldProcess(pvs_server, "PVS_server.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPVSServerActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PVS_server.async_introduce(session, contxt.Addresses, contxt.FirstPort, contxt.LastPort, contxt.Site);

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
                    string objRef = XenAPI.PVS_server.introduce(session, contxt.Addresses, contxt.FirstPort, contxt.LastPort, contxt.Site);

                    if (PassThru)
                    {
                        XenAPI.PVS_server obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PVS_server.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordForget(string pvs_server)
        {
            if (!ShouldProcess(pvs_server, "PVS_server.forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPVSServerActionForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PVS_server.async_forget(session, pvs_server);

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
                    XenAPI.PVS_server.forget(session, pvs_server);

                    if (PassThru)
                    {
                        var obj = XenAPI.PVS_server.get_record(session, pvs_server);
                        if (obj != null)
                            obj.opaque_ref = pvs_server;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPVSServerAction
    {
        Introduce,
        Forget
    }

    public class XenPVSServerActionIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string[] Addresses { get; set; }

        [Parameter]
        public long FirstPort { get; set; }

        [Parameter]
        public long LastPort { get; set; }

        [Parameter]
        public XenRef<XenAPI.PVS_site> Site { get; set; }
    
    }

    public class XenPVSServerActionForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
