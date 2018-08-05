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
    [Cmdlet(VerbsCommon.Get, "XenPoolUpdateProperty", SupportsShouldProcess = false)]
    public class GetXenPoolUpdateProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Pool_update PoolUpdate { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Pool_update> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPoolUpdateProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pool_update = ParsePoolUpdate();

            switch (XenProperty)
            {
                case XenPoolUpdateProperty.Uuid:
                    ProcessRecordUuid(pool_update);
                    break;
                case XenPoolUpdateProperty.NameLabel:
                    ProcessRecordNameLabel(pool_update);
                    break;
                case XenPoolUpdateProperty.NameDescription:
                    ProcessRecordNameDescription(pool_update);
                    break;
                case XenPoolUpdateProperty.Version:
                    ProcessRecordVersion(pool_update);
                    break;
                case XenPoolUpdateProperty.InstallationSize:
                    ProcessRecordInstallationSize(pool_update);
                    break;
                case XenPoolUpdateProperty.Key:
                    ProcessRecordKey(pool_update);
                    break;
                case XenPoolUpdateProperty.AfterApplyGuidance:
                    ProcessRecordAfterApplyGuidance(pool_update);
                    break;
                case XenPoolUpdateProperty.Vdi:
                    ProcessRecordVdi(pool_update);
                    break;
                case XenPoolUpdateProperty.Hosts:
                    ProcessRecordHosts(pool_update);
                    break;
                case XenPoolUpdateProperty.OtherConfig:
                    ProcessRecordOtherConfig(pool_update);
                    break;
                case XenPoolUpdateProperty.EnforceHomogeneity:
                    ProcessRecordEnforceHomogeneity(pool_update);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePoolUpdate()
        {
            string pool_update = null;

            if (PoolUpdate != null)
                pool_update = (new XenRef<XenAPI.Pool_update>(PoolUpdate)).opaque_ref;
            else if (Ref != null)
                pool_update = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PoolUpdate', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PoolUpdate));
            }

            return pool_update;
        }

        private void ProcessRecordUuid(string pool_update)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_update.get_uuid(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string pool_update)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_update.get_name_label(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string pool_update)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_update.get_name_description(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVersion(string pool_update)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_update.get_version(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordInstallationSize(string pool_update)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Pool_update.get_installation_size(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordKey(string pool_update)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Pool_update.get_key(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAfterApplyGuidance(string pool_update)
        {
            RunApiCall(()=>
            {
                    List<update_after_apply_guidance> obj = XenAPI.Pool_update.get_after_apply_guidance(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVdi(string pool_update)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Pool_update.get_vdi(session, pool_update);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHosts(string pool_update)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Pool_update.get_hosts(session, pool_update);

                        var records = new List<XenAPI.Host>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordOtherConfig(string pool_update)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Pool_update.get_other_config(session, pool_update);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordEnforceHomogeneity(string pool_update)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Pool_update.get_enforce_homogeneity(session, pool_update);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPoolUpdateProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Version,
        InstallationSize,
        Key,
        AfterApplyGuidance,
        Vdi,
        Hosts,
        OtherConfig,
        EnforceHomogeneity
    }

}
