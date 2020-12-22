﻿using System;
using System.Text;
using Xamarin.Forms;
using MQTTnet.Client.Options;
using System.Collections.ObjectModel;
using MQTTnet.ClientLib;
using MQTTnet;

namespace MobileApp.Services
{
    public class MQTTClient
    {
        //public ObservableCollection<MQTTMessage> MQMessage { get; set; } //Stores all incoming messages from the MQTT broker
        public MQTTMessageStore MQTTMessageStore { get; set; }
        public bool IsClientConnected { get; private set; } = false;

        public event Action MessageReceived; //Fired when a message is recieved
        public event Action ConnectionStatusChanged; //Fired when connectionstatus is changed, DashBoardViewModel listens to these events
        public bool HasBeenConnected { get; set; } = false;
        public bool ForceDisconnect { get; set; } = false;

        public async void Connect(string IP, int port)
        {
            //This initialises the connection with our MQTT broker. Values are hardcoded atm but this should be changed
            //so that our Connect method at least takes a IP address
            this.MQTTMessageStore = new MQTTMessageStore();
            MqttService.MqttClient.Init("XamarinMobileClient", new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                  .WithCleanSession(true)
                                                                                                                  .WithTcpServer(IP, port)
                                                                                                                  .Build());
            this.AttachEventListeners();
            _ = await MqttService.MqttClient.Connect();
            this.HasBeenConnected = true;
        }


        private void AttachEventListeners()
        {
            if (HasBeenConnected)
            {
                this.DetachEventListeners();
            }
            MqttService.MqttClient.Connected += MqttClient_Connected; //Binds our MqttClient_Connected when MqttService.MqttClient.Connected event is fired
            MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
        }

        private void DetachEventListeners()
        {
            MqttService.MqttClient.Connected -= MqttClient_Connected;
            MqttService.MqttClient.MessageReceived -= MqttClient_MessageReceived;
            MqttService.MqttClient.Disconnected -= MqttClient_Disconnected;
        }

        private void WriteLog(MQTTMessage message)
        {
            //Only invoke the event when the message is added to the store
            if (this.MQTTMessageStore.AddMessage(message))
            {
                MessageReceived?.Invoke();
            }
        }

        private void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            //We defaulty listen to the switches channel. Will want to change this later depending on added buttons and such
            UpdateConnectionStatus();
            Subscribe("switches");
            Subscribe("wateringSystemFeedback");
        }

        private async void Subscribe(string channel)
        {
            _ = await MqttService.MqttClient.Subscribe(channel, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            UpdateConnectionStatus();
            if (!this.ForceDisconnect)
            {
                _ = await MqttService.MqttClient.Reconnect();
                UpdateConnectionStatus();
            }
            this.ForceDisconnect = false;
        }

        public void Disconnect()
        {
            this.ForceDisconnect = true;
            MqttService.MqttClient.Disconnect();
        }


        private void UpdateConnectionStatus()
        {
            IsClientConnected = MqttService.MqttClient.IsConnected();
            ConnectionStatusChanged?.Invoke();
        }

        private void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            MQTTMessage message = new MQTTMessage(e.ApplicationMessage.Topic, 
                                                  Encoding.UTF8.GetString(e.ApplicationMessage.Payload), 
                                                  DateTime.Now);
            WriteLog(message);
        }


        public async void Publish(string topic, string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(message))
                await MqttService.MqttClient.Publish(new MqttApplicationMessage
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
