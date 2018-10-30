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
    [Cmdlet(VerbsCommon.Get, "XenHostProperty", SupportsShouldProcess = false)]
    public class GetXenHostProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Host XenHost { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Host> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenHostProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenHostProperty.Log:
                    _context = new XenHostPropertyLogDynamicParameters();
                    return _context;
                case XenHostProperty.VmsWhichPreventEvacuation:
                    _context = new XenHostPropertyVmsWhichPreventEvacuationDynamicParameters();
                    return _context;
                case XenHostProperty.UncooperativeResidentVMs:
                    _context = new XenHostPropertyUncooperativeResidentVMsDynamicParameters();
                    return _context;
                case XenHostProperty.ManagementInterface:
                    _context = new XenHostPropertyManagementInterfaceDynamicParameters();
                    return _context;
                case XenHostProperty.ServerCertificate:
                    _context = new XenHostPropertyServerCertificateDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string host = ParseXenHost();

            switch (XenProperty)
            {
                case XenHostProperty.Uuid:
                    ProcessRecordUuid(host);
                    break;
                case XenHostProperty.NameLabel:
                    ProcessRecordNameLabel(host);
                    break;
                case XenHostProperty.NameDescription:
                    ProcessRecordNameDescription(host);
                    break;
                case XenHostProperty.MemoryOverhead:
                    ProcessRecordMemoryOverhead(host);
                    break;
                case XenHostProperty.AllowedOperations:
                    ProcessRecordAllowedOperations(host);
                    break;
                case XenHostProperty.CurrentOperations:
                    ProcessRecordCurrentOperations(host);
                    break;
                case XenHostProperty.APIVersionMajor:
                    ProcessRecordAPIVersionMajor(host);
                    break;
                case XenHostProperty.APIVersionMinor:
                    ProcessRecordAPIVersionMinor(host);
                    break;
                case XenHostProperty.APIVersionVendor:
                    ProcessRecordAPIVersionVendor(host);
                    break;
                case XenHostProperty.APIVersionVendorImplementation:
                    ProcessRecordAPIVersionVendorImplementation(host);
                    break;
                case XenHostProperty.Enabled:
                    ProcessRecordEnabled(host);
                    break;
                case XenHostProperty.SoftwareVersion:
                    ProcessRecordSoftwareVersion(host);
                    break;
                case XenHostProperty.OtherConfig:
                    ProcessRecordOtherConfig(host);
                    break;
                case XenHostProperty.Capabilities:
                    ProcessRecordCapabilities(host);
                    break;
                case XenHostProperty.CpuConfiguration:
                    ProcessRecordCpuConfiguration(host);
                    break;
                case XenHostProperty.SchedPolicy:
                    ProcessRecordSchedPolicy(host);
                    break;
                case XenHostProperty.SupportedBootloaders:
                    ProcessRecordSupportedBootloaders(host);
                    break;
                case XenHostProperty.ResidentVMs:
                    ProcessRecordResidentVMs(host);
                    break;
                case XenHostProperty.Logging:
                    ProcessRecordLogging(host);
                    break;
                case XenHostProperty.PIFs:
                    ProcessRecordPIFs(host);
                    break;
                case XenHostProperty.SuspendImageSr:
                    ProcessRecordSuspendImageSr(host);
                    break;
                case XenHostProperty.CrashDumpSr:
                    ProcessRecordCrashDumpSr(host);
                    break;
                case XenHostProperty.Crashdumps:
                    ProcessRecordCrashdumps(host);
                    break;
                case XenHostProperty.Patches:
                    ProcessRecordPatches(host);
                    break;
                case XenHostProperty.Updates:
                    ProcessRecordUpdates(host);
                    break;
                case XenHostProperty.PBDs:
                    ProcessRecordPBDs(host);
                    break;
                case XenHostProperty.HostCPUs:
                    ProcessRecordHostCPUs(host);
                    break;
                case XenHostProperty.CpuInfo:
                    ProcessRecordCpuInfo(host);
                    break;
                case XenHostProperty.Hostname:
                    ProcessRecordHostname(host);
                    break;
                case XenHostProperty.Address:
                    ProcessRecordAddress(host);
                    break;
                case XenHostProperty.Metrics:
                    ProcessRecordMetrics(host);
                    break;
                case XenHostProperty.LicenseParams:
                    ProcessRecordLicenseParams(host);
                    break;
                case XenHostProperty.HaStatefiles:
                    ProcessRecordHaStatefiles(host);
                    break;
                case XenHostProperty.HaNetworkPeers:
                    ProcessRecordHaNetworkPeers(host);
                    break;
                case XenHostProperty.Blobs:
                    ProcessRecordBlobs(host);
                    break;
                case XenHostProperty.Tags:
                    ProcessRecordTags(host);
                    break;
                case XenHostProperty.ExternalAuthType:
                    ProcessRecordExternalAuthType(host);
                    break;
                case XenHostProperty.ExternalAuthServiceName:
                    ProcessRecordExternalAuthServiceName(host);
                    break;
                case XenHostProperty.ExternalAuthConfiguration:
                    ProcessRecordExternalAuthConfiguration(host);
                    break;
                case XenHostProperty.Edition:
                    ProcessRecordEdition(host);
                    break;
                case XenHostProperty.LicenseServer:
                    ProcessRecordLicenseServer(host);
                    break;
                case XenHostProperty.BiosStrings:
                    ProcessRecordBiosStrings(host);
                    break;
                case XenHostProperty.PowerOnMode:
                    ProcessRecordPowerOnMode(host);
                    break;
                case XenHostProperty.PowerOnConfig:
                    ProcessRecordPowerOnConfig(host);
                    break;
                case XenHostProperty.LocalCacheSr:
                    ProcessRecordLocalCacheSr(host);
                    break;
                case XenHostProperty.ChipsetInfo:
                    ProcessRecordChipsetInfo(host);
                    break;
                case XenHostProperty.PCIs:
                    ProcessRecordPCIs(host);
                    break;
                case XenHostProperty.PGPUs:
                    ProcessRecordPGPUs(host);
                    break;
                case XenHostProperty.PUSBs:
                    ProcessRecordPUSBs(host);
                    break;
                case XenHostProperty.SslLegacy:
                    ProcessRecordSslLegacy(host);
                    break;
                case XenHostProperty.GuestVCPUsParams:
                    ProcessRecordGuestVCPUsParams(host);
                    break;
                case XenHostProperty.Display:
                    ProcessRecordDisplay(host);
                    break;
                case XenHostProperty.VirtualHardwarePlatformVersions:
                    ProcessRecordVirtualHardwarePlatformVersions(host);
                    break;
                case XenHostProperty.ControlDomain:
                    ProcessRecordControlDomain(host);
                    break;
                case XenHostProperty.UpdatesRequiringReboot:
                    ProcessRecordUpdatesRequiringReboot(host);
                    break;
                case XenHostProperty.Features:
                    ProcessRecordFeatures(host);
                    break;
                case XenHostProperty.Log:
                    ProcessRecordLog(host);
                    break;
                case XenHostProperty.DataSources:
                    ProcessRecordDataSources(host);
                    break;
                case XenHostProperty.VmsWhichPreventEvacuation:
                    ProcessRecordVmsWhichPreventEvacuation(host);
                    break;
                case XenHostProperty.UncooperativeResidentVMs:
                    ProcessRecordUncooperativeResidentVMs(host);
                    break;
                case XenHostProperty.ManagementInterface:
                    ProcessRecordManagementInterface(host);
                    break;
                case XenHostProperty.SystemStatusCapabilities:
                    ProcessRecordSystemStatusCapabilities(host);
                    break;
                case XenHostProperty.Servertime:
                    ProcessRecordServertime(host);
                    break;
                case XenHostProperty.ServerLocaltime:
                    ProcessRecordServerLocaltime(host);
                    break;
                case XenHostProperty.ServerCertificate:
                    ProcessRecordServerCertificate(host);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseXenHost()
        {
            string host = null;

            if (XenHost != null)
                host = (new XenRef<XenAPI.Host>(XenHost)).opaque_ref;
            else if (Ref != null)
                host = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'XenHost', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    XenHost));
            }

            return host;
        }

        private void ProcessRecordUuid(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_uuid(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_name_label(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_name_description(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMemoryOverhead(string host)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host.get_memory_overhead(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAllowedOperations(string host)
        {
            RunApiCall(()=>
            {
                    List<host_allowed_operations> obj = XenAPI.Host.get_allowed_operations(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCurrentOperations(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_current_operations(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordAPIVersionMajor(string host)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host.get_API_version_major(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAPIVersionMinor(string host)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Host.get_API_version_minor(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAPIVersionVendor(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_API_version_vendor(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAPIVersionVendorImplementation(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_API_version_vendor_implementation(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordEnabled(string host)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Host.get_enabled(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSoftwareVersion(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_software_version(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordOtherConfig(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_other_config(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordCapabilities(string host)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.get_capabilities(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCpuConfiguration(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_cpu_configuration(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordSchedPolicy(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_sched_policy(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSupportedBootloaders(string host)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.get_supported_bootloaders(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordResidentVMs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_resident_VMs(session, host);

                        var records = new List<XenAPI.VM>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VM.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordLogging(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_logging(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPIFs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_PIFs(session, host);

                        var records = new List<XenAPI.PIF>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PIF.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSuspendImageSr(string host)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host.get_suspend_image_sr(session, host);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCrashDumpSr(string host)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host.get_crash_dump_sr(session, host);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordCrashdumps(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_crashdumps(session, host);

                        var records = new List<XenAPI.Host_crashdump>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host_crashdump.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPatches(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_patches(session, host);

                        var records = new List<XenAPI.Host_patch>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host_patch.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordUpdates(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_updates(session, host);

                        var records = new List<XenAPI.Pool_update>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Pool_update.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPBDs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_PBDs(session, host);

                        var records = new List<XenAPI.PBD>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PBD.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordHostCPUs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_host_CPUs(session, host);

                        var records = new List<XenAPI.Host_cpu>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Host_cpu.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordCpuInfo(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_cpu_info(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordHostname(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_hostname(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAddress(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_address(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMetrics(string host)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host.get_metrics(session, host);

                        XenAPI.Host_metrics obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host_metrics.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLicenseParams(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_license_params(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordHaStatefiles(string host)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.get_ha_statefiles(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHaNetworkPeers(string host)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.get_ha_network_peers(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBlobs(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_blobs(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordTags(string host)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Host.get_tags(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordExternalAuthType(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_external_auth_type(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordExternalAuthServiceName(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_external_auth_service_name(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordExternalAuthConfiguration(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_external_auth_configuration(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordEdition(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_edition(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordLicenseServer(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_license_server(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordBiosStrings(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_bios_strings(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPowerOnMode(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_power_on_mode(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPowerOnConfig(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_power_on_config(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLocalCacheSr(string host)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host.get_local_cache_sr(session, host);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordChipsetInfo(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_chipset_info(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPCIs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_PCIs(session, host);

                        var records = new List<XenAPI.PCI>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PCI.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPGPUs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_PGPUs(session, host);

                        var records = new List<XenAPI.PGPU>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PGPU.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPUSBs(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_PUSBs(session, host);

                        var records = new List<XenAPI.PUSB>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.PUSB.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordSslLegacy(string host)
        {
            RunApiCall(()=>
            {
                    bool obj = XenAPI.Host.get_ssl_legacy(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordGuestVCPUsParams(string host)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Host.get_guest_VCPUs_params(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordDisplay(string host)
        {
            RunApiCall(()=>
            {
                    host_display obj = XenAPI.Host.get_display(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVirtualHardwarePlatformVersions(string host)
        {
            RunApiCall(()=>
            {
                    long[] obj = XenAPI.Host.get_virtual_hardware_platform_versions(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordControlDomain(string host)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Host.get_control_domain(session, host);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordUpdatesRequiringReboot(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_updates_requiring_reboot(session, host);

                        var records = new List<XenAPI.Pool_update>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Pool_update.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordFeatures(string host)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Host.get_features(session, host);

                        var records = new List<XenAPI.Feature>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Feature.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordLog(string host)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenHostPropertyLogDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_get_log(session, host);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    string obj = XenAPI.Host.get_log(session, host);

                        WriteObject(obj, true);
                }

            });
        }

        private void ProcessRecordDataSources(string host)
        {
            RunApiCall(()=>
            {
                    List<XenAPI.Data_source> obj = XenAPI.Host.get_data_sources(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVmsWhichPreventEvacuation(string host)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenHostPropertyVmsWhichPreventEvacuationDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_get_vms_which_prevent_evacuation(session, host);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    var dict = XenAPI.Host.get_vms_which_prevent_evacuation(session, host);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
                }

            });
        }

        private void ProcessRecordUncooperativeResidentVMs(string host)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenHostPropertyUncooperativeResidentVMsDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_get_uncooperative_resident_VMs(session, host);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    var refs = XenAPI.Host.get_uncooperative_resident_VMs(session, host);

                        var records = new List<XenAPI.VM>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.VM.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
                }

            });
        }

        private void ProcessRecordManagementInterface(string host)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenHostPropertyManagementInterfaceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_get_management_interface(session, host);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    string objRef = XenAPI.Host.get_management_interface(session, host);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                }

            });
        }

        private void ProcessRecordSystemStatusCapabilities(string host)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Host.get_system_status_capabilities(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordServertime(string host)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Host.get_servertime(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordServerLocaltime(string host)
        {
            RunApiCall(()=>
            {
                    DateTime obj = XenAPI.Host.get_server_localtime(session, host);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordServerCertificate(string host)
        {
            RunApiCall(()=>
            {
                var contxt = _context as XenHostPropertyServerCertificateDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.Host.async_get_server_certificate(session, host);

                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                }
                else
                {
                    string obj = XenAPI.Host.get_server_certificate(session, host);

                        WriteObject(obj, true);
                }

            });
        }

        #endregion
    }

    public enum XenHostProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        MemoryOverhead,
        AllowedOperations,
        CurrentOperations,
        APIVersionMajor,
        APIVersionMinor,
        APIVersionVendor,
        APIVersionVendorImplementation,
        Enabled,
        SoftwareVersion,
        OtherConfig,
        Capabilities,
        CpuConfiguration,
        SchedPolicy,
        SupportedBootloaders,
        ResidentVMs,
        Logging,
        PIFs,
        SuspendImageSr,
        CrashDumpSr,
        Crashdumps,
        Patches,
        Updates,
        PBDs,
        HostCPUs,
        CpuInfo,
        Hostname,
        Address,
        Metrics,
        LicenseParams,
        HaStatefiles,
        HaNetworkPeers,
        Blobs,
        Tags,
        ExternalAuthType,
        ExternalAuthServiceName,
        ExternalAuthConfiguration,
        Edition,
        LicenseServer,
        BiosStrings,
        PowerOnMode,
        PowerOnConfig,
        LocalCacheSr,
        ChipsetInfo,
        PCIs,
        PGPUs,
        PUSBs,
        SslLegacy,
        GuestVCPUsParams,
        Display,
        VirtualHardwarePlatformVersions,
        ControlDomain,
        UpdatesRequiringReboot,
        Features,
        Log,
        DataSources,
        VmsWhichPreventEvacuation,
        UncooperativeResidentVMs,
        ManagementInterface,
        SystemStatusCapabilities,
        Servertime,
        ServerLocaltime,
        ServerCertificate
    }

    public class XenHostPropertyLogDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostPropertyVmsWhichPreventEvacuationDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostPropertyUncooperativeResidentVMsDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostPropertyManagementInterfaceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenHostPropertyServerCertificateDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
