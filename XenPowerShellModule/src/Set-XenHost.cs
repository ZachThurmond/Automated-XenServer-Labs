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
    [Cmdlet(VerbsCommon.Set, "XenHost", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Host))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenHost : XenServerCmdlet
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


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _powerOnModeIsSpecified
                       ^ _sslLegacyIsSpecified;
            }
        }

        [Parameter]
        public string NameLabel
        {
            get { return _nameLabel; }
            set
            {
                _nameLabel = value;
                _nameLabelIsSpecified = true;
            }
        }
        private string _nameLabel;
        private bool _nameLabelIsSpecified;

        [Parameter]
        public string NameDescription
        {
            get { return _nameDescription; }
            set
            {
                _nameDescription = value;
                _nameDescriptionIsSpecified = true;
            }
        }
        private string _nameDescription;
        private bool _nameDescriptionIsSpecified;

        [Parameter]
        public Hashtable OtherConfig
        {
            get { return _otherConfig; }
            set
            {
                _otherConfig = value;
                _otherConfigIsSpecified = true;
            }
        }
        private Hashtable _otherConfig;
        private bool _otherConfigIsSpecified;

        [Parameter]
        public Hashtable Logging
        {
            get { return _logging; }
            set
            {
                _logging = value;
                _loggingIsSpecified = true;
            }
        }
        private Hashtable _logging;
        private bool _loggingIsSpecified;

        [Parameter]
        public XenRef<XenAPI.SR> SuspendImageSr
        {
            get { return _suspendImageSr; }
            set
            {
                _suspendImageSr = value;
                _suspendImageSrIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _suspendImageSr;
        private bool _suspendImageSrIsSpecified;

        [Parameter]
        public XenRef<XenAPI.SR> CrashDumpSr
        {
            get { return _crashDumpSr; }
            set
            {
                _crashDumpSr = value;
                _crashDumpSrIsSpecified = true;
            }
        }
        private XenRef<XenAPI.SR> _crashDumpSr;
        private bool _crashDumpSrIsSpecified;

        [Parameter]
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                _hostname = value;
                _hostnameIsSpecified = true;
            }
        }
        private string _hostname;
        private bool _hostnameIsSpecified;

        [Parameter]
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                _addressIsSpecified = true;
            }
        }
        private string _address;
        private bool _addressIsSpecified;

        [Parameter]
        public string[] Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                _tagsIsSpecified = true;
            }
        }
        private string[] _tags;
        private bool _tagsIsSpecified;

        [Parameter]
        public Hashtable LicenseServer
        {
            get { return _licenseServer; }
            set
            {
                _licenseServer = value;
                _licenseServerIsSpecified = true;
            }
        }
        private Hashtable _licenseServer;
        private bool _licenseServerIsSpecified;

        [Parameter]
        public Hashtable GuestVCPUsParams
        {
            get { return _guestVCPUsParams; }
            set
            {
                _guestVCPUsParams = value;
                _guestVCPUsParamsIsSpecified = true;
            }
        }
        private Hashtable _guestVCPUsParams;
        private bool _guestVCPUsParamsIsSpecified;

        [Parameter]
        public host_display Display
        {
            get { return _display; }
            set
            {
                _display = value;
                _displayIsSpecified = true;
            }
        }
        private host_display _display;
        private bool _displayIsSpecified;

        [Parameter]
        public string HostnameLive
        {
            get { return _hostnameLive; }
            set
            {
                _hostnameLive = value;
                _hostnameLiveIsSpecified = true;
            }
        }
        private string _hostnameLive;
        private bool _hostnameLiveIsSpecified;

        [Parameter]
        public KeyValuePair<string, Hashtable> PowerOnMode
        {
            get { return _powerOnMode; }
            set
            {
                _powerOnMode = value;
                _powerOnModeIsSpecified = true;
            }
        }
        private KeyValuePair<string, Hashtable> _powerOnMode;
        private bool _powerOnModeIsSpecified;

        [Parameter]
        public string CpuFeatures
        {
            get { return _cpuFeatures; }
            set
            {
                _cpuFeatures = value;
                _cpuFeaturesIsSpecified = true;
            }
        }
        private string _cpuFeatures;
        private bool _cpuFeaturesIsSpecified;

        [Parameter]
        public bool SslLegacy
        {
            get { return _sslLegacy; }
            set
            {
                _sslLegacy = value;
                _sslLegacyIsSpecified = true;
            }
        }
        private bool _sslLegacy;
        private bool _sslLegacyIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host = ParseXenHost();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(host);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(host);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(host);
            if (_loggingIsSpecified)
                ProcessRecordLogging(host);
            if (_suspendImageSrIsSpecified)
                ProcessRecordSuspendImageSr(host);
            if (_crashDumpSrIsSpecified)
                ProcessRecordCrashDumpSr(host);
            if (_hostnameIsSpecified)
                ProcessRecordHostname(host);
            if (_addressIsSpecified)
                ProcessRecordAddress(host);
            if (_tagsIsSpecified)
                ProcessRecordTags(host);
            if (_licenseServerIsSpecified)
                ProcessRecordLicenseServer(host);
            if (_guestVCPUsParamsIsSpecified)
                ProcessRecordGuestVCPUsParams(host);
            if (_displayIsSpecified)
                ProcessRecordDisplay(host);
            if (_hostnameLiveIsSpecified)
                ProcessRecordHostnameLive(host);
            if (_powerOnModeIsSpecified)
                ProcessRecordPowerOnMode(host);
            if (_cpuFeaturesIsSpecified)
                ProcessRecordCpuFeatures(host);
            if (_sslLegacyIsSpecified)
                ProcessRecordSslLegacy(host);

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

        private void ProcessRecordNameLabel(string host)
        {
            if (!ShouldProcess(host, "Host.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_name_label(session, host, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string host)
        {
            if (!ShouldProcess(host, "Host.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_name_description(session, host, NameDescription);

            });
        }

        private void ProcessRecordOtherConfig(string host)
        {
            if (!ShouldProcess(host, "Host.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_other_config(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordLogging(string host)
        {
            if (!ShouldProcess(host, "Host.set_logging"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_logging(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(Logging));

            });
        }

        private void ProcessRecordSuspendImageSr(string host)
        {
            if (!ShouldProcess(host, "Host.set_suspend_image_sr"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_suspend_image_sr(session, host, SuspendImageSr);

            });
        }

        private void ProcessRecordCrashDumpSr(string host)
        {
            if (!ShouldProcess(host, "Host.set_crash_dump_sr"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_crash_dump_sr(session, host, CrashDumpSr);

            });
        }

        private void ProcessRecordHostname(string host)
        {
            if (!ShouldProcess(host, "Host.set_hostname"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_hostname(session, host, Hostname);

            });
        }

        private void ProcessRecordAddress(string host)
        {
            if (!ShouldProcess(host, "Host.set_address"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_address(session, host, Address);

            });
        }

        private void ProcessRecordTags(string host)
        {
            if (!ShouldProcess(host, "Host.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_tags(session, host, Tags);

            });
        }

        private void ProcessRecordLicenseServer(string host)
        {
            if (!ShouldProcess(host, "Host.set_license_server"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_license_server(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(LicenseServer));

            });
        }

        private void ProcessRecordGuestVCPUsParams(string host)
        {
            if (!ShouldProcess(host, "Host.set_guest_VCPUs_params"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_guest_VCPUs_params(session, host, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(GuestVCPUsParams));

            });
        }

        private void ProcessRecordDisplay(string host)
        {
            if (!ShouldProcess(host, "Host.set_display"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_display(session, host, Display);

            });
        }

        private void ProcessRecordHostnameLive(string host)
        {
            if (!ShouldProcess(host, "Host.set_hostname_live"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_hostname_live(session, host, HostnameLive);

            });
        }

        private void ProcessRecordPowerOnMode(string host)
        {
            if (!ShouldProcess(host, "Host.set_power_on_mode"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_set_power_on_mode(session, host, PowerOnMode.Key, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(PowerOnMode.Value));

                }
                else
                {
                    XenAPI.Host.set_power_on_mode(session, host, PowerOnMode.Key, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(PowerOnMode.Value));

                }

            });
        }

        private void ProcessRecordCpuFeatures(string host)
        {
            if (!ShouldProcess(host, "Host.set_cpu_features"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Host.set_cpu_features(session, host, CpuFeatures);

            });
        }

        private void ProcessRecordSslLegacy(string host)
        {
            if (!ShouldProcess(host, "Host.set_ssl_legacy"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_set_ssl_legacy(session, host, SslLegacy);

                }
                else
                {
                    XenAPI.Host.set_ssl_legacy(session, host, SslLegacy);

                }

            });
        }

        #endregion
    }
}
