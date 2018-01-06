/* Copyright (c) Microsoft Corporation
 * 
 * All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
 * 
 * See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.
*/

namespace Desfire
{
    /// <summary>
    /// Class DesfireCommand extends the Iso7816.ApduCommand and provides
    /// mappings to Iso7816 command fields
    /// </summary>
    public class DesfireCommand : Iso7816.ApduCommand
    {
        public byte Command
        {
            set { base.INS = value; }
            get { return base.INS; }
        }
        public byte[] Data
        {
            set { base.CommandData = value; }
            get { return base.CommandData; }
        }
        public enum CommandType : byte
        {
            GetVersion = 0x60,
            GetAdditionalFrame = 0xAF,
            SelectApplication = 0x5A,
            ReadData = 0xBD,
            ReadRecord = 0xBB
        };

        public DesfireCommand()
            : base((byte)Iso7816.Cla.ProprietaryCla9x, 0, 0, 0, null, 0)
        {
            ApduResponseType = typeof(Desfire.DesfireResponse);
        }
        public DesfireCommand(CommandType cmd, byte[] data)
            : base((byte)Iso7816.Cla.ProprietaryCla9x, (byte)cmd, 0x00, 0x00, data, 0x00)
        {
            ApduResponseType = typeof(Desfire.DesfireResponse);
        }
    }
    /// <summary>
    /// Class DesfireResponse extends the Iso7816.ApduResponse.
    /// </summary>
    public class DesfireResponse : Iso7816.ApduResponse
    {
        public DesfireResponse()
            : base()
        { }
        public override string SWTranslation
        {
            get
            {
                if (SW1 != 0x91)
                {
                    return "Unknown";
                }
                switch (SW2)
                {
                    case 0x00:
                        return "Success";

                    case 0xAF:
                        return "Additional frames expected";

                    default:
                        return "Unknown";
                }
            }
        }
        public override bool Succeeded
        {
            get
            {
                return SW == 0x9100;
            }
        }
        public bool SubsequentFrame
        {
            get
            {
                return SW == 0x91AF;
            }
        }
        public bool BoundaryError
        {
            get
            {
                return SW == 0x91BE;
            }
        }
    }
}
