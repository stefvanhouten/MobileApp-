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

        //Name or Topic should not be empty and therefore we enforce a check prior to saving the data
        private bool isEmpty()
        {
            if (Name == "" || Topic == "")
            {
                return true;
            }
            return false;
        }
        public void Save()
        {
            if(!isEmpty())
            {
                Button.Name = Name;
                Button.Topic = Topic;

                App.IOTDatabase.SaveItemAsync(Button);
                InvokeUpdate();
                PopStack();
            } else
            {
                //CREATE SOME POP-UP BOX LATER ON...
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
