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
        private string _CurrentCoffeeStatus = "Coffee";
        private string _CurrentTempStatus = "NO DATA AVAILABLE";
        private string _currentGroundMoisture = "NO DATA AVAILABLE";
        private string _CurrentWaterStatus = "WateringSystem";
        private string _CurrentDateTime = "SELECT A DATE";
        private TimeSpan _SelectedTime;
        private DateTime _SelectedDate;

        public DateTime MinimumDate { get; private set; }
        public DateTime MaximumDate { get; private set; }

        public Command<string> GroundMoistButtonClickCommand { get; private set; }
        public Command<string> CoffeeSwitchClickCommand { get; private set; }
        public Command<string> TempButtonClickCommand { get; private set; }
        public Command<string> WaterSwitchClickCommand { get; private set; }
        public Command<string> StartTimerCommand { get; private set; }
        public Command SelectedDateCommand { get; private set; }

        public string CurrentCoffeStatus
        {
            get { return _CurrentCoffeeStatus; }
            set
            {
                _CurrentCoffeeStatus = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTempStatus
        {
            get { return _CurrentTempStatus; }
            set
            {
                _CurrentTempStatus = value;
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
            get { return _CurrentWaterStatus; }
            set
            {
                _CurrentWaterStatus = value;
                OnPropertyChanged();
            }
        }

        public string CurrentDateTime
        {
            get { return _CurrentDateTime; }
            set
            {
                _CurrentDateTime = value;
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
            GroundMoistButtonClickCommand = new Command<string>(NavigateToGraph);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            TempButtonClickCommand = new Command<string>(TempButtonClick);
            WaterSwitchClickCommand = new Command<string>(WaterSwitchClick);
            StartTimerCommand = new Command<string>(StartTimer);

            MinimumDate = DateTime.Today;
            MaximumDate = MinimumDate.AddDays(5);
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.TimeOfDay;
            App.Client.MessageReceived += Update;
        }

        private void Update()
        {
            this.GetLatestTempStatus();
            this.GetLatestWaterStatus();
            this.GetLatestCoffeeStatus();
            this.GetLatestMoistureStatus();
        }

        public void GetLatestCoffeeStatus()
        {
            MQTTMessage latestCoffeeStatus = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee");
            if (latestCoffeeStatus != null)
            {
                CurrentCoffeStatus = latestCoffeeStatus.Message;
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
                CurrentGroundMoisture = $"Ground moisture: {latestMoisture.Message}%";
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

        public void TempButtonClick(string Temp)
        {
            GetLatestTempStatus();
            NavigateToGraph(Temp);
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