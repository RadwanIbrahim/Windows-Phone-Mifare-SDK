using Pcsc;
using Pcsc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;

namespace Mifare
{

    public class MifareClassic : ICardReader
    {
        private SmartCardConnection _MifareSmartCardConnection;
        private SmartCard _MifareSmartCard;
        private Mifare.AccessHandler mfStdAccess;
        private CardName CurrentCardName = CardName.Unknown;
        private Object CardConnectionLock = new Object();
        public async Task ConnectAsync(SmartCard smartcard)
        {
            _MifareSmartCard = smartcard;
            var newConnection = await smartcard.ConnectAsync();
            lock (CardConnectionLock)
            {
                if (_MifareSmartCardConnection != null)
                {
                    _MifareSmartCardConnection.Dispose();
                }
                _MifareSmartCardConnection = newConnection;
            }
            mfStdAccess = new Mifare.AccessHandler(_MifareSmartCardConnection);
            IccDetection cardIdentification = new IccDetection(_MifareSmartCard, _MifareSmartCardConnection);
            await cardIdentification.DetectCardTypeAync();
            CurrentCardName = cardIdentification.PcscCardName;
            if (!(CurrentCardName == CardName.MifareStandard1K || CurrentCardName == CardName.MifareStandard4K))
                throw new Mifare.Exceptions.CardTypeException("this Card is not mifare classic card");
        }
        public void ReleaseConnection()
        {
            lock (CardConnectionLock)
            {
                if (_MifareSmartCardConnection != null)
                {
                    _MifareSmartCardConnection.Dispose();
                }
            }
        }
        public string GetCardName()
        {
            return CurrentCardName.ToString();
        }
        public async Task<string> GetCardIDAsync()
        {
            return BitConverter.ToString(await mfStdAccess.GetUidAsync());
        }
        public Task LoadKeyAsync(byte[] mifareKey, KeyTypeEnum keySlotNumber = 0)
        {
            return mfStdAccess.LoadKeyAsync(mifareKey, (byte)keySlotNumber);

        }
        public Task Authenticate(int sector, int datablock, KeyTypeEnum keytype)
        {
            return mfStdAccess.Authenticate(GetblockAddress(sector, datablock),
                     (keytype == KeyTypeEnum.KeyA) ? GeneralAuthenticate.GeneralAuthenticateKeyType.MifareKeyA : GeneralAuthenticate.GeneralAuthenticateKeyType.PicoTagPassKeyB,
                     (byte)keytype);
        }
        public Task<byte[]> ReadAsync(int sector, int datablock)
        {
            return mfStdAccess.ReadAsync(GetblockAddress(sector, datablock));
        }
        public Task WriteAsync(int sector, int datablock, byte[] data)
        {
            return mfStdAccess.WriteAsync(Convert.ToByte(GetblockAddress(sector, datablock)), data);
        }
        private ushort GetblockAddress(int sector, int datablock)
        {
            if (CurrentCardName == CardName.MifareStandard1K)
            {
                if (sector > 15 || sector < 0 || datablock < 0 || datablock > 4)
                    throw new ArgumentOutOfRangeException("sector or block number out of range");

                return Convert.ToUInt16((sector * 4) + datablock);
            }
            else
            {
                if (sector > 39 || sector < 0 || datablock < 0 || datablock > 15 || (sector < 32 && datablock > 4))
                    throw new ArgumentOutOfRangeException("sector or block number out of range");


                if (sector < 32)
                {
                    return Convert.ToUInt16((sector * 4) + datablock);
                }
                else
                {
                    return Convert.ToUInt16((31 * 4) + ((sector - 31) * 16) + datablock);
                }
            }
        }
    }
}
