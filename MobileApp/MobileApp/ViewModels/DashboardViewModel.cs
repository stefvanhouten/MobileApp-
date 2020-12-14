using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MobileApp.Models;
using MobileApp.Data;
using System;
using MobileApp.Views;
using System.Threading.Tasks;
using System.ComponentModel;
using MobileApp.CustomElement;

namespace MobileApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _isConnected; //Both these fields have getters/setters below. This is because XAMARIN is a bitch. (I approve of this comment)
        private ObservableCollection<string> _messages;
        public Command<string> PublishCommand { get; private set; }
        public static List<IOTButton> IOTButtons { get; set; } = new List<IOTButton>();
        public ObservableCollection<Button> CompletedButtons { get; set; } = new ObservableCollection<Button>();
        public Command ShowNewView { get; private set; }
        public Command ShowCmsView { get; private set; }

        //This is the constructor of our class
        public DashboardViewModel()
        {
            InitializeAsync();
        }

        //warning: async void! 
        public async void InitializeAsync()
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

            ButtonCreationViewModel.IOTButtonsDatabaseUpdated += BuildDynamicButtons;
            //Bind the Publish method to the PublishCommand property which is called from DasBoardPage.xaml button click
            PublishCommand = new Command<string>(Publish);

            //a reference to redirect to the page on which buttons are generated
            ShowNewView = new Command(NavigateToButtonCreationPage);
            ShowCmsView = new Command<CustomButton>(NavigateToButtonActivator);

            //retrieve database rows and build buttons based on retrieved information
            BuildDynamicButtons();
        }

        //helper method
        //can be set to static, since it should not create a new instance for buttons
        public async void BuildDynamicButtons()
        {
            //wait until all data has been received
            IOTButtons = await App.IOTDatabase.GetItemsAsync();

            CompletedButtons.Clear();

            //instantiate buttion variable for the generated buttons
            CustomButton button;
            Button redirectButton;

            //loop through all button properties and read it
            foreach (IOTButton buttonProperties in IOTButtons)
            {
                //generate the button with the properties
                button = new CustomButton();
                button.CustomID = buttonProperties.ID;
                button.Text = $"{buttonProperties.Name}";
                button.Command = PublishCommand;
                button.CommandParameter = buttonProperties.Topic;
                button.HeightRequest = 75;

                redirectButton = new Button
                {
                    Text = buttonProperties.Name,
                    Command = ShowCmsView,
                    CommandParameter = button,
                    HeightRequest = 75,
                };
                //add the button to an ObservableCollection of buttons
                CompletedButtons.Add(redirectButton);
            }
        }

        public void NavigateToButtonCreationPage()
        {
            Application.Current.MainPage.Navigation.PopAsync();
            Application.Current.MainPage.Navigation.PushAsync(new ButtonCreationPage(), true);
        }

        public void NavigateToButtonActivator(CustomButton button)
        {
            Application.Current.MainPage.Navigation.PopAsync();
            Application.Current.MainPage.Navigation.PushAsync(new ButtonUsagePage(button), true);
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
            App.Client.Publish("switches", message);
        }
    }
}