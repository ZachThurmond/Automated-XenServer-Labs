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
    [Cmdlet(VerbsCommon.Get, "XenPIFProperty", SupportsShouldProcess = false)]
    public class GetXenPIFProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PIF PIF { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PIF> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPIFProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pif = ParsePIF();

            switch (XenProperty)
            {
                case XenPIFProperty.Uuid:
                    ProcessRecordUuid(pif);
                    break;
                case XenPIFProperty.Device:
                    ProcessRecordDevice(pif);
                    break;
                case XenPIFProperty.Network:
                    ProcessRecordNetwork(pif);
                    break;
                case XenPIFProperty.Host:
                    ProcessRecordHost(pif);
                    break;
                case XenPIFProperty.MAC:
                    ProcessRecordMAC(pif);
                    break;
                case XenPIFProperty.MTU:
                    ProcessRecordMTU(pif);
                    break;
                case XenPIFProperty.VLAN:
                    ProcessRecordVLAN(pif);
                    break;
                case XenPIFProperty.Metrics:
                    ProcessRecordMetrics(pif);
                    break;
                case XenPIFProperty.Physical:
                    ProcessRecordPhysical(pif);
                    break;
                case XenPIFProperty.CurrentlyAttached:
                    ProcessRecordCurrentlyAttached(pif);
                    break;
                case XenPIFProperty.IpConfigurationMode:
                    ProcessRecordIpConfigurationMode(pif);
                    break;
                case XenPIFProperty.IP:
                    ProcessRecordIP(pif);
                    break;
                case XenPIFProperty.Netmask:
                    ProcessRecordNetmask(pif);
                    break;
                case XenPIFProperty.Gateway:
                    ProcessRecordGateway(pif);
                    break;
                case XenPIFProperty.DNS:
                    ProcessRecordDNS(pif);
                    break;
                case XenPIFProperty.BondSlaveOf:
                    ProcessRecordBondSlaveOf(pif);
                    break;
                case XenPIFProperty.BondMasterOf:
                    ProcessRecordBondMasterOf(pif);
                    break;
                case XenPIFProperty.VLANMasterOf:
                    ProcessRecordVLANMasterOf(pif);
                    break;
                case XenPIFProperty.VLANSlaveOf:
                    ProcessRecordVLANSlaveOf(pif);
                    break;
                case XenPIFProperty.Management:
                    ProcessRecordManagement(pif);
                    break;
                case XenPIFProperty.OtherConfig:
                    ProcessRecordOtherConfig(pif);
                    break;
                case XenPIFProperty.DisallowUnplug:
                    ProcessRecordDisallowUnplug(pif);
                    break;
                case XenPIFProperty.TunnelAccessPIFOf:
                    ProcessRecordTunnelAccessPIFOf(pif);
                    break;
                case XenPIFProperty.TunnelTransportPIFOf:
                    ProcessRecordTunnelTransportPIFOf(pif);
                    break;
                case XenPIFProperty.Ipv6ConfigurationMode:
                    ProcessRecordIpv6ConfigurationMode(pif);
                    break;
                case XenPIFProperty.IPv6:
                    ProcessRecordIPv6(pif);
                    break;
                case XenPIFProperty.Ipv6Gateway:
                    ProcessRecordIpv6Gateway(pif);
                    break;
                case XenPIFProperty.PrimaryAddressType:
                    ProcessRecordPrimaryAddressType(pif);
                    break;
                case XenPIFProperty.Managed:
                    ProcessRecordManaged(pif);
                    break;
                case XenPIFProperty.Properties:
                    ProcessRecordProperties(pif);
                    break;
                case XenPIFProperty.Capabilities:
                    ProcessRecordCapabilities(pif);
                    break;
                case XenPIFProperty.IgmpSnoopingStatus:
                    ProcessRecordIgmpSnoopingStatus(pif);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePIF()
        {
            string pif = null;

            if (PIF != null)
                pif = (new XenRef<XenAPI.PIF>(PIF)).opaque_ref;
            else if (Ref != null)
                pif = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PIF', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PIF));
            }

            return pif;
        }

        private void ProcessRecordUuid(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_uuid(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDevice(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_device(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNetwork(string pif)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PIF.get_network(session, pif);

                        XenAPI.Network obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Network.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string pif)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PIF.get_host(session, pif);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMAC(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_MAC(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMTU(string pif)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PIF.get_MTU(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVLAN(string pif)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PIF.get_VLAN(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetrics(string pif)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PIF.get_metrics(session, pif);

                        XenAPI.PIF_metrics obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF_metrics.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPhysical(string pif)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF.get_physical(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentlyAttached(string pif)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF.get_currently_attached(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIpConfigurationMode(string pif)
        {
            RunApiCall(()=>
            {
                    ip_configuration_mode obj = XenAPI.PIF.get_ip_configuration_mode(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIP(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_IP(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNetmask(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_netmask(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGateway(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_gateway(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordDNS(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_DNS(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBondSlaveOf(string pif)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PIF.get_bond_slave_of(session, pif);

                        XenAPI.Bond obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Bond.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBondMasterOf(string pif)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PIF.get_bond_master_of(session, pif);

                        var records = new List<XenAPI.Bond>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Bond.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordVLANMasterOf(string pif)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PIF.get_VLAN_master_of(session, pif);

                        XenAPI.VLAN obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VLAN.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVLANSlaveOf(string pif)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PIF.get_VLAN_slave_of(session, pif);

                        var records = new List<XenAPI.VLAN>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VLAN.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordManagement(string pif)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF.get_management(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string pif)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PIF.get_other_config(session, pif);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDisallowUnplug(string pif)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF.get_disallow_unplug(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordTunnelAccessPIFOf(string pif)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PIF.get_tunnel_access_PIF_of(session, pif);

                        var records = new List<XenAPI.Tunnel>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Tunnel.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordTunnelTransportPIFOf(string pif)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.PIF.get_tunnel_transport_PIF_of(session, pif);

                        var records = new List<XenAPI.Tunnel>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Tunnel.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordIpv6ConfigurationMode(string pif)
        {
            RunApiCall(()=>
            {
                    ipv6_configuration_mode obj = XenAPI.PIF.get_ipv6_configuration_mode(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIPv6(string pif)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.PIF.get_IPv6(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIpv6Gateway(string pif)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PIF.get_ipv6_gateway(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPrimaryAddressType(string pif)
        {
            RunApiCall(()=>
            {
                    primary_address_type obj = XenAPI.PIF.get_primary_address_type(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordManaged(string pif)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.PIF.get_managed(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProperties(string pif)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.PIF.get_properties(session, pif);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordCapabilities(string pif)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.PIF.get_capabilities(session, pif);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordIgmpSnoopingStatus(string pif)
        {
            RunApiCall(()=>
            {
                    pif_igmp_status obj = XenAPI.PIF.get_igmp_snooping_status(session, pif);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPIFProperty
    {
        Uuid,
        Device,
        Network,
        Host,
        MAC,
        MTU,
        VLAN,
        Metrics,
        Physical,
        CurrentlyAttached,
        IpConfigurationMode,
        IP,
        Netmask,
        Gateway,
        DNS,
        BondSlaveOf,
        BondMasterOf,
        VLANMasterOf,
        VLANSlaveOf,
        Management,
        OtherConfig,
        DisallowUnplug,
        TunnelAccessPIFOf,
        TunnelTransportPIFOf,
        Ipv6ConfigurationMode,
        IPv6,
        Ipv6Gateway,
        PrimaryAddressType,
        Managed,
        Properties,
        Capabilities,
        IgmpSnoopingStatus
    }

}
