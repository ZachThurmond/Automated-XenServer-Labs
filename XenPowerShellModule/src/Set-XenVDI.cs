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
    [Cmdlet(VerbsCommon.Set, "XenVDI", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.VDI))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class SetXenVDI : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VDI VDI { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VDI> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("name_label")]
        public string Name { get; set; }


        protected override bool GenerateAsyncParam
        {
            get
            {
                return _nameLabelIsSpecified
                       ^ _nameDescriptionIsSpecified
                       ^ _onBootIsSpecified
                       ^ _allowCachingIsSpecified;
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
        public Hashtable XenstoreData
        {
            get { return _xenstoreData; }
            set
            {
                _xenstoreData = value;
                _xenstoreDataIsSpecified = true;
            }
        }
        private Hashtable _xenstoreData;
        private bool _xenstoreDataIsSpecified;

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
        public bool Managed
        {
            get { return _managed; }
            set
            {
                _managed = value;
                _managedIsSpecified = true;
            }
        }
        private bool _managed;
        private bool _managedIsSpecified;

        [Parameter]
        public bool Sharable
        {
            get { return _sharable; }
            set
            {
                _sharable = value;
                _sharableIsSpecified = true;
            }
        }
        private bool _sharable;
        private bool _sharableIsSpecified;

        [Parameter]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                _readOnlyIsSpecified = true;
            }
        }
        private bool _readOnly;
        private bool _readOnlyIsSpecified;

        [Parameter]
        public bool Missing
        {
            get { return _missing; }
            set
            {
                _missing = value;
                _missingIsSpecified = true;
            }
        }
        private bool _missing;
        private bool _missingIsSpecified;

        [Parameter]
        public long VirtualSize
        {
            get { return _virtualSize; }
            set
            {
                _virtualSize = value;
                _virtualSizeIsSpecified = true;
            }
        }
        private long _virtualSize;
        private bool _virtualSizeIsSpecified;

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

        [Parameter]
        public bool IsASnapshot
        {
            get { return _isASnapshot; }
            set
            {
                _isASnapshot = value;
                _isASnapshotIsSpecified = true;
            }
        }
        private bool _isASnapshot;
        private bool _isASnapshotIsSpecified;

        [Parameter]
        public XenRef<XenAPI.VDI> SnapshotOf
        {
            get { return _snapshotOf; }
            set
            {
                _snapshotOf = value;
                _snapshotOfIsSpecified = true;
            }
        }
        private XenRef<XenAPI.VDI> _snapshotOf;
        private bool _snapshotOfIsSpecified;

        [Parameter]
        public DateTime SnapshotTime
        {
            get { return _snapshotTime; }
            set
            {
                _snapshotTime = value;
                _snapshotTimeIsSpecified = true;
            }
        }
        private DateTime _snapshotTime;
        private bool _snapshotTimeIsSpecified;

        [Parameter]
        public XenRef<XenAPI.Pool> MetadataOfPool
        {
            get { return _metadataOfPool; }
            set
            {
                _metadataOfPool = value;
                _metadataOfPoolIsSpecified = true;
            }
        }
        private XenRef<XenAPI.Pool> _metadataOfPool;
        private bool _metadataOfPoolIsSpecified;

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
        public on_boot OnBoot
        {
            get { return _onBoot; }
            set
            {
                _onBoot = value;
                _onBootIsSpecified = true;
            }
        }
        private on_boot _onBoot;
        private bool _onBootIsSpecified;

        [Parameter]
        public bool AllowCaching
        {
            get { return _allowCaching; }
            set
            {
                _allowCaching = value;
                _allowCachingIsSpecified = true;
            }
        }
        private bool _allowCaching;
        private bool _allowCachingIsSpecified;

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vdi = ParseVDI();

            if (_otherConfigIsSpecified)
                ProcessRecordOtherConfig(vdi);
            if (_xenstoreDataIsSpecified)
                ProcessRecordXenstoreData(vdi);
            if (_smConfigIsSpecified)
                ProcessRecordSmConfig(vdi);
            if (_tagsIsSpecified)
                ProcessRecordTags(vdi);
            if (_managedIsSpecified)
                ProcessRecordManaged(vdi);
            if (_sharableIsSpecified)
                ProcessRecordSharable(vdi);
            if (_readOnlyIsSpecified)
                ProcessRecordReadOnly(vdi);
            if (_missingIsSpecified)
                ProcessRecordMissing(vdi);
            if (_virtualSizeIsSpecified)
                ProcessRecordVirtualSize(vdi);
            if (_physicalUtilisationIsSpecified)
                ProcessRecordPhysicalUtilisation(vdi);
            if (_isASnapshotIsSpecified)
                ProcessRecordIsASnapshot(vdi);
            if (_snapshotOfIsSpecified)
                ProcessRecordSnapshotOf(vdi);
            if (_snapshotTimeIsSpecified)
                ProcessRecordSnapshotTime(vdi);
            if (_metadataOfPoolIsSpecified)
                ProcessRecordMetadataOfPool(vdi);
            if (_nameLabelIsSpecified)
                ProcessRecordNameLabel(vdi);
            if (_nameDescriptionIsSpecified)
                ProcessRecordNameDescription(vdi);
            if (_onBootIsSpecified)
                ProcessRecordOnBoot(vdi);
            if (_allowCachingIsSpecified)
                ProcessRecordAllowCaching(vdi);

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

                        var obj = XenAPI.VDI.get_record(session, vdi);
                        if (obj != null)
                            obj.opaque_ref = vdi;
                        WriteObject(obj, true);

                    }
                });

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVDI()
        {
            string vdi = null;

            if (VDI != null)
                vdi = (new XenRef<XenAPI.VDI>(VDI)).opaque_ref;
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.VDI.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    vdi = xenRef.opaque_ref;
            }
            else if (Name != null)
            {
                var xenRefs = XenAPI.VDI.get_by_name_label(session, Name);
                if (xenRefs.Count == 1)
                    vdi = xenRefs[0].opaque_ref;
                else if (xenRefs.Count > 1)
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException(string.Format("More than one XenAPI.VDI with name label {0} exist", Name)),
                        string.Empty,
                        ErrorCategory.InvalidArgument,
                        Name));
            }
            else if (Ref != null)
                vdi = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VDI', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VDI));
            }

            return vdi;
        }

        private void ProcessRecordOtherConfig(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_other_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_other_config(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(OtherConfig));

            });
        }

        private void ProcessRecordXenstoreData(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_xenstore_data"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_xenstore_data(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(XenstoreData));

            });
        }

        private void ProcessRecordSmConfig(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_sm_config"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_sm_config(session, vdi, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(SmConfig));

            });
        }

        private void ProcessRecordTags(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_tags"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_tags(session, vdi, Tags);

            });
        }

        private void ProcessRecordManaged(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_managed"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_managed(session, vdi, Managed);

            });
        }

        private void ProcessRecordSharable(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_sharable"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_sharable(session, vdi, Sharable);

            });
        }

        private void ProcessRecordReadOnly(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_read_only"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_read_only(session, vdi, ReadOnly);

            });
        }

        private void ProcessRecordMissing(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_missing"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_missing(session, vdi, Missing);

            });
        }

        private void ProcessRecordVirtualSize(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_virtual_size"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_virtual_size(session, vdi, VirtualSize);

            });
        }

        private void ProcessRecordPhysicalUtilisation(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_physical_utilisation"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_physical_utilisation(session, vdi, PhysicalUtilisation);

            });
        }

        private void ProcessRecordIsASnapshot(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_is_a_snapshot"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_is_a_snapshot(session, vdi, IsASnapshot);

            });
        }

        private void ProcessRecordSnapshotOf(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_snapshot_of"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_snapshot_of(session, vdi, SnapshotOf);

            });
        }

        private void ProcessRecordSnapshotTime(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_snapshot_time"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_snapshot_time(session, vdi, SnapshotTime);

            });
        }

        private void ProcessRecordMetadataOfPool(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_metadata_of_pool"))
                return;

            RunApiCall(()=>
            {
                    XenAPI.VDI.set_metadata_of_pool(session, vdi, MetadataOfPool);

            });
        }

        private void ProcessRecordNameLabel(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_name_label"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_set_name_label(session, vdi, NameLabel);

                }
                else
                {
                    XenAPI.VDI.set_name_label(session, vdi, NameLabel);

                }

            });
        }

        private void ProcessRecordNameDescription(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_name_description"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_set_name_description(session, vdi, NameDescription);

                }
                else
                {
                    XenAPI.VDI.set_name_description(session, vdi, NameDescription);

                }

            });
        }

        private void ProcessRecordOnBoot(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_on_boot"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_set_on_boot(session, vdi, OnBoot);

                }
                else
                {
                    XenAPI.VDI.set_on_boot(session, vdi, OnBoot);

                }

            });
        }

        private void ProcessRecordAllowCaching(string vdi)
        {
            if (!ShouldProcess(vdi, "VDI.set_allow_caching"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.VDI.async_set_allow_caching(session, vdi, AllowCaching);

                }
                else
                {
                    XenAPI.VDI.set_allow_caching(session, vdi, AllowCaching);

                }

            });
        }

        #endregion
    }
}
