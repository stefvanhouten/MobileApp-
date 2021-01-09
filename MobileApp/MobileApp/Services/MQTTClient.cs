using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.ClientLib;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Services
{
    public sealed class MQTTClient : MQTTBase, IMQTT
    {
        public MQTTMessageStore MQTTMessageStore { get; set; }
        public bool IsClientConnected { get; private set; } = false;

        public event Action MessageReceived; //Fired when a message is recieved
        public event Action ConnectionStatusChanged; //Fired when connectionstatus is changed, DashBoardViewModel listens to these events
        public bool HasBeenConnected { get; set; } = false;
        public bool ForceDisconnect { get; set; } = false;
        public bool initialConnectSuccess { get; set; } = false;

        public MQTTClient()
        {
            this.MQTTMessageStore = new MQTTMessageStore();
        }

        public async Task<bool> Connect(string IP, int port, string password)
        {
            //This initialises the connection with our MQTT broker. Values are hardcoded atm but this should be changed
            //so that our Connect method at least takes a IP address
            this.MQTTMessageStore = new MQTTMessageStore();
            MqttService.MqttClient.Init("XamarinMobileClient", new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                  .WithCredentials("stef", password)
                                                                                                                  .WithCleanSession(true)
                                                                                                                  .WithTcpServer(IP, port)
                                                                                                                  .Build());
            this.AttachEventListeners();
            this.initialConnectSuccess = await MqttService.MqttClient.Connect();
            this.HasBeenConnected = true;
            return MqttService.MqttClient.IsConnected();
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

        protected override void WriteLog(MQTTMessage message)
        {
            //Only invoke the event when the message is added to the store
            if (this.MQTTMessageStore.AddMessage(message))
            {
                MessageReceived?.Invoke();
            }
        }

        protected override void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            //We defaulty listen to the switches channel. Will want to change this later depending on added buttons and such
            this.UpdateConnectionStatus();
            this.Subscribe("Coffee");

            this.Subscribe("WateringSystem");
            this.Subscribe("WateringSystem/Status");
            this.Subscribe("WateringSystem/Feedback");

            this.Subscribe("Plant/Temperature");
            this.Subscribe("Plant/Moisture");
            this.Subscribe("Plant/Humidity");
        }

        public async void Subscribe(string channel)
        {
            _ = await MqttService.MqttClient.Subscribe(channel, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        protected override async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            this.UpdateConnectionStatus();
            if (!this.ForceDisconnect && this.initialConnectSuccess)
            {
                _ = await MqttService.MqttClient.Reconnect();
                this.UpdateConnectionStatus();
            }
            this.ForceDisconnect = false;
        }

        public async Task<bool> Disconnect()
        {
            this.ForceDisconnect = true;
            await MqttService.MqttClient.Disconnect();
            this.UpdateConnectionStatus();
            return MqttService.MqttClient.IsConnected();
        }


        protected override void UpdateConnectionStatus()
        {
            this.IsClientConnected = MqttService.MqttClient.IsConnected();
            this.ConnectionStatusChanged?.Invoke();
        }

        protected override void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            MQTTMessage message = new MQTTMessage(e.ApplicationMessage.Topic, 
                                                  Encoding.UTF8.GetString(e.ApplicationMessage.Payload), 
                                                  DateTime.Now);
            this.WriteLog(message);
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
