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
    [Cmdlet(VerbsLifecycle.Invoke, "XenLVHD", SupportsShouldProcess = true)]
    public class InvokeXenLVHD : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.LVHD LVHD { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.LVHD> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenLVHDAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenLVHDAction.EnableThinProvisioning:
                    _context = new XenLVHDActionEnableThinProvisioningDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string lvhd = ParseLVHD();

            switch (XenAction)
            {
                case XenLVHDAction.EnableThinProvisioning:
                    ProcessRecordEnableThinProvisioning(lvhd);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseLVHD()
        {
            string lvhd = null;

            if (LVHD != null)
                lvhd = (new XenRef<XenAPI.LVHD>(LVHD)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.LVHD.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    lvhd = xenRef.opaque_ref;
            }
            else if (Ref != null)
                lvhd = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'LVHD', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    LVHD));
            }

            return lvhd;
        }

        private void ProcessRecordEnableThinProvisioning(string lvhd)
        {
            if (!ShouldProcess(lvhd, "LVHD.enable_thin_provisioning"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenLVHDActionEnableThinProvisioningDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.LVHD.async_enable_thin_provisioning(session, contxt.XenHost, contxt.SR, contxt.InitialAllocation, contxt.AllocationQuantum);

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
                    string obj = XenAPI.LVHD.enable_thin_provisioning(session, contxt.XenHost, contxt.SR, contxt.InitialAllocation, contxt.AllocationQuantum);

                    if (PassThru)
                    {
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenLVHDAction
    {
        EnableThinProvisioning
    }

    public class XenLVHDActionEnableThinProvisioningDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public XenRef<XenAPI.SR> SR { get; set; }

        [Parameter]
        public long InitialAllocation { get; set; }

        [Parameter]
        public long AllocationQuantum { get; set; }
    
    }

}
