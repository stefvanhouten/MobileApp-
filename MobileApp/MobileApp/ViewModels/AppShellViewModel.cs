using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private bool _connected;
        public bool Connected {
            get { return this._connected; } 
            set { 
                this._connected = value;
                OnPropertyChanged();
            } 
        }
        public AppShellViewModel()
        {
            App.Client.ConnectionStatusChanged += UpdateNavigation;
        }

        private void UpdateNavigation()
        {
            this.Connected = App.Client.IsClientConnected;
        }
    }
}
