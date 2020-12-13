﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonCreationPage : ContentPage
    {
        public ButtonCreationPage()
        {
            InitializeComponent();
            this.BindingContext = new ButtonCreationViewModel();
        }
    }
}