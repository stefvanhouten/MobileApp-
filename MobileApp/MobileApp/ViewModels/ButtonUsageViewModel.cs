using System;
using MobileApp.Models;
using MobileApp.Data;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MobileApp.Views;
using System.Collections.ObjectModel;

namespace MobileApp.ViewModels
{
    class ButtonUsageViewModel : BaseViewModel
    {
        public ObservableCollection<Button> PublishButtons { get; private set; } = new ObservableCollection<Button>();
        public Command DeleteBtn {get; set;}
        public ButtonUsageViewModel()
        {
            Title = "Management Systeem";
            DeleteBtn = new Command<IOTButton>(DeleteButton);
        }

        //SET ASYNC, TO CALL ASYNC METHODS FROM WITHIN CONSTRUCTOR IF REQUIRED
        public async void InitializeAsync()
        {
            ButtonCreationViewModel.InvokeUpdateEvent();
        }

        public async void DeleteButton(IOTButton item)
        {
            await App.IOTDatabase.DeleteItemAsync(item);
        }
    }
}
