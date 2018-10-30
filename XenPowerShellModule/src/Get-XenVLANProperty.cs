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
    [Cmdlet(VerbsCommon.Get, "XenVLANProperty", SupportsShouldProcess = false)]
    public class GetXenVLANProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VLAN VLAN { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VLAN> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVLANProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vlan = ParseVLAN();

            switch (XenProperty)
            {
                case XenVLANProperty.Uuid:
                    ProcessRecordUuid(vlan);
                    break;
                case XenVLANProperty.TaggedPIF:
                    ProcessRecordTaggedPIF(vlan);
                    break;
                case XenVLANProperty.UntaggedPIF:
                    ProcessRecordUntaggedPIF(vlan);
                    break;
                case XenVLANProperty.Tag:
                    ProcessRecordTag(vlan);
                    break;
                case XenVLANProperty.OtherConfig:
                    ProcessRecordOtherConfig(vlan);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVLAN()
        {
            string vlan = null;

            if (VLAN != null)
                vlan = (new XenRef<XenAPI.VLAN>(VLAN)).opaque_ref;
            else if (Ref != null)
                vlan = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VLAN', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VLAN));
            }

            return vlan;
        }

        private void ProcessRecordUuid(string vlan)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VLAN.get_uuid(session, vlan);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTaggedPIF(string vlan)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VLAN.get_tagged_PIF(session, vlan);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUntaggedPIF(string vlan)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VLAN.get_untagged_PIF(session, vlan);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTag(string vlan)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.VLAN.get_tag(session, vlan);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string vlan)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.VLAN.get_other_config(session, vlan);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenVLANProperty
    {
        Uuid,
        TaggedPIF,
        UntaggedPIF,
        Tag,
        OtherConfig
    }

}
