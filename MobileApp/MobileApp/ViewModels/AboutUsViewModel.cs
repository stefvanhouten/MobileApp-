using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class AboutUsViewModel : BaseViewModel
    {
        public ICommand OpenWebCommand { get; }

        public AboutUsViewModel()
        {
            Title = "About us";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/stefvanhouten/IOTMobileApp"));
        }
    }
}