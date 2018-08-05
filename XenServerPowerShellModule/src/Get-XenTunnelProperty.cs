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
    [Cmdlet(VerbsCommon.Get, "XenTunnelProperty", SupportsShouldProcess = false)]
    public class GetXenTunnelProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Tunnel Tunnel { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Tunnel> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenTunnelProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string tunnel = ParseTunnel();

            switch (XenProperty)
            {
                case XenTunnelProperty.Uuid:
                    ProcessRecordUuid(tunnel);
                    break;
                case XenTunnelProperty.AccessPIF:
                    ProcessRecordAccessPIF(tunnel);
                    break;
                case XenTunnelProperty.TransportPIF:
                    ProcessRecordTransportPIF(tunnel);
                    break;
                case XenTunnelProperty.Status:
                    ProcessRecordStatus(tunnel);
                    break;
                case XenTunnelProperty.OtherConfig:
                    ProcessRecordOtherConfig(tunnel);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseTunnel()
        {
            string tunnel = null;

            if (Tunnel != null)
                tunnel = (new XenRef<XenAPI.Tunnel>(Tunnel)).opaque_ref;
            else if (Ref != null)
                tunnel = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Tunnel', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Tunnel));
            }

            return tunnel;
        }

        private void ProcessRecordUuid(string tunnel)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Tunnel.get_uuid(session, tunnel);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAccessPIF(string tunnel)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Tunnel.get_access_PIF(session, tunnel);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTransportPIF(string tunnel)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Tunnel.get_transport_PIF(session, tunnel);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordStatus(string tunnel)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Tunnel.get_status(session, tunnel);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordOtherConfig(string tunnel)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Tunnel.get_other_config(session, tunnel);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenTunnelProperty
    {
        Uuid,
        AccessPIF,
        TransportPIF,
        Status,
        OtherConfig
    }

}
