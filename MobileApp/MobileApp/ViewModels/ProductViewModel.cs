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
        public Command<string>GroundMoistButtonClickCommand { get; set; }


        public ProductViewModel()
        {
            GroundMoistButtonClickCommand = new Command<string>(GroundMoistButtonClick);
        }


        public void GroundMoistButtonClick(string WateringSystemFeedback)
        {

            Application.Current.MainPage.Navigation.PushAsync(new GroundMoisturePage(WateringSystemFeedback), true);
        }

    }
}
