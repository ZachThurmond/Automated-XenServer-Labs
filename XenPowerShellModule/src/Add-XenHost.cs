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
    [Cmdlet(VerbsCommon.Add, "XenHost", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Host))]
    [OutputType(typeof(void))]
    public class AddXenHost : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host XenHost { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


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
        public KeyValuePair<string, string> Logging
        {
            get { return _logging; }
            set
            {
                _logging = value;
                _loggingIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _logging;
        private bool _loggingIsSpecified;

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
        public KeyValuePair<string, string> LicenseServer
        {
            get { return _licenseServer; }
            set
            {
                _licenseServer = value;
                _licenseServerIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _licenseServer;
        private bool _licenseServerIsSpecified;

        [Parameter]
        public KeyValuePair<string, string> GuestVCPUsParams
        {
            get { return _guestVCPUsParams; }
            set
            {
                _guestVCPUsParams = value;
                _guestVCPUsParamsIsSpecified = true;
            }
        }
        private KeyValuePair<string, string> _guestVCPUsParams;
        private bool _guestVCPUsParamsIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host = ParseXenHost();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(host);
            if (_loggingIsSpecified)
                ProcessRecordLogging(host);
            if (_tagsIsSpecified)
                ProcessRecordTags(host);
            if (_licenseServerIsSpecified)
                ProcessRecordLicenseServer(host);
            if (_guestVCPUsParamsIsSpecified)
                ProcessRecordGuestVCPUsParams(host);

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

                        var obj = XenAPI.Host.get_record(session, host);
                        if (obj != null)
                            obj.opaque_ref = host;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseXenHost()
        {
            string host = null;

            if (XenHost != null)
                host = (new XenRef<XenAPI.Host>(XenHost)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Host.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    host = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.Host.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    host = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.Host with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                host = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'XenHost', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    XenHost));
            }

            return host;
        }

        private void ProcessRecordOtherConfig(string host)
        {
            if (!ShouldProcess(host, "Host.add_to_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.add_to_other_config(session, host, OtherConfig.Key, OtherConfig.Value);

            });
        }

        private void ProcessRecordLogging(string host)
        {
            if (!ShouldProcess(host, "Host.add_to_logging"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.add_to_logging(session, host, Logging.Key, Logging.Value);

            });
        }

        private void ProcessRecordTags(string host)
        {
            if (!ShouldProcess(host, "Host.add_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.add_tags(session, host, Tags);

            });
        }

        private void ProcessRecordLicenseServer(string host)
        {
            if (!ShouldProcess(host, "Host.add_to_license_server"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.add_to_license_server(session, host, LicenseServer.Key, LicenseServer.Value);

            });
        }

        private void ProcessRecordGuestVCPUsParams(string host)
        {
            if (!ShouldProcess(host, "Host.add_to_guest_VCPUs_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.add_to_guest_VCPUs_params(session, host, GuestVCPUsParams.Key, GuestVCPUsParams.Value);

            });
        }

        #endregion
    }
}
