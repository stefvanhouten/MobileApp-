﻿using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Services
{
    public sealed class MQTTMockClient : MQTTBase, IMQTT
    {
        public MQTTMessageStore MQTTMessageStore { get; set; }
        public bool IsClientConnected { get; private set; } = false;

        public event Action MessageReceived; //Fired when a message is recieved
        public event Action ConnectionStatusChanged; //Fired when connectionstatus is changed, DashBoardViewModel listens to these events
        public bool HasBeenConnected { get; set; } = false;

        public MQTTMockClient()
        {
            this.MQTTMessageStore = new MQTTMessageStore();
        }

        public async Task<bool> Connect(string IP, int port, string password)
        {
            this.MQTTMessageStore = new MQTTMessageStore();
            await Task.Delay(1000);
            this.MqttClient_Connected(this, null);
            this.HasBeenConnected = true;
            return true;
        }

        public async Task<bool> Disconnect()
        {
            this.IsClientConnected = false;
            this.UpdateConnectionStatus();
            return true;
        }

        public void Publish(string topic, string message)
        {
            if(topic == "WateringSystem")
            {
                topic = "WateringSystem/Status";
            }
            if(topic == "cmnd/coffee/POWER")
            {
                topic = "Coffee/Status";
            }
            MQTTMessage newMessage = new MQTTMessage(topic,
                                                  message,
                                                  DateTime.Now);
            this.WriteLog(newMessage);
        }

        public void Subscribe(string channel)
        {
            const string COFFEE = "Coffee/Status";
            const string WATERING_SYSTEM_STATUS = "WateringSystem/Status";
            const string WATERING_SYSTEM_FEEDBACK = "WateringSystem/Feedback";
            const string PLANT_TEMPERATURE = "Plant/Temperature";
            const string PLANT_HUMIDITY = "Plant/Humidity";
            const string PLANT_MOISTURE = "Plant/Moisture";

            Random rng = new Random();
            if (!HasBeenConnected)
            {
                if(channel == COFFEE)
                {
                    string payload = "OFF";
                    this.WriteLog(new MQTTMessage(COFFEE,
                                                    payload,
                                                    DateTime.Now));
                }
                if (channel == WATERING_SYSTEM_STATUS)
                {
                    string payload = "OFF";
                    this.WriteLog(new MQTTMessage(WATERING_SYSTEM_STATUS,
                                                    payload,
                                                    DateTime.Now));
                }
                if (channel == WATERING_SYSTEM_FEEDBACK)
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage("switches",
                                                      "somerandommessage",
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
                }
                if (channel == PLANT_TEMPERATURE)
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage(PLANT_TEMPERATURE,
                                                      rng.Next(18, 25).ToString(),
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
                if (channel == PLANT_HUMIDITY)
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage(PLANT_HUMIDITY,
                                                      rng.Next(40, 70).ToString(),
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
                if (channel == PLANT_MOISTURE)
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage(PLANT_MOISTURE,
                                                      rng.Next(40, 70).ToString(),
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
            }
        }

        protected override void WriteLog(MQTTMessage message)
        {
            if (this.MQTTMessageStore.AddMessage(message))
            {
                MessageReceived?.Invoke();
            }
        }

        protected override void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            this.UpdateConnectionStatus();
            List<string> topicsToSubscribeTo = new List<string>() { "Coffee/Status", "WateringSystem", "WateringSystem/Status", "WateringSystem/Feedback", "Plant/Temperature", "Plant/Moisture", "Plant/Humidity" };
            foreach (string topic in topicsToSubscribeTo)
            {
                this.Subscribe(topic);
            }
        }

        protected override void MqttClient_Disconnected(object sender, MqttClientDisconnectedEventArgs e) => throw new NotImplementedException();
        protected override void UpdateConnectionStatus()
        {
            this.IsClientConnected = true;
            this.ConnectionStatusChanged?.Invoke();
        }
        protected override void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            MQTTMessage message = new MQTTMessage(e.ApplicationMessage.Topic,
                                                  Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                                                  DateTime.Now);
            this.WriteLog(message);
        }
    }
}
