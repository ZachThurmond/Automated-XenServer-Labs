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
    [Cmdlet(VerbsCommon.Set, "XenSR", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.SR))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenSR : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SR SR { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SR> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _sharedIsSpecified
                       ^ _nameLabelIsSpecified
                       ^ _nameDescriptionIsSpecified;
            }
        }

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
        public Hashtable SmConfig
        {
            get { return _smConfig; }
            set
            {
                _smConfig = value;
                _smConfigIsSpecified = true;
            }
        }
        private Hashtable _smConfig;
        private bool _smConfigIsSpecified;

        [Parameter]
        public bool Shared
        {
            get { return _shared; }
            set
            {
                _shared = value;
                _sharedIsSpecified = true;
            }
        }
        private bool _shared;
        private bool _sharedIsSpecified;

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
        public long PhysicalSize
        {
            get { return _physicalSize; }
            set
            {
                _physicalSize = value;
                _physicalSizeIsSpecified = true;
            }
        }
        private long _physicalSize;
        private bool _physicalSizeIsSpecified;

        [Parameter]
        public long VirtualAllocation
        {
            get { return _virtualAllocation; }
            set
            {
                _virtualAllocation = value;
                _virtualAllocationIsSpecified = true;
            }
        }
        private long _virtualAllocation;
        private bool _virtualAllocationIsSpecified;

        [Parameter]
        public long PhysicalUtilisation
        {
            get { return _physicalUtilisation; }
            set
            {
                _physicalUtilisation = value;
                _physicalUtilisationIsSpecified = true;
            }
        }
        private long _physicalUtilisation;
        private bool _physicalUtilisationIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sr = ParseSR();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(sr);
            if (_tagsIsSpecified)
                ProcessRecordTags(sr);
            if (_smConfigIsSpecified)
                ProcessRecordSmConfig(sr);
            if (_sharedIsSpecified)
                ProcessRecordShared(sr);
            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(sr);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(sr);
            if (_physicalSizeIsSpecified)
                ProcessRecordPhysicalSize(sr);
            if (_virtualAllocationIsSpecified)
                ProcessRecordVirtualAllocation(sr);
            if (_physicalUtilisationIsSpecified)
                ProcessRecordPhysicalUtilisation(sr);

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

                        var obj = XenAPI.SR.get_record(session, sr);
                        if (obj != null)
                            obj.opaque_ref = sr;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSR()
        {
            string sr = null;

            if (SR != null)
                sr = (new XenRef<XenAPI.SR>(SR)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.SR.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    sr = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.SR.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    sr = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.SR with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                sr = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'SR', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    SR));
            }

            return sr;
        }

        private void ProcessRecordOtherConfig(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_other_config(session, sr, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordTags(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_tags(session, sr, Tags);

            });
        }

        private void ProcessRecordSmConfig(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_sm_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_sm_config(session, sr, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(SmConfig));

            });
        }

        private void ProcessRecordShared(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_shared"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_set_shared(session, sr, Shared);

                }
                else
                {
                    XenAPI.SR.set_shared(session, sr, Shared);

                }

            });
        }

        private void ProcessRecordNameLabel(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_name_label"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_set_name_label(session, sr, NameLabel);

                }
                else
                {
                    XenAPI.SR.set_name_label(session, sr, NameLabel);

                }

            });
        }

        private void ProcessRecordNameDescription(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_name_description"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_set_name_description(session, sr, NameDescription);

                }
                else
                {
                    XenAPI.SR.set_name_description(session, sr, NameDescription);

                }

            });
        }

        private void ProcessRecordPhysicalSize(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_physical_size"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_physical_size(session, sr, PhysicalSize);

            });
        }

        private void ProcessRecordVirtualAllocation(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_virtual_allocation"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_virtual_allocation(session, sr, VirtualAllocation);

            });
        }

        private void ProcessRecordPhysicalUtilisation(string sr)
        {
            if (!ShouldProcess(sr, "SR.set_physical_utilisation"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.SR.set_physical_utilisation(session, sr, PhysicalUtilisation);

            });
        }

        #endregion
    }
}
