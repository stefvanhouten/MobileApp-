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
        public Grid GridLayout { get; set; }
        public ChartView ChartName { get; set; }
        public GroundMoistureViewModel(string topic, ChartView chartName = null)
        {
            ChartName = chartName;
            Topic = topic;
            Title = "Ground Moisture";
<<<<<<< HEAD
            DatabaseData = new List<MoistMeter>();
            MQTTMessages = new List<MQTTMessage>();
            Entries = new List<Entry>();

            initCycle();

            Device.StartTimer(TimeSpan.FromSeconds(1800), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    // get latest message send to the broker
                    MQTTMessages.Add(App.Client.MQTTMessageStore.GetLatestMessageFromTopic(Topic));

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

        public async void initCycle(bool cycle = false)
        {
            List<MoistMeter> data = await App.MoistMeterDatabase.GetItemsAsync();

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
            compressedMoistMeter.Target = Topic;
            switch (Topic)
            {
                case "wateringSystemFeedback":
                    compressedMoistMeter.MoisturePercentage = Convert.ToDouble(data.Message);
                    compressedMoistMeter.DateTime = data.Date;
                    break;
                case "test":
                    break;
            }

            await App.MoistMeterDatabase.SaveItemAsync(compressedMoistMeter);
        }

        private async void CreateXamlView()
        {
            Grid gridLayout = new Grid();
            int rowIndex = 0;
            int columnIndex = 0;

            foreach (MoistMeter extractedData in DatabaseData)
            {
                Entries.Add(new Entry(float.Parse(Convert.ToString(extractedData.MoisturePercentage)))
                {
                    Color = SKColor.Parse("#104ce3"),
                    ValueLabel = $"{extractedData.DateTime}",
                });

                gridLayout.RowDefinitions.Add(new RowDefinition());
                gridLayout.ColumnDefinitions.Add(new ColumnDefinition());
                gridLayout.ColumnDefinitions.Add(new ColumnDefinition());

                gridLayout.Children.Add(new Label()
                {
                    Text = $"{extractedData.MoisturePercentage}%",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                }, columnIndex, rowIndex);

                columnIndex++;

                gridLayout.Children.Add(new Label()
                {
                    Text = $"{extractedData.DateTime}",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                }, columnIndex, rowIndex);

                columnIndex++;
                rowIndex++;
            }
            //GridLayout = GridLayout;
            ChartName.Chart = new LineChart { Entries = Entries };
=======
            App.Client.MQTTMessageStore.GetAllMessagesFromTopic(topic);
>>>>>>> origin/boris
        }
    }
}