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
    [Cmdlet(VerbsLifecycle.Invoke, "XenVMAppliance", SupportsShouldProcess = true)]
    public class InvokeXenVMAppliance : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VM_appliance VMAppliance { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VM_appliance> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMApplianceAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenVMApplianceAction.Start:
                    _context = new XenVMApplianceActionStartDynamicParameters();
                    return _context;
                case XenVMApplianceAction.CleanShutdown:
                    _context = new XenVMApplianceActionCleanShutdownDynamicParameters();
                    return _context;
                case XenVMApplianceAction.HardShutdown:
                    _context = new XenVMApplianceActionHardShutdownDynamicParameters();
                    return _context;
                case XenVMApplianceAction.Shutdown:
                    _context = new XenVMApplianceActionShutdownDynamicParameters();
                    return _context;
                case XenVMApplianceAction.AssertCanBeRecovered:
                    _context = new XenVMApplianceActionAssertCanBeRecoveredDynamicParameters();
                    return _context;
                case XenVMApplianceAction.Recover:
                    _context = new XenVMApplianceActionRecoverDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vm_appliance = ParseVMAppliance();

            switch (XenAction)
            {
                case XenVMApplianceAction.Start:
                    ProcessRecordStart(vm_appliance);
                    break;
                case XenVMApplianceAction.CleanShutdown:
                    ProcessRecordCleanShutdown(vm_appliance);
                    break;
                case XenVMApplianceAction.HardShutdown:
                    ProcessRecordHardShutdown(vm_appliance);
                    break;
                case XenVMApplianceAction.Shutdown:
                    ProcessRecordShutdown(vm_appliance);
                    break;
                case XenVMApplianceAction.AssertCanBeRecovered:
                    ProcessRecordAssertCanBeRecovered(vm_appliance);
                    break;
                case XenVMApplianceAction.Recover:
                    ProcessRecordRecover(vm_appliance);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMAppliance()
        {
            string vm_appliance = null;

            if (VMAppliance != null)
                vm_appliance = (new XenRef<XenAPI.VM_appliance>(VMAppliance)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VM_appliance.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vm_appliance = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VM_appliance.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vm_appliance = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VM_appliance with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                vm_appliance = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMAppliance', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMAppliance));
            }

            return vm_appliance;
        }

        private void ProcessRecordStart(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.start"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionStartDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_start(session, vm_appliance, contxt.Paused);

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
                    XenAPI.VM_appliance.start(session, vm_appliance, contxt.Paused);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordCleanShutdown(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.clean_shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionCleanShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_clean_shutdown(session, vm_appliance);

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
                    XenAPI.VM_appliance.clean_shutdown(session, vm_appliance);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordHardShutdown(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.hard_shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionHardShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_hard_shutdown(session, vm_appliance);

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
                    XenAPI.VM_appliance.hard_shutdown(session, vm_appliance);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordShutdown(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.shutdown"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionShutdownDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_shutdown(session, vm_appliance);

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
                    XenAPI.VM_appliance.shutdown(session, vm_appliance);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordAssertCanBeRecovered(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.assert_can_be_recovered"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionAssertCanBeRecoveredDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_assert_can_be_recovered(session, vm_appliance, contxt.SessionTo);

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
                    XenAPI.VM_appliance.assert_can_be_recovered(session, vm_appliance, contxt.SessionTo);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordRecover(string vm_appliance)
        {
            if (!ShouldProcess(vm_appliance, "VM_appliance.recover"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenVMApplianceActionRecoverDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VM_appliance.async_recover(session, vm_appliance, contxt.SessionTo, contxt.Force);

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
                    XenAPI.VM_appliance.recover(session, vm_appliance, contxt.SessionTo, contxt.Force);

                    if (PassThru)
                    {
                        var obj = XenAPI.VM_appliance.get_record(session, vm_appliance);
                        if (obj != null)
                            obj.opaque_ref = vm_appliance;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenVMApplianceAction
    {
        Start,
        CleanShutdown,
        HardShutdown,
        Shutdown,
        AssertCanBeRecovered,
        Recover
    }

    public class XenVMApplianceActionStartDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public bool Paused { get; set; }
 
    }

    public class XenVMApplianceActionCleanShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMApplianceActionHardShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMApplianceActionShutdownDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenVMApplianceActionAssertCanBeRecoveredDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }
 
    }

    public class XenVMApplianceActionRecoverDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Session> SessionTo { get; set; }

        [Parameter]
        public bool Force { get; set; }
  
    }

}
