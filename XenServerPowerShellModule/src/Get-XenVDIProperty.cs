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
    [Cmdlet(VerbsCommon.Get, "XenVDIProperty", SupportsShouldProcess = false)]
    public class GetXenVDIProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VDI VDI { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VDI> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVDIProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vdi = ParseVDI();

            switch (XenProperty)
            {
                case XenVDIProperty.Uuid:
                    ProcessRecordUuid(vdi);
                    break;
                case XenVDIProperty.NameLabel:
                    ProcessRecordNameLabel(vdi);
                    break;
                case XenVDIProperty.NameDescription:
                    ProcessRecordNameDescription(vdi);
                    break;
                case XenVDIProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(vdi);
                    break;
                case XenVDIProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(vdi);
                    break;
                case XenVDIProperty.SR:
                    ProcessRecordSR(vdi);
                    break;
                case XenVDIProperty.VBDs:
                    ProcessRecordVBDs(vdi);
                    break;
                case XenVDIProperty.CrashDumps:
                    ProcessRecordCrashDumps(vdi);
                    break;
                case XenVDIProperty.VirtualSize:
                    ProcessRecordVirtualSize(vdi);
                    break;
                case XenVDIProperty.PhysicalUtilisation:
                    ProcessRecordPhysicalUtilisation(vdi);
                    break;
                case XenVDIProperty.Type:
                    ProcessRecordType(vdi);
                    break;
                case XenVDIProperty.Sharable:
                    ProcessRecordSharable(vdi);
                    break;
                case XenVDIProperty.ReadOnly:
                    ProcessRecordReadOnly(vdi);
                    break;
                case XenVDIProperty.OtherConfig:
                    ProcessRecordOtherConfig(vdi);
                    break;
                case XenVDIProperty.StorageLock:
                    ProcessRecordStorageLock(vdi);
                    break;
                case XenVDIProperty.Location:
                    ProcessRecordLocation(vdi);
                    break;
                case XenVDIProperty.Managed:
                    ProcessRecordManaged(vdi);
                    break;
                case XenVDIProperty.Missing:
                    ProcessRecordMissing(vdi);
                    break;
                case XenVDIProperty.Parent:
                    ProcessRecordParent(vdi);
                    break;
                case XenVDIProperty.XenstoreData:
                    ProcessRecordXenstoreData(vdi);
                    break;
                case XenVDIProperty.SmConfig:
                    ProcessRecordSmConfig(vdi);
                    break;
                case XenVDIProperty.IsASnapshot:
                    ProcessRecordIsASnapshot(vdi);
                    break;
                case XenVDIProperty.SnapshotOf:
                    ProcessRecordSnapshotOf(vdi);
                    break;
                case XenVDIProperty.Snapshots:
                    ProcessRecordSnapshots(vdi);
                    break;
                case XenVDIProperty.SnapshotTime:
                    ProcessRecordSnapshotTime(vdi);
                    break;
                case XenVDIProperty.Tags:
                    ProcessRecordTags(vdi);
                    break;
                case XenVDIProperty.AllowCaching:
                    ProcessRecordAllowCaching(vdi);
                    break;
                case XenVDIProperty.OnBoot:
                    ProcessRecordOnBoot(vdi);
                    break;
                case XenVDIProperty.MetadataOfPool:
                    ProcessRecordMetadataOfPool(vdi);
                    break;
                case XenVDIProperty.MetadataLatest:
                    ProcessRecordMetadataLatest(vdi);
                    break;
                case XenVDIProperty.IsToolsIso:
                    ProcessRecordIsToolsIso(vdi);
                    break;
                case XenVDIProperty.CbtEnabled:
                    ProcessRecordCbtEnabled(vdi);
                    break;
                case XenVDIProperty.NbdInfo:
                    ProcessRecordNbdInfo(vdi);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVDI()
        {
            string vdi = null;

            if (VDI != null)
                vdi = (new XenRef<XenAPI.VDI>(VDI)).opaque_ref;
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

        private void ProcessRecordUuid(string vdi)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VDI.get_uuid(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string vdi)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VDI.get_name_label(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string vdi)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VDI.get_name_description(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string vdi)
        {
            RunApiCall(()=>
            {
                    List<vdi_operations> obj = XenAPI.VDI.get_allowed_operations(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string vdi)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VDI.get_current_operations(session, vdi);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSR(string vdi)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VDI.get_SR(session, vdi);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVBDs(string vdi)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VDI.get_VBDs(session, vdi);

                        var records = new List<XenAPI.VBD>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VBD.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordCrashDumps(string vdi)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VDI.get_crash_dumps(session, vdi);

                        var records = new List<XenAPI.Crashdump>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Crashdump.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVirtualSize(string vdi)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VDI.get_virtual_size(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPhysicalUtilisation(string vdi)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VDI.get_physical_utilisation(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string vdi)
        {
            RunApiCall(()=>
            {
                    vdi_type obj = XenAPI.VDI.get_type(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSharable(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_sharable(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordReadOnly(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_read_only(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vdi)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VDI.get_other_config(session, vdi);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordStorageLock(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_storage_lock(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLocation(string vdi)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VDI.get_location(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordManaged(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_managed(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMissing(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_missing(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordParent(string vdi)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VDI.get_parent(session, vdi);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordXenstoreData(string vdi)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VDI.get_xenstore_data(session, vdi);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSmConfig(string vdi)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VDI.get_sm_config(session, vdi);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordIsASnapshot(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_is_a_snapshot(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSnapshotOf(string vdi)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VDI.get_snapshot_of(session, vdi);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSnapshots(string vdi)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VDI.get_snapshots(session, vdi);

                        var records = new List<XenAPI.VDI>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VDI.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSnapshotTime(string vdi)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VDI.get_snapshot_time(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTags(string vdi)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.VDI.get_tags(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowCaching(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_allow_caching(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOnBoot(string vdi)
        {
            RunApiCall(()=>
            {
                    on_boot obj = XenAPI.VDI.get_on_boot(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetadataOfPool(string vdi)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VDI.get_metadata_of_pool(session, vdi);

                        XenAPI.Pool obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Pool.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetadataLatest(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_metadata_latest(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsToolsIso(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_is_tools_iso(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCbtEnabled(string vdi)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VDI.get_cbt_enabled(session, vdi);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNbdInfo(string vdi)
        {
            RunApiCall(()=>
            {
                    List<XenAPI.Vdi_nbd_server_info> obj = XenAPI.VDI.get_nbd_info(session, vdi);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVDIProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        AllowedOperations,
        CurrentOperations,
        SR,
        VBDs,
        CrashDumps,
        VirtualSize,
        PhysicalUtilisation,
        Type,
        Sharable,
        ReadOnly,
        OtherConfig,
        StorageLock,
        Location,
        Managed,
        Missing,
        Parent,
        XenstoreData,
        SmConfig,
        IsASnapshot,
        SnapshotOf,
        Snapshots,
        SnapshotTime,
        Tags,
        AllowCaching,
        OnBoot,
        MetadataOfPool,
        MetadataLatest,
        IsToolsIso,
        CbtEnabled,
        NbdInfo
    }

}
