using SQLite;

namespace MobileApp.Models
{
    public class IOTButton
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string ImageName { get; set; }
    }
}
