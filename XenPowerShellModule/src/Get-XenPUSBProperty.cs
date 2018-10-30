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
    [Cmdlet(VerbsCommon.Get, "XenPUSBProperty", SupportsShouldProcess = false)]
    public class GetXenPUSBProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PUSB PUSB { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PUSB> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPUSBProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pusb = ParsePUSB();

            switch (XenProperty)
            {
                case XenPUSBProperty.Uuid:
                    ProcessRecordUuid(pusb);
                    break;
                case XenPUSBProperty.USBGroup:
                    ProcessRecordUSBGroup(pusb);
                    break;
                case XenPUSBProperty.Host:
                    ProcessRecordHost(pusb);
                    break;
                case XenPUSBProperty.Path:
                    ProcessRecordPath(pusb);
                    break;
                case XenPUSBProperty.VendorId:
                    ProcessRecordVendorId(pusb);
                    break;
                case XenPUSBProperty.VendorDesc:
                    ProcessRecordVendorDesc(pusb);
                    break;
                case XenPUSBProperty.ProductId:
                    ProcessRecordProductId(pusb);
                    break;
                case XenPUSBProperty.ProductDesc:
                    ProcessRecordProductDesc(pusb);
                    break;
                case XenPUSBProperty.Serial:
                    ProcessRecordSerial(pusb);
                    break;
                case XenPUSBProperty.Version:
                    ProcessRecordVersion(pusb);
                    break;
                case XenPUSBProperty.Description:
                    ProcessRecordDescription(pusb);
                    break;
                case XenPUSBProperty.PassthroughEnabled:
                    ProcessRecordPassthroughEnabled(pusb);
                    break;
                case XenPUSBProperty.OtherConfig:
                    ProcessRecordOtherConfig(pusb);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePUSB()
        {
            string pusb = null;

            if (PUSB != null)
                pusb = (new XenRef<XenAPI.PUSB>(PUSB)).opaque_ref;
            else if (Ref != null)
                pusb = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PUSB', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PUSB));
            }

            return pusb;
        }

        private void ProcessRecordUuid(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_uuid(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUSBGroup(string pusb)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PUSB.get_USB_group(session, pusb);

                        XenAPI.USB_group obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.USB_group.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string pusb)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PUSB.get_host(session, pusb);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPath(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_path(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorId(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_vendor_id(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorDesc(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_vendor_desc(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProductId(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_product_id(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProductDesc(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_product_desc(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSerial(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_serial(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_version(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDescription(string pusb)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PUSB.get_description(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPassthroughEnabled(string pusb)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PUSB.get_passthrough_enabled(session, pusb);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string pusb)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PUSB.get_other_config(session, pusb);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenPUSBProperty
    {
        Uuid,
        USBGroup,
        Host,
        Path,
        VendorId,
        VendorDesc,
        ProductId,
        ProductDesc,
        Serial,
        Version,
        Description,
        PassthroughEnabled,
        OtherConfig
    }

}
