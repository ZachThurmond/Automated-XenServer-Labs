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
    [Cmdlet(VerbsCommon.Get, "XenHostCpuProperty", SupportsShouldProcess = false)]
    public class GetXenHostCpuProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host_cpu HostCpu { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host_cpu> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostCpuProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host_cpu = ParseHostCpu();

            switch (XenProperty)
            {
                case XenHostCpuProperty.Uuid:
                    ProcessRecordUuid(host_cpu);
                    break;
                case XenHostCpuProperty.Host:
                    ProcessRecordHost(host_cpu);
                    break;
                case XenHostCpuProperty.Number:
                    ProcessRecordNumber(host_cpu);
                    break;
                case XenHostCpuProperty.Vendor:
                    ProcessRecordVendor(host_cpu);
                    break;
                case XenHostCpuProperty.Speed:
                    ProcessRecordSpeed(host_cpu);
                    break;
                case XenHostCpuProperty.Modelname:
                    ProcessRecordModelname(host_cpu);
                    break;
                case XenHostCpuProperty.Family:
                    ProcessRecordFamily(host_cpu);
                    break;
                case XenHostCpuProperty.Model:
                    ProcessRecordModel(host_cpu);
                    break;
                case XenHostCpuProperty.Stepping:
                    ProcessRecordStepping(host_cpu);
                    break;
                case XenHostCpuProperty.Flags:
                    ProcessRecordFlags(host_cpu);
                    break;
                case XenHostCpuProperty.Features:
                    ProcessRecordFeatures(host_cpu);
                    break;
                case XenHostCpuProperty.Utilisation:
                    ProcessRecordUtilisation(host_cpu);
                    break;
                case XenHostCpuProperty.OtherConfig:
                    ProcessRecordOtherConfig(host_cpu);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseHostCpu()
        {
            string host_cpu = null;

            if (HostCpu != null)
                host_cpu = (new XenRef<XenAPI.Host_cpu>(HostCpu)).opaque_ref;
            else if (Ref != null)
                host_cpu = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'HostCpu', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    HostCpu));
            }

            return host_cpu;
        }

        private void ProcessRecordUuid(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_uuid(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host_cpu.get_host(session, host_cpu);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNumber(string host_cpu)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_cpu.get_number(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendor(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_vendor(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSpeed(string host_cpu)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_cpu.get_speed(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordModelname(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_modelname(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFamily(string host_cpu)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_cpu.get_family(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordModel(string host_cpu)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host_cpu.get_model(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStepping(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_stepping(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFlags(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_flags(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFeatures(string host_cpu)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host_cpu.get_features(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUtilisation(string host_cpu)
        {
            RunApiCall(()=>
            {
                    double obj = XenAPI.Host_cpu.get_utilisation(session, host_cpu);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string host_cpu)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host_cpu.get_other_config(session, host_cpu);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenHostCpuProperty
    {
        Uuid,
        Host,
        Number,
        Vendor,
        Speed,
        Modelname,
        Family,
        Model,
        Stepping,
        Flags,
        Features,
        Utilisation,
        OtherConfig
    }

}
