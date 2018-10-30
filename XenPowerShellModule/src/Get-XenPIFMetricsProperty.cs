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
    [Cmdlet(VerbsCommon.Get, "XenPIFMetricsProperty", SupportsShouldProcess = false)]
    public class GetXenPIFMetricsProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PIF_metrics PIFMetrics { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PIF_metrics> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPIFMetricsProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pif_metrics = ParsePIFMetrics();

            switch (XenProperty)
            {
                case XenPIFMetricsProperty.Uuid:
                    ProcessRecordUuid(pif_metrics);
                    break;
                case XenPIFMetricsProperty.IoReadKbs:
                    ProcessRecordIoReadKbs(pif_metrics);
                    break;
                case XenPIFMetricsProperty.IoWriteKbs:
                    ProcessRecordIoWriteKbs(pif_metrics);
                    break;
                case XenPIFMetricsProperty.Carrier:
                    ProcessRecordCarrier(pif_metrics);
                    break;
                case XenPIFMetricsProperty.VendorId:
                    ProcessRecordVendorId(pif_metrics);
                    break;
                case XenPIFMetricsProperty.VendorName:
                    ProcessRecordVendorName(pif_metrics);
                    break;
                case XenPIFMetricsProperty.DeviceId:
                    ProcessRecordDeviceId(pif_metrics);
                    break;
                case XenPIFMetricsProperty.DeviceName:
                    ProcessRecordDeviceName(pif_metrics);
                    break;
                case XenPIFMetricsProperty.Speed:
                    ProcessRecordSpeed(pif_metrics);
                    break;
                case XenPIFMetricsProperty.Duplex:
                    ProcessRecordDuplex(pif_metrics);
                    break;
                case XenPIFMetricsProperty.PciBusPath:
                    ProcessRecordPciBusPath(pif_metrics);
                    break;
                case XenPIFMetricsProperty.LastUpdated:
                    ProcessRecordLastUpdated(pif_metrics);
                    break;
                case XenPIFMetricsProperty.OtherConfig:
                    ProcessRecordOtherConfig(pif_metrics);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePIFMetrics()
        {
            string pif_metrics = null;

            if (PIFMetrics != null)
                pif_metrics = (new XenRef<XenAPI.PIF_metrics>(PIFMetrics)).opaque_ref;
            else if (Ref != null)
                pif_metrics = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PIFMetrics', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PIFMetrics));
            }

            return pif_metrics;
        }

        private void ProcessRecordUuid(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_uuid(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIoReadKbs(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.PIF_metrics.get_io_read_kbs(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIoWriteKbs(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.PIF_metrics.get_io_write_kbs(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCarrier(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF_metrics.get_carrier(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorId(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_vendor_id(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorName(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_vendor_name(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDeviceId(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_device_id(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDeviceName(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_device_name(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSpeed(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PIF_metrics.get_speed(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDuplex(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF_metrics.get_duplex(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPciBusPath(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF_metrics.get_pci_bus_path(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastUpdated(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.PIF_metrics.get_last_updated(session, pif_metrics);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string pif_metrics)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PIF_metrics.get_other_config(session, pif_metrics);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenPIFMetricsProperty
    {
        Uuid,
        IoReadKbs,
        IoWriteKbs,
        Carrier,
        VendorId,
        VendorName,
        DeviceId,
        DeviceName,
        Speed,
        Duplex,
        PciBusPath,
        LastUpdated,
        OtherConfig
    }

}
