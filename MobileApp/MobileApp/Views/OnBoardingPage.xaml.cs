using MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnBoardingPage : ContentPage
    {
        public OnBoardingPage()
        {
            InitializeComponent();
            this.BindingContext = new OnBoardingViewModel();
        }
    }
}