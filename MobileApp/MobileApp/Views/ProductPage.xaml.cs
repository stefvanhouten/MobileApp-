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
        public int Milliseconds { get; }

        public ProductPage()
        {
            InitializeComponent();
            this.BindingContext = new ProductViewModel();
        }

        public void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
           DateLabel.Text = e.NewDate.ToString();
           

        }


    }
}