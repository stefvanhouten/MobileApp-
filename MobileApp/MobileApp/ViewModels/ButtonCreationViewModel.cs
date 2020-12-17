using MobileApp.Models;
using System;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ButtonCreationViewModel : BaseViewModel
    {
        //ATTENTION: KEEP VARIABLES FOR GENERIC THINGS SUCH AS ERRORS AS CLOSE TO EACH OTHER
        //FOR EASIER MAINTAINABILITY
        private string _name;
        private string _errorLabelIsVisible = "false";
        private string _topic;

        public static event Action IOTButtonsDatabaseUpdated;

        public IOTButton Button { get; private set; } = null;
        public Command ClickCommand { get; set; }

        public string ImageName { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                ErrorLabelIsVisible = "false";
                OnPropertyChanged();
            }
        }

        public string Topic
        {
            get
            {
                return _topic;
            }
            set
            {
                _topic = value;
                ErrorLabelIsVisible = "false";
                OnPropertyChanged();
            }
        }

        public string ErrorLabelIsVisible
        {
            get { return _errorLabelIsVisible; }
            set
            {
                _errorLabelIsVisible = value;
                OnPropertyChanged();
            }
        }

        public ButtonCreationViewModel()
        {
            Title = "New Topic";
            Button = new IOTButton();
            ClickCommand = new Command(Save);
        }

        public async void Save()
        {
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Topic) && !String.IsNullOrEmpty(ImageName))
            {
                Button.Name = Name;
                Button.Topic = Topic;
                Button.ImageName = $"{ImageName.ToLower()}.png";

                await App.IOTDatabase.SaveItemAsync(Button);
                InvokeUpdate();
                PopStack();
            }
            else
            {
                ErrorLabelIsVisible = "true";
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