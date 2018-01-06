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
using System.Threading.Tasks;
using Windows.Devices.SmartCards;

using Pcsc;
using System;

namespace MifareUltralight
{
    /// <summary>
    /// Access handler class for MifareUL based ICC. It provides wrappers for different MifareUL 
    /// commands
    /// </summary>
    public class AccessHandler
    {
        /// <summary>
        /// connection object to smart card
        /// </summary>
        private SmartCardConnection connectionObject { set; get; }
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ScConnection">
        /// connection object to a MifareUL ICC
        /// </param>
        public AccessHandler(SmartCardConnection ScConnection)
        {
            connectionObject = ScConnection;
        }
        /// <summary>
        /// Wrapper method to read 16 bytes (4 pages) starting at pageAddress
        /// </summary>
        /// <param name="pageAddress">
        /// start page to read
        /// </param>
        /// <returns>
        /// byte array of 16 bytes
        /// </returns>
        public async Task<byte[]> ReadAsync(byte pageAddress)
        {
            var apduRes = await connectionObject.TransceiveAsync(new MifareUltralight.Read(pageAddress));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure reading MIFARE Ultralight card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
        /// <summary>
        /// Wrapper method write 4 bytes at the pageAddress
        /// </param name="pageAddress">
        /// page address to write
        /// </param>
        /// byte array of the data to write
        /// </returns>
        public async void WriteAsync(byte pageAddress, byte[] data)
        {
            if (data.Length != 4)
            {
                throw new NotSupportedException();
            }

            var apduRes = await connectionObject.TransceiveAsync(new MifareUltralight.Write(pageAddress, ref data));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing MIFARE Ultralight card, " + apduRes.ToString());
            }
        }
        /// <summary>
        /// Wrapper method get the MifareUL ICC UID
        /// </summary>
        /// <returns>
        /// byte array UID
        /// </returns>
        public async Task<byte[]> GetUidAsync()
        {
            var apduRes = await connectionObject.TransceiveAsync(new MifareUltralight.GetUid());

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure getting UID of MIFARE Ultralight card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
    }
}
