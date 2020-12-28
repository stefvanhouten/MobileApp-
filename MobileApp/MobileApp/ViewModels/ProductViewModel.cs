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

        public bool CoffeeStatusIsOff;
        public Command<string>GroundMoistButtonClickCommand { get; set; }
        public Command<string>CoffeeSwitchClickCommand { get; set; }
        public Command<string> WaterButtonClickCommand { get; set; }

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

        public ProductViewModel()
        {
            GroundMoistButtonClickCommand = new Command<string>(GroundMoistButtonClick);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
            WaterButtonClickCommand = new Command<string>(WaterButtonClick);
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

        public void WaterButtonClick(string Temp)
        {
            GetLatestTemp();
        }

    }
}
