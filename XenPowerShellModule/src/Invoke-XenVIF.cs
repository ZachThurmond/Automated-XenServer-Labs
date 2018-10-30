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
    [Cmdlet(VerbsLifecycle.Invoke, "XenVIF", SupportsShouldProcess = true)]
    public class InvokeXenVIF : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VIF VIF { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VIF> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenVIFAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenVIFAction.Plug:
                    _context = new XenVIFActionPlugDynamicParameters();
                    return _context;
                case XenVIFAction.Unplug:
                    _context = new XenVIFActionUnplugDynamicParameters();
                    return _context;
                case XenVIFAction.UnplugForce:
                    _context = new XenVIFActionUnplugForceDynamicParameters();
                    return _context;
                case XenVIFAction.Move:
                    _context = new XenVIFActionMoveDynamicParameters();
                    return _context;
                case XenVIFAction.ConfigureIpv4:
                    _context = new XenVIFActionConfigureIpv4DynamicParameters();
                    return _context;
                case XenVIFAction.ConfigureIpv6:
                    _context = new XenVIFActionConfigureIpv6DynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vif = ParseVIF();

            switch (XenAction)
            {
                case XenVIFAction.Plug:
                    ProcessRecordPlug(vif);
                    break;
                case XenVIFAction.Unplug:
                    ProcessRecordUnplug(vif);
                    break;
                case XenVIFAction.UnplugForce:
                    ProcessRecordUnplugForce(vif);
                    break;
                case XenVIFAction.Move:
                    ProcessRecordMove(vif);
                    break;
                case XenVIFAction.ConfigureIpv4:
                    ProcessRecordConfigureIpv4(vif);
                    break;
                case XenVIFAction.ConfigureIpv6:
                    ProcessRecordConfigureIpv6(vif);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVIF()
        {
            string vif = null;

            if (VIF != null)
                vif = (new XenRef<XenAPI.VIF>(VIF)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VIF.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vif = xenRef.opaque_ref;
            }
            else if (Ref != null)
                vif = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VIF', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VIF));
            }

            return vif;
        }

        private void ProcessRecordPlug(string vif)
        {
            if (!ShouldProcess(vif, "VIF.plug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionPlugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_plug(session, vif);

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
                    XenAPI.VIF.plug(session, vif);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnplug(string vif)
        {
            if (!ShouldProcess(vif, "VIF.unplug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionUnplugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_unplug(session, vif);

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
                    XenAPI.VIF.unplug(session, vif);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnplugForce(string vif)
        {
            if (!ShouldProcess(vif, "VIF.unplug_force"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionUnplugForceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_unplug_force(session, vif);

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
                    XenAPI.VIF.unplug_force(session, vif);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordMove(string vif)
        {
            if (!ShouldProcess(vif, "VIF.move"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionMoveDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_move(session, vif, contxt.Network);

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
                    XenAPI.VIF.move(session, vif, contxt.Network);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordConfigureIpv4(string vif)
        {
            if (!ShouldProcess(vif, "VIF.configure_ipv4"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionConfigureIpv4DynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_configure_ipv4(session, vif, contxt.Mode, contxt.Address, contxt.Gateway);

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
                    XenAPI.VIF.configure_ipv4(session, vif, contxt.Mode, contxt.Address, contxt.Gateway);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordConfigureIpv6(string vif)
        {
            if (!ShouldProcess(vif, "VIF.configure_ipv6"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVIFActionConfigureIpv6DynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VIF.async_configure_ipv6(session, vif, contxt.Mode, contxt.Address, contxt.Gateway);

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
                    XenAPI.VIF.configure_ipv6(session, vif, contxt.Mode, contxt.Address, contxt.Gateway);

                    if (PassThru)
                    {
                        var obj = XenAPI.VIF.get_record(session, vif);
                        if (obj != null)
                            obj.opaque_ref = vif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenVIFAction
    {
        Plug,
        Unplug,
        UnplugForce,
        Move,
        ConfigureIpv4,
        ConfigureIpv6
    }

    public class XenVIFActionPlugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVIFActionUnplugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVIFActionUnplugForceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVIFActionMoveDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }
 
    }

    public class XenVIFActionConfigureIpv4DynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public vif_ipv4_configuration_mode Mode { get; set; }

        [Parameter]
        public string Address { get; set; }

        [Parameter]
        public string Gateway { get; set; }
   
    }

    public class XenVIFActionConfigureIpv6DynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public vif_ipv6_configuration_mode Mode { get; set; }

        [Parameter]
        public string Address { get; set; }

        [Parameter]
        public string Gateway { get; set; }
   
    }

}
