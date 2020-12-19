using MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MobileApp.Services;

namespace MobileApp.ViewModels
{
    class GroundMoistureViewModel : BaseViewModel
    {
        public MoistMeter CompressedMoistMeter { get; set; }
        public ObservableCollection<MQTTMessage> MQTTMessages { get; set; } 
        public GroundMoistureViewModel(string topic)
        {
            Title = "Ground Moisture";
            App.Client.GetAllMessagesFromTopic(topic);
            CompressedMoistMeter = new MoistMeter();
            CompressedMoistMeter.Moisture = "sum";
        }
    }
}
