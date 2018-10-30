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
    [Cmdlet(VerbsCommon.Get, "XenHostMetricsProperty", SupportsShouldProcess = false)]
    public class GetXenHostMetricsProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host_metrics HostMetrics { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host_metrics> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostMetricsProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host_metrics = ParseHostMetrics();

            switch (XenProperty)
            {
                case XenHostMetricsProperty.Uuid:
                    ProcessRecordUuid(host_metrics);
                    break;
                case XenHostMetricsProperty.MemoryTotal:
                    ProcessRecordMemoryTotal(host_metrics);
                    break;
                case XenHostMetricsProperty.MemoryFree:
                    ProcessRecordMemoryFree(host_metrics);
                    break;
                case XenHostMetricsProperty.Live:
                    ProcessRecordLive(host_metrics);
                    break;
                case XenHostMetricsProperty.LastUpdated:
                    ProcessRecordLastUpdated(host_metrics);
                    break;
                case XenHostMetricsProperty.OtherConfig:
                    ProcessRecordOtherConfig(host_metrics);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseHostMetrics()
        {
            string host_metrics = null;

            if (HostMetrics != null)
                host_metrics = (new XenRef<XenAPI.Host_metrics>(HostMetrics)).opaque_ref;
            else if (Ref != null)
                host_metrics = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'HostMetrics', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    HostMetrics));
            }

            return host_metrics;
        }

        private void ProcessRecordUuid(string host_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_metrics.get_uuid(session, host_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryTotal(string host_metrics)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_metrics.get_memory_total(session, host_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryFree(string host_metrics)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_metrics.get_memory_free(session, host_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLive(string host_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Host_metrics.get_live(session, host_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastUpdated(string host_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Host_metrics.get_last_updated(session, host_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string host_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host_metrics.get_other_config(session, host_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenHostMetricsProperty
    {
        Uuid,
        MemoryTotal,
        MemoryFree,
        Live,
        LastUpdated,
        OtherConfig
    }

}
