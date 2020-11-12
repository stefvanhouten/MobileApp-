using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MobileApp.Views;


namespace MobileApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //App.Client.Disconnected += AlertUserDisconnected;
        }

        private async void AlertUserDisconnected()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Alert", "Lost connection with network!", "Ok");
            });
        }
    }
}
