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
    [Cmdlet(VerbsCommon.Get, "XenVIFMetricsProperty", SupportsShouldProcess = false)]
    public class GetXenVIFMetricsProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VIF_metrics VIFMetrics { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VIF_metrics> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVIFMetricsProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vif_metrics = ParseVIFMetrics();

            switch (XenProperty)
            {
                case XenVIFMetricsProperty.Uuid:
                    ProcessRecordUuid(vif_metrics);
                    break;
                case XenVIFMetricsProperty.IoReadKbs:
                    ProcessRecordIoReadKbs(vif_metrics);
                    break;
                case XenVIFMetricsProperty.IoWriteKbs:
                    ProcessRecordIoWriteKbs(vif_metrics);
                    break;
                case XenVIFMetricsProperty.LastUpdated:
                    ProcessRecordLastUpdated(vif_metrics);
                    break;
                case XenVIFMetricsProperty.OtherConfig:
                    ProcessRecordOtherConfig(vif_metrics);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVIFMetrics()
        {
            string vif_metrics = null;

            if (VIFMetrics != null)
                vif_metrics = (new XenRef<XenAPI.VIF_metrics>(VIFMetrics)).opaque_ref;
            else if (Ref != null)
                vif_metrics = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VIFMetrics', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VIFMetrics));
            }

            return vif_metrics;
        }

        private void ProcessRecordUuid(string vif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VIF_metrics.get_uuid(session, vif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIoReadKbs(string vif_metrics)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.VIF_metrics.get_io_read_kbs(session, vif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIoWriteKbs(string vif_metrics)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.VIF_metrics.get_io_write_kbs(session, vif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastUpdated(string vif_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VIF_metrics.get_last_updated(session, vif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vif_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VIF_metrics.get_other_config(session, vif_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenVIFMetricsProperty
    {
        Uuid,
        IoReadKbs,
        IoWriteKbs,
        LastUpdated,
        OtherConfig
    }

}
