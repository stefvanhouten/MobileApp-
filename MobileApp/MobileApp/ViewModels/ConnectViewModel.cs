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
        private string _errorLabelIsVisible = "false";
        private string _errorLabelMessage;
        private string _ipInput;

        public Command ConnectClickCommand { get; set; } 
        public int PortInput { get; set; } = 1883; //Default port
        
        public string ErrorLabelIsVisible {
                    get { return _errorLabelIsVisible; }
                    set
                    {
                        _errorLabelIsVisible = value;
                        OnPropertyChanged();
                    }
                }
        public string ErrorLabelMessage
        {
            get { return _errorLabelMessage;  }
            set
            {
                _errorLabelMessage = value;
                OnPropertyChanged();
            }
        }

        public string IPInput {
                    get { return _ipInput; }
                    set
                    {
                        _ipInput = value;
                         IpInput_TextChanged();
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
 
        public async void ConnectClick()
        {
           
            if (IsValidInput())
            {
                if (App.Client.IsClientConnected)
                {
                    App.Client.Disconnect();
                }

                const int TIMEOUT_DELAY = 2000;
                var task = App.Client.Connect(this.IPInput, this.PortInput);
                if (await Task.WhenAny(task, Task.Delay(TIMEOUT_DELAY)) == task)
                {
                    // task completed within timeout
                    await Shell.Current.GoToAsync("//dashboard");
                }
                else
                {
                    ErrorLabelMessage = "Could not connect to the server. Please try again";
                    ErrorLabelIsVisible = "true";

                }
            }
            else
            {
                ErrorLabelMessage = "The IP you entered was invalid";
                ErrorLabelIsVisible = "true";
            }
        }

        public void IpInput_TextChanged()
        {
                ErrorLabelIsVisible = "false";
        }

    }
}