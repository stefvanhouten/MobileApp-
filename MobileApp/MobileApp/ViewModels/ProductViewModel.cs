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
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    class ProductViewModel : BaseViewModel, INotifyPropertyChanged
    {

        private string _CurrentCoffeeStatus = "Coffee";
        private string _CurrentTempStatus = "Plant/Temperature";
        private string _CurrentWaterStatus = "WateringSystem";
        private int CountSeconds;

        public bool CoffeeStatusIsOff;
        public bool WaterStatusIsOff;
        public Command<string>GroundMoistButtonClickCommand { get; set; }
        public Command<string>CoffeeSwitchClickCommand { get; set; }
        public Command<string>TempButtonClickCommand { get; set; }
        public Command<string> WaterSwitchClickCommand { get; set; }
        public string _CurrentDateTime = DateTime.Now.ToString();
        public TimeSpan SelectedTime { get; set; }
        public DateTime SelectedDate { get; set; }
        

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
            GroundMoistButtonClickCommand = new Command<string>(GroundMoistButtonClick);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            TempButtonClickCommand = new Command<string>(TempButtonClick);
            WaterSwitchClickCommand = new Command<string>(WaterSwitchClick);
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

        DatePicker datePicker = new DatePicker
        {
            MinimumDate = new DateTime(2020, 12, 30),
            MaximumDate = new DateTime(2050, 12, 31),
            Date = new DateTime(2020, 12, 30)
        };

        TimePicker timepicker = new TimePicker
        {
            Time = new TimeSpan(4, 20, 69),
            
        };

        
       



    }
}
