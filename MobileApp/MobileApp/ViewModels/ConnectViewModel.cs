using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ConnectViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _errorLabelIsVisible = "false";
        private string _errorLabelMessage;
        private string _ipInput;
        private string _connectPageLabelMessage = "Connect";
        private string _connecting = "false";

        public Command ConnectClickCommand { get; set; } 
        public int PortInput { get; set; } = 1883; //Default port
        
        public string ErrorLabelIsVisible 
        {
            get { return _errorLabelIsVisible; }
            private set
            {
                _errorLabelIsVisible = value;
                OnPropertyChanged();
            }
        }

        public string Connecting
        {
            get { return _connecting; }
            private set { 
                _connecting = value;
                OnPropertyChanged();
            }
        }

        public string ConnectPageLabelMessage
        {
            get { return _connectPageLabelMessage; }
            private set
            {
                _connectPageLabelMessage = value;
                OnPropertyChanged();
            }
        }
        public string ErrorLabelMessage
        {
            get { return _errorLabelMessage;  }
            private set
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
            if (!IsValidInput())
            {
                this.SetErrorMessageAndShowLabel("The IP you entered was invalid!");
                return;
            }

            if (App.Client.IsClientConnected)
                await App.Client.Disconnect();

            this.ConnectPageLabelMessage = "Connecting";
            this.Connecting = "true";

            if (await this.IsClientConnected())
            {
                await Shell.Current.GoToAsync("//dashboard");
                //GoToAsync doesn't propperly tell when the page has been changed. If we do it after the await the user will see the cleanup.
                this.CleanupAfterDelay();
                return;
            }
          
            this.SetErrorMessageAndShowLabel("Could not connect to the server. Please try again!");
            this.ResetToDefaultXAML();
        }

        private void CleanupAfterDelay()
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ResetToDefaultXAML();
                });
                return false;
            });
        }

        private void ResetToDefaultXAML()
        {
            this.Connecting = "false";
            this.ConnectPageLabelMessage = "Connect";
        }

        private async Task<bool> IsClientConnected()
        {
            const int TIMEOUT_DELAY = 2000;
            var task = App.Client.Connect(this.IPInput, this.PortInput);
            if (await Task.WhenAny(task, Task.Delay(TIMEOUT_DELAY)) == task)
            {
                // task completed within timeout
                return true;
            }
            return false;
        }

        private void SetErrorMessageAndShowLabel(string message)
        {
            this.ErrorLabelMessage = message;
            this.ErrorLabelIsVisible = "true";
        }

        public void IpInput_TextChanged()
        {
                ErrorLabelIsVisible = "false";
        }

    }
}