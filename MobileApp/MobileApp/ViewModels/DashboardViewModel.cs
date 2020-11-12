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
        public ObservableCollection<string> MQMessage
        {
            get { return _messages; }
            private set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private string _isConnected;
        public string IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public Command PublishCommand { get; }


        public DashboardViewModel()
        {
            Title = "Dashboard";
            IsConnected = "Disconnected";

            App.Client.Connect();
            App.Client.MessageReceived += NewMessage;
            App.Client.ConnectionStatusChanged += UpdateConnectionStatus;

            PublishCommand = new Command(Publish);
        }

        private void UpdateConnectionStatus()
        {
            if (App.Client.IsClientConnected)
            {
                IsConnected = "Connected";
                return;
            }
            IsConnected = "Disconnected";
        }

        private void NewMessage()
        {
            this.MQMessage = App.Client.MQMessage;
        }

        private void Publish()
        {
            App.Client.Publish("switches", $"Hello from xamarin");
        }
    }
}