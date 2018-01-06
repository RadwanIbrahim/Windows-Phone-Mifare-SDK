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

namespace Felica
{
    /// <summary>
    /// Access handler class for Felica based ICC. It provides wrappers for different Felica 
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
        /// connection object to a Felica ICC
        /// </param>
        public AccessHandler(SmartCardConnection ScConnection)
        {
            connectionObject = ScConnection;
        }
        /// <summary>
        /// Wrapper method to read data from the felica card
        /// </summary>
        /// <param name="serviceCount">
        /// The number of service
        /// </param>
        /// <param name="serviceCodeList">
        /// The service code list in little endian format
        /// </param>
        /// </param>
        /// <param name="blockCount">
        /// The number of blocks to read
        /// </param>
        /// </param>
        /// <param name="blockList">
        /// The list of blocks to be read. Must be multiples of 2 or 3.
        /// </param>
        /// <returns>
        /// byte array of the read data
        /// </returns>
        public async Task<byte[]> ReadAsync(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
        {
            if (serviceCount != 1 || serviceCodeList.Length != 2)
            {
                throw new NotSupportedException();
            }

            var apduRes = await connectionObject.TransceiveAsync(new Felica.Check(serviceCount, serviceCodeList, blockCount, blockList));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
        /// <summary>
        /// Wrapper method to write data to the felica card
        /// </summary>
        /// <param name="serviceCount">
        /// The number of service
        /// </param>
        /// <param name="serviceCodeList">
        /// The service code list in little endian format
        /// </param>
        /// </param>
        /// <param name="blockCount">
        /// The number of blocks to read
        /// </param>
        /// </param>
        /// <param name="blockList">
        /// The list of blocks to be read. Must be multiples of 2 or 3.
        /// </param>
        /// /// <param name="blockData">
        /// The data to write for the corresponding blocks. Must be multiple of 16 of the block count.
        /// </param>
        public async Task WriteAsync(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList, byte[] blockData)
        {
            if (serviceCount != 1 || serviceCodeList.Length != 2)
            {
                throw new NotSupportedException();
            }

            if (blockData.Length != blockCount * 16)
            {
                throw new InvalidOperationException("Invalid blockData size");
            }

            var apduRes = await connectionObject.TransceiveAsync(new Felica.Update(serviceCount, serviceCodeList, blockCount, blockList, blockData));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing Felica card, " + apduRes.ToString());
            }
        }
        /// <summary>
        /// Wrapper method get the Felica UID
        /// </summary>
        /// <returns>
        /// byte array UID
        /// </returns>
        public async Task<byte[]> GetUidAsync()
        {
            var apduRes = await connectionObject.TransceiveAsync(new Felica.GetUid());

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure getting UID of Felica card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
    }
}
