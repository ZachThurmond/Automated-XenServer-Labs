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
    [Cmdlet(VerbsCommon.Get, "XenSubjectProperty", SupportsShouldProcess = false)]
    public class GetXenSubjectProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Subject Subject { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Subject> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenSubjectProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string subject = ParseSubject();

            switch (XenProperty)
            {
                case XenSubjectProperty.Uuid:
                    ProcessRecordUuid(subject);
                    break;
                case XenSubjectProperty.SubjectIdentifier:
                    ProcessRecordSubjectIdentifier(subject);
                    break;
                case XenSubjectProperty.OtherConfig:
                    ProcessRecordOtherConfig(subject);
                    break;
                case XenSubjectProperty.Roles:
                    ProcessRecordRoles(subject);
                    break;
                case XenSubjectProperty.PermissionsNameLabel:
                    ProcessRecordPermissionsNameLabel(subject);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSubject()
        {
            string subject = null;

            if (Subject != null)
                subject = (new XenRef<XenAPI.Subject>(Subject)).opaque_ref;
            else if (Ref != null)
                subject = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Subject', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Subject));
            }

            return subject;
        }

        private void ProcessRecordUuid(string subject)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Subject.get_uuid(session, subject);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSubjectIdentifier(string subject)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Subject.get_subject_identifier(session, subject);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordOtherConfig(string subject)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Subject.get_other_config(session, subject);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordRoles(string subject)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Subject.get_roles(session, subject);

                        var records = new List<XenAPI.Role>();

                        foreach (var _ref in refs)
                        {
                            if (_ref.opaque_ref == "OpaqueRef:NULL")
                                continue;

                            var record = XenAPI.Role.get_record(session, _ref);
                            record.opaque_ref = _ref.opaque_ref;
                            records.Add(record);
                        }

                        WriteObject(records, true);
            });
        }

        private void ProcessRecordPermissionsNameLabel(string subject)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Subject.get_permissions_name_label(session, subject);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenSubjectProperty
    {
        Uuid,
        SubjectIdentifier,
        OtherConfig,
        Roles,
        PermissionsNameLabel
    }

}
