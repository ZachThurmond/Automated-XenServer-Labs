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
    [Cmdlet(VerbsCommon.Get, "XenCrashdumpProperty", SupportsShouldProcess = false)]
    public class GetXenCrashdumpProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Crashdump Crashdump { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Crashdump> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenCrashdumpProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string crashdump = ParseCrashdump();

            switch (XenProperty)
            {
                case XenCrashdumpProperty.Uuid:
                    ProcessRecordUuid(crashdump);
                    break;
                case XenCrashdumpProperty.VM:
                    ProcessRecordVM(crashdump);
                    break;
                case XenCrashdumpProperty.VDI:
                    ProcessRecordVDI(crashdump);
                    break;
                case XenCrashdumpProperty.OtherConfig:
                    ProcessRecordOtherConfig(crashdump);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseCrashdump()
        {
            string crashdump = null;

            if (Crashdump != null)
                crashdump = (new XenRef<XenAPI.Crashdump>(Crashdump)).opaque_ref;
            else if (Ref != null)
                crashdump = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Crashdump', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Crashdump));
            }

            return crashdump;
        }

        private void ProcessRecordUuid(string crashdump)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Crashdump.get_uuid(session, crashdump);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVM(string crashdump)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Crashdump.get_VM(session, crashdump);

                        XenAPI.VM obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VM.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordVDI(string crashdump)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Crashdump.get_VDI(session, crashdump);

                        XenAPI.VDI obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.VDI.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string crashdump)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Crashdump.get_other_config(session, crashdump);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenCrashdumpProperty
    {
        Uuid,
        VM,
        VDI,
        OtherConfig
    }

}
