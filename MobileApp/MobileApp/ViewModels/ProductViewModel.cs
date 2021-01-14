using MobileApp.Services;
using MobileApp.Views;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Timers;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    internal class ProductViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _isConnected;
        private string _currentCoffeeStatus = "Coffee";
        private string _currentTemperatureStatus = "NO DATA AVAILABLE";
        private string _currentHumidityStatus = "NO DATA AVAILABLE";
        private string _currentGroundMoisture = "NO DATA AVAILABLE";
        private string _currentWateringStatus = "WateringSystem";
        private string _currentDateAndTime = "SELECT A DATE";
        private string _errorLabelIsVisible = "false";
        private string _errorLabelMessage;
        private TimeSpan _SelectedTime;
        private DateTime _SelectedDate;

        private Timer SetCoffeeTimer { get; set; }
        private Timer ClearErrorMessageTimer { get; set; }

        public DateTime MinimumDate { get; private set; }
        public DateTime MaximumDate { get; private set; }
        private DateTime LastTimeWatered { get; set; }

        public Command<string> NavigateCommand { get; private set; }
        public Command<string> CoffeeSwitchClickCommand { get; private set; }
        public Command<string> TempButtonClickCommand { get; private set; }
        public Command<string> WaterSwitchClickCommand { get; private set; }
        public Command<string> StartTimerCommand { get; private set; }
        public Command SelectedDateCommand { get; private set; }

        public string CurrentCoffeeStatus
        {
            get { return _currentCoffeeStatus; }
            set
            {
                _currentCoffeeStatus = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTempStatus
        {
            get { return _currentTemperatureStatus; }
            set
            {
                _currentTemperatureStatus = value;
                OnPropertyChanged();
            }
        }

        public string CurrentHumidityStatus
        {
            get { return _currentHumidityStatus; }
            set
            {
                _currentHumidityStatus = value;
                OnPropertyChanged();
            }
        }

        public string IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public string CurrentGroundMoisture
        {
            get { return _currentGroundMoisture; }
            set
            {
                _currentGroundMoisture = value;
                OnPropertyChanged();
            }
        }

        public string CurrentWaterStatus
        {
            get { return _currentWateringStatus; }
            set
            {
                _currentWateringStatus = value;
                OnPropertyChanged();
            }
        }

        public string CurrentDateTime
        {
            get { return _currentDateAndTime; }
            set
            {
                _currentDateAndTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                _SelectedDate = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan SelectedTime
        {
            get { return _SelectedTime; }
            set
            {
                _SelectedTime = value;
                OnPropertyChanged();
            }
        }

        public string ErrorLabelMessage
        {
            get { return _errorLabelMessage; }
            set
            {
                _errorLabelMessage = value;
                OnPropertyChanged();
            }
        }

        public string ErrorLabelIsVisible
        {
            get { return _errorLabelIsVisible; }
            private set
            {
                _errorLabelIsVisible = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel()
        {
            this.Title = "Product";
            this.NavigateCommand = new Command<string>(NavigateToGraph);
            this.CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            this.WaterSwitchClickCommand = new Command<string>(WaterSwitchClick);
            this.StartTimerCommand = new Command<string>(StartTimer);
            this.IsConnected = "Connected";

            this.MinimumDate = DateTime.Today;
            this.MaximumDate = MinimumDate.AddDays(5);
            this.SelectedDate = DateTime.Today;
            this.SelectedTime = DateTime.Now.TimeOfDay;
            this.LastTimeWatered = DateTime.Today;

            this.ClearErrorMessageTimer = new Timer();
            this.SetCoffeeTimer = new Timer();
            App.Client.MessageReceived += Update;
            App.Client.ConnectionStatusChanged += UpdateConnectionStatus;
        }

        private void Update()
        {
            this.GetLatestTempStatus();
            this.GetLatestWaterStatus();
            this.GetLatestCoffeeStatus();
            this.GetLatestMoistureStatus();
            this.GetLatestHumidityStatus();
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

        public void GetLatestCoffeeStatus()
        {
            MQTTMessage latestCoffeeStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee/Status");
            if (latestCoffeeStatus != null)
            {
                JObject json = JObject.Parse(latestCoffeeStatus.Message);
                CurrentCoffeeStatus = json["POWER"].Value<string>();
            }
        }

        public void GetLatestTempStatus()
        {
            MQTTMessage latestTemperature = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Plant/Temperature");
            if (latestTemperature != null)
            {
                CurrentTempStatus = $"Temperature: {latestTemperature.Message}°C";
            }
        }

        public void GetLatestMoistureStatus()
        {
            MQTTMessage latestMoisture = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Plant/Moisture");
            if (latestMoisture != null)
            {
                CurrentGroundMoisture = $"Soil moisture: {latestMoisture.Message}%";
            }
        }

        public void GetLatestHumidityStatus()
        {
            MQTTMessage latesHumidity = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Plant/Humidity");
            if (latesHumidity != null)
            {
                CurrentHumidityStatus = $"Humidity: {latesHumidity.Message}%";
            }
        }

        public void GetLatestWaterStatus()
        {
            MQTTMessage latestWaterStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("WateringSystem/Status");
            if (latestWaterStatus != null)
            {
                CurrentWaterStatus = latestWaterStatus.Message;
            }
        }

        public void NavigateToGraph(string WateringSystemFeedback)
        {
            Application.Current.MainPage.Navigation.PushAsync(new GroundMoisturePage(WateringSystemFeedback), true);
        }

        public void CoffeeSwitchClick(string CoffeeOnOffFeedback)
        {
            MQTTMessage coffeeStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee/Status");
            if (coffeeStatus == null)
            {
                this.SetErrorMessageAndShowLabel("Cannot use button when no information about the current state is available");
                return;
            }
            JObject json = JObject.Parse(coffeeStatus.Message);
            string status = json["POWER"].Value<string>();
            if (status == "ON")
            {
                App.Client.Publish("cmnd/coffee/POWER", "OFF");
            }
            else
            {
                App.Client.Publish("cmnd/coffee/POWER", "ON");
            }
        }

        public void WaterSwitchClick(string WaterSystemOnOffFeedback)
        {
            MQTTMessage wateringStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("WateringSystem/Status");
            if(LastTimeWatered != null)
            {
                if ((DateTime.Now - LastTimeWatered).TotalSeconds <= 5)
                {
                    this.SetErrorMessageAndShowLabel("Watering the plant has a 5 second cooldown to prevent accidents!");
                    return;
                }
            }
            if (wateringStatus == null)
            {
                this.SetErrorMessageAndShowLabel("Cannot use button when no information about the current state is available");
                return;
            }

            if (wateringStatus.Message == "ON")
            {
                App.Client.Publish("WateringSystem", "OFF");
            }
            else
            {
                App.Client.Publish("WateringSystem", "ON");
            }
            LastTimeWatered = DateTime.Now;
        }

        private void StartTimer(String TimerStart)
        {
            TimeSpan selectedTime = this.SelectedTime;
            DateTime selectedDate = this.SelectedDate.Date;
            DateTime combined = selectedDate + selectedTime;
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = combined.Subtract(currentTime);

            if (currentTime.Ticks >= combined.Ticks)
            {
                return;
            }

            ulong millisecondsUntillTrigger = (ulong)timeDifference.TotalMilliseconds;
            this.CurrentDateTime = combined.ToString();
            this.SetCoffeeTimer.Dispose();
            this.SetCoffeeTimer = new Timer
            {
                Interval = millisecondsUntillTrigger
            };
            this.SetCoffeeTimer.Elapsed += this.TurnCoffeeOn;
            this.SetCoffeeTimer.Enabled = true;
            this.SetCoffeeTimer.Start();
        }


        private void TurnCoffeeOn(object source, ElapsedEventArgs e)
        {
            this.SetCoffeeTimer.Dispose();
            App.Client.Publish("coffee/POWER", "ON");
        }


        private void SetErrorMessageAndShowLabel(string message)
        {
            this.ClearErrorMessageTimer.Dispose();
            this.ErrorLabelMessage = message;
            this.ErrorLabelIsVisible = "true";
            this.ClearErrorMessageTimer = new Timer(5000);
            this.ClearErrorMessageTimer.Elapsed += this.HideErrorMessageLabel;
            this.ClearErrorMessageTimer.Enabled = true;

        }
        private void HideErrorMessageLabel(object source, ElapsedEventArgs e)
        {
            this.ClearErrorMessageTimer.Dispose();
            this.ErrorLabelIsVisible = "false";
        }
    }
}