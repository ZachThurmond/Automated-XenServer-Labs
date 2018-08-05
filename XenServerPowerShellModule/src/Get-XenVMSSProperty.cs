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
    [Cmdlet(VerbsCommon.Get, "XenVMSSProperty", SupportsShouldProcess = false)]
    public class GetXenVMSSProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VMSS VMSS { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VMSS> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVMSSProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vmss = ParseVMSS();

            switch (XenProperty)
            {
                case XenVMSSProperty.Uuid:
                    ProcessRecordUuid(vmss);
                    break;
                case XenVMSSProperty.NameLabel:
                    ProcessRecordNameLabel(vmss);
                    break;
                case XenVMSSProperty.NameDescription:
                    ProcessRecordNameDescription(vmss);
                    break;
                case XenVMSSProperty.Enabled:
                    ProcessRecordEnabled(vmss);
                    break;
                case XenVMSSProperty.Type:
                    ProcessRecordType(vmss);
                    break;
                case XenVMSSProperty.RetainedSnapshots:
                    ProcessRecordRetainedSnapshots(vmss);
                    break;
                case XenVMSSProperty.Frequency:
                    ProcessRecordFrequency(vmss);
                    break;
                case XenVMSSProperty.Schedule:
                    ProcessRecordSchedule(vmss);
                    break;
                case XenVMSSProperty.LastRunTime:
                    ProcessRecordLastRunTime(vmss);
                    break;
                case XenVMSSProperty.VMs:
                    ProcessRecordVMs(vmss);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVMSS()
        {
            string vmss = null;

            if (VMSS != null)
                vmss = (new XenRef<XenAPI.VMSS>(VMSS)).opaque_ref;
            else if (Ref != null)
                vmss = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VMSS', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VMSS));
            }

            return vmss;
        }

        private void ProcessRecordUuid(string vmss)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMSS.get_uuid(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string vmss)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMSS.get_name_label(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string vmss)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VMSS.get_name_description(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordEnabled(string vmss)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.VMSS.get_enabled(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string vmss)
        {
            RunApiCall(()=>
            {
                    vmss_type obj = XenAPI.VMSS.get_type(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRetainedSnapshots(string vmss)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VMSS.get_retained_snapshots(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFrequency(string vmss)
        {
            RunApiCall(()=>
            {
                    vmss_frequency obj = XenAPI.VMSS.get_frequency(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSchedule(string vmss)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VMSS.get_schedule(session, vmss);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLastRunTime(string vmss)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.VMSS.get_last_run_time(session, vmss);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVMs(string vmss)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.VMSS.get_VMs(session, vmss);

                        var records = new List<XenAPI.VM>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VM.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        #endregion
    }

    public enum XenVMSSProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Enabled,
        Type,
        RetainedSnapshots,
        Frequency,
        Schedule,
        LastRunTime,
        VMs
    }

}
