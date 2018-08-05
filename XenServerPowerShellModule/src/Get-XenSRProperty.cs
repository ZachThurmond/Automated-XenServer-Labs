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
    [Cmdlet(VerbsCommon.Get, "XenSRProperty", SupportsShouldProcess = false)]
    public class GetXenSRProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SR SR { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SR> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenSRProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sr = ParseSR();

            switch (XenProperty)
            {
                case XenSRProperty.Uuid:
                    ProcessRecordUuid(sr);
                    break;
                case XenSRProperty.NameLabel:
                    ProcessRecordNameLabel(sr);
                    break;
                case XenSRProperty.NameDescription:
                    ProcessRecordNameDescription(sr);
                    break;
                case XenSRProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(sr);
                    break;
                case XenSRProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(sr);
                    break;
                case XenSRProperty.VDIs:
                    ProcessRecordVDIs(sr);
                    break;
                case XenSRProperty.PBDs:
                    ProcessRecordPBDs(sr);
                    break;
                case XenSRProperty.VirtualAllocation:
                    ProcessRecordVirtualAllocation(sr);
                    break;
                case XenSRProperty.PhysicalUtilisation:
                    ProcessRecordPhysicalUtilisation(sr);
                    break;
                case XenSRProperty.PhysicalSize:
                    ProcessRecordPhysicalSize(sr);
                    break;
                case XenSRProperty.Type:
                    ProcessRecordType(sr);
                    break;
                case XenSRProperty.ContentType:
                    ProcessRecordContentType(sr);
                    break;
                case XenSRProperty.Shared:
                    ProcessRecordShared(sr);
                    break;
                case XenSRProperty.OtherConfig:
                    ProcessRecordOtherConfig(sr);
                    break;
                case XenSRProperty.Tags:
                    ProcessRecordTags(sr);
                    break;
                case XenSRProperty.SmConfig:
                    ProcessRecordSmConfig(sr);
                    break;
                case XenSRProperty.Blobs:
                    ProcessRecordBlobs(sr);
                    break;
                case XenSRProperty.LocalCacheEnabled:
                    ProcessRecordLocalCacheEnabled(sr);
                    break;
                case XenSRProperty.IntroducedBy:
                    ProcessRecordIntroducedBy(sr);
                    break;
                case XenSRProperty.Clustered:
                    ProcessRecordClustered(sr);
                    break;
                case XenSRProperty.IsToolsSr:
                    ProcessRecordIsToolsSr(sr);
                    break;
                case XenSRProperty.SupportedTypes:
                    ProcessRecordSupportedTypes(sr);
                    break;
                case XenSRProperty.DataSources:
                    ProcessRecordDataSources(sr);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSR()
        {
            string sr = null;

            if (SR != null)
                sr = (new XenRef<XenAPI.SR>(SR)).opaque_ref;
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

        private void ProcessRecordUuid(string sr)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SR.get_uuid(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string sr)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SR.get_name_label(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string sr)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SR.get_name_description(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string sr)
        {
            RunApiCall(()=>
            {
                    List<storage_operations> obj = XenAPI.SR.get_allowed_operations(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string sr)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SR.get_current_operations(session, sr);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordVDIs(string sr)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.SR.get_VDIs(session, sr);

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

        private void ProcessRecordPBDs(string sr)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.SR.get_PBDs(session, sr);

                        var records = new List<XenAPI.PBD>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PBD.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVirtualAllocation(string sr)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.SR.get_virtual_allocation(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPhysicalUtilisation(string sr)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.SR.get_physical_utilisation(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPhysicalSize(string sr)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.SR.get_physical_size(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string sr)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SR.get_type(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordContentType(string sr)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SR.get_content_type(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordShared(string sr)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.SR.get_shared(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string sr)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SR.get_other_config(session, sr);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordTags(string sr)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.SR.get_tags(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSmConfig(string sr)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SR.get_sm_config(session, sr);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordBlobs(string sr)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SR.get_blobs(session, sr);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLocalCacheEnabled(string sr)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.SR.get_local_cache_enabled(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIntroducedBy(string sr)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.SR.get_introduced_by(session, sr);

                        XenAPI.DR_task obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.DR_task.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordClustered(string sr)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.SR.get_clustered(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIsToolsSr(string sr)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.SR.get_is_tools_sr(session, sr);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSupportedTypes(string sr)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.SR.get_supported_types(session);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDataSources(string sr)
        {
            RunApiCall(()=>
            {
                    List<XenAPI.Data_source> obj = XenAPI.SR.get_data_sources(session, sr);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenSRProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        AllowedOperations,
        CurrentOperations,
        VDIs,
        PBDs,
        VirtualAllocation,
        PhysicalUtilisation,
        PhysicalSize,
        Type,
        ContentType,
        Shared,
        OtherConfig,
        Tags,
        SmConfig,
        Blobs,
        LocalCacheEnabled,
        IntroducedBy,
        Clustered,
        IsToolsSr,
        SupportedTypes,
        DataSources
    }

}
