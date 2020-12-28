using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.ViewModels
{
    class GroundMoistureViewModel : BaseViewModel
    {
        public GroundMoistureViewModel(string topic)
        {
            Title = "Ground Moisture";
            App.Client.MQTTMessageStore.GetAllMessagesFromTopic(topic);
        }
    }
}
