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
    [Cmdlet(VerbsCommon.Get, "XenPCIProperty", SupportsShouldProcess = false)]
    public class GetXenPCIProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PCI PCI { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PCI> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPCIProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pci = ParsePCI();

            switch (XenProperty)
            {
                case XenPCIProperty.Uuid:
                    ProcessRecordUuid(pci);
                    break;
                case XenPCIProperty.ClassName:
                    ProcessRecordClassName(pci);
                    break;
                case XenPCIProperty.VendorName:
                    ProcessRecordVendorName(pci);
                    break;
                case XenPCIProperty.DeviceName:
                    ProcessRecordDeviceName(pci);
                    break;
                case XenPCIProperty.Host:
                    ProcessRecordHost(pci);
                    break;
                case XenPCIProperty.PciId:
                    ProcessRecordPciId(pci);
                    break;
                case XenPCIProperty.Dependencies:
                    ProcessRecordDependencies(pci);
                    break;
                case XenPCIProperty.OtherConfig:
                    ProcessRecordOtherConfig(pci);
                    break;
                case XenPCIProperty.SubsystemVendorName:
                    ProcessRecordSubsystemVendorName(pci);
                    break;
                case XenPCIProperty.SubsystemDeviceName:
                    ProcessRecordSubsystemDeviceName(pci);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePCI()
        {
            string pci = null;

            if (PCI != null)
                pci = (new XenRef<XenAPI.PCI>(PCI)).opaque_ref;
            else if (Ref != null)
                pci = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PCI', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PCI));
            }

            return pci;
        }

        private void ProcessRecordUuid(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_uuid(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordClassName(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_class_name(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendorName(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_vendor_name(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDeviceName(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_device_name(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string pci)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PCI.get_host(session, pci);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPciId(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_pci_id(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDependencies(string pci)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PCI.get_dependencies(session, pci);

                        var records = new List<XenAPI.PCI>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PCI.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordOtherConfig(string pci)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PCI.get_other_config(session, pci);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSubsystemVendorName(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_subsystem_vendor_name(session, pci);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSubsystemDeviceName(string pci)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PCI.get_subsystem_device_name(session, pci);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPCIProperty
    {
        Uuid,
        ClassName,
        VendorName,
        DeviceName,
        Host,
        PciId,
        Dependencies,
        OtherConfig,
        SubsystemVendorName,
        SubsystemDeviceName
    }

}
