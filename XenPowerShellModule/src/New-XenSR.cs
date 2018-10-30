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
    [Cmdlet(VerbsCommon.New, "XenSR", DefaultParameterSetName = "Hashtable", SupportsShouldProcess = true)]
    [OutputType(typeof(XenAPI.SR))]
    [OutputType(typeof(XenAPI.Task))]
    [OutputType(typeof(void))]
    public class NewXenSRCommand : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter(ParameterSetName = "Hashtable", Mandatory = true)]
        public Hashtable HashTable { get; set; }

        [Parameter(ParameterSetName = "Record", Mandatory = true)]
        public XenAPI.SR Record { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public XenRef<XenAPI.Host> XenHost { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable DeviceConfig { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public long PhysicalSize { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string NameLabel { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string NameDescription { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string Type { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public string ContentType { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public bool Shared { get; set; }

        [Parameter(ParameterSetName = "Fields")]
        public Hashtable SmConfig { get; set; }

        protected override bool GenerateAsyncParam
        {
            get { return true; }
        }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();
            if (Record != null)
            {
                NameLabel = Record.name_label;
                NameDescription = Record.name_description;
                PhysicalSize = Record.physical_size;
                Type = Record.type;
                ContentType = Record.content_type;
                Shared = Record.shared;
                SmConfig = CommonCmdletFunctions.ConvertDictionaryToHashtable(Record.sm_config);
            }
            else if (HashTable != null)
            {
                NameLabel = Marshalling.ParseString(HashTable, "name_label");
                NameDescription = Marshalling.ParseString(HashTable, "name_description");
                PhysicalSize = Marshalling.ParseLong(HashTable, "physical_size");
                Type = Marshalling.ParseString(HashTable, "type");
                ContentType = Marshalling.ParseString(HashTable, "content_type");
                Shared = Marshalling.ParseBool(HashTable, "shared");
                SmConfig = (Marshalling.ParseHashTable(HashTable, "sm_config"));
            }
            if (!ShouldProcess(session.Url, "SR.create"))
                return;

            RunApiCall(()=>
            {
                var contxt = _context as XenServerCmdletDynamicParameters;

                if (contxt != null && contxt.Async)
                {
                    taskRef = XenAPI.SR.async_create(session, XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(DeviceConfig), PhysicalSize, NameLabel, NameDescription, Type, ContentType, Shared, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(SmConfig));

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
                    string objRef = XenAPI.SR.create(session, XenHost, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(DeviceConfig), PhysicalSize, NameLabel, NameDescription, Type, ContentType, Shared, CommonCmdletFunctions.ConvertHashTableToDictionary<string, string>(SmConfig));

                    if (PassThru)
                    {
                        XenAPI.SR obj = null;

                        if (objRef != "OpaqueRef:NULL")
                        {
                            obj = XenAPI.SR.get_record(session, objRef);
                            obj.opaque_ref = objRef;
                        }

                        WriteObject(obj, true);
                    }
                }

            });

            UpdateSessions();
        }

        #endregion
   }
}
