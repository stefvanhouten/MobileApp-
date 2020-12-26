using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.Services
{
    public interface IMQTT
    {
        MQTTMessageStore MQTTMessageStore { get; set; }
        bool IsClientConnected { get; }

        event Action MessageReceived;
        event Action ConnectionStatusChanged;
        bool HasBeenConnected { get; set; }

        void Connect(string IP, int port);
        void Subscribe(string channel);
        void Disconnect();
        void Publish(string topic, string message);
    }
}
