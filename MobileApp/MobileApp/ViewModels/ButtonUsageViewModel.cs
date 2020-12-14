using System;
using MobileApp.Models;
using MobileApp.Data;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MobileApp.Views;
using System.Collections.ObjectModel;
using MobileApp.CustomElement;

namespace MobileApp.ViewModels
{
    class ButtonUsageViewModel : BaseViewModel
    {
        public Command DeleteBtn {get; set;}
        public CustomButton ResponseButton { get; set; }
        public ButtonUsageViewModel(CustomButton button)
        {
            Title = "Management Systeem";
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
