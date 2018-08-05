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
    [Cmdlet(VerbsCommon.Add, "XenPool", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Pool))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class AddXenPool : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool Pool { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _guestAgentConfigIsSpecified;
            }
        }

        [Parameter]
        public KeyValuePair<string, string> OtherConfig
        {
            get { return _otherConfig; }
            set
            {
                _otherConfig = value;
                _otherConfigIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _otherConfig;
        private bool _otherConfigIsSpecified;

        [Parameter]
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                _tagsIsSpecified = true;
            }
        }
        private string _tags;
        private bool _tagsIsSpecified;

        [Parameter]
        public KeyValuePair<string, string> GuiConfig
        {
            get { return _guiConfig; }
            set
            {
                _guiConfig = value;
                _guiConfigIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _guiConfig;
        private bool _guiConfigIsSpecified;

        [Parameter]
        public KeyValuePair<string, string> HealthCheckConfig
        {
            get { return _healthCheckConfig; }
            set
            {
                _healthCheckConfig = value;
                _healthCheckConfigIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _healthCheckConfig;
        private bool _healthCheckConfigIsSpecified;

        [Parameter]
        public KeyValuePair<string, string> GuestAgentConfig
        {
            get { return _guestAgentConfig; }
            set
            {
                _guestAgentConfig = value;
                _guestAgentConfigIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _guestAgentConfig;
        private bool _guestAgentConfigIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool = ParsePool();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(pool);
            if (_tagsIsSpecified)
                ProcessRecordTags(pool);
            if (_guiConfigIsSpecified)
                ProcessRecordGuiConfig(pool);
            if (_healthCheckConfigIsSpecified)
                ProcessRecordHealthCheckConfig(pool);
            if (_guestAgentConfigIsSpecified)
                ProcessRecordGuestAgentConfig(pool);

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

                        var obj = XenAPI.Pool.get_record(session, pool);
                        if (obj != null)
                            obj.opaque_ref = pool;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePool()
        {
            string pool = null;

            if (Pool != null)
                pool = (new XenRef<XenAPI.Pool>(Pool)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Pool.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pool = xenRef.opaque_ref;
            }
            else if (Ref != null)
                pool = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Pool', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Pool));
            }

            return pool;
        }

        private void ProcessRecordOtherConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.add_to_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.add_to_other_config(session, pool, OtherConfig.Key, OtherConfig.Value);

            });
        }

        private void ProcessRecordTags(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.add_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.add_tags(session, pool, Tags);

            });
        }

        private void ProcessRecordGuiConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.add_to_gui_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.add_to_gui_config(session, pool, GuiConfig.Key, GuiConfig.Value);

            });
        }

        private void ProcessRecordHealthCheckConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.add_to_health_check_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Pool.add_to_health_check_config(session, pool, HealthCheckConfig.Key, HealthCheckConfig.Value);

            });
        }

        private void ProcessRecordGuestAgentConfig(string pool)
        {
            if (!ShouldProcess(session.Url, "Pool.add_to_guest_agent_config"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Pool.async_add_to_guest_agent_config(session, pool, GuestAgentConfig.Key, GuestAgentConfig.Value);

                }
                else
                {
                    XenAPI.Pool.add_to_guest_agent_config(session, pool, GuestAgentConfig.Key, GuestAgentConfig.Value);

                }

            });
        }

        #endregion
    }
}
