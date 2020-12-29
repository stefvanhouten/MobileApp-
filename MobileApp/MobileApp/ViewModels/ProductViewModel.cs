using MobileApp.Services;
using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    class ProductViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _CurrentCoffeeStatus = "coffee";
        private string _CurrentTempStatus = "Temp";
        private string _CurrentWaterStatus = "Water";

        public bool CoffeeStatusIsOff;
        public bool WaterStatusIsOff;
        public Command<string>GroundMoistButtonClickCommand { get; set; }
        public Command<string>CoffeeSwitchClickCommand { get; set; }
        public Command<string>TempButtonClickCommand { get; set; }
        public Command<string> WaterSwitchClickCommand { get; set; } 

        public void GetLatestCoffee()
        {
           MQTTMessage msg = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("coffee");
            if (msg != null)
            {
                CurrentCoffeStatus = msg.Message;
            }
        }

        public void GetLatestTemp()
        {
            MQTTMessage msg2 = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("wateringSystemFeedback");
            if (msg2 != null)
            {
                CurrentTempStatus = msg2.Message;
            }
        }

        public void GetLatestWater()
        {
            MQTTMessage msg3 = App.Client.MQTTMessageStore.GetLatestMessageFromTopic("water");
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
                App.Client.Publish("coffee", "ON");
                CurrentCoffeStatus = "ON";
            }
            else
            {
                App.Client.Publish("coffee", "OFF");
                CurrentCoffeStatus = "OFF";
            }
        }
        public void WaterSwitchClick(string CoffeeOnOffFeedback)
        {
            OnOfWater();

            if (WaterStatusIsOff)
            {
                App.Client.Publish("water", "ON");
                CurrentWaterStatus = "ON";
            }
            else
            {
                App.Client.Publish("water", "OFF");
                CurrentWaterStatus = "OFF";
            }
        }
        public void TempButtonClick(string Temp)
        {
            GetLatestTemp();
        }

    }
}
