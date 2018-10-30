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
    [Cmdlet(VerbsCommon.Get, "XenAuthProperty", SupportsShouldProcess = false)]
    public class GetXenAuthProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Auth Auth { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Auth> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenAuthProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenAuthProperty.SubjectIdentifier:
                    _context = new XenAuthPropertySubjectIdentifierDynamicParameters();
                    return _context;
                case XenAuthProperty.SubjectInformationFromIdentifier:
                    _context = new XenAuthPropertySubjectInformationFromIdentifierDynamicParameters();
                    return _context;
                case XenAuthProperty.GroupMembership:
                    _context = new XenAuthPropertyGroupMembershipDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string auth = ParseAuth();

            switch (XenProperty)
            {
                case XenAuthProperty.SubjectIdentifier:
                    ProcessRecordSubjectIdentifier(auth);
                    break;
                case XenAuthProperty.SubjectInformationFromIdentifier:
                    ProcessRecordSubjectInformationFromIdentifier(auth);
                    break;
                case XenAuthProperty.GroupMembership:
                    ProcessRecordGroupMembership(auth);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseAuth()
        {
            string auth = null;

            if (Auth != null)
                auth = (new XenRef<XenAPI.Auth>(Auth)).opaque_ref;
            else if (Ref != null)
                auth = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Auth', 'Ref' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Auth));
            }

            return auth;
        }

        private void ProcessRecordSubjectIdentifier(string auth)
        {
            RunApiCall(()=>
            {var contxt = _context as XenAuthPropertySubjectIdentifierDynamicParameters;

                    string obj = XenAPI.Auth.get_subject_identifier(session, contxt.SubjectName);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSubjectInformationFromIdentifier(string auth)
        {
            RunApiCall(()=>
            {var contxt = _context as XenAuthPropertySubjectInformationFromIdentifierDynamicParameters;

                    var dict = XenAPI.Auth.get_subject_information_from_identifier(session, contxt.SubjectIdentifier);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordGroupMembership(string auth)
        {
            RunApiCall(()=>
            {var contxt = _context as XenAuthPropertyGroupMembershipDynamicParameters;

                    string[] obj = XenAPI.Auth.get_group_membership(session, contxt.SubjectIdentifier);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenAuthProperty
    {
        SubjectIdentifier,
        SubjectInformationFromIdentifier,
        GroupMembership
    }

    public class XenAuthPropertySubjectIdentifierDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string SubjectName { get; set; }
 
    }

    public class XenAuthPropertySubjectInformationFromIdentifierDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string SubjectIdentifier { get; set; }
 
    }

    public class XenAuthPropertyGroupMembershipDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string SubjectIdentifier { get; set; }
 
    }

}
