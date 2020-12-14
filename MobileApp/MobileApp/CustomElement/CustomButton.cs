using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileApp.CustomElement
{
    //NOTICE: EVERY PROPERTY WITHIN THE CLASS SHOULD START WITH "Custom", TO SEPERATE CUSTOM CLASS PROPERTIES FROM INHERITED PROPERTIES
    //THE SAME GOES FOR THE ELEMENTS YOU GENERATE.
    //GENERATE THEM WITH "Custom" IN FRONT OF THE DEFAULT ELEMENT NAME
    public class CustomButton : Button
    {
        public int CustomID { get; set; }

        public CustomButton()
        {
        }
    }
}
