using MQTTnet;
using System;

namespace MobileApp.Services
{
    public abstract class MQTTBase
    {
        protected abstract void WriteLog(MQTTMessage message);
        protected abstract void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e);
        protected abstract void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e);
        protected abstract void UpdateConnectionStatus();
        protected abstract void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e);
    }
}
