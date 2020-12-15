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
        //ATTENTION: KEEP VARIABLES FOR GENERIC THINGS SUCH AS ERRORS AS CLOSE TO EACH OTHER
        //FOR EASIER MAINTAINABILITY
        private string _errormsg;

        private string _bgcolor;
        public IOTButton Button { get; private set; } = null;
        public Command ClickCommand { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }

        public static event Action IOTButtonsDatabaseUpdated;


        public ButtonCreationViewModel()
        {
            Title = "New Topic";
            Button = new IOTButton();
            ClickCommand = new Command(Save);
        }

        public void InputTextChanged()
        {
            ErrorMsg = "";
            BgColor = "White";
        }


        public string ErrorMsg
        {
            get 
            { 
                return _errormsg; 
            }
            set
            {
                _errormsg = value;
                OnPropertyChanged();
            }
        }
        public string BgColor
        {
            get 
            { 
                return _bgcolor; 
            }
            set
            {
                _bgcolor = value;
                OnPropertyChanged();
            }
        }

        public async void Save()
        {
            if(!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Topic))
            {
                Button.Name = Name;
                Button.Topic = Topic;

                await App.IOTDatabase.SaveItemAsync(Button);
                InvokeUpdate();
                PopStack();
            } else
            {
                ErrorMsg = "Vergeet niet beide velden in te vullen!";
                BgColor = "red";
            }
        }

        //HELPER METHOD
        //helper methods should be static, to help the class in which it resides
        //the InvokeUpdate has been created, to prevent cluttering from one method that should not handle multiple actions
        //Most importantly, invoking the body of InvokeUpdate, is NOT possible from within other classes.
        public static void InvokeUpdate()
        {
            IOTButtonsDatabaseUpdated?.Invoke();
        }

        public void PopStack()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
