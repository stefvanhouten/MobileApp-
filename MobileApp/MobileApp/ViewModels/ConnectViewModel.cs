using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ConnectViewModel : BaseViewModel, INotifyPropertyChanged
    {
        /* Code seems to be working for the most part, however it is not working as intended. 
         * Whenever invalid input is provided for IPInput the message is raised which is good, 
         * when the user starts typing the message goes away, also good.
         * However, after the message goes away the small white underline of the label remains. 
         * This needs to be fixed by either a conditional render or another solution.
         * Also It is still possible to provide invalid port input, this should also raise a message. 
         */
        public Command ConnectClickCommand { get; set; }
        public Command DeleteError { get; set; } //Another unused thing
        public int PortInput { get; set; } = 1883; //Default port
        private string _errormsg; //Private fields/properties should always be on the top off the class
        private string _bgcolor; //Private fields/properties should always be on the top off the class
        private string _ipinput; //Private fields/properties should always be on the top off the class
        public int InputCounter = 0; //Unused field, can be removed
        public int OldInput; //Unused field, can be removed

        public string IPInput {
            get
            {
                return _ipinput;
            }
            set
            {
                _ipinput = value;
                IpInput_TextChanged();
            }
        
        }

        //Methods should be below the constructor and not mixed in with properties
        public void IpInput_TextChanged()
        {
            ErrorMsg = "";
            BgColor = "White";
        }


        public string ErrorMsg
        {
            get { return _errormsg; }
            //Be consistent with your whitespaces and stuff, rule 25:35 uses a different syntax than this property
            set
            {
                _errormsg = value;
                OnPropertyChanged();
            }
        }
        public string BgColor
        {
            get { return _bgcolor; }
            //Be consistent with your whitespaces and stuff, rule 25:35 uses a different syntax than this property
            set
            {
                _bgcolor = value;
                OnPropertyChanged();
            }
        }
        
        //??????????????
        public void DoDeleteError()
        {

        }


        public ConnectViewModel()
        {
            Title = "ConnectPage";
            ConnectClickCommand = new Command(ConnectClick);
            DeleteError = new Command(DoDeleteError); //?????????

        }//Always have at least one whitespace between methods/constructor. Can be more but be consistent
        //Compares Input to Valid IPAdresses
        private bool IsValidInput()
        { 
            IPAddress ip;

            return IPAddress.TryParse(IPInput, out ip);
        }
        
        public void ConnectClick()
        {
           
            if (IsValidInput())
            {
                App.ServerIP = this.IPInput;
                App.ServerPort = this.PortInput;
                Application.Current.MainPage.Navigation.PushAsync(new DashboardPage(), false);
                Console.WriteLine(PortInput);
            }else
            {
                //All the code over here is off indentwise. Fix this by using tabs
               ErrorMsg  = "Error IP is not Valid";
                BgColor = "red";
            }
        }
       
    }
}