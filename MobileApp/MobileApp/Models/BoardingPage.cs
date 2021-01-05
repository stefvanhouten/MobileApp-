using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.Models
{
    public class BoardingPage
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 200;
    }
}
