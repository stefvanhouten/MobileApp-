using System;
using System.Text;

namespace MobileApp.Services
{
    public class MQTTMessage
    {
        public string Topic { get; private set; }
        public string Message { get; private set; }
        public StringBuilder FormattedString { get; private set; }
        public DateTime Date { get; private set; }

        public MQTTMessage(string topic, string message, DateTime date)
        {
            this.Topic = topic;
            this.Message = message;
            this.Date = date;

            StringBuilder str = new StringBuilder();
            str.AppendLine($"Topic : { this.Topic}");
            str.AppendLine($"Time : { this.Date }");
            str.AppendLine($"Payload : {this.Message}");
            this.FormattedString = str;
        }

        /// <summary>
        /// Concatenate all important properties into one string. 
        /// </summary>
        /// <returns>String with all important properties</returns>
        public string GetFullMessageAsString()
        {
            return $"{this.Topic} {this.Message} {this.Date}";
        }
    }
}
