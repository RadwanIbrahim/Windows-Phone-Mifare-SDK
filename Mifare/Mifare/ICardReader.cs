using Pcsc.Common;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;

namespace Mifare
{
    /// <summary>
    /// Key to use to login into the sector
    /// </summary>
    public enum KeyTypeEnum : byte
    {
        KeyA = 0,
        KeyB = 1,
        KeyDefaultF = 2
    }

    /// <summary>
    /// Possible card types that have been approached to the reader. Only MiFARE_1K and MiFARE_4K are currently supported
    /// </summary>
    public enum CardTypeEnum
    {
        Unknown,
        MiFARE_Light,
        MiFARE_1K,
        MiFARE_4K,
        MiFARE_DESFire,
        MiFARE_UltraLight
    };

    /// <summary>
    /// interface for a generic card reader
    /// </summary>
    public interface ICardReader
    {
        /// <summary>
        /// returns the type of the card in the reader
        /// </summary>
        /// <returns>returns IccDetection</returns>
        /// 
        Task ConnectAsync(SmartCard smartcard);
        void ReleaseConnection();
        string GetCardName();
        Task<string> GetCardIDAsync();
        Task Authenticate(int sector, int datablock, KeyTypeEnum keytype);

        /// <summary>
        /// Login into the given sector using the given key
        /// </summary>
        /// <param name="mifareKey">key to use</param>
        /// <param name="keySlotNumber">sector to login into</param>
        Task LoadKeyAsync(byte[] mifareKey, KeyTypeEnum keySlotNumber = 0);

        /// <summary>
        /// read a datablock from a sector 
        /// </summary>
        /// <param name="sector">sector to read</param>
        /// <param name="datablock">datablock to read</param>
        /// <returns> data  from the datablock </returns>

        Task<byte[]> ReadAsync(int sector, int datablock);

        /// <summary>
        /// write data in a datablock
        /// </summary>
        /// <param name="sector">sector to write</param>
        /// <param name="datablock">datablock to write</param>
        /// <param name="data">data to write. this is a 16-bytes array</param>

        Task WriteAsync(int sector, int datablock, byte[] data);

    }


}
