using MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MobileApp.Services;
using Xamarin.Forms;
using System.Linq;

namespace MobileApp.ViewModels
{
    class GroundMoistureViewModel : BaseViewModel
    {
        public string Topic { get; private set; }
        public ObservableCollection<MQTTMessage> MQTTMessages { get; set; }

        TableView Table = new TableView();
        public GroundMoistureViewModel(string topic)
        {
            Topic = topic;
            Title = "Ground Moisture";

            // !!called every 30 minutes -> 1800 seconds!!
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                // get all messages send to MQTT server
                //do not check whether it has been set to null! This is redundant!
                //either a filled observablecollection is send back or an empty observablecollection
                MQTTMessages = App.Client.MQTTMessageStore.GetAllMessagesFromTopic(Topic);

                //initialize the methods to update the database every half an hour...
                //and retrieve from the database every half an hour
                init();

                // return true to repeat counting, false to stop timer
                return true;
            });
        }

        public async void init()
        {
            MQTToDatabase();
            CreateXamlTable();
        }

        public async void MQTToDatabase()
        {
            foreach(MQTTMessage data in MQTTMessages)
            {
                MoistMeter compressedMoistMeter = new MoistMeter();
                compressedMoistMeter.MoisturePercentage = data.Message;
                Console.WriteLine(compressedMoistMeter.MoisturePercentage);
                compressedMoistMeter.DateTime = data.Date;
                await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
            }
        }

        private async void CreateXamlTable()
        {
            List<MoistMeter> moistureData = await App.MoistMeterDatabase.GetItemsAsync();

            TableView Table = new TableView();
            Table.Intent = TableIntent.Settings;

            StackLayout Layout = new StackLayout() { Orientation = StackOrientation.Vertical };

            foreach (MoistMeter extractedData in moistureData)
            {
                Layout.Children.Add(new Label()
                {
                    Text = extractedData.MoisturePercentage,
                });
                Layout.Children.Add(new Label()
                {
                    Text = $"{extractedData.DateTime}",
                });
            }
            Table.Root = new TableRoot()
            {
                new TableSection("Ground moisture history")
                {
                    new ViewCell() {View = Layout}
                }
            };
            Content = Table;
        }
    }
}