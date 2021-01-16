using Microcharts;
using Microcharts.Forms;
using MobileApp.Models;
using MobileApp.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Entry = Microcharts.Entry;

namespace MobileApp.ViewModels
{
    class GroundMoistureViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public string Topic { get; private set; }
        public ChartView ChartName { get; set; }
        public string Date { get; set; }

        public GroundMoistureViewModel(string topic, ChartView chartName = null)
        {
            ChartName = chartName;
            Topic = topic;
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Title = topic.Split('/')[1] + " - " + Date;

            InitCycle(true);   
        }

        public async void InitCycle(bool FirstCall = false)
        {
            //await App.MoistMeterDatabase.EmptyDatabase();
            MQTTMessage latestMessage = App.Client.MQTTMessageStore.GetLatestMessageFromTopic(this.Topic);
            if (latestMessage != null)
            {
                 await SaveEntryToDatabase(latestMessage);
            }
            CreateXamlView();

            if (FirstCall)
            {
                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        InitCycle();
                    });

                    //return true to keep loop going, false to stop timer
                    return true;
                });
            }
        }

        public async Task<int> SaveEntryToDatabase(MQTTMessage latestMessage)
        {
            if(latestMessage == null)
            {
                return 0;
            }

            MoistMeter compressedMoistMeter = new MoistMeter
            {
                Topic = Topic,
                Data = Convert.ToDouble(latestMessage.Message)
            };

            return await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
        }

        private async void CreateXamlView()
        {
            List<MoistMeter> data = await App.MoistMeterDatabase.GetItemByTopicName(Topic);
            List<Entry> Entries = new List<Entry>();
            const string HIGH_DATA_COLOR = "0fdb16";
            const string MIDDLE_DATA_COLOR = "e8a425";
            const string LOW_DATA_COLOR = "e82525";
            double valueLabel;
            string colorHEX = LOW_DATA_COLOR;


            foreach (MoistMeter extractedData in data)
            {
                valueLabel = extractedData.Data;
                if(valueLabel >= 30 && valueLabel <= 60)
                {
                    colorHEX = MIDDLE_DATA_COLOR;
                }else if(valueLabel > 60)
                {
                    colorHEX = HIGH_DATA_COLOR;
                }

                string time = string.Format(extractedData.DateTime.ToShortTimeString());
                Entries.Add(new Entry(float.Parse(Convert.ToString(valueLabel)))
                {
                    Color = SKColor.Parse(colorHEX),
                    ValueLabel = $"{valueLabel}",
                    Label = time,
                });
            };

            ChartName.Chart =  new LineChart 
            { 
                Entries = Entries,
                LabelTextSize = 35,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 18,
            };
        }
    }
}