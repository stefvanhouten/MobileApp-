using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Services
{
    public class MQTTMockClient : MQTTBase, IMQTT
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

        public async Task<bool> Connect(string IP, int port)
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
            MQTTMessage newMessage = new MQTTMessage(topic,
                                                  message,
                                                  DateTime.Now);
            this.WriteLog(newMessage);
        }

        public void Subscribe(string channel)
        {
            Random rng = new Random();
            if (!HasBeenConnected)
            {
                if (channel == "wateringSystemFeedback")
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage("wateringSystemFeedback",
                                                      rng.Next(1, 100).ToString(),
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                }
                if(channel == "coffee")
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        string payload = "ON";
                        if (rng.Next(0, 2) == 0)
                        {
                            payload = "OFF";
                        }
                        this.WriteLog(new MQTTMessage("coffee",
                                                      payload,
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
                }
                if(channel == "switches")
                {
                    System.Threading.Timer timer = new System.Threading.Timer((e) =>
                    {
                        this.WriteLog(new MQTTMessage("switches",
                                                      "somerandommessage",
                                                      DateTime.Now));
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
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
            this.Subscribe("switches");
            this.Subscribe("coffee");
            this.Subscribe("wateringSystemFeedback");
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
