using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MobileApp.Views;
using MobileApp.ViewModels;

namespace MobileApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = new AppShellViewModel();
        }
    }
}
