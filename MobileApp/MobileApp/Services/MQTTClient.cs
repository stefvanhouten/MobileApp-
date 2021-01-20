using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.ClientLib;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Attempts to establish a password secured connection with a MQTT broker based on provided credentials.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <returns>Boolean based on if the connection was succesful or not</returns>
        public async Task<bool> Connect(string IP, int port, string password)
        {
            //This initialises the connection with our MQTT broker. Values are hardcoded atm but this should be changed
            //so that our Connect method at least takes a IP address
            this.MQTTMessageStore = new MQTTMessageStore();
            MqttService.MqttClient.Init("XamarinMobileClient", new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                  .WithCredentials("MobileClient", password)
                                                                                                                  .WithCleanSession(true)
                                                                                                                  .WithTcpServer(IP, port)
                                                                                                                  .Build());
            this.AttachEventListeners();
            //This property is used to prevent an endless reconnecting loop on unsecessful connection. 
            this.initialConnectSuccess = await MqttService.MqttClient.Connect();
            this.HasBeenConnected = true;
            return MqttService.MqttClient.IsConnected();
        }

        /// <summary>
        /// Attaches our custom methods to the most important events raised by the MqttService. 
        /// </summary>
        private void AttachEventListeners()
        {
            //If we have been connected before we want to detach the old event listeners and attach the new ones after.
            if (HasBeenConnected)
            {
                this.DetachEventListeners();
            }
            MqttService.MqttClient.Connected += MqttClient_Connected; //Binds our MqttClient_Connected when MqttService.MqttClient.Connected event is fired
            MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
        }

        /// <summary>
        /// Detaches our custom methods.
        /// </summary>
        private void DetachEventListeners()
        {
            MqttService.MqttClient.Connected -= MqttClient_Connected;
            MqttService.MqttClient.MessageReceived -= MqttClient_MessageReceived;
            MqttService.MqttClient.Disconnected -= MqttClient_Disconnected;
        }

        /// <summary>
        /// Writes a message to our MQTTMessageStore to preserve a log of previous messages. 
        /// Invokes the MessageReceived event to alert all listeners that a new message has come in. 
        /// </summary>
        /// <param name="message"></param>
        protected override void WriteLog(MQTTMessage message)
        {
            //Only invoke the event when the message is added to the store
            if (this.MQTTMessageStore.AddMessage(message))
            {
                MessageReceived?.Invoke();
            }
        }

        /// <summary>
        /// Subscribes to all the topics that we are interested in when a connection with the MQTT broker is established.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            //We defaulty listen to the switches channel. Will want to change this later depending on added buttons and such
            this.UpdateConnectionStatus();
            List<string> topicsToSubscribeTo = new List<string>() { 
                "Coffee", 
                "WateringSystem", 
                "WateringSystem/Status", 
                "WateringSystem/Feedback", 
                "Plant/Temperature", 
                "Plant/Moisture", 
                "Plant/Humidity",
                "Coffee/Status"

            };
            foreach (string topic in topicsToSubscribeTo)
            {
                this.Subscribe(topic);
            }
        }

        /// <summary>
        /// Subscribe to a topic. 
        /// </summary>
        /// <param name="channel"></param>
        public async void Subscribe(string channel)
        {
            _ = await MqttService.MqttClient.Subscribe(channel, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        /// <summary>
        /// Handles reconnecting(or not) whenever a client disconnects. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Disconnect the client from the MQTT session. 
        /// </summary>
        /// <returns>Boolean that represents the clients connection status.</returns>
        public async Task<bool> Disconnect()
        {
            this.ForceDisconnect = true;
            await MqttService.MqttClient.Disconnect();
            this.UpdateConnectionStatus();
            return MqttService.MqttClient.IsConnected();
        }

        /// <summary>
        /// Updates the current status of the MQTT client. Invokes an update to let all listeners know that the status has changed. 
        /// </summary>
        protected override void UpdateConnectionStatus()
        {
            this.IsClientConnected = MqttService.MqttClient.IsConnected();
            this.ConnectionStatusChanged?.Invoke();
        }

        /// <summary>
        /// Catches the incomming MQTTMessage and writes it to the MQTTMessageStore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            MQTTMessage message = new MQTTMessage(e.ApplicationMessage.Topic, 
                                                  Encoding.UTF8.GetString(e.ApplicationMessage.Payload), 
                                                  DateTime.Now);
            this.WriteLog(message);
        }

        /// <summary>
        /// Publish a message on a given topic. 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public async void Publish(string topic, string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(message))
                await MqttService.MqttClient.Publish(new MqttApplicationMessage
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetBytes(message)
                }); ;
        }
    }
}
