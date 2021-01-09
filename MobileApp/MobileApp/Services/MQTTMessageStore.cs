using System.Collections.Generic;
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

            if (!IsValidMessage(message))
            {
                return false;
            }

            lock (this.Messages)
            {
                this.Messages.Add(message);
                this.Truncate();
            }

            return true;
        }

        private bool IsValidMessage(MQTTMessage message)
        {
            if (this.IsDuplicate(message) || Messages == null)
            {
                return false;
            }

            return true;
        }

        private bool IsDuplicate(MQTTMessage message)
        {
            lock (this.Messages)
            {
                foreach (MQTTMessage storedMessage in this.Messages)
                {
                    if (storedMessage != null)
                    {
                        if (storedMessage.Compare().Equals(message.Compare()))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Truncate()
        {
            while (this.Messages.Count >= 50)
            {
                this.Messages.RemoveAt(0);
            }
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
