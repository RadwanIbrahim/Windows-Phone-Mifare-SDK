using Mifare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private SmartCardReader CardReader = null;
        MifareCard MifareCard = null;
        private Object CardConnectionLock = new Object();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.UnhandledException += Current_UnhandledException;
            GetDevices();
        }

        async private void GetDevices()
        {
            try
            {
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(SmartCardReader.GetDeviceSelector(SmartCardReaderKind.Nfc));

                // There is a bug on some devices that were updated to WP8.1 where an NFC SmartCardReader is
                // enumerated despite that the device does not support it. As a workaround, we can do an additonal check
                // to ensure the device truly does support it.
                var workaroundDetect = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{50DD5230-BA8A-11D1-BF5D-0000F805F530}\" AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True");

                if (workaroundDetect.Count == 0 || devices.Count == 0)
                {
                    PopupMessage("No Reader Found!");
                }

                CardReader = await SmartCardReader.FromIdAsync(devices.First().Id);
                MifareCard = new MifareCard(new MifareClassic());

                CardReader.CardAdded += CardAdded;
                CardReader.CardRemoved += CardRemoved;
            }
            catch (Exception e)
            {
                PopupMessage("Exception: " + e.Message);
            }
        }

        private async void CardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                (CardStatus.Fill as SolidColorBrush).Color = Colors.Red;
                ControlPanel.IsEnabled = false;
            });

            MifareCard.ReleaseConnection();
        }

        private async void CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            try
            {
                await MifareCard.ConnectAsync(args.SmartCard);
                var cardname = MifareCard.GetCardName();
                var cardid = await MifareCard.GetCardIDAsync();

                byte[] KeyA = { 0xEE, 0x14, 0xB8, 0x5E, 0xAC, 0x78 };
                byte[] KeyB = { 0x2B, 0x44, 0xD1, 0x6B, 0x6C, 0x7D };

                byte[] madkey = { 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5 };
                byte[] madkeyb = { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };

                await MifareCard.LoadKeyAsync(Mifare.DefaultKeys.FactoryDefault, KeyTypeEnum.KeyA);
                await MifareCard.LoadKeyAsync(Mifare.DefaultKeys.FactoryDefault, KeyTypeEnum.KeyB);
                await MifareCard.LoadKeyAsync(Mifare.DefaultKeys.FactoryDefault, KeyTypeEnum.KeyDefaultF);
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    (CardStatus.Fill as SolidColorBrush).Color = Colors.Green;
                    ControlPanel.IsEnabled = true;
                    CardId.Text = cardid;
                    CardType.Text = cardname;
                });
            }
            catch (Exception ex)
            {
                PopupMessage(ex.Message);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {

        }

        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }


        public async void PopupMessage(string message)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var dlg = new MessageDialog(message);
                await dlg.ShowAsync();
            });
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Write(object sender, RoutedEventArgs e)
        {
            try
            {
                int discard = 0;
                var bytes = Mifare.Utility.HexEncoding.GetBytes(WriteData.Text, out discard);
                var bytes2 = new byte[] {0x87, 0xD6 ,0x12 ,0x00 ,0x78 ,0x29, 0xED, 0xFF, 0x87, 0xD6 ,0x12 ,0x00 ,0x02, 0xFD, 0x02, 0xFD, };
                await MifareCard.WrirteDataAsync(int.Parse(WriteSectorNumber.Text), int.Parse(WriteDataBlockNumber.Text), bytes2);
                await MifareCard.FlushAsync();
            }
            catch (Exception ex)
            {
                PopupMessage(ex.Message);
            }
        }

        private async void Read(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = await MifareCard.ReadDataAsync(int.Parse(ReadSectorNumber.Text), int.Parse(ReadDataBlockNumber.Text), 16);

                Data.Text += BitConverter.ToString(data);
                Data.Text += Environment.NewLine;
            }
            catch (Exception ex)
            {
                PopupMessage(ex.Message);
            }
        }

        private async void Format(object sender, RoutedEventArgs e)
        {

            Sector sector0 = MifareCard.GetSector(2);
            var sector0access = await sector0.Access();


            // format 

            sector0access.DataAreas[0].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
            sector0access.DataAreas[0].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
            sector0access.DataAreas[0].Increment = DataAreaAccessCondition.ConditionEnum.KeyA;
            sector0access.DataAreas[0].Decrement = DataAreaAccessCondition.ConditionEnum.KeyA;

            sector0access.DataAreas[1].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
            sector0access.DataAreas[1].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
            sector0access.DataAreas[1].Increment = DataAreaAccessCondition.ConditionEnum.KeyA;
            sector0access.DataAreas[1].Decrement = DataAreaAccessCondition.ConditionEnum.KeyA;

            sector0access.DataAreas[2].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
            sector0access.DataAreas[2].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
            sector0access.DataAreas[2].Increment = DataAreaAccessCondition.ConditionEnum.KeyA;
            sector0access.DataAreas[2].Decrement = DataAreaAccessCondition.ConditionEnum.KeyA;

            sector0access.Trailer.KeyARead = TrailerAccessCondition.ConditionEnum.Never;
            sector0access.Trailer.KeyAWrite = TrailerAccessCondition.ConditionEnum.KeyB;
            sector0access.Trailer.AccessBitsRead = TrailerAccessCondition.ConditionEnum.KeyAOrB;
            sector0access.Trailer.AccessBitsWrite = TrailerAccessCondition.ConditionEnum.KeyB;
            sector0access.Trailer.KeyBRead = TrailerAccessCondition.ConditionEnum.Never;
            sector0access.Trailer.KeyBWrite = TrailerAccessCondition.ConditionEnum.KeyB;
            await sector0.SetKeyA("FFFFFFFFFFFF");
            await sector0.SetKeyB("FFFFFFFFFFFF");
            
            // sector0access.MADVersion = AccessConditions.MADVersionEnum.Version1;

            await sector0.FlushTrailer("FFFFFFFFFFFF", "FFFFFFFFFFFF");

        }
    }
}
