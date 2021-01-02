using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MobileApp.Models;

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
                    lock (this.Messages)
                    {
                        this.Messages.Add(message);
                        this.Truncate();
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        private void Truncate()
        {
            while (this.Messages.Count >= 15)
            {
                this.Messages.RemoveAt(0);
            }
        }

        private bool CheckIfDuplicate(MQTTMessage message)
        {
            lock (this.Messages)
            {
                foreach (MQTTMessage storedMessage in this.Messages)
                {
                    if(storedMessage != null)
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

        public List<MQTTMessage> GetAllMessagesFromTopic(string topic)
        {
            List<MQTTMessage> AllMessagesMatchingTopic = new List<MQTTMessage>();

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
