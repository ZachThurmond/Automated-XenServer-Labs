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
    [Cmdlet(VerbsCommon.Get, "XenBondProperty", SupportsShouldProcess = false)]
    public class GetXenBondProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.Bond Bond { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.Bond> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenBondProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string bond = ParseBond();

            switch (XenProperty)
            {
                case XenBondProperty.Uuid:
                    ProcessRecordUuid(bond);
                    break;
                case XenBondProperty.Master:
                    ProcessRecordMaster(bond);
                    break;
                case XenBondProperty.Slaves:
                    ProcessRecordSlaves(bond);
                    break;
                case XenBondProperty.OtherConfig:
                    ProcessRecordOtherConfig(bond);
                    break;
                case XenBondProperty.PrimarySlave:
                    ProcessRecordPrimarySlave(bond);
                    break;
                case XenBondProperty.Mode:
                    ProcessRecordMode(bond);
                    break;
                case XenBondProperty.Properties:
                    ProcessRecordProperties(bond);
                    break;
                case XenBondProperty.LinksUp:
                    ProcessRecordLinksUp(bond);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseBond()
        {
            string bond = null;

            if (Bond != null)
                bond = (new XenRef<XenAPI.Bond>(Bond)).opaque_ref;
            else if (Ref != null)
                bond = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'Bond', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    Bond));
            }

            return bond;
        }

        private void ProcessRecordUuid(string bond)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.Bond.get_uuid(session, bond);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMaster(string bond)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Bond.get_master(session, bond);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordSlaves(string bond)
        {
            RunApiCall(()=>
            {
                    var refs = XenAPI.Bond.get_slaves(session, bond);

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

        private void ProcessRecordOtherConfig(string bond)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Bond.get_other_config(session, bond);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordPrimarySlave(string bond)
        {
            RunApiCall(()=>
            {
                    string objRef = XenAPI.Bond.get_primary_slave(session, bond);

                        XenAPI.PIF obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.PIF.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordMode(string bond)
        {
            RunApiCall(()=>
            {
                    bond_mode obj = XenAPI.Bond.get_mode(session, bond);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProperties(string bond)
        {
            RunApiCall(()=>
            {
                    var dict = XenAPI.Bond.get_properties(session, bond);

                        Hashtable ht = CommonCmdletFunctions.ConvertDictionaryToHashtable(dict);
                        WriteObject(ht, true);
            });
        }

        private void ProcessRecordLinksUp(string bond)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.Bond.get_links_up(session, bond);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenBondProperty
    {
        Uuid,
        Master,
        Slaves,
        OtherConfig,
        PrimarySlave,
        Mode,
        Properties,
        LinksUp
    }

}
