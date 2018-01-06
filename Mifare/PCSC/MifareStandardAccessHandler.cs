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

namespace Mifare
{
    public class DefaultKeys
    {
        public static readonly byte[] FactoryDefault = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
    }



    /// <summary>
    /// Access handler class for Mifare Standard/Classic based ICC
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
        /// connection object to a Mifare Standard ICC
        /// </param>
        public AccessHandler(SmartCardConnection ScConnection)
        {
            connectionObject = ScConnection;
        }
        /// <summary>
        /// Wrapper method to read 16 bytes
        /// </summary>
        /// <param name="pageAddress">
        /// start page to read
        /// </param>
        /// <returns>
        /// byte array of 16 bytes
        /// </returns>
        public async Task LoadKeyAsync(byte[] mifareKey, byte keySlotNumber = 0)
        {
            var apduRes = await connectionObject.TransceiveAsync(new Mifare.LoadKey(mifareKey, keySlotNumber));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure loading key for MIFARE Standard card, " + apduRes.ToString());
            }

            return;
        }


        public async Task Authenticate(ushort blockNumber, GeneralAuthenticate.GeneralAuthenticateKeyType keyType, byte keySlotNumber = 0)
        {
            var genAuthRes = await connectionObject.TransceiveAsync(new Mifare.GeneralAuthenticate(blockNumber, keySlotNumber, keyType));
            if (!genAuthRes.Succeeded)
            {
                throw new Exception("Failure authenticating to MIFARE Standard card, " + genAuthRes.ToString());
            }
        }


        /// <summary>
        /// Reads 16 bytes
        /// </summary>
        /// <param name="blockNumber">
        /// Block number to read
        /// </param>
        /// <param name="keyType">
        /// Choose MIFARE key A or B
        /// </param>
        /// <param name="keySlotNumber">
        /// Slot where key has previously been stored with a previous call to Load Keys
        /// </param>
        /// <returns>
        /// byte array of 16 bytes
        /// </returns>
        public async Task<byte[]> ReadAsync(ushort blockNumber)
        {
            var readRes = await connectionObject.TransceiveAsync(new Mifare.Read(blockNumber));
            if (!readRes.Succeeded)
            {
                throw new Exception("Failure reading MIFARE Standard card, " + readRes.ToString());
            }

            return readRes.ResponseData;
        }
        /// <summary>
        /// Wrapper method write 16 bytes at the pageAddress
        /// </param name="blockNumber">
        /// Block number
        /// </param>
        /// <param name="keyType">
        /// Choose MIFARE key A or B
        /// </param>
        /// <param name="keySlotNumber">
        /// Slot where key has previously been stored with a previous call to Load Keys
        /// </param>
        /// byte array of the data to write
        /// </returns>
        public async Task WriteAsync(byte blockNumber, byte[] data)
        {
            if (data.Length != 16)
            {
                throw new NotSupportedException();
            }

            var apduRes = await connectionObject.TransceiveAsync(new Mifare.Write(blockNumber, ref data));
            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing MIFARE Standard card, " + apduRes.ToString());
            }
        }


        public async Task Increment(byte address, byte[] value)
        {
            if (value.Length != 4)
            {
                throw new ArgumentOutOfRangeException("value must be 4 byte");
            }

            var apduRes = await connectionObject.TransceiveAsync(new Mifare.Increment(address, ref value));
            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing MIFARE Standard card, " + apduRes.ToString());
            }
        }


        public async Task Decrement(byte address, byte[] value)
        {
            if (value.Length != 4)
            {
                throw new ArgumentOutOfRangeException("value must be 4 byte");
            }

            var apduRes = await connectionObject.TransceiveAsync(new Mifare.Decrement(address, ref value));
            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing MIFARE Standard card, " + apduRes.ToString());
            }
        }

        public async Task ReStore(byte Source, byte Destination )
        {
            var apduRes = await connectionObject.TransceiveAsync(new Mifare.Restore(Source, Destination));
            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing MIFARE Standard card, " + apduRes.ToString());
            }
        }

        /// <summary>
        /// Wrapper method get the Mifare Standard ICC UID
        /// </summary>
        /// <returns>
        /// byte array UID
        /// </returns>
        public async Task<byte[]> GetUidAsync()
        {
            var apduRes = await connectionObject.TransceiveAsync(new Mifare.GetUid());
            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure getting UID of MIFARE Standard card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
    }
}
