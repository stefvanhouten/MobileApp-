using MobileApp.Services;
using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    class ProductViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private System.Timers.Timer MyTimer;
        private string _CurrentCoffeeStatus = "Coffee";
        private string _CurrentTempStatus = "Plant/Temperature";
        private string _CurrentWaterStatus = "WateringSystem";
        private string _CurrentDateTime = DateTime.Now.ToString();
        private string _TheNewDate;
        private TimeSpan _SelectedTime;
        private DateTime _SelectedDate;
       
        
        public bool CoffeeStatusIsOff;
        public bool WaterStatusIsOff;
        public Command<string>GroundMoistButtonClickCommand { get; set; }
        public Command<string>CoffeeSwitchClickCommand { get; set; }
        public Command<string>TempButtonClickCommand { get; set; }
        public Command<string>WaterSwitchClickCommand { get; set; }
        public Command<string>StartTimerCommand { get; set; }
        public Command SelectedDateCommand { get; set; }

        //TimePicker timepicker = new TimePicker();

        //DatePicker datePicker = new DatePicker
        //{
        //    MinimumDate = new DateTime(2020, 12, 30),
        //    MaximumDate = new DateTime(2050, 12, 31),
        //    Date = DateTime.Today
        //};

        public void GetLatestCoffee()
        {
           MQTTMessage msg = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Coffee");
            if (msg != null)
            {
                CurrentCoffeStatus = msg.Message;
            }
        }

        public void GetLatestTemp()
        {
            MQTTMessage msg2 = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("Plant/Temperature");
            if (msg2 != null)
            {
                CurrentTempStatus = msg2.Message;
            }
        }

        public void GetLatestWater()
        {
            MQTTMessage msg3 = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("WateringSystem/Status");
            if (msg3 != null)
            {
                CurrentWaterStatus = msg3.Message;
            }
        }


        public string CurrentCoffeStatus {
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

        public string TheNewDate
        {
            get { return _TheNewDate; }
            set
            {
                _TheNewDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                _SelectedDate = value;
                this.Test();
                OnPropertyChanged();
            }
        }
        
        public TimeSpan SelectedTime
        {
            get { return _SelectedTime; }
            set
            {
                _SelectedTime = value;
                this.Test2();
                OnPropertyChanged();
            }
        }

        public void OnOfSwitch()
        {
            GetLatestCoffee();
            if (CurrentCoffeStatus == "OFF")
            {
                CoffeeStatusIsOff = true;
            }
            if (CurrentCoffeStatus == "ON")
            {
                CoffeeStatusIsOff = false;
            }
        }
        public void OnOfWater()
        {
            GetLatestWater();
            if (CurrentWaterStatus == "OFF")
            {
                WaterStatusIsOff = true;
            }
            if (CurrentWaterStatus == "ON")
            {
                WaterStatusIsOff = false;
            }
        }

        public ProductViewModel()
        {
            Title = "Product";
            GroundMoistButtonClickCommand = new Command<string>(GroundMoistButtonClick);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            TempButtonClickCommand = new Command<string>(TempButtonClick);
            WaterSwitchClickCommand = new Command<string>(WaterSwitchClick);
            StartTimerCommand = new Command<string>(StartTimer);
            SelectedDateCommand = new Command(Test);
            SelectedDate = DateTime.Now;
            SelectedTime = DateTime.Now.TimeOfDay;
        }
        public void Test()
        {
            var test = this.SelectedDate;

        }

        public void Test2()
        {
            
        }


        public void GroundMoistButtonClick(string WateringSystemFeedback)
        {
            Application.Current.MainPage.Navigation.PushAsync(new GroundMostuirePage(WateringSystemFeedback), true);
        }

        public void CoffeeSwitchClick(string CoffeeOnOffFeedback)
        {
            OnOfSwitch();

            if (CoffeeStatusIsOff)
            {
                App.Client.Publish("Coffee", "ON");
                CurrentCoffeStatus = "ON";
            }
            else
            {
                App.Client.Publish("Coffee", "OFF");
                CurrentCoffeStatus = "OFF";
            }
        }
        public void WaterSwitchClick(string WaterSystemOnOffFeedback)
        {
            OnOfWater();

            if (WaterStatusIsOff)
            {
                App.Client.Publish("WateringSystem/Status", "ON");
                CurrentWaterStatus = "ON";
            }
            else
            {
                App.Client.Publish("WateringSystem/Status", "OFF");
                CurrentWaterStatus = "OFF";
            }
        }
        public void TempButtonClick(string Temp)
        {
            GetLatestTemp();
        }

       

        public void GiveCoffeeOnPickedMoment()
        {
            
        }

        //public void DateAndTime()
        //{
        //    SelectedTime = timepicker.Time;
        //    DateTime result = SelectedDate + SelectedTime;
        //}

        private void StartTimer(String TimerStart)
        {
            //MyTimer.Interval = 1000;
            //MyTimer.Elapsed += OnIntervalEvent;
            //MyTimer.Start();
            TimeSpan time = this.SelectedTime;
            DateTime date = this.SelectedDate;
            DateTime combined = date + time;
            TimeSpan span = combined.Subtract(DateTime.Now);
            int secondsUntillTrigger = (int)span.TotalSeconds;

        }

        private static void OnIntervalEvent(object source, ElapsedEventArgs e)
        {

        }




    }
}
