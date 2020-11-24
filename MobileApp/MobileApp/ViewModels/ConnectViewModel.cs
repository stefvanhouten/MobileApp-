using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ConnectViewModel : BaseViewModel
    {
        
        public Command ConnectClickCommand { get; set; }
        public string PortInput { get; set; }
        public string IPInput { get; set; }

      
        public ConnectViewModel()
        {
            Title = "ConnectPage";
            PortInput = "1883";
            IPInput = "";
            ConnectClickCommand = new Command(ConnectClick);
            
           
        }

       

        public void ConnectClick()
        {
            Console.WriteLine(PortInput);

        }
       
        
    }
}