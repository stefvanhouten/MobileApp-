using MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {

        public DashboardPage()
        {
            InitializeComponent();
            this.BindingContext = new DashboardViewModel();
        }
    }
}