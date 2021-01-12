using MobileApp.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ConnectViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _errorLabelIsVisible = "false";
        private string _errorLabelMessage;
        private string _ipInput;
        private string _password;
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
            set
            {
                _connecting = value;
                OnPropertyChanged();
            }
        }

        public string ConnectPageLabelMessage
        {
            get { return _connectPageLabelMessage; }
            set
            {
                _connectPageLabelMessage = value;
                OnPropertyChanged();
            }
        }

        public string ErrorLabelMessage
        {
            get { return _errorLabelMessage; }
            set
            {
                _errorLabelMessage = value;
                OnPropertyChanged();
            }
        }

        public string IPInput
        {
            get { return _ipInput; }
            set
            {
                _ipInput = value;
                OnPropertyChanged();
                InputChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                InputChanged();
                OnPropertyChanged();
            }
        }

        public ConnectViewModel()
        {
            Title = "ConnectPage";
            ConnectClickCommand = new Command(ConnectClick);
        }

        public async void ConnectClick()
        {
            if (!this.IsValidIP4(this.IPInput) && App.Client is MQTTClient)
            {
                this.SetErrorMessageAndShowLabel("The IP you entered was invalid!");
                return;
            }

            if(App.Client is MQTTMockClient && this.IPInput == "validation")
            {
                this.SetErrorMessageAndShowLabel("Show validation for mock purposes");
                return;
            }

            if (!this.IsValidPort(this.PortInput))
            {
                this.SetErrorMessageAndShowLabel("The Port you entered was invalid! Please try a port between 100 and 10000");
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
            this.ResetToDefaultStatus();
        }

        private bool IsValidIP4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        private bool IsValidPort(int portNumber)
        {
            if(portNumber != null && portNumber >= 100 && portNumber <= 10000)
            {
                return true;
            }
            return false;
        }

        private void CleanupAfterDelay()
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ResetToDefaultStatus();
                });
                return false;
            });
        }

        private void ResetToDefaultStatus()
        {
            this.Connecting = "false";
            this.ConnectPageLabelMessage = "Connect";
        }

        private async Task<bool> IsClientConnected()
        {
            const int TIMEOUT_DELAY = 2000;
            var task = App.Client.Connect(this.IPInput, this.PortInput, this.Password);
            if (await Task.WhenAny(task, Task.Delay(TIMEOUT_DELAY)) == task)
            {
                // task completed within timeout
                return task.Result;
            }
            return false;
        }

        private void SetErrorMessageAndShowLabel(string message)
        {
            this.ErrorLabelMessage = message;
            this.ErrorLabelIsVisible = "true";
        }

        public void InputChanged()
        {
            ErrorLabelIsVisible = "false";
        }
    }
}