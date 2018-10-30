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
    [Cmdlet(VerbsCommon.Get, "XenHostCrashdumpProperty", SupportsShouldProcess = false)]
    public class GetXenHostCrashdumpProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host_crashdump HostCrashdump { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host_crashdump> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostCrashdumpProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host_crashdump = ParseHostCrashdump();

            switch (XenProperty)
            {
                case XenHostCrashdumpProperty.Uuid:
                    ProcessRecordUuid(host_crashdump);
                    break;
                case XenHostCrashdumpProperty.Host:
                    ProcessRecordHost(host_crashdump);
                    break;
                case XenHostCrashdumpProperty.Timestamp:
                    ProcessRecordTimestamp(host_crashdump);
                    break;
                case XenHostCrashdumpProperty.Size:
                    ProcessRecordSize(host_crashdump);
                    break;
                case XenHostCrashdumpProperty.OtherConfig:
                    ProcessRecordOtherConfig(host_crashdump);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseHostCrashdump()
        {
            string host_crashdump = null;

            if (HostCrashdump != null)
                host_crashdump = (new XenRef<XenAPI.Host_crashdump>(HostCrashdump)).opaque_ref;
            else if (Ref != null)
                host_crashdump = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'HostCrashdump', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    HostCrashdump));
            }

            return host_crashdump;
        }

        private void ProcessRecordUuid(string host_crashdump)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_crashdump.get_uuid(session, host_crashdump);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string host_crashdump)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host_crashdump.get_host(session, host_crashdump);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTimestamp(string host_crashdump)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Host_crashdump.get_timestamp(session, host_crashdump);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSize(string host_crashdump)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_crashdump.get_size(session, host_crashdump);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string host_crashdump)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host_crashdump.get_other_config(session, host_crashdump);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenHostCrashdumpProperty
    {
        Uuid,
        Host,
        Timestamp,
        Size,
        OtherConfig
    }

}
