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
    [Cmdlet(VerbsLifecycle.Invoke, "XenSDNController", SupportsShouldProcess = true)]
    public class InvokeXenSDNController : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SDN_controller SDNController { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SDN_controller> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenSDNControllerAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenSDNControllerAction.Introduce:
                    _context = new XenSDNControllerActionIntroduceDynamicParameters();
                    return _context;
                case XenSDNControllerAction.Forget:
                    _context = new XenSDNControllerActionForgetDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sdn_controller = ParseSDNController();

            switch (XenAction)
            {
                case XenSDNControllerAction.Introduce:
                    ProcessRecordIntroduce(sdn_controller);
                    break;
                case XenSDNControllerAction.Forget:
                    ProcessRecordForget(sdn_controller);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSDNController()
        {
            string sdn_controller = null;

            if (SDNController != null)
                sdn_controller = (new XenRef<XenAPI.SDN_controller>(SDNController)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.SDN_controller.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    sdn_controller = xenRef.opaque_ref;
            }
            else if (Ref != null)
                sdn_controller = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'SDNController', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    SDNController));
            }

            return sdn_controller;
        }

        private void ProcessRecordIntroduce(string sdn_controller)
        {
            if (!ShouldProcess(sdn_controller, "SDN_controller.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSDNControllerActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SDN_controller.async_introduce(session, contxt.Protocol, contxt.Address, contxt.Port);

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
                    string objRef = XenAPI.SDN_controller.introduce(session, contxt.Protocol, contxt.Address, contxt.Port);

                    if (PassThru)
                    {
                        XenAPI.SDN_controller obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SDN_controller.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordForget(string sdn_controller)
        {
            if (!ShouldProcess(sdn_controller, "SDN_controller.forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenSDNControllerActionForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SDN_controller.async_forget(session, sdn_controller);

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
                    XenAPI.SDN_controller.forget(session, sdn_controller);

                    if (PassThru)
                    {
                        var obj = XenAPI.SDN_controller.get_record(session, sdn_controller);
                        if (obj != null)
                            obj.opaque_ref = sdn_controller;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenSDNControllerAction
    {
        Introduce,
        Forget
    }

    public class XenSDNControllerActionIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public sdn_controller_protocol Protocol { get; set; }

        [Parameter]
        public string Address { get; set; }

        [Parameter]
        public long Port { get; set; }
   
    }

    public class XenSDNControllerActionForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
