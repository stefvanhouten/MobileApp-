using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MobileApp.Models;

namespace MobileApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private string _isConnected; //Both these fields have getters/setters below. This is because XAMARIN is a bitch.
        private ObservableCollection<string> _messages;

        public Command<string> PublishCommand { get; }
        public List<IOTButton> IOTButtons { get; set; } = new List<IOTButton>();

        //This is the constructor of our class
        public DashboardViewModel()
        {
            //Set the Title property, this is inherited from BaseViewModel
            Title = "Dashboard";
            IsConnected = "Disconnected";

            /*  App is globally accessible. It refers to the App.xaml.cs file and this is where we stored some
             *  important static properties. In this case we want to connect to our MQTT broker(server) when
             *  DashboardViewModel is instantiated.
            */
            App.Client.Connect();
            /* The Client(Services/MQTTClient) triggers a few Events when something important happens,
             * one of these events is MessageReceived. When this event is triggered we want to call 
             * our method from this class and handle what will happen. In this case we display the message
             * but this is for development purposes only.
             */
            App.Client.MessageReceived += NewMessage;
            //Same as the above, but this time when we disconnect/connect to/from the server update our page.
            App.Client.ConnectionStatusChanged += UpdateConnectionStatus;
            //Bind the Publish method to the PublishCommand property which is called from DasBoardPage.xaml button click
            PublishCommand = new Command<string>(Publish);
        }

        public string IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                OnPropertyChanged(); //This is very important, without XAMARIN doesn't know to refresh the UI and old values will not be updated
            }
        }

        public ObservableCollection<string> MQMessage
        {
            get { return _messages; }
            private set
            {
                _messages = value;
                OnPropertyChanged(); //This is very important, without XAMARIN doesn't know to refresh the UI and old values will not be updated
            }
        }

        private void UpdateConnectionStatus()
        {
            if (App.Client.IsClientConnected)
            {
                IsConnected = "Connected";
            }
            else
            {
                IsConnected = "Disconnected";
            }
        }

        private void NewMessage()
        {
            /*  This sets the MQMessage list that is stored in Client to our class.
             *  We do this because we might want to mutate the list here but we might want to have access
             *  to the full log in the MQTTClient class.
             */
            this.MQMessage = App.Client.MQMessage;
        }

        private void Publish(string message)
        {
            //Publish a message to the switches channel. This shouldn't be hardcoded and both should take a variable.
            App.Client.Publish("wateringSystem", message);
        }
    }
}