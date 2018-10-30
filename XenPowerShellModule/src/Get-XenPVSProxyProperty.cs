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
    [Cmdlet(VerbsCommon.Get, "XenPVSProxyProperty", SupportsShouldProcess = false)]
    public class GetXenPVSProxyProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PVS_proxy PVSProxy { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PVS_proxy> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPVSProxyProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pvs_proxy = ParsePVSProxy();

            switch (XenProperty)
            {
                case XenPVSProxyProperty.Uuid:
                    ProcessRecordUuid(pvs_proxy);
                    break;
                case XenPVSProxyProperty.Site:
                    ProcessRecordSite(pvs_proxy);
                    break;
                case XenPVSProxyProperty.VIF:
                    ProcessRecordVIF(pvs_proxy);
                    break;
                case XenPVSProxyProperty.CurrentlyAttached:
                    ProcessRecordCurrentlyAttached(pvs_proxy);
                    break;
                case XenPVSProxyProperty.Status:
                    ProcessRecordStatus(pvs_proxy);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePVSProxy()
        {
            string pvs_proxy = null;

            if (PVSProxy != null)
                pvs_proxy = (new XenRef<XenAPI.PVS_proxy>(PVSProxy)).opaque_ref;
            else if (Ref != null)
                pvs_proxy = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PVSProxy', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PVSProxy));
            }

            return pvs_proxy;
        }

        private void ProcessRecordUuid(string pvs_proxy)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_proxy.get_uuid(session, pvs_proxy);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSite(string pvs_proxy)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_proxy.get_site(session, pvs_proxy);

                        XenAPI.PVS_site obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PVS_site.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVIF(string pvs_proxy)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_proxy.get_VIF(session, pvs_proxy);

                        XenAPI.VIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentlyAttached(string pvs_proxy)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PVS_proxy.get_currently_attached(session, pvs_proxy);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStatus(string pvs_proxy)
        {
            RunApiCall(()=>
            {
                    pvs_proxy_status obj = XenAPI.PVS_proxy.get_status(session, pvs_proxy);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPVSProxyProperty
    {
        Uuid,
        Site,
        VIF,
        CurrentlyAttached,
        Status
    }

}
