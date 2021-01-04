
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
            BoardingPageList.Add(new BoardingPage() { Name = "Welcome to our app", 
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate " +
                "velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 
                ImageUrl = "placeholder" });
            BoardingPageList.Add(new BoardingPage() { Name = "TEST", Description = "HELLO WORLD", ImageUrl = "placeholder" });
            BoardingPageList.Add(new BoardingPage() { Name = "TEST", Description = "HELLO WORLD", ImageUrl = "placeholder" });
            BoardingPageList.Add(new BoardingPage() { Name = "TEST", Description = "HELLO WORLD", ImageUrl = "placeholder" });
        }

        public void SkipClicked()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }
    }
}