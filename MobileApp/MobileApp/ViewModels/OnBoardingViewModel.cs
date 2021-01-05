
using MobileApp.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class OnBoardingViewModel : BaseViewModel
    {
        public List<BoardingPage> BoardingPageList { get; set; } = new List<BoardingPage>();
        public Command SkipCommand { get; private set; }


        public OnBoardingViewModel()
        {
            SkipCommand = new Command(SkipClicked);
            BoardingPageList.Add(new BoardingPage() { Name = "Welcome home!", 
                Description = "Welcome to our home automation assistant. Our goal is to make your life easier and we do just that with this app!", 
                ImageUrl = "home_automation.png" });
            BoardingPageList.Add(new BoardingPage() { 
                Name = "Connecting", 
                Description = "First of all we need to connect to our communication server. The IP addres should be your IP4 addres of the network you are on. " +
                "By default the server uses PORT 1883.", 
                ImageUrl = "connect.png"
            });
            BoardingPageList.Add(new BoardingPage() { 
                Name = "The dashboard", 
                Description = "On the dashboard you can see your connection status and add custom buttons if you wish to expand your app.", 
                ImageUrl = "dashboard.png", 
                Height = 300 });
        }

        public void SkipClicked()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }
    }
}