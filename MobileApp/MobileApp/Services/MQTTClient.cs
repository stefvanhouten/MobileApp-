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
        public ObservableCollection<string> MQMessage { get; set; } //Stores all incoming messages from the MQTT broker
        public bool IsClientConnected { get; private set; } = false;

        public event Action MessageReceived; //Fired when a message is recieved
        public event Action ConnectionStatusChanged; //Fired when connectionstatus is changed, DashBoardViewModel listens to these events

        public MQTTClient()
        {
            MQMessage = new ObservableCollection<string>();
        }

        public async void Connect()
        {
            //This initialises the connection with our MQTT broker. Values are hardcoded atm but this should be changed
            //so that our Connect method at least takes a IP address
            MQTTnet.ClientLib.MqttService.MqttClient.Init("XamarinMobileClient", new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                  .WithCleanSession(true)
                                                                                                                  .WithTcpServer(App.ServerIP, App.ServerPort)
                                                                                                                  .Build());
            MQTTnet.ClientLib.MqttService.MqttClient.Connected += MqttClient_Connected; //Binds our MqttClient_Connected when MqttService.MqttClient.Connected event is fired
            MQTTnet.ClientLib.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MQTTnet.ClientLib.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
            /*  Since we are dealing with asynchronous actions we need to wait for this to be done.
             *  Otherwise we will get unexpected behaviour which is not good.
             *  
             *  Asynchronous actions will be done on another thread meaning:
             *  - Code below will be executed before the async method is done
             *  
             *  for example:
             *  int x = someAsyncMethod();
             *  x += 5;
             *  
             *  This code will crash because the value of x is not set yet. To fix this we do the following:
             *  int x = await someAsyncMethod();
             *  x += 5;
             *  
             *  Now we will wait for someAsyncMethod() to finish and then we will continue.
             *  Sometimes you don't want to wait because what the method is doing is not important for now, 
             *  in this case we want to wait though.
             */
            await MQTTnet.ClientLib.MqttService.MqttClient.Connect(); 
        }


        private void WriteLog(string msg)
        {
            MQMessage.Add(msg);
            /*  We only add to this list when we receive a message. For that reason we can invoke the MessageReceived
             *  event so that our listeners(DashBoardViewModel) can act. 
             *  
             *  The ? in this method is an Elvis operator. 
             *  Basicly what it does is checking if MessageReceived is null, if not then invoke.
             */
            MessageReceived?.Invoke(); 
        }

        private async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            //We want to update the status of our application because we have disconnected
            UpdateConnectionStatus();
            //Wait untill we reconnect
            await MQTTnet.ClientLib.MqttService.MqttClient.Reconnect();
            //Update status of the application because we have reconnected
            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            IsClientConnected = MQTTnet.ClientLib.MqttService.MqttClient.IsConnected();
            ConnectionStatusChanged?.Invoke();
        }

        private void MqttClient_MessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            //Builds the structure of our string with the incomming message details
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
            //We defaulty listen to the switches channel. Will want to change this later depending on added buttons and such
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
