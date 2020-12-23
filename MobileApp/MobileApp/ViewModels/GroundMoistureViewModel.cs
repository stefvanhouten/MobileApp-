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
        public ObservableCollection<MoistMeter> MoistureData { get; private set; } = new ObservableCollection<MoistMeter>();
        public GroundMoistureViewModel(string topic)
        {
            Topic = topic;
            Title = "Ground Moisture";

            // !!called every 30 minutes -> 1800 seconds!!
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                // get all messages send to MQTT server
                MQTTMessages = App.Client.MQTTMessageStore.GetAllMessagesFromTopic(Topic);

                if (MQTTMessages == null)
                {
                    MQTTMessages = new ObservableCollection<MQTTMessage>();
                }

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
            DatabaseToApp();
        }

        public async void MQTToDatabase()
        {
            foreach(MQTTMessage data in MQTTMessages)
            {
                MoistMeter compressedMoistMeter = new MoistMeter();
                compressedMoistMeter.MoisturePercentage = data.Message;
                compressedMoistMeter.DateTime = data.Date;
                await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
            }
        }

        public async void DatabaseToApp()
        {
            List<MoistMeter> moistureData = await App.MoistMeterDatabase.GetItemsAsync();
            if (moistureData.Any())
            {
                foreach (MoistMeter singleDataSet in moistureData)
                {
                    MoistureData.Add(singleDataSet);
                }
            }
        }
    }
}