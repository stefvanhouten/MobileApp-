using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Services
{
    public interface IMQTT
    {
        MQTTMessageStore MQTTMessageStore { get; set; }
        bool IsClientConnected { get; }

        event Action MessageReceived;
        event Action ConnectionStatusChanged;
        bool HasBeenConnected { get; set; }

        Task<bool> Connect(string IP, int port);
        void Subscribe(string channel);
        Task<bool> Disconnect();
        void Publish(string topic, string message);
    }
}
