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
        public List<MQTTMessage> MQTTMessages { get; set; }
        private List<MoistMeter> DatabaseData { get; set; }
        public GroundMoistureViewModel(string topic)
        {
            Topic = topic;
            Title = "Ground Moisture";

            //get all messages the first time the app starts
            //do not check whether it has been set to null! This is redundant!
            //either a filled observablecollection is send back or an empty observablecollection
            MQTTMessages = App.Client.MQTTMessageStore.GetAllMessagesFromTopic(Topic);

            //init the first cycle for comparison
            initCycle();

            //after the first cycle, clear the List
            //this prevents the second cycle from comparing old data and thus decreases load time
            MQTTMessages.Clear();

            // !!called every 30 minutes -> 1800 seconds!!
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                // get latest message send to the broker
                MQTTMessages.Add(App.Client.MQTTMessageStore.GetLatestMessageFromTopic(Topic));

                //initialize second cycle to update database and view every half an hour
                initCycle();

                // return true to repeat counting, false to stop timer
                return true;
            });
        }

        public async void initCycle()
        {
            //instantiate the first cycle
            GetDatabaseData();

            //compare the first cycle to the messages retrieved
            //insert/skip data
            CompareDatabaseToServer();

            ////prepare second cycle to create a view
            //GetDatabaseData();

            ////create view
            //CreateXamlTable();
        }

        private async void GetDatabaseData()
        {
            DatabaseData = await App.MoistMeterDatabase.GetItemsAsync();
        }

        //compare the retrieved data based on a combination of % and dateTime.
        //if BOTH are set -> duplicate -> do not insert
        private void CompareDatabaseToServer()
        {
            foreach (MQTTMessage serverData in MQTTMessages.ToList())
            {
                int exists = 0;
                for (int i = 0; i < DatabaseData.Count; i++)
                {
                    if (DatabaseData[i].MoisturePercentage == serverData.Message && DatabaseData[i].DateTime == serverData.Date)
                    {
                        exists += 1;
                    }
                }
                if (exists == 0)
                {
                    MQTToDatabase(serverData);
                }
            }
        }

        public async void MQTToDatabase(MQTTMessage data)
        {
            MoistMeter compressedMoistMeter = new MoistMeter();
            compressedMoistMeter.MoisturePercentage = data.Message;
            compressedMoistMeter.DateTime = data.Date;
            await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
        }

        private async void CreateXamlTable()
        {
            TableView table = new TableView();
            table.Intent = TableIntent.Settings;

            StackLayout layout = new StackLayout() { Orientation = StackOrientation.Vertical };

            foreach (MoistMeter extractedData in DatabaseData)
            {
                layout.Children.Add(new Label()
                {
                    Text = extractedData.MoisturePercentage,
                });
                layout.Children.Add(new Label()
                {
                    Text = $"{extractedData.DateTime}",
                });
            }
            table.Root = new TableRoot()
            {
                new TableSection("Ground moisture history")
                {
                    new ViewCell() {View = layout}
                }
            };
            Content = table;
        }
    }
}