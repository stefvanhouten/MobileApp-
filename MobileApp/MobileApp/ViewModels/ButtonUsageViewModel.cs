﻿using MobileApp.CustomElement;
using MobileApp.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    class ButtonUsageViewModel : BaseViewModel
    {
        public Command DeleteBtn {get; set;}
        public CustomButton ResponseButton { get; set; }
        public ObservableCollection<Button> CompletedButtons { get; set; } = new ObservableCollection<Button>();

        public ButtonUsageViewModel(CustomButton button)
        {
            Title = "Management Systeem";
            this.CompletedButtons.Add(button);
            ResponseButton = button;
            DeleteBtn = new Command(DeleteButton);
        }

        public async void DeleteButton()
        {
            int PK = ResponseButton.CustomID;
            IOTButton toDelete = await App.IOTDatabase.GetItemAsync(PK);
            await App.IOTDatabase.DeleteItemAsync(toDelete);
            ButtonCreationViewModel.InvokeUpdate();
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
