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
    [Cmdlet(VerbsCommon.Get, "XenPVSCacheStorageProperty", SupportsShouldProcess = false)]
    public class GetXenPVSCacheStorageProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.PVS_cache_storage PVSCacheStorage { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.PVS_cache_storage> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenPVSCacheStorageProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string pvs_cache_storage = ParsePVSCacheStorage();

            switch (XenProperty)
            {
                case XenPVSCacheStorageProperty.Uuid:
                    ProcessRecordUuid(pvs_cache_storage);
                    break;
                case XenPVSCacheStorageProperty.Host:
                    ProcessRecordHost(pvs_cache_storage);
                    break;
                case XenPVSCacheStorageProperty.SR:
                    ProcessRecordSR(pvs_cache_storage);
                    break;
                case XenPVSCacheStorageProperty.Site:
                    ProcessRecordSite(pvs_cache_storage);
                    break;
                case XenPVSCacheStorageProperty.Size:
                    ProcessRecordSize(pvs_cache_storage);
                    break;
                case XenPVSCacheStorageProperty.VDI:
                    ProcessRecordVDI(pvs_cache_storage);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParsePVSCacheStorage()
        {
            string pvs_cache_storage = null;

            if (PVSCacheStorage != null)
                pvs_cache_storage = (new XenRef<XenAPI.PVS_cache_storage>(PVSCacheStorage)).opaque_ref;
            else if (Ref != null)
                pvs_cache_storage = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'PVSCacheStorage', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    PVSCacheStorage));
            }

            return pvs_cache_storage;
        }

        private void ProcessRecordUuid(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.PVS_cache_storage.get_uuid(session, pvs_cache_storage);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordHost(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_cache_storage.get_host(session, pvs_cache_storage);

                        XenAPI.Host obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Host.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSR(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_cache_storage.get_SR(session, pvs_cache_storage);

                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSite(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_cache_storage.get_site(session, pvs_cache_storage);

                        XenAPI.PVS_site obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PVS_site.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSize(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.PVS_cache_storage.get_size(session, pvs_cache_storage);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVDI(string pvs_cache_storage)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.PVS_cache_storage.get_VDI(session, pvs_cache_storage);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenPVSCacheStorageProperty
    {
        Uuid,
        Host,
        SR,
        Site,
        Size,
        VDI
    }

}
