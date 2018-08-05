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
    [Cmdlet(VerbsCommon.New, "XenMessage", DefaultParameterSetName = "Hashtable", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.Message))]
    [OutputType(typeof(void))]
    public class NewXenMessageCommand : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "Hashtable", Mandatory = true)]
        public Hashtable HashTable { get; set; }

        [Parameter(ParameterSetName = "Record", Mandatory = true)]
        public XenAPI.Message Record { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long Priority { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public cls Cls { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string ObjUuid { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string Body { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();
            if (Record != null)
            {
                Name = Record.name;
                Priority = Record.priority;
                Cls = Record.cls;
                ObjUuid = Record.obj_uuid;
                Body = Record.body;
            }
            else if (HashTable != null)
            {
                Name = Marshalling.ParseString(HashTable, "name");
                Priority = Marshalling.ParseLong(HashTable, "priority");
                Cls = (cls)CommonCmdletFunctions.EnumParseDefault(typeof(cls), Marshalling.ParseString(HashTable, "cls"));
                ObjUuid = Marshalling.ParseString(HashTable, "obj_uuid");
                Body = Marshalling.ParseString(HashTable, "body");
            }
            if (!ShouldProcess(session.Url, "Message.create"))
                return;

            RunApiCall(()=>
            {
                    string objRef = XenAPI.Message.create(session, Name, Priority, Cls, ObjUuid, Body);

                    if (PassThru)
                    {
                        XenAPI.Message obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.Message.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
            });

            UpdateSessions();
        }

        #endregion
   }
}
