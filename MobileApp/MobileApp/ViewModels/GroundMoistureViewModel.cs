using MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MobileApp.Services;
using Xamarin.Forms;
using Entry = Microcharts.Entry;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SkiaSharp;
using Microcharts.Forms;
using Microcharts;

namespace MobileApp.ViewModels
{
    class GroundMoistureViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public string Topic { get; private set; }
        public List<MQTTMessage> MQTTMessages { get; set; }
        private List<MoistMeter> DatabaseData { get; set; }
        public List<Entry> Entries { get; set; }
        public ChartView ChartName { get; set; }

        Dictionary<string, string> DynamicTitles = new Dictionary<string, string>
        {
            {"Plant/Moisture", "Ground Moisture" },
            {"Plant/Temperature", "Temperature"},
        };

        public GroundMoistureViewModel(string topic, ChartView chartName = null)
        {
            ChartName = chartName;
            Topic = topic;
            if (DynamicTitles.ContainsKey(topic))
            {
                Title = DynamicTitles[topic];
            }

            DatabaseData = new List<MoistMeter>();
            MQTTMessages = new List<MQTTMessage>();
            Entries = new List<Entry>();

            initCycle();

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    // get latest message send to the broker
                    MQTTMessage msg = App.Client.MQTTMessageStore.GetLatestMessageFromTopic(Topic);
                    if (msg != null)
                    {
                        MQTTMessages.Add(msg);
                    }

                    //initialize second cycle to update database and view every half an hour
                    initCycle(true);

                    //Clear the List
                    //this prevents the cycle from comparing old data and thus decreases load time
                    MQTTMessages.Clear();
                });

                //return true to keep loop going, false to stop timer
                return true;
            });
        }

        public void GetLatestMessage()
        {
            // get latest message send to the broker
            MQTTMessage msg = App.Client.MQTTMessageStore.GetLatestMessageFromTopic(Topic);
            if (msg != null)
            {
                MQTTMessages.Add(msg);
            }
        }

        public async void initCycle(bool cycle = false)
        {
           // await App.MoistMeterDatabase.EmptyDatabase();
            List<MoistMeter> data = await App.MoistMeterDatabase.GetItemByColumnAsync(Topic);

            if (data != null)
            {
                DatabaseData = data;
            }

            if (!cycle)
            {
                //create view
                CreateXamlView();
            } else if (cycle)
            {
                //compare the retrieved data against the database
                //this prevents duplicates
                CompareDatabaseToServer();

                //create view
                CreateXamlView();

            }

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
                    if (DatabaseData[i].MoisturePercentage == Convert.ToDouble(serverData.Message) && DatabaseData[i].DateTime == serverData.Date)
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
            compressedMoistMeter.Target = Topic;
            switch (Topic)
            {
                case "Plant/Moisture":
                    compressedMoistMeter.MoisturePercentage = Convert.ToDouble(data.Message);
                    compressedMoistMeter.DateTime = data.Date;
                    break;
                case "Plant/Temperature":
                    compressedMoistMeter.Temperature = Convert.ToDouble(data.Message);
                    compressedMoistMeter.DateTime = data.Date;
                    break;
                case "Plant/Humidity":
                    compressedMoistMeter.Humidity = Convert.ToDouble(data.Message);
                    compressedMoistMeter.DateTime = data.Date;
                    break;
            }
            if (data != null)
            {
                await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
            }
        }

        private async void CreateXamlView()
        {
            //TAGS FOR ENTRY
            double valueLabel = 0;
            string colorHEX = "";
            //#0fdb16 GREEN HIGH MOISTURE
            //#e8a425 //ORANGE MIDDLE MOISTURE
            //#e82525 //RED LOW MOISTURE

            foreach (MoistMeter extractedData in DatabaseData)
            {
                switch (Topic)
                {
                    case "Plant/Moisture":
                        valueLabel = extractedData.MoisturePercentage;
                        break;
                    case "Plant/Temperature":
                        valueLabel = extractedData.Temperature;
                        break;
                    case "Plant/Humidity":
                        valueLabel = extractedData.Humidity;
                        break;
                }

                if (valueLabel < 30)
                {
                    colorHEX = "#e82525";
                } else if (valueLabel > 40 && valueLabel < 60)
                {
                    colorHEX = "#e8a425";
                } else 
                {
                    colorHEX = "#0fdb16";
                }

                Entries.Add(new Entry(float.Parse(Convert.ToString(valueLabel)))
                {
                    Color = SKColor.Parse(colorHEX),
                    ValueLabel = $"{valueLabel}",
                    Label = $"{extractedData.DateTime}",
                });
            };
            ChartName.Chart = new LineChart 
            { 
                Entries = Entries,
                LabelTextSize = 35,

            };
        }
    }
}