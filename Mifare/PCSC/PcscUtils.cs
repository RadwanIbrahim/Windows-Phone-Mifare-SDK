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
using System;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using Windows.Storage.Streams;

namespace Pcsc
{
    public static class SmartCardConnectionExtension
    {
        /// <summary>
        /// Extension method to SmartCardConnection class similar to Transmit asyc method, however it accepts PCSC SDK commands.
        /// </summary>
        /// <param name="apduCommand">
        /// APDU command object to send to the ICC
        /// </param>
        /// <param name="connection">
        /// SmartCardConnection object
        /// </param>
        /// <returns>APDU response object of type defined by the APDU command object</returns>
        public static async Task<Iso7816.ApduResponse> TransceiveAsync(this SmartCardConnection connection, Iso7816.ApduCommand apduCommand)
        {
            Iso7816.ApduResponse apduRes = Activator.CreateInstance(apduCommand.ApduResponseType) as Iso7816.ApduResponse;

            IBuffer responseBuf = await connection.TransmitAsync(apduCommand.GetBuffer());

            apduRes.ExtractResponse(responseBuf);

            return apduRes;
        }
    }
}
