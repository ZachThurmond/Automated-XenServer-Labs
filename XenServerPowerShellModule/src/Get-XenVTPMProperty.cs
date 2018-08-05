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
    [Cmdlet(VerbsCommon.Get, "XenVTPMProperty", SupportsShouldProcess = false)]
    public class GetXenVTPMProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.VTPM VTPM { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.VTPM> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenVTPMProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string vtpm = ParseVTPM();

            switch (XenProperty)
            {
                case XenVTPMProperty.Uuid:
                    ProcessRecordUuid(vtpm);
                    break;
                case XenVTPMProperty.VM:
                    ProcessRecordVM(vtpm);
                    break;
                case XenVTPMProperty.Backend:
                    ProcessRecordBackend(vtpm);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseVTPM()
        {
            string vtpm = null;

            if (VTPM != null)
                vtpm = (new XenRef<XenAPI.VTPM>(VTPM)).opaque_ref;
            else if (Ref != null)
                vtpm = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'VTPM', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    VTPM));
            }

            return vtpm;
        }

        private void ProcessRecordUuid(string vtpm)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.VTPM.get_uuid(session, vtpm);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVM(string vtpm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VTPM.get_VM(session, vtpm);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordBackend(string vtpm)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.VTPM.get_backend(session, vtpm);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenVTPMProperty
    {
        Uuid,
        VM,
        Backend
    }

}
