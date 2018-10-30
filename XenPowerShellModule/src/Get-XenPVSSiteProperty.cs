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
    [Cmdlet(VerbsCommon.Get, "XenPVSSiteProperty", SupportsShouldProcess = false)]
    public class GetXenPVSSiteProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PVS_site PVSSite { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PVS_site> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPVSSiteProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pvs_site = ParsePVSSite();

            switch (XenProperty)
            {
                case XenPVSSiteProperty.Uuid:
                    ProcessRecordUuid(pvs_site);
                    break;
                case XenPVSSiteProperty.NameLabel:
                    ProcessRecordNameLabel(pvs_site);
                    break;
                case XenPVSSiteProperty.NameDescription:
                    ProcessRecordNameDescription(pvs_site);
                    break;
                case XenPVSSiteProperty.PVSUuid:
                    ProcessRecordPVSUuid(pvs_site);
                    break;
                case XenPVSSiteProperty.CacheStorage:
                    ProcessRecordCacheStorage(pvs_site);
                    break;
                case XenPVSSiteProperty.Servers:
                    ProcessRecordServers(pvs_site);
                    break;
                case XenPVSSiteProperty.Proxies:
                    ProcessRecordProxies(pvs_site);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePVSSite()
        {
            string pvs_site = null;

            if (PVSSite != null)
                pvs_site = (new XenRef<XenAPI.PVS_site>(PVSSite)).opaque_ref;
            else if (Ref != null)
                pvs_site = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PVSSite', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PVSSite));
            }

            return pvs_site;
        }

        private void ProcessRecordUuid(string pvs_site)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_site.get_uuid(session, pvs_site);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string pvs_site)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_site.get_name_label(session, pvs_site);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string pvs_site)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_site.get_name_description(session, pvs_site);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPVSUuid(string pvs_site)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_site.get_PVS_uuid(session, pvs_site);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCacheStorage(string pvs_site)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PVS_site.get_cache_storage(session, pvs_site);

                        var records = new List<XenAPI.PVS_cache_storage>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PVS_cache_storage.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordServers(string pvs_site)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PVS_site.get_servers(session, pvs_site);

                        var records = new List<XenAPI.PVS_server>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PVS_server.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordProxies(string pvs_site)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PVS_site.get_proxies(session, pvs_site);

                        var records = new List<XenAPI.PVS_proxy>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PVS_proxy.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        #endregion
    }

    public enum XenPVSSiteProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        PVSUuid,
        CacheStorage,
        Servers,
        Proxies
    }

}
