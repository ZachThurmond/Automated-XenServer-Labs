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
    [Cmdlet(VerbsCommon.Get, "XenRoleProperty", SupportsShouldProcess = false)]
    public class GetXenRoleProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Role Role { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Role> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenRoleProperty XenProperty { get; set; }

        #endregion

        public override object GetDynamicParameters()
        {
            switch (XenProperty)
            {
                case XenRoleProperty.ByPermission:
                    _context = new XenRolePropertyByPermissionDynamicParameters();
                    return _context;
                case XenRoleProperty.ByPermissionNameLabel:
                    _context = new XenRolePropertyByPermissionNameLabelDynamicParameters();
                    return _context;
                default:
                    return null;
            }
        }

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string role = ParseRole();

            switch (XenProperty)
            {
                case XenRoleProperty.Uuid:
                    ProcessRecordUuid(role);
                    break;
                case XenRoleProperty.NameLabel:
                    ProcessRecordNameLabel(role);
                    break;
                case XenRoleProperty.NameDescription:
                    ProcessRecordNameDescription(role);
                    break;
                case XenRoleProperty.Subroles:
                    ProcessRecordSubroles(role);
                    break;
                case XenRoleProperty.Permissions:
                    ProcessRecordPermissions(role);
                    break;
                case XenRoleProperty.PermissionsNameLabel:
                    ProcessRecordPermissionsNameLabel(role);
                    break;
                case XenRoleProperty.ByPermission:
                    ProcessRecordByPermission(role);
                    break;
                case XenRoleProperty.ByPermissionNameLabel:
                    ProcessRecordByPermissionNameLabel(role);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseRole()
        {
            string role = null;

            if (Role != null)
                role = (new XenRef<XenAPI.Role>(Role)).opaque_ref;
            else if (Ref != null)
                role = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Role', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Role));
            }

            return role;
        }

        private void ProcessRecordUuid(string role)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Role.get_uuid(session, role);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameLabel(string role)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Role.get_name_label(session, role);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordNameDescription(string role)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Role.get_name_description(session, role);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSubroles(string role)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Role.get_subroles(session, role);

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

        private void ProcessRecordPermissions(string role)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Role.get_permissions(session, role);

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

        private void ProcessRecordPermissionsNameLabel(string role)
        {
            RunApiCall(()=>
            {
                    string[] obj = XenAPI.Role.get_permissions_name_label(session, role);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordByPermission(string role)
        {
            RunApiCall(()=>
            {var contxt = _context as XenRolePropertyByPermissionDynamicParameters;

                    var refs = XenAPI.Role.get_by_permission(session, contxt.Permission);

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

        private void ProcessRecordByPermissionNameLabel(string role)
        {
            RunApiCall(()=>
            {var contxt = _context as XenRolePropertyByPermissionNameLabelDynamicParameters;

                    var refs = XenAPI.Role.get_by_permission_name_label(session, contxt.Label);

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

        #endregion
    }

    public enum XenRoleProperty
    {
        Uuid,
        NameLabel,
        NameDescription,
        Subroles,
        Permissions,
        PermissionsNameLabel,
        ByPermission,
        ByPermissionNameLabel
    }

    public class XenRolePropertyByPermissionDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public XenRef<XenAPI.Role> Permission { get; set; }
 
    }

    public class XenRolePropertyByPermissionNameLabelDynamicParameters : IXenServerDynamicParameter
    {
        [Parameter]
        public string Label { get; set; }
 
    }

}
