using SQLite;
using System;

namespace MobileApp.Models
{
    public class MoistMeter
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Target { get; set; }
        public double MoisturePercentage { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime DateTime { get; set; }
    }
}