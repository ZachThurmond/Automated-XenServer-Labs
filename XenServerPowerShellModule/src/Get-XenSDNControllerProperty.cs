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
    [Cmdlet(VerbsCommon.Get, "XenSDNControllerProperty", SupportsShouldProcess = false)]
    public class GetXenSDNControllerProperty : XenServerCmdlet
    {
        #region Cmdlet Parameters

        [Parameter(ParameterSetName = "XenObject", Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public XenAPI.SDN_controller SDNController { get; set; }

        [Parameter(ParameterSetName = "Ref", Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("opaque_ref")]
        public XenRef<XenAPI.SDN_controller> Ref { get; set; }


        [Parameter(Mandatory = true)]
        public XenSDNControllerProperty XenProperty { get; set; }

        #endregion

        #region Cmdlet Methods

        protected override void ProcessRecord()
        {
            GetSession();

            string sdn_controller = ParseSDNController();

            switch (XenProperty)
            {
                case XenSDNControllerProperty.Uuid:
                    ProcessRecordUuid(sdn_controller);
                    break;
                case XenSDNControllerProperty.Protocol:
                    ProcessRecordProtocol(sdn_controller);
                    break;
                case XenSDNControllerProperty.Address:
                    ProcessRecordAddress(sdn_controller);
                    break;
                case XenSDNControllerProperty.Port:
                    ProcessRecordPort(sdn_controller);
                    break;
            }

            UpdateSessions();
        }

        #endregion

        #region Private Methods

        private string ParseSDNController()
        {
            string sdn_controller = null;

            if (SDNController != null)
                sdn_controller = (new XenRef<XenAPI.SDN_controller>(SDNController)).opaque_ref;
            else if (Ref != null)
                sdn_controller = Ref.opaque_ref;
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("At least one of the parameters 'SDNController', 'Ref', 'Uuid' must be set"),
                    string.Empty,
                    ErrorCategory.InvalidArgument,
                    SDNController));
            }

            return sdn_controller;
        }

        private void ProcessRecordUuid(string sdn_controller)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SDN_controller.get_uuid(session, sdn_controller);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordProtocol(string sdn_controller)
        {
            RunApiCall(()=>
            {
                    sdn_controller_protocol obj = XenAPI.SDN_controller.get_protocol(session, sdn_controller);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordAddress(string sdn_controller)
        {
            RunApiCall(()=>
            {
                    string obj = XenAPI.SDN_controller.get_address(session, sdn_controller);

                        WriteObject(obj, true);
            });
        }

        private void ProcessRecordPort(string sdn_controller)
        {
            RunApiCall(()=>
            {
                    long obj = XenAPI.SDN_controller.get_port(session, sdn_controller);

                        WriteObject(obj, true);
            });
        }

        #endregion
    }

    public enum XenSDNControllerProperty
    {
        Uuid,
        Protocol,
        Address,
        Port
    }

}
