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
    [Cmdlet(VerbsLifecycle.Invoke, "XenPIF", SupportsShouldProcess = true)]
    public class InvokeXenPIF : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PIF PIF { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PIF> Ref { get; set; }

        [Parameter(ParameterSetName = "Uuid", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public Guid Uuid { get; set; }


        [Parameter(Mandatory = true)]
        public XenPIFAction XenAction { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenAction)
            {
                case XenPIFAction.CreateVLAN:
                    _context = new XenPIFActionCreateVLANDynamicParameters();
                    return _context;
                case XenPIFAction.ReconfigureIp:
                    _context = new XenPIFActionReconfigureIpDynamicParameters();
                    return _context;
                case XenPIFAction.ReconfigureIpv6:
                    _context = new XenPIFActionReconfigureIpv6DynamicParameters();
                    return _context;
                case XenPIFAction.Scan:
                    _context = new XenPIFActionScanDynamicParameters();
                    return _context;
                case XenPIFAction.Introduce:
                    _context = new XenPIFActionIntroduceDynamicParameters();
                    return _context;
                case XenPIFAction.Forget:
                    _context = new XenPIFActionForgetDynamicParameters();
                    return _context;
                case XenPIFAction.Unplug:
                    _context = new XenPIFActionUnplugDynamicParameters();
                    return _context;
                case XenPIFAction.Plug:
                    _context = new XenPIFActionPlugDynamicParameters();
                    return _context;
                case XenPIFAction.DbIntroduce:
                    _context = new XenPIFActionDbIntroduceDynamicParameters();
                    return _context;
                case XenPIFAction.DbForget:
                    _context = new XenPIFActionDbForgetDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pif = ParsePIF();

            switch (XenAction)
            {
                case XenPIFAction.CreateVLAN:
                    ProcessRecordCreateVLAN(pif);
                    break;
                case XenPIFAction.ReconfigureIp:
                    ProcessRecordReconfigureIp(pif);
                    break;
                case XenPIFAction.ReconfigureIpv6:
                    ProcessRecordReconfigureIpv6(pif);
                    break;
                case XenPIFAction.Scan:
                    ProcessRecordScan(pif);
                    break;
                case XenPIFAction.Introduce:
                    ProcessRecordIntroduce(pif);
                    break;
                case XenPIFAction.Forget:
                    ProcessRecordForget(pif);
                    break;
                case XenPIFAction.Unplug:
                    ProcessRecordUnplug(pif);
                    break;
                case XenPIFAction.Plug:
                    ProcessRecordPlug(pif);
                    break;
                case XenPIFAction.DbIntroduce:
                    ProcessRecordDbIntroduce(pif);
                    break;
                case XenPIFAction.DbForget:
                    ProcessRecordDbForget(pif);
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
            else if (Uuid != Guid.Empty)
            {
                var xenRef = XenAPI.PIF.get_by_uuid(session, Uuid.ToString());
                if (xenRef != null)
                    pif = xenRef.opaque_ref;
            }
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

        private void ProcessRecordCreateVLAN(string pif)
        {
            if (!ShouldProcess(pif, "PIF.create_VLAN"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionCreateVLANDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_create_VLAN(session, contxt.Device, contxt.Network, contxt.XenHost, contxt.VLAN);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    string objRef = XenAPI.PIF.create_VLAN(session, contxt.Device, contxt.Network, contxt.XenHost, contxt.VLAN);

                    if (PassThru)
                    {
                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordReconfigureIp(string pif)
        {
            if (!ShouldProcess(pif, "PIF.reconfigure_ip"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionReconfigureIpDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_reconfigure_ip(session, pif, contxt.Mode, contxt.IP, contxt.Netmask, contxt.Gateway, contxt.DNS);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.reconfigure_ip(session, pif, contxt.Mode, contxt.IP, contxt.Netmask, contxt.Gateway, contxt.DNS);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordReconfigureIpv6(string pif)
        {
            if (!ShouldProcess(pif, "PIF.reconfigure_ipv6"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionReconfigureIpv6DynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_reconfigure_ipv6(session, pif, contxt.Mode, contxt.IPv6, contxt.Gateway, contxt.DNS);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.reconfigure_ipv6(session, pif, contxt.Mode, contxt.IPv6, contxt.Gateway, contxt.DNS);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordScan(string pif)
        {
            if (!ShouldProcess(pif, "PIF.scan"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionScanDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_scan(session, contxt.XenHost);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.scan(session, contxt.XenHost);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordIntroduce(string pif)
        {
            if (!ShouldProcess(pif, "PIF.introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_introduce(session, contxt.XenHost, contxt.MAC, contxt.Device, contxt.Managed);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    string objRef = XenAPI.PIF.introduce(session, contxt.XenHost, contxt.MAC, contxt.Device, contxt.Managed);

                    if (PassThru)
                    {
                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordForget(string pif)
        {
            if (!ShouldProcess(pif, "PIF.forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_forget(session, pif);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.forget(session, pif);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordUnplug(string pif)
        {
            if (!ShouldProcess(pif, "PIF.unplug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionUnplugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_unplug(session, pif);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.unplug(session, pif);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordPlug(string pif)
        {
            if (!ShouldProcess(pif, "PIF.plug"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionPlugDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_plug(session, pif);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.plug(session, pif);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDbIntroduce(string pif)
        {
            if (!ShouldProcess(pif, "PIF.db_introduce"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionDbIntroduceDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_db_introduce(session, contxt.Device, contxt.Network, contxt.XenHost, contxt.MAC, contxt.MTU, contxt.VLAN, contxt.Physical, contxt.IpConfigurationMode, contxt.IP, contxt.Netmask, contxt.Gateway, contxt.DNS, contxt.BondSlaveOf, contxt.VLANMasterOf, contxt.Management, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.DisallowUnplug, contxt.Ipv6ConfigurationMode, contxt.IPv6, contxt.Ipv6Gateway, contxt.PrimaryAddressType, contxt.Managed, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Properties));

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    string objRef = XenAPI.PIF.db_introduce(session, contxt.Device, contxt.Network, contxt.XenHost, contxt.MAC, contxt.MTU, contxt.VLAN, contxt.Physical, contxt.IpConfigurationMode, contxt.IP, contxt.Netmask, contxt.Gateway, contxt.DNS, contxt.BondSlaveOf, contxt.VLANMasterOf, contxt.Management, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.OtherConfig), contxt.DisallowUnplug, contxt.Ipv6ConfigurationMode, contxt.IPv6, contxt.Ipv6Gateway, contxt.PrimaryAddressType, contxt.Managed, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(contxt.Properties));

                    if (PassThru)
                    {
                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });
        }

        private void ProcessRecordDbForget(string pif)
        {
            if (!ShouldProcess(pif, "PIF.db_forget"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenPIFActionDbForgetDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.PIF.async_db_forget(session, pif);

                    if (PassThru)
                    {
                        XenAPI.Task taskObj = null;
                        if (taskRef != "OpaqueRef:NULL")
                        {
                            taskObj = XenAPI.Task.get_record(session, taskRef.opaque_ref);
                            taskObj.opaque_ref = taskRef.opaque_ref;
                        }

                        WriteObject(taskObj, true);
                    }
                }
                else
                {
                    XenAPI.PIF.db_forget(session, pif);

                    if (PassThru)
                    {
                        var obj = XenAPI.PIF.get_record(session, pif);
                        if (obj != null)
                            obj.opaque_ref = pif;
                        WriteObject(obj, true);
                    }
                }

            });
        }

        #endregion
    }

    public enum XenPIFAction
    {
        CreateVLAN,
        ReconfigureIp,
        ReconfigureIpv6,
        Scan,
        Introduce,
        Forget,
        Unplug,
        Plug,
        DbIntroduce,
        DbForget
    }

    public class XenPIFActionCreateVLANDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Device { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public long VLAN { get; set; }
    
    }

    public class XenPIFActionReconfigureIpDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public ip_configuration_mode Mode { get; set; }

        [Parameter]
        public string IP { get; set; }

        [Parameter]
        public string Netmask { get; set; }

        [Parameter]
        public string Gateway { get; set; }

        [Parameter]
        public string DNS { get; set; }
     
    }

    public class XenPIFActionReconfigureIpv6DynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public ipv6_configuration_mode Mode { get; set; }

        [Parameter]
        public string IPv6 { get; set; }

        [Parameter]
        public string Gateway { get; set; }

        [Parameter]
        public string DNS { get; set; }
    
    }

    public class XenPIFActionScanDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }
 
    }

    public class XenPIFActionIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public string MAC { get; set; }

        [Parameter]
        public string Device { get; set; }

        [Parameter]
        public bool Managed { get; set; }
    
    }

    public class XenPIFActionForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPIFActionUnplugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPIFActionPlugDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

    public class XenPIFActionDbIntroduceDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

        [Parameter]
        public string Device { get; set; }

        [Parameter]
        public XenRef<XenAPI.Network> Network { get; set; }

        [Parameter]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter]
        public string MAC { get; set; }

        [Parameter]
        public long MTU { get; set; }

        [Parameter]
        public long VLAN { get; set; }

        [Parameter]
        public bool Physical { get; set; }

        [Parameter]
        public ip_configuration_mode IpConfigurationMode { get; set; }

        [Parameter]
        public string IP { get; set; }

        [Parameter]
        public string Netmask { get; set; }

        [Parameter]
        public string Gateway { get; set; }

        [Parameter]
        public string DNS { get; set; }

        [Parameter]
        public XenRef<XenAPI.Bond> BondSlaveOf { get; set; }

        [Parameter]
        public XenRef<XenAPI.VLAN> VLANMasterOf { get; set; }

        [Parameter]
        public bool Management { get; set; }

        [Parameter]
        public Hashtable OtherConfig { get; set; }

        [Parameter]
        public bool DisallowUnplug { get; set; }

        [Parameter]
        public ipv6_configuration_mode Ipv6ConfigurationMode { get; set; }

        [Parameter]
        public string[] IPv6 { get; set; }

        [Parameter]
        public string Ipv6Gateway { get; set; }

        [Parameter]
        public primary_address_type PrimaryAddressType { get; set; }

        [Parameter]
        public bool Managed { get; set; }

        [Parameter]
        public Hashtable Properties { get; set; }
                       
    }

    public class XenPIFActionDbForgetDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public SwitchParameter Async { get; set; }

    }

}
