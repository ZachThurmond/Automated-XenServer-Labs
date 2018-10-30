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
    [Cmdlet(VerbsCommon.Get, "XenPoolPatchProperty", SupportsShouldProcess = false)]
    public class GetXenPoolPatchProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool_patch PoolPatch { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool_patch> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPoolPatchProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool_patch = ParsePoolPatch();

            switch (XenProperty)
            {
                case XenPoolPatchProperty.Uuid:
                    ProcessRecordUuid(pool_patch);
                    break;
                case XenPoolPatchProperty.NameLabel:
                    ProcessRecordNameLabel(pool_patch);
                    break;
                case XenPoolPatchProperty.NameDescription:
                    ProcessRecordNameDescription(pool_patch);
                    break;
                case XenPoolPatchProperty.Version:
                    ProcessRecordVersion(pool_patch);
                    break;
                case XenPoolPatchProperty.Size:
                    ProcessRecordSize(pool_patch);
                    break;
                case XenPoolPatchProperty.PoolApplied:
                    ProcessRecordPoolApplied(pool_patch);
                    break;
                case XenPoolPatchProperty.HostPatches:
                    ProcessRecordHostPatches(pool_patch);
                    break;
                case XenPoolPatchProperty.AfterApplyGuidance:
                    ProcessRecordAfterApplyGuidance(pool_patch);
                    break;
                case XenPoolPatchProperty.PoolUpdate:
                    ProcessRecordPoolUpdate(pool_patch);
                    break;
                case XenPoolPatchProperty.OtherConfig:
                    ProcessRecordOtherConfig(pool_patch);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePoolPatch()
        {
            string pool_patch = null;

            if (PoolPatch != null)
                pool_patch = (new XenRef<XenAPI.Pool_patch>(PoolPatch)).opaque_ref;
            else if (Ref != null)
                pool_patch = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PoolPatch', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PoolPatch));
            }

            return pool_patch;
        }

        private void ProcessRecordUuid(string pool_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_patch.get_uuid(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string pool_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_patch.get_name_label(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string pool_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_patch.get_name_description(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string pool_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_patch.get_version(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSize(string pool_patch)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Pool_patch.get_size(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPoolApplied(string pool_patch)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Pool_patch.get_pool_applied(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHostPatches(string pool_patch)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Pool_patch.get_host_patches(session, pool_patch);

                        var records = new List<XenAPI.Host_patch>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host_patch.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordAfterApplyGuidance(string pool_patch)
        {
            RunApiCall(()=>
            {
                    List<after_apply_guidance> obj = XenAPI.Pool_patch.get_after_apply_guidance(session, pool_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPoolUpdate(string pool_patch)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Pool_patch.get_pool_update(session, pool_patch);

                        XenAPI.Pool_update obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Pool_update.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string pool_patch)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Pool_patch.get_other_config(session, pool_patch);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenPoolPatchProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Version,
        Size,
        PoolApplied,
        HostPatches,
        AfterApplyGuidance,
        PoolUpdate,
        OtherConfig
    }

}
