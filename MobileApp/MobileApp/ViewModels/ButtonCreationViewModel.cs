using System;
using MobileApp.Models;
using MobileApp.Data;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MobileApp.Views;

namespace MobileApp.ViewModels
{
    public class ButtonCreationViewModel : BaseViewModel
    {
        public IOTButton Button { get; private set; } = null;
        public Command ClickCommand { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Image { get; set; }

        public static event Action IOTButtonsDatabaseUpdated;


        public ButtonCreationViewModel()
        {
            Title = "New Topic";

            Button = new IOTButton();

            ClickCommand = new Command(Save);
        }

        //CREATE A METHOD TO CHECK WHETHER PROPERTIES ARE EMPTY???
        public void Save()
        {
            Button.Name = Name;
            Button.Topic = Topic;
            // Button.Image = Image;

            App.IOTDatabase.SaveItemAsync(Button);
            IOTButtonsDatabaseUpdated?.Invoke();
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
