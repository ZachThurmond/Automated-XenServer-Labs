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
    [Cmdlet(VerbsData.ConvertTo, "XenRef")]
    [OutputType(typeof(IXenObject))]
    public class ConvertToXenRefCommand : PSCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public IXenObject XenObject { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            XenAPI.Session session = XenObject as XenAPI.Session;
            if (session != null)
            {
                WriteObject(new XenRef<XenAPI.Session>(session));
                return;
            }
            XenAPI.Auth auth = XenObject as XenAPI.Auth;
            if (auth != null)
            {
                WriteObject(new XenRef<XenAPI.Auth>(auth));
                return;
            }
            XenAPI.Subject subject = XenObject as XenAPI.Subject;
            if (subject != null)
            {
                WriteObject(new XenRef<XenAPI.Subject>(subject));
                return;
            }
            XenAPI.Role role = XenObject as XenAPI.Role;
            if (role != null)
            {
                WriteObject(new XenRef<XenAPI.Role>(role));
                return;
            }
            XenAPI.Task task = XenObject as XenAPI.Task;
            if (task != null)
            {
                WriteObject(new XenRef<XenAPI.Task>(task));
                return;
            }
            XenAPI.Event evt = XenObject as XenAPI.Event;
            if (evt != null)
            {
                WriteObject(new XenRef<XenAPI.Event>(evt));
                return;
            }
            XenAPI.Pool pool = XenObject as XenAPI.Pool;
            if (pool != null)
            {
                WriteObject(new XenRef<XenAPI.Pool>(pool));
                return;
            }
            XenAPI.Pool_patch pool_patch = XenObject as XenAPI.Pool_patch;
            if (pool_patch != null)
            {
                WriteObject(new XenRef<XenAPI.Pool_patch>(pool_patch));
                return;
            }
            XenAPI.Pool_update pool_update = XenObject as XenAPI.Pool_update;
            if (pool_update != null)
            {
                WriteObject(new XenRef<XenAPI.Pool_update>(pool_update));
                return;
            }
            XenAPI.VM vm = XenObject as XenAPI.VM;
            if (vm != null)
            {
                WriteObject(new XenRef<XenAPI.VM>(vm));
                return;
            }
            XenAPI.VM_metrics vm_metrics = XenObject as XenAPI.VM_metrics;
            if (vm_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.VM_metrics>(vm_metrics));
                return;
            }
            XenAPI.VM_guest_metrics vm_guest_metrics = XenObject as XenAPI.VM_guest_metrics;
            if (vm_guest_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.VM_guest_metrics>(vm_guest_metrics));
                return;
            }
            XenAPI.VMPP vmpp = XenObject as XenAPI.VMPP;
            if (vmpp != null)
            {
                WriteObject(new XenRef<XenAPI.VMPP>(vmpp));
                return;
            }
            XenAPI.VMSS vmss = XenObject as XenAPI.VMSS;
            if (vmss != null)
            {
                WriteObject(new XenRef<XenAPI.VMSS>(vmss));
                return;
            }
            XenAPI.VM_appliance vm_appliance = XenObject as XenAPI.VM_appliance;
            if (vm_appliance != null)
            {
                WriteObject(new XenRef<XenAPI.VM_appliance>(vm_appliance));
                return;
            }
            XenAPI.DR_task dr_task = XenObject as XenAPI.DR_task;
            if (dr_task != null)
            {
                WriteObject(new XenRef<XenAPI.DR_task>(dr_task));
                return;
            }
            XenAPI.Host host = XenObject as XenAPI.Host;
            if (host != null)
            {
                WriteObject(new XenRef<XenAPI.Host>(host));
                return;
            }
            XenAPI.Host_crashdump host_crashdump = XenObject as XenAPI.Host_crashdump;
            if (host_crashdump != null)
            {
                WriteObject(new XenRef<XenAPI.Host_crashdump>(host_crashdump));
                return;
            }
            XenAPI.Host_patch host_patch = XenObject as XenAPI.Host_patch;
            if (host_patch != null)
            {
                WriteObject(new XenRef<XenAPI.Host_patch>(host_patch));
                return;
            }
            XenAPI.Host_metrics host_metrics = XenObject as XenAPI.Host_metrics;
            if (host_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.Host_metrics>(host_metrics));
                return;
            }
            XenAPI.Host_cpu host_cpu = XenObject as XenAPI.Host_cpu;
            if (host_cpu != null)
            {
                WriteObject(new XenRef<XenAPI.Host_cpu>(host_cpu));
                return;
            }
            XenAPI.Network network = XenObject as XenAPI.Network;
            if (network != null)
            {
                WriteObject(new XenRef<XenAPI.Network>(network));
                return;
            }
            XenAPI.VIF vif = XenObject as XenAPI.VIF;
            if (vif != null)
            {
                WriteObject(new XenRef<XenAPI.VIF>(vif));
                return;
            }
            XenAPI.VIF_metrics vif_metrics = XenObject as XenAPI.VIF_metrics;
            if (vif_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.VIF_metrics>(vif_metrics));
                return;
            }
            XenAPI.PIF pif = XenObject as XenAPI.PIF;
            if (pif != null)
            {
                WriteObject(new XenRef<XenAPI.PIF>(pif));
                return;
            }
            XenAPI.PIF_metrics pif_metrics = XenObject as XenAPI.PIF_metrics;
            if (pif_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.PIF_metrics>(pif_metrics));
                return;
            }
            XenAPI.Bond bond = XenObject as XenAPI.Bond;
            if (bond != null)
            {
                WriteObject(new XenRef<XenAPI.Bond>(bond));
                return;
            }
            XenAPI.VLAN vlan = XenObject as XenAPI.VLAN;
            if (vlan != null)
            {
                WriteObject(new XenRef<XenAPI.VLAN>(vlan));
                return;
            }
            XenAPI.SM sm = XenObject as XenAPI.SM;
            if (sm != null)
            {
                WriteObject(new XenRef<XenAPI.SM>(sm));
                return;
            }
            XenAPI.SR sr = XenObject as XenAPI.SR;
            if (sr != null)
            {
                WriteObject(new XenRef<XenAPI.SR>(sr));
                return;
            }
            XenAPI.LVHD lvhd = XenObject as XenAPI.LVHD;
            if (lvhd != null)
            {
                WriteObject(new XenRef<XenAPI.LVHD>(lvhd));
                return;
            }
            XenAPI.VDI vdi = XenObject as XenAPI.VDI;
            if (vdi != null)
            {
                WriteObject(new XenRef<XenAPI.VDI>(vdi));
                return;
            }
            XenAPI.VBD vbd = XenObject as XenAPI.VBD;
            if (vbd != null)
            {
                WriteObject(new XenRef<XenAPI.VBD>(vbd));
                return;
            }
            XenAPI.VBD_metrics vbd_metrics = XenObject as XenAPI.VBD_metrics;
            if (vbd_metrics != null)
            {
                WriteObject(new XenRef<XenAPI.VBD_metrics>(vbd_metrics));
                return;
            }
            XenAPI.PBD pbd = XenObject as XenAPI.PBD;
            if (pbd != null)
            {
                WriteObject(new XenRef<XenAPI.PBD>(pbd));
                return;
            }
            XenAPI.Crashdump crashdump = XenObject as XenAPI.Crashdump;
            if (crashdump != null)
            {
                WriteObject(new XenRef<XenAPI.Crashdump>(crashdump));
                return;
            }
            XenAPI.VTPM vtpm = XenObject as XenAPI.VTPM;
            if (vtpm != null)
            {
                WriteObject(new XenRef<XenAPI.VTPM>(vtpm));
                return;
            }
            XenAPI.Console console = XenObject as XenAPI.Console;
            if (console != null)
            {
                WriteObject(new XenRef<XenAPI.Console>(console));
                return;
            }
            XenAPI.User user = XenObject as XenAPI.User;
            if (user != null)
            {
                WriteObject(new XenRef<XenAPI.User>(user));
                return;
            }
            XenAPI.Data_source data_source = XenObject as XenAPI.Data_source;
            if (data_source != null)
            {
                WriteObject(new XenRef<XenAPI.Data_source>(data_source));
                return;
            }
            XenAPI.Blob blob = XenObject as XenAPI.Blob;
            if (blob != null)
            {
                WriteObject(new XenRef<XenAPI.Blob>(blob));
                return;
            }
            XenAPI.Message message = XenObject as XenAPI.Message;
            if (message != null)
            {
                WriteObject(new XenRef<XenAPI.Message>(message));
                return;
            }
            XenAPI.Secret secret = XenObject as XenAPI.Secret;
            if (secret != null)
            {
                WriteObject(new XenRef<XenAPI.Secret>(secret));
                return;
            }
            XenAPI.Tunnel tunnel = XenObject as XenAPI.Tunnel;
            if (tunnel != null)
            {
                WriteObject(new XenRef<XenAPI.Tunnel>(tunnel));
                return;
            }
            XenAPI.PCI pci = XenObject as XenAPI.PCI;
            if (pci != null)
            {
                WriteObject(new XenRef<XenAPI.PCI>(pci));
                return;
            }
            XenAPI.PGPU pgpu = XenObject as XenAPI.PGPU;
            if (pgpu != null)
            {
                WriteObject(new XenRef<XenAPI.PGPU>(pgpu));
                return;
            }
            XenAPI.GPU_group gpu_group = XenObject as XenAPI.GPU_group;
            if (gpu_group != null)
            {
                WriteObject(new XenRef<XenAPI.GPU_group>(gpu_group));
                return;
            }
            XenAPI.VGPU vgpu = XenObject as XenAPI.VGPU;
            if (vgpu != null)
            {
                WriteObject(new XenRef<XenAPI.VGPU>(vgpu));
                return;
            }
            XenAPI.VGPU_type vgpu_type = XenObject as XenAPI.VGPU_type;
            if (vgpu_type != null)
            {
                WriteObject(new XenRef<XenAPI.VGPU_type>(vgpu_type));
                return;
            }
            XenAPI.PVS_site pvs_site = XenObject as XenAPI.PVS_site;
            if (pvs_site != null)
            {
                WriteObject(new XenRef<XenAPI.PVS_site>(pvs_site));
                return;
            }
            XenAPI.PVS_server pvs_server = XenObject as XenAPI.PVS_server;
            if (pvs_server != null)
            {
                WriteObject(new XenRef<XenAPI.PVS_server>(pvs_server));
                return;
            }
            XenAPI.PVS_proxy pvs_proxy = XenObject as XenAPI.PVS_proxy;
            if (pvs_proxy != null)
            {
                WriteObject(new XenRef<XenAPI.PVS_proxy>(pvs_proxy));
                return;
            }
            XenAPI.PVS_cache_storage pvs_cache_storage = XenObject as XenAPI.PVS_cache_storage;
            if (pvs_cache_storage != null)
            {
                WriteObject(new XenRef<XenAPI.PVS_cache_storage>(pvs_cache_storage));
                return;
            }
            XenAPI.Feature feature = XenObject as XenAPI.Feature;
            if (feature != null)
            {
                WriteObject(new XenRef<XenAPI.Feature>(feature));
                return;
            }
            XenAPI.SDN_controller sdn_controller = XenObject as XenAPI.SDN_controller;
            if (sdn_controller != null)
            {
                WriteObject(new XenRef<XenAPI.SDN_controller>(sdn_controller));
                return;
            }
            XenAPI.Vdi_nbd_server_info vdi_nbd_server_info = XenObject as XenAPI.Vdi_nbd_server_info;
            if (vdi_nbd_server_info != null)
            {
                WriteObject(new XenRef<XenAPI.Vdi_nbd_server_info>(vdi_nbd_server_info));
                return;
            }
            XenAPI.PUSB pusb = XenObject as XenAPI.PUSB;
            if (pusb != null)
            {
                WriteObject(new XenRef<XenAPI.PUSB>(pusb));
                return;
            }
            XenAPI.USB_group usb_group = XenObject as XenAPI.USB_group;
            if (usb_group != null)
            {
                WriteObject(new XenRef<XenAPI.USB_group>(usb_group));
                return;
            }
            XenAPI.VUSB vusb = XenObject as XenAPI.VUSB;
            if (vusb != null)
            {
                WriteObject(new XenRef<XenAPI.VUSB>(vusb));
                return;
            }
        }

        #endregion

    }
}
