using MobileApp.Services;
using MobileApp.Views;
using System;
using System.ComponentModel;
using System.Timers;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    internal class ProductViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private Timer MyTimer;
        private string _isConnected;
        private string _currentCoffeeStatus = "Coffee";
        private string _currentTemperatureStatus = "NO DATA AVAILABLE";
        private string _currentHumidityStatus = "NO DATA AVAILABLE";
        private string _currentGroundMoisture = "NO DATA AVAILABLE";
        private string _currentWateringStatus = "WateringSystem";
        private string _currentDateAndTime = "SELECT A DATE";

        private TimeSpan _SelectedTime;
        private DateTime _SelectedDate;

        public DateTime MinimumDate { get; private set; }
        public DateTime MaximumDate { get; private set; }

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

        public ProductViewModel()
        {
            Title = "Product";
            NavigateCommand = new Command<string>(NavigateToGraph);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            WaterSwitchClickCommand = new Command<string>(WaterSwitchClick);
            StartTimerCommand = new Command<string>(StartTimer);
            IsConnected = "Connected";

            MinimumDate = DateTime.Today;
            MaximumDate = MinimumDate.AddDays(5);
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.TimeOfDay;

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
            MQTTMessage latestCoffeeStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee");
            if (latestCoffeeStatus != null)
            {
                CurrentCoffeeStatus = latestCoffeeStatus.Message;
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
            MQTTMessage coffeeStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee");
            if (coffeeStatus == null)
            {
                return;
            }

            if (coffeeStatus.Message == "ON")
            {
                App.Client.Publish("Coffee", "OFF");
            }
            else
            {
                App.Client.Publish("Coffee", "ON");
            }
        }

        public void WaterSwitchClick(string WaterSystemOnOffFeedback)
        {
            MQTTMessage wateringStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("WateringSystem/Status");
            if (wateringStatus == null)
            {
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
            CurrentDateTime = combined.ToString();
            MyTimer = new Timer
            {
                Interval = millisecondsUntillTrigger
            };
            MyTimer.Elapsed += OnIntervalEvent;
            MyTimer.Enabled = true;
            MyTimer.Start();
        }


        private void OnIntervalEvent(object source, ElapsedEventArgs e)
        {
            MyTimer.Enabled = false;
            App.Client.Publish("coffee/POWER", "ON");
        }
    }
}