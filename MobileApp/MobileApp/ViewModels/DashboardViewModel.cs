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
using MobileApp.Services;

namespace MobileApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _isConnected; //Both these fields have getters/setters below. This is because XAMARIN is a bitch. (I approve of this comment)

        public Command<CustomButton> PublishCommand { get; private set; }
        public static List<IOTButton> IOTButtons { get; set; } = new List<IOTButton>();
        public ObservableCollection<Button> CompletedButtons { get; set; } = new ObservableCollection<Button>();
        public Command ShowNewView { get; private set; }
        public Command ShowCmsView { get; private set; }

        private string test = "hello world";
        public string TestLabel { get { return test; } set { test = value; OnPropertyChanged(); } }

        public string IsConnected
        {
            get 
            { 
                return _isConnected; 
            }
            private set
            {
                _isConnected = value;
                OnPropertyChanged(); //This is very important, without XAMARIN doesn't know to refresh the UI and old values will not be updated
            }
        }

        //This is the constructor of our class
        public DashboardViewModel()
        {
            //Set the Title property, this is inherited from BaseViewModel
            Title = "Dashboard";
            IsConnected = "Disconnected";
            InitializeAsync();
        }

        //warning: async void! 
        public async void InitializeAsync()
        {
            App.Client.ConnectionStatusChanged += UpdateConnectionStatus;
            App.Client.MessageReceived += Test;
            ButtonCreationViewModel.IOTButtonsDatabaseUpdated += BuildDynamicButtons;
            //Bind the Publish method to the PublishCommand property which is called from DasBoardPage.xaml button click
            PublishCommand = new Command<CustomButton>(Publish);

            //a reference to redirect to the page on which buttons are generated
            ShowNewView = new Command(NavigateToButtonCreationPage);
            ShowCmsView = new Command<CustomButton>(NavigateToButtonActivator);
            this.UpdateConnectionStatus();
            //retrieve database rows and build buttons based on retrieved information
            this.BuildDynamicButtons();
        }

        public void Test()
        {
            MQTTMessage mqttMessage = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee");
            if(mqttMessage != null)
            {
                TestLabel = mqttMessage.Message;
            }
        }

        //helper method
        //can be set to static, since it should not create a new instance for buttons
        public async void BuildDynamicButtons()
        {
            //wait until all data has been received
            IOTButtons = await App.IOTDatabase.GetItemsAsync();
            CompletedButtons.Clear();

            //instantiate button variable for the generated buttons
            Button redirectButton;

            //loop through all button properties and read it
            foreach (IOTButton buttonProperties in IOTButtons)
            {
                //generate the button with the properties
                CustomButton button = new CustomButton()
                {
                    CustomID = buttonProperties.ID,
                    CustomTopic = buttonProperties.Topic,
                    CustomPayload = buttonProperties.Payload,
                    Text = buttonProperties.Name,
                    Command = PublishCommand,
                    ImageSource = buttonProperties.ImageName,
                    HeightRequest = 75,
                };
                button.CommandParameter = button;

                redirectButton = new Button
                {
                    Text = buttonProperties.Name,
                    Command = ShowCmsView,
                    CommandParameter = button,
                    ImageSource = buttonProperties.ImageName,
                };
              
                //add the button to an ObservableCollection of buttons
                CompletedButtons.Add(redirectButton);
            }
        }

        public void NavigateToButtonCreationPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new ButtonCreationPage(), true);
        }

        public void NavigateToButtonActivator(CustomButton button)
        {
            Application.Current.MainPage.Navigation.PushAsync(new ButtonUsagePage(button), true);
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

        private void Publish(CustomButton sender)
        {
            App.Client.Publish(sender.CustomTopic, sender.CustomPayload);
        }
    }
}