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
        private string _CurrentCoffeeStatus = "Coffee";

        public bool CoffeeStatusIsOff;
        public Command<string>GroundMoistButtonClickCommand { get; set; }
        public Command<string>CoffeeSwitchClickCommand { get; set; }

        public string CurrentCoffeStatus {
            get { return _CurrentCoffeeStatus; }
            set
            {
               _CurrentCoffeeStatus = value;
                OnPropertyChanged();
            }
        }

        public void OnOfSwitch()
        {
            if (CoffeeStatusIsOff)
            {
                CurrentCoffeStatus = "Coffee Off";
                CoffeeStatusIsOff = false;
            }
            else
            {
                CurrentCoffeStatus = "Coffee On";
                CoffeeStatusIsOff = true;
            }
        }

        public ProductViewModel()
        {
            GroundMoistButtonClickCommand = new Command<string>(GroundMoistButtonClick);
            CoffeeSwitchClickCommand = new Command<string>(CoffeeSwitchClick);
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
                
            }
            else
            {
                App.Client.Publish("Coffee", "OFF");
                
            }
          


        }
    }
}
