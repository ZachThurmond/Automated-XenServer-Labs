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
    [Cmdlet(VerbsCommon.Get, "XenHostPatchProperty", SupportsShouldProcess = false)]
    public class GetXenHostPatchProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host_patch HostPatch { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host_patch> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostPatchProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host_patch = ParseHostPatch();

            switch (XenProperty)
            {
                case XenHostPatchProperty.Uuid:
                    ProcessRecordUuid(host_patch);
                    break;
                case XenHostPatchProperty.NameLabel:
                    ProcessRecordNameLabel(host_patch);
                    break;
                case XenHostPatchProperty.NameDescription:
                    ProcessRecordNameDescription(host_patch);
                    break;
                case XenHostPatchProperty.Version:
                    ProcessRecordVersion(host_patch);
                    break;
                case XenHostPatchProperty.Host:
                    ProcessRecordHost(host_patch);
                    break;
                case XenHostPatchProperty.Applied:
                    ProcessRecordApplied(host_patch);
                    break;
                case XenHostPatchProperty.TimestampApplied:
                    ProcessRecordTimestampApplied(host_patch);
                    break;
                case XenHostPatchProperty.Size:
                    ProcessRecordSize(host_patch);
                    break;
                case XenHostPatchProperty.PoolPatch:
                    ProcessRecordPoolPatch(host_patch);
                    break;
                case XenHostPatchProperty.OtherConfig:
                    ProcessRecordOtherConfig(host_patch);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseHostPatch()
        {
            string host_patch = null;

            if (HostPatch != null)
                host_patch = (new XenRef<XenAPI.Host_patch>(HostPatch)).opaque_ref;
            else if (Ref != null)
                host_patch = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'HostPatch', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    HostPatch));
            }

            return host_patch;
        }

        private void ProcessRecordUuid(string host_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_patch.get_uuid(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string host_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_patch.get_name_label(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string host_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_patch.get_name_description(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string host_patch)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_patch.get_version(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string host_patch)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host_patch.get_host(session, host_patch);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordApplied(string host_patch)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Host_patch.get_applied(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTimestampApplied(string host_patch)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Host_patch.get_timestamp_applied(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSize(string host_patch)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_patch.get_size(session, host_patch);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPoolPatch(string host_patch)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host_patch.get_pool_patch(session, host_patch);

                        XenAPI.Pool_patch obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Pool_patch.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string host_patch)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host_patch.get_other_config(session, host_patch);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenHostPatchProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Version,
        Host,
        Applied,
        TimestampApplied,
        Size,
        PoolPatch,
        OtherConfig
    }

}
