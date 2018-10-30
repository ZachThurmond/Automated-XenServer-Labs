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
    [Cmdlet(VerbsLifecycle.Invoke, "XenVBD", SupportsShouldProcess = true)]
    public class InvokeXenVBD : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VBD VBD { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VBD> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenVBDAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenVBDAction.Eject:
                    _context = new XenVBDActionEjectDynamicParameters();
                    return _context;
                case XenVBDAction.Insert:
                    _context = new XenVBDActionInsertDynamicParameters();
                    return _context;
                case XenVBDAction.Plug:
                    _context = new XenVBDActionPlugDynamicParameters();
                    return _context;
                case XenVBDAction.Unplug:
                    _context = new XenVBDActionUnplugDynamicParameters();
                    return _context;
                case XenVBDAction.UnplugForce:
                    _context = new XenVBDActionUnplugForceDynamicParameters();
                    return _context;
                case XenVBDAction.AssertAttachable:
                    _context = new XenVBDActionAssertAttachableDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vbd = ParseVBD();

            switch (XenAction)
            {
                case XenVBDAction.Eject:
                    ProcessRecordEject(vbd);
                    break;
                case XenVBDAction.Insert:
                    ProcessRecordInsert(vbd);
                    break;
                case XenVBDAction.Plug:
                    ProcessRecordPlug(vbd);
                    break;
                case XenVBDAction.Unplug:
                    ProcessRecordUnplug(vbd);
                    break;
                case XenVBDAction.UnplugForce:
                    ProcessRecordUnplugForce(vbd);
                    break;
                case XenVBDAction.AssertAttachable:
                    ProcessRecordAssertAttachable(vbd);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVBD()
        {
            string vbd = null;

            if (VBD != null)
                vbd = (new XenRef<XenAPI.VBD>(VBD)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VBD.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vbd = xenRef.opaque_ref;
            }
            else if (Ref != null)
                vbd = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VBD', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VBD));
            }

            return vbd;
        }

        private void ProcessRecordEject(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.eject"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionEjectDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_eject(session, vbd);

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
                    XenAPI.VBD.eject(session, vbd);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordInsert(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.insert"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionInsertDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_insert(session, vbd, contxt.VDI);

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
                    XenAPI.VBD.insert(session, vbd, contxt.VDI);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPlug(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.plug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionPlugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_plug(session, vbd);

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
                    XenAPI.VBD.plug(session, vbd);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnplug(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.unplug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionUnplugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_unplug(session, vbd);

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
                    XenAPI.VBD.unplug(session, vbd);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnplugForce(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.unplug_force"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionUnplugForceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_unplug_force(session, vbd);

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
                    XenAPI.VBD.unplug_force(session, vbd);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordAssertAttachable(string vbd)
        {
            if (!ShouldProcess(vbd, "VBD.assert_attachable"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVBDActionAssertAttachableDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VBD.async_assert_attachable(session, vbd);

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
                    XenAPI.VBD.assert_attachable(session, vbd);

                    if (PassThru)
                    {
                        var obj = XenAPI.VBD.get_record(session, vbd);
                        if (obj != null)
                            obj.opaque_ref = vbd;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenVBDAction
    {
        Eject,
        Insert,
        Plug,
        Unplug,
        UnplugForce,
        AssertAttachable
    }

    public class XenVBDActionEjectDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVBDActionInsertDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.VDI> VDI { get; set; }
 
    }

    public class XenVBDActionPlugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVBDActionUnplugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVBDActionUnplugForceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVBDActionAssertAttachableDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
