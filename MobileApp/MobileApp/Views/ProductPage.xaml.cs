using MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductPage : ContentPage
    {
        public ProductPage()
        {
            InitializeComponent();
            this.BindingContext = new ProductViewModel();
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {

        }

        private void DatePicker_DateSelected_1(object sender, DateChangedEventArgs e)
        {

        }

        private void DatePicker_DateSelected_2(object sender, DateChangedEventArgs e)
        {

        }
    }
}