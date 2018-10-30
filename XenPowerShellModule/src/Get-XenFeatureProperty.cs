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
    [Cmdlet(VerbsCommon.Get, "XenFeatureProperty", SupportsShouldProcess = false)]
    public class GetXenFeatureProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Feature Feature { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Feature> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenFeatureProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string feature = ParseFeature();

            switch (XenProperty)
            {
                case XenFeatureProperty.Uuid:
                    ProcessRecordUuid(feature);
                    break;
                case XenFeatureProperty.NameLabel:
                    ProcessRecordNameLabel(feature);
                    break;
                case XenFeatureProperty.NameDescription:
                    ProcessRecordNameDescription(feature);
                    break;
                case XenFeatureProperty.Enabled:
                    ProcessRecordEnabled(feature);
                    break;
                case XenFeatureProperty.Experimental:
                    ProcessRecordExperimental(feature);
                    break;
                case XenFeatureProperty.Version:
                    ProcessRecordVersion(feature);
                    break;
                case XenFeatureProperty.Host:
                    ProcessRecordHost(feature);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseFeature()
        {
            string feature = null;

            if (Feature != null)
                feature = (new XenRef<XenAPI.Feature>(Feature)).opaque_ref;
            else if (Ref != null)
                feature = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Feature', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Feature));
            }

            return feature;
        }

        private void ProcessRecordUuid(string feature)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Feature.get_uuid(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string feature)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Feature.get_name_label(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string feature)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Feature.get_name_description(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordEnabled(string feature)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Feature.get_enabled(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordExperimental(string feature)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Feature.get_experimental(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string feature)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Feature.get_version(session, feature);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string feature)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Feature.get_host(session, feature);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenFeatureProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Enabled,
        Experimental,
        Version,
        Host
    }

}
