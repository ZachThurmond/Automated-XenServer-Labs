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
    [Cmdlet(VerbsCommon.Get, "XenUSBGroupProperty", SupportsShouldProcess = false)]
    public class GetXenUSBGroupProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.USB_group USBGroup { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.USB_group> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenUSBGroupProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string usb_group = ParseUSBGroup();

            switch (XenProperty)
            {
                case XenUSBGroupProperty.Uuid:
                    ProcessRecordUuid(usb_group);
                    break;
                case XenUSBGroupProperty.NameLabel:
                    ProcessRecordNameLabel(usb_group);
                    break;
                case XenUSBGroupProperty.NameDescription:
                    ProcessRecordNameDescription(usb_group);
                    break;
                case XenUSBGroupProperty.PUSBs:
                    ProcessRecordPUSBs(usb_group);
                    break;
                case XenUSBGroupProperty.VUSBs:
                    ProcessRecordVUSBs(usb_group);
                    break;
                case XenUSBGroupProperty.OtherConfig:
                    ProcessRecordOtherConfig(usb_group);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseUSBGroup()
        {
            string usb_group = null;

            if (USBGroup != null)
                usb_group = (new XenRef<XenAPI.USB_group>(USBGroup)).opaque_ref;
            else if (Ref != null)
                usb_group = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'USBGroup', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    USBGroup));
            }

            return usb_group;
        }

        private void ProcessRecordUuid(string usb_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.USB_group.get_uuid(session, usb_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string usb_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.USB_group.get_name_label(session, usb_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string usb_group)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.USB_group.get_name_description(session, usb_group);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPUSBs(string usb_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.USB_group.get_PUSBs(session, usb_group);

                        var records = new List<XenAPI.PUSB>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PUSB.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVUSBs(string usb_group)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.USB_group.get_VUSBs(session, usb_group);

                        var records = new List<XenAPI.VUSB>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VUSB.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordOtherConfig(string usb_group)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.USB_group.get_other_config(session, usb_group);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenUSBGroupProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        PUSBs,
        VUSBs,
        OtherConfig
    }

}
