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
    [Cmdlet(VerbsCommon.Get, "XenUserProperty", SupportsShouldProcess = false)]
    public class GetXenUserProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.User User { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.User> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenUserProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string user = ParseUser();

            switch (XenProperty)
            {
                case XenUserProperty.Uuid:
                    ProcessRecordUuid(user);
                    break;
                case XenUserProperty.ShortName:
                    ProcessRecordShortName(user);
                    break;
                case XenUserProperty.Fullname:
                    ProcessRecordFullname(user);
                    break;
                case XenUserProperty.OtherConfig:
                    ProcessRecordOtherConfig(user);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseUser()
        {
            string user = null;

            if (User != null)
                user = (new XenRef<XenAPI.User>(User)).opaque_ref;
            else if (Ref != null)
                user = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'User', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    User));
            }

            return user;
        }

        private void ProcessRecordUuid(string user)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.User.get_uuid(session, user);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordShortName(string user)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.User.get_short_name(session, user);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordFullname(string user)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.User.get_fullname(session, user);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string user)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.User.get_other_config(session, user);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        #endregion
    }

    public enum XenUserProperty
    {
        Uuid,
        ShortName,
        Fullname,
        OtherConfig
    }

}
