using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileApp.Services
{
    public class MQTTMessageStore
    {
        public List<MQTTMessage> Messages { get; private set; }

        public MQTTMessageStore()
        {
            this.Messages = new List<MQTTMessage>();
        }

        public bool AddMessage(MQTTMessage message)
        {
            if (!this.CheckIfDuplicate(message))
            {
                if(message != null)
                {
                    this.Messages.Add(message);
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool CheckIfDuplicate(MQTTMessage message)
        {
            foreach (MQTTMessage storedMessage in this.Messages.ToList())
            {
                if (storedMessage.Compare().Equals(message.Compare()))
                {
                    return true;
                }
            }
            return false;
        }

        public List<MQTTMessage> GetAllMessagesFromTopic(string topic)
        {
            List<MQTTMessage> AllMessagesMatchingTopic = new List<MQTTMessage>();

            foreach (MQTTMessage message in this.Messages.ToList())
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
