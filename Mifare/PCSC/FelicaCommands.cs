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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace Felica
{
    /// <summary>
    /// Felica Check command when sent to the card the card is expected to read the byte of blocks
    /// specified in the block list. The service code should be in little endian format.
    /// </summary>
    public class Check : Pcsc.ReadBinary
    {
        public byte ServiceCount { get; private set; }
        public byte[] ServiceCodeList { get; private set; }

        public byte BlockCount {get; private set; }
        public byte[] BlockList { get; private set; }

        public Check(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
            : base(0, 0)
        {
            ServiceCount = serviceCount;
            ServiceCodeList = serviceCodeList;
            BlockCount = blockCount;
            BlockList = blockList;
            base.CommandData = GetDataIn(serviceCount, serviceCodeList, blockCount, blockList);
        }

        private static byte[] GetDataIn(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
        {
            DataWriter dataWriter = new DataWriter();

            dataWriter.WriteByte(serviceCount);
            dataWriter.WriteBytes(serviceCodeList);
            dataWriter.WriteByte(blockCount);
            dataWriter.WriteBytes(blockList);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
    /// <summary>
    /// Felica Update command when sent to the card the card is expected to write the byte of blocks
    /// specified in the block list. The service code should be in little endian format.
    /// </summary>
    public class Update : Pcsc.UpdateBinary
    {
        public byte ServiceCount {get; private set; }
        public byte[] ServiceCodeList { get; private set; }

        public byte BlockCount {get; private set; }
        public byte[] BlockList { get; private set; }

        public Update(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList, byte[] blockData)
            : base(0, GetDataIn(serviceCount, serviceCodeList, blockCount, blockList, blockData))
        {
            ServiceCount = serviceCount;
            ServiceCodeList = serviceCodeList;
            BlockCount = blockCount;
            BlockList = blockList;
        }

        private static byte[] GetDataIn(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList, byte[] blockData)
        {
            DataWriter dataWriter = new DataWriter();

            dataWriter.WriteByte(serviceCount);
            dataWriter.WriteBytes(serviceCodeList);
            dataWriter.WriteByte(blockCount);
            dataWriter.WriteBytes(blockList);
            dataWriter.WriteBytes(blockData);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
    /// <summary>
    /// Felica GetUid command
    /// </summary>
    public class GetUid : Pcsc.GetUid
    {
        public GetUid()
            : base()
        {
        }
    }
    /// <summary>
    /// Felica GetHistoricalBytes command
    /// </summary>
    public class GetHistoricalBytes : Pcsc.GetHistoricalBytes
    {
        public GetHistoricalBytes()
            : base()
        {
        }
    }
    /// <summary>
    /// Felica response APDU
    /// </summary>
    public class ApduResponse : Pcsc.ApduResponse
    {
        public ApduResponse()
            : base()
        {
        }
    }
}
