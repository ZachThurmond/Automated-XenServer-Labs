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
    [Cmdlet(VerbsCommon.Remove, "XenPGPUProperty", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.PGPU))]
    [OutputType(typeof(XenAPI.Task))]
    public class RemoveXenPGPUProperty : XenServerCmdlet
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


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _enabledVGPUTypesIsSpecified;
            }
        }

        [Parameter]
        public string OtherConfig
        {
            get { return _otherConfig; }
            set
            {
                _otherConfig = value;
                _otherConfigIsSpecified = true;
            }
        }
        private string _otherConfig;
        private bool _otherConfigIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VGPU_type> EnabledVGPUTypes
        {
            get { return _enabledVGPUTypes; }
            set
            {
                _enabledVGPUTypes = value;
                _enabledVGPUTypesIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VGPU_type> _enabledVGPUTypes;
        private bool _enabledVGPUTypesIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pgpu = ParsePGPU();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(pgpu);
            if (_enabledVGPUTypesIsSpecified)
                ProcessRecordEnabledVGPUTypes(pgpu);

            if (!PassThru)
                return;

            RunApiCall(() =>
                {
                    var contxt = _context as XenServerCmdletDynamicParameters;

                    if (contxt != null && contxt.Async)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != null && taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                    else
                    {

                        var obj = XenAPI.PGPU.get_record(session, pgpu);
                        if (obj != null)
                            obj.opaque_ref = pgpu;
                        WriteObject(obj, true);

                    }
                });

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

        private void ProcessRecordOtherConfig(string pgpu)
        {
            if (!ShouldProcess(pgpu, "PGPU.remove_from_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.PGPU.remove_from_other_config(session, pgpu, OtherConfig);

            });
        }

        private void ProcessRecordEnabledVGPUTypes(string pgpu)
        {
            if (!ShouldProcess(pgpu, "PGPU.remove_enabled_VGPU_types"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PGPU.async_remove_enabled_VGPU_types(session, pgpu, EnabledVGPUTypes);

                }
                else
                {
                    XenAPI.PGPU.remove_enabled_VGPU_types(session, pgpu, EnabledVGPUTypes);

                }

            });
        }

        #endregion
    }
}
