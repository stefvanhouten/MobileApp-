using System;
using System.Text;
using Xamarin.Forms;
using MQTTnet.Client.Options;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MQTTnet;


namespace MobileApp.Services
{
    public class MQTTClient
    {
        public ObservableCollection<string> MQMessage { get; set; }
        public bool IsClientConnected { get; private set; } = false;

        public event Action MessageReceived;
        public event Action Disconnected;
        public event Action ConnectionStatusChanged;

        public MQTTClient()
        {
            MQMessage = new ObservableCollection<string>();
        }

        public async void Connect()
        {
            MQTTnet.ClientLib.MqttService.MqttClient.Init("lens_qIgokVUiNGrI8ym3kK0yMkhoowK", new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                  .WithCleanSession(true)
                                                                                                                  .WithTcpServer("192.168.1.71", 1883)
                                                                                                                  .Build());
            MQTTnet.ClientLib.MqttService.MqttClient.Connected += MqttClient_Connected;
            MQTTnet.ClientLib.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MQTTnet.ClientLib.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
            await MQTTnet.ClientLib.MqttService.MqttClient.Connect();
        }


        private void WriteLog(string msg)
        {
            MQMessage.Insert(0, msg);
            MessageReceived?.Invoke();
        }

        private async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            UpdateConnectionStatus();
            if (e.ClientWasConnected)
            {
                Disconnected?.Invoke();
            }
            await MQTTnet.ClientLib.MqttService.MqttClient.Reconnect();
            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            IsClientConnected = MQTTnet.ClientLib.MqttService.MqttClient.IsConnected();
            ConnectionStatusChanged?.Invoke();
        }

        private void MqttClient_MessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            if (MQMessage.Count > 10)
            {
                MQMessage.Clear();
            }
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT MessageReceived!");
            str.AppendLine($"Content Type : {e.ApplicationMessage.ContentType}");
            str.AppendLine($"Topic : { e.ApplicationMessage.Topic}");
            str.AppendLine($"Time : { DateTime.Now }");
            str.AppendLine($"Payload : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            WriteLog(str.ToString());

        }

        private void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            UpdateConnectionStatus();
            Subscribe("switches");
        }

        private async void Subscribe(string channel)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Subscribe(channel, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        public async void Publish(string topic, string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(message))
                await MQTTnet.ClientLib.MqttService.MqttClient.Publish(new MqttApplicationMessage
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetBytes(message)
                }); ;
        }

        //private async void UnsubscribeButton_Clicked(object sender, EventArgs e)
        //{
        //    await MQTTnet.ClientLib.MqttService.MqttClient.Unsubscribe(TopicEntry.Text);

        //}
    }
}
