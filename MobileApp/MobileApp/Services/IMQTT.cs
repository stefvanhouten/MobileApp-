using System;
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

        Task<bool> Connect(string IP, int port, string password);
        void Subscribe(string channel);
        Task<bool> Disconnect();
        void Publish(string topic, string message);
    }
}
