using System.Collections.ObjectModel;
using System.Linq;

namespace MobileApp.Services
{
    public class MQTTMessageStore
    {
        public ObservableCollection<MQTTMessage> Messages { get; private set; } = new ObservableCollection<MQTTMessage>();

        public bool AddMessage(MQTTMessage message)
        {
            if (!this.CheckIfDuplicate(message))
            {
                this.Messages.Add(message);
                return true;
            }
            return false;
        }

        private bool CheckIfDuplicate(MQTTMessage message)
        {
            foreach (MQTTMessage storedMessage in this.Messages)
            {
                if (storedMessage.Compare().Equals(message.Compare()))
                {
                    return true;
                }
            }
            return false;
        }

        public ObservableCollection<MQTTMessage> GetAllMessagesFromTopic(string topic)
        {
            ObservableCollection<MQTTMessage> AllMessagesMatchingTopic = new ObservableCollection<MQTTMessage>();

            foreach (MQTTMessage message in this.Messages)
            {
                if (message.Topic.Equals(topic))
                {
                    AllMessagesMatchingTopic.Add(message);
                }
            }

            return AllMessagesMatchingTopic;
        }

        public MQTTMessage GetLatestMessageFromTopic(string topic)
        {
            return this.Messages.LastOrDefault(msg => msg.Topic == topic);
        }
    }
}
