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
    [Cmdlet(VerbsCommon.Set, "XenNetwork", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Network))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenNetwork : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Network Network { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Network> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _defaultLockingModeIsSpecified;
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
        public long MTU
        {
            get { return _mTU; }
            set
            {
                _mTU = value;
                _mTUIsSpecified = true;
            }
        }
        private long _mTU;
        private bool _mTUIsSpecified;

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
        public network_default_locking_mode DefaultLockingMode
        {
            get { return _defaultLockingMode; }
            set
            {
                _defaultLockingMode = value;
                _defaultLockingModeIsSpecified = true;
            }
        }
        private network_default_locking_mode _defaultLockingMode;
        private bool _defaultLockingModeIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string network = ParseNetwork();

            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(network);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(network);
            if (_mTUIsSpecified)
                ProcessRecordMTU(network);
            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(network);
            if (_tagsIsSpecified)
                ProcessRecordTags(network);
            if (_defaultLockingModeIsSpecified)
                ProcessRecordDefaultLockingMode(network);

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

                        var obj = XenAPI.Network.get_record(session, network);
                        if (obj != null)
                            obj.opaque_ref = network;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseNetwork()
        {
            string network = null;

            if (Network != null)
                network = (new XenRef<XenAPI.Network>(Network)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.Network.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    network = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.Network.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    network = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.Network with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                network = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Network', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Network));
            }

            return network;
        }

        private void ProcessRecordNameLabel(string network)
        {
            if (!ShouldProcess(network, "Network.set_name_label"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Network.set_name_label(session, network, NameLabel);

            });
        }

        private void ProcessRecordNameDescription(string network)
        {
            if (!ShouldProcess(network, "Network.set_name_description"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Network.set_name_description(session, network, NameDescription);

            });
        }

        private void ProcessRecordMTU(string network)
        {
            if (!ShouldProcess(network, "Network.set_MTU"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Network.set_MTU(session, network, MTU);

            });
        }

        private void ProcessRecordOtherConfig(string network)
        {
            if (!ShouldProcess(network, "Network.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Network.set_other_config(session, network, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordTags(string network)
        {
            if (!ShouldProcess(network, "Network.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.Network.set_tags(session, network, Tags);

            });
        }

        private void ProcessRecordDefaultLockingMode(string network)
        {
            if (!ShouldProcess(network, "Network.set_default_locking_mode"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Network.async_set_default_locking_mode(session, network, DefaultLockingMode);

                }
                else
                {
                    XenAPI.Network.set_default_locking_mode(session, network, DefaultLockingMode);

                }

            });
        }

        #endregion
    }
}
