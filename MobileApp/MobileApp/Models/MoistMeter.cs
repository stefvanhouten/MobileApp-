using SQLite;
using System;

namespace MobileApp.Models
{
    public class MoistMeter
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string MoisturePercentage { get; set; }
        public DateTime DateTime { get; set; }
    }
}