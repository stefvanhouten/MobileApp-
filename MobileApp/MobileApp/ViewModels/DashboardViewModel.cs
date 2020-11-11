using System;
using System.Text;
using Xamarin.Forms;
using MQTTnet.Client.Options;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MQTTnet;

namespace MobileApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private ObservableCollection<string> _messages;
        public ObservableCollection<string> MQMessage { 
            get { return _messages; } 
            set {
                _messages = value; 
                OnPropertyChanged(); 
            } 
        }

        string test = string.Empty;
        public string Test
        {
            get { return test; }
            set { SetProperty(ref test, value); }
        }

        public Command ConnectCommand { get; }
        public Command PublishCommand { get; }


        public DashboardViewModel()
        {
            ConnectCommand = new Command(Connect);
            PublishCommand = new Command(Publish);
            Title = "Dashboard";
            Test = "Test";
            MQMessage = new ObservableCollection<string>();
            WriteLog("HALLO WORLD");
        }

        private async void Connect()
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
            Device.BeginInvokeOnMainThread(() =>
            {
                MQMessage.Add(msg);
            });

        }
        private async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT Disconnected");
            str.AppendLine($"Client connected : {e.ClientWasConnected}");
            str.AppendLine($"Exception Message : {e.Exception?.Message}");
            WriteLog(str.ToString());
            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(async (s) =>
            {
                await MQTTnet.ClientLib.MqttService.MqttClient.Reconnect();
            });
        }

        private void MqttClient_MessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            MQMessage.Clear();
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT MessageReceived!");
            str.AppendLine($"Content Type : {e.ApplicationMessage.ContentType}");
            str.AppendLine($"Topic : { e.ApplicationMessage.Topic}");
            str.AppendLine($"Payload : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            WriteLog(str.ToString());
        }

        private void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT Connected!");
            str.AppendLine($"Result code : {e.AuthenticateResult.ResultCode}");
            WriteLog(str.ToString());
            Subscribe("switches");
        }

        private async void Subscribe(string channel)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Subscribe(channel, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async void Publish()
        {
            string topic = "switches";
            string message = "Hello from xamerin";

            if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(message))
                await MQTTnet.ClientLib.MqttService.MqttClient.Publish(new MqttApplicationMessage
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetBytes(message)
                });;
        }

        //private async void UnsubscribeButton_Clicked(object sender, EventArgs e)
        //{
        //    await MQTTnet.ClientLib.MqttService.MqttClient.Unsubscribe(TopicEntry.Text);

        //}

        protected bool SetProperty<T>(ref T backingStore, T value,
           [CallerMemberName] string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}