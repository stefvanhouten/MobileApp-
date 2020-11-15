using System;
using System.Text;
using Xamarin.Forms;
using MQTTnet.Client.Options;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MQTTnet;
using MobileApp.Models;
using System.Threading.Tasks;

namespace MobileApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private string _isConnected;
        private ObservableCollection<string> _messages;

        public Command<string> PublishCommand { get; }
        public List<Models.IOTButton> IOTButtons { get; set; } = new List<Models.IOTButton>();

        public DashboardViewModel()
        {
            Title = "Dashboard";
            IsConnected = "Disconnected";

            App.Client.Connect();
            App.Client.MessageReceived += NewMessage;
            App.Client.ConnectionStatusChanged += UpdateConnectionStatus;

            PublishCommand = new Command<string>(Publish);
            AddDefaultButtons();
        }

        public string IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MQMessage
        {
            get { return _messages; }
            private set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private async void AddDefaultButtons()
        {
            IOTButtons = await App.IOTDatabase.GetItemsAsync();
            if (IOTButtons.Count == 0)
            {
                GenerateDefaultButtons();
            }
        }

        private async void GenerateDefaultButtons()
        {
            Models.IOTButton coffeeBtn = new Models.IOTButton();
            coffeeBtn.Name = "Coffee";
            coffeeBtn.Topic = "coffee";
            coffeeBtn.Image = "coffee.png";

            await App.IOTDatabase.SaveItemAsync(coffeeBtn);
            //AddDefaultButtons();
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

        private void Publish(string message)
        {
            App.Client.Publish("switches", message);
        }
    }
}