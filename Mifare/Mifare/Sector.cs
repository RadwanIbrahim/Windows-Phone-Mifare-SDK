using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mifare.Exceptions;
using Mifare.Utility;
using System.Threading.Tasks;


namespace Mifare
{
    public class Sector
    {
        #region Private functions
        private MifareCard _Card;
        private int _Sector;

        private DataBlock[] _DataBlocks;
        private AccessConditions _Access;
        #endregion

        #region Constructor
        internal Sector(MifareCard card, int sector)
        {
            _Card = card;
            _Sector = sector;

            _DataBlocks = new DataBlock[NumDataBlocks];
            _Access = null;
        }
        #endregion

        #region Properties

        #region Access
        /// <summary>
        /// Sector access conditions 
        /// </summary>
        public async Task<AccessConditions> Access()
        {

            if (_Access == null)
            {
                Byte[] data = await GetData(GetTrailerBlockIndex());
                _Access = AccessBits.GetAccessConditions(data);
            }

            return _Access;

        }
        #endregion

        #region KeyA
        /// <summary>
        /// Key A for the sector. This needs to be set when setting the access conditions. Key could not be read from card
        /// </summary>
        public async Task SetKeyA(string Key)
        {
            int discarded;
            Byte[] keyA = HexEncoding.GetBytes(Key, out discarded);

            DataBlock db = await GetDataBlockInt(GetTrailerBlockIndex());
            Array.Copy(keyA, 0, db.Data, 0, 6);

        }
        public async Task<string> GetKeyA()
        {
            Byte[] data = await GetData(GetTrailerBlockIndex());
            Byte[] keyA = new Byte[6];
            Array.Copy(data, 0, keyA, 0, 6);

            return HexEncoding.ToString(keyA);

        }
        #endregion

        #region KeyB
        /// <summary>
        /// Key B for the sector. This needs to be set when setting the access conditions. Key could not be read from card
        /// </summary>
        /// 

        public async Task SetKeyB(string Key)
        {
            int discarded;
            Byte[] keyB = HexEncoding.GetBytes(Key, out discarded);

            DataBlock db = await GetDataBlockInt(GetTrailerBlockIndex());
            Array.Copy(keyB, 0, db.Data, 10, 6);
        }
        public async Task<string> GetKeyB()
        {
            Byte[] data = await GetData(GetTrailerBlockIndex());
            Byte[] keyB = new Byte[6];
            Array.Copy(data, 10, keyB, 0, 6);

            return HexEncoding.ToString(keyB);
        }



        #endregion

        #region DataLength
        /// <summary>
        /// number of data bytes in the sector (trailer datablock is excluded)
        /// </summary>
        public int DataLength
        {
            get
            {
                return (NumDataBlocks - 1) * DataBlock.Length;
            }
        }
        #endregion

        #region TotalLength
        /// <summary>
        /// number of bytes in the sector (including trailer datablock)
        /// </summary>
        public int TotalLength
        {
            get
            {
                return NumDataBlocks * DataBlock.Length;
            }
        }
        #endregion

        #region NumDataBlocks
        /// <summary>
        /// number of datablocks in the sector
        /// </summary>
        public int NumDataBlocks
        {
            get
            {
                if (_Sector < 32)
                    return 4;

                return 16;
            }
        }
        #endregion

        #endregion

        #region Public functions

        #region GetData
        /// <summary>
        /// read data from a datablock
        /// </summary>
        /// <param name="block">index of the datablock</param>
        /// <returns>data read (always 16 bytes)</returns>
        /// <remarks>may throw CardLoginException and CardReadException</remarks>
        public async Task<Byte[]> GetData(int block)
        {
            DataBlock db = await GetDataBlockInt(block);
            if (db == null)
                return null;

            return db.Data;
        }
        #endregion

        #region SetData
        /// <summary>
        /// write data in the sector
        /// </summary>
        /// <param name="data">data to write</param>
        /// <param name="firstBlock">the index of the block to start write</param>
        /// <remarks>may throw CardLoginException and CardWriteException.
        /// if the length of the data to write overcomes the number of datablocks, the remaining data is not written
        /// </remarks>
        public async Task SetData(Byte[] data, int firstBlock)
        {
            int blockIdx = firstBlock;
            int bytesWritten = 0;

            while ((blockIdx < (NumDataBlocks - 1)) && (bytesWritten < data.Length))
            {
                int numBytes = Math.Min(DataBlock.Length, data.Length - bytesWritten);

                Byte[] blockData = await GetData(blockIdx);
                Array.Copy(data, bytesWritten, blockData, 0, numBytes);

                bytesWritten += numBytes;
                blockIdx++;
            }
        }
        #endregion

        #region Flush
        /// <summary>
        /// commit changes to card
        /// </summary>
        /// <remarks>may throw CardLoginException and CardWriteException</remarks>
        public async Task Flush()
        {
            foreach (DataBlock dataBlock in _DataBlocks)
            {
                if (dataBlock == null)
                    continue;

                if (dataBlock.IsTrailer)
                    continue;

                if (dataBlock.IsChanged)
                    await FlushDataBlock(dataBlock);
            }
        }
        #endregion

        #region FlushTrailer
        /// <summary>
        /// commit changes made to trailer datablock
        /// </summary>
        /// <remarks>may throw CardLoginException and CardWriteException</remarks>
        public async Task FlushTrailer(String keyA, String keyB)
        {
            DataBlock dataBlock = _DataBlocks[GetTrailerBlockIndex()];
            if (dataBlock == null)
                return;

            await SetKeyA(keyA);
            await SetKeyB(keyB);

            Byte[] data = AccessBits.CalculateAccessBits(await Access());
            Array.Copy(data, 0, dataBlock.Data, 6, 4);

            if (dataBlock.IsChanged)
               await FlushDataBlock(dataBlock);

            _Card.ActiveSector = -1;
        }
        #endregion


        #endregion
        #region Private functions

        #region GetDataBlockInt
        private async Task<DataBlock> GetDataBlockInt(int block)
        {
            DataBlock db = _DataBlocks[block];

            if (db != null)
                return db;

            if (_Card.ActiveSector != _Sector)
            {
                await _Card.Reader.Authenticate(_Sector, block, KeyTypeEnum.KeyA);

                Byte[] data;

                data = await _Card.Reader.ReadAsync(_Sector, block);

                db = new DataBlock(block, data, (block == GetTrailerBlockIndex()));
                _DataBlocks[block] = db;
            }

            return db;
        }

        #endregion

        #region FlushDataBlock
        private async Task FlushDataBlock(DataBlock dataBlock)
        {
            if (_Card.ActiveSector != _Sector)
            {

                await _Card.Reader.Authenticate(_Sector, dataBlock.Number, await GetWriteKey(dataBlock.Number));
                _Card.ActiveSector = _Sector;
            }

            await _Card.Reader.WriteAsync(_Sector, dataBlock.Number, dataBlock.Data);
        }
        #endregion

        #region GetWriteKey
        private async Task<KeyTypeEnum> GetWriteKey(int datablock)
        {
            var access = await Access();
            if (access == null)
                return KeyTypeEnum.KeyDefaultF;

            if (datablock == 3)
                return await GetTrailerWriteKey();


            return (access.DataAreas[Math.Min(datablock, access.DataAreas.Length - 1)].Write == DataAreaAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;
        }
        #endregion

        #region GetTrailerWriteKey
        private async Task<KeyTypeEnum> GetTrailerWriteKey()
        {
            var access = await Access();
            if (access == null)
                return KeyTypeEnum.KeyDefaultF;

            return (access.Trailer.AccessBitsWrite == TrailerAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;
        }
        #endregion

        #region GetTrailerBlockIndex
        private int GetTrailerBlockIndex()
        {
            return NumDataBlocks - 1;
        }
        #endregion

        #endregion

    }
}
