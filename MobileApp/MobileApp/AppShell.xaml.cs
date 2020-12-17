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
            this.AsyncInit(); 
        }

        public async void AsyncInit()
        {
            InitializeComponent();
            await Navigation.PushModalAsync(new ConnectPage());
        }
    }
}
