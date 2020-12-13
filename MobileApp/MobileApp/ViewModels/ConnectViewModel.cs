using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ConnectViewModel : BaseViewModel
    {
        
        public Command ConnectClickCommand { get; set; }
        public int PortInput { get; set; } = 1883; //Default port
        public string IPInput { get; set; } = "127.0.0.1"; //Default localhost
   
        

      
        public ConnectViewModel()
        {
            Title = "ConnectPage";
            ConnectClickCommand = new Command(ConnectClick);
        }

        //Compares Input to Valid IPAdresses
        private bool IsValidInput()
        { 
            IPAddress ip;

            return IPAddress.TryParse(IPInput, out ip);
        }
        
        public void ConnectClick()
        {
            if (IsValidInput())
            {
                App.ServerIP = this.IPInput;
                App.ServerPort = this.PortInput;
                Application.Current.MainPage.Navigation.PushAsync(new DashboardPage(), false);
                Console.WriteLine(PortInput);
            }
        }
        
        
    }
}