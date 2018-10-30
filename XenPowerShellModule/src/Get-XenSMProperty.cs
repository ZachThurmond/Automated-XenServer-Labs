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
    [Cmdlet(VerbsCommon.Get, "XenSMProperty", SupportsShouldProcess = false)]
    public class GetXenSMProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SM SM { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SM> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenSMProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sm = ParseSM();

            switch (XenProperty)
            {
                case XenSMProperty.Uuid:
                    ProcessRecordUuid(sm);
                    break;
                case XenSMProperty.NameLabel:
                    ProcessRecordNameLabel(sm);
                    break;
                case XenSMProperty.NameDescription:
                    ProcessRecordNameDescription(sm);
                    break;
                case XenSMProperty.Type:
                    ProcessRecordType(sm);
                    break;
                case XenSMProperty.Vendor:
                    ProcessRecordVendor(sm);
                    break;
                case XenSMProperty.Copyright:
                    ProcessRecordCopyright(sm);
                    break;
                case XenSMProperty.Version:
                    ProcessRecordVersion(sm);
                    break;
                case XenSMProperty.RequiredApiVersion:
                    ProcessRecordRequiredApiVersion(sm);
                    break;
                case XenSMProperty.Configuration:
                    ProcessRecordConfiguration(sm);
                    break;
                case XenSMProperty.Capabilities:
                    ProcessRecordCapabilities(sm);
                    break;
                case XenSMProperty.Features:
                    ProcessRecordFeatures(sm);
                    break;
                case XenSMProperty.OtherConfig:
                    ProcessRecordOtherConfig(sm);
                    break;
                case XenSMProperty.DriverFilename:
                    ProcessRecordDriverFilename(sm);
                    break;
                case XenSMProperty.RequiredClusterStack:
                    ProcessRecordRequiredClusterStack(sm);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSM()
        {
            string sm = null;

            if (SM != null)
                sm = (new XenRef<XenAPI.SM>(SM)).opaque_ref;
            else if (Ref != null)
                sm = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'SM', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    SM));
            }

            return sm;
        }

        private void ProcessRecordUuid(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_uuid(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_name_label(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_name_description(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordType(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_type(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVendor(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_vendor(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCopyright(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_copyright(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_version(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRequiredApiVersion(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_required_api_version(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordConfiguration(string sm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SM.get_configuration(session, sm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordCapabilities(string sm)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.SM.get_capabilities(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFeatures(string sm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SM.get_features(session, sm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordOtherConfig(string sm)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.SM.get_other_config(session, sm);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDriverFilename(string sm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SM.get_driver_filename(session, sm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordRequiredClusterStack(string sm)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.SM.get_required_cluster_stack(session, sm);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenSMProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Type,
        Vendor,
        Copyright,
        Version,
        RequiredApiVersion,
        Configuration,
        Capabilities,
        Features,
        OtherConfig,
        DriverFilename,
        RequiredClusterStack
    }

}
