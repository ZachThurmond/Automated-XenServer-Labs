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
    [Cmdlet(VerbsCommon.Get, "XenPVSServerProperty", SupportsShouldProcess = false)]
    public class GetXenPVSServerProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PVS_server PVSServer { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PVS_server> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPVSServerProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pvs_server = ParsePVSServer();

            switch (XenProperty)
            {
                case XenPVSServerProperty.Uuid:
                    ProcessRecordUuid(pvs_server);
                    break;
                case XenPVSServerProperty.Addresses:
                    ProcessRecordAddresses(pvs_server);
                    break;
                case XenPVSServerProperty.FirstPort:
                    ProcessRecordFirstPort(pvs_server);
                    break;
                case XenPVSServerProperty.LastPort:
                    ProcessRecordLastPort(pvs_server);
                    break;
                case XenPVSServerProperty.Site:
                    ProcessRecordSite(pvs_server);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePVSServer()
        {
            string pvs_server = null;

            if (PVSServer != null)
                pvs_server = (new XenRef<XenAPI.PVS_server>(PVSServer)).opaque_ref;
            else if (Ref != null)
                pvs_server = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PVSServer', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PVSServer));
            }

            return pvs_server;
        }

        private void ProcessRecordUuid(string pvs_server)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_server.get_uuid(session, pvs_server);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAddresses(string pvs_server)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.PVS_server.get_addresses(session, pvs_server);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFirstPort(string pvs_server)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PVS_server.get_first_port(session, pvs_server);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLastPort(string pvs_server)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PVS_server.get_last_port(session, pvs_server);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSite(string pvs_server)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_server.get_site(session, pvs_server);

                        XenAPI.PVS_site obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PVS_site.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPVSServerProperty
    {
        Uuid,
        Addresses,
        FirstPort,
        LastPort,
        Site
    }

}
