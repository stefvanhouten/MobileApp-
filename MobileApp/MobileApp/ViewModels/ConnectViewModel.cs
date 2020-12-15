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
    public class ConnectViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _errormsg; 
        private string _bgcolor; 
        private string _ipinput; 
        public Command ConnectClickCommand { get; set; } 
        public int PortInput { get; set; } = 1883; //Default port

        public string IPInput {
                    get { return _ipinput; }
                    set
                    {
                        _ipinput = value;
                         IpInput_TextChanged();
                    }
        
                 }

        public string ErrorMsg {
                    get { return _errormsg; }
                    set
                    {
                        _errormsg = value;
                        OnPropertyChanged();
                    }
                }

        public string BgColor {
                    get { return _bgcolor; }
                    set
                    {
                        _bgcolor = value;
                        OnPropertyChanged();
                    }
                }
        
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
            }else
            {
                ErrorMsg = "Error IP is not Valid";
                BgColor = "red";
            }
        }

        public void IpInput_TextChanged()
        {
            ErrorMsg = "";
            BgColor = "White";
        }

    }
}