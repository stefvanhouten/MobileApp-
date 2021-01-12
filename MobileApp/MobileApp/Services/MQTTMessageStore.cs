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

        /// <summary>
        /// Adds a MQTTMessage to the Messages list.
        /// Locks the Messages list while perfomerming operations to the list.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Boolean depending on if message was added sucessfully or not.</returns>
        public bool AddMessage(MQTTMessage message)
        {
            if (!IsValidMessage(message))
            {
                return false;
            }

            /* It is important to lock the Messages list because the asyncronous nature of our application.
             * We do not want to add a message to the list while we are performing another operation with said list.
             * Doing so would also crash the application as provide unwanted behaviour
             */
            lock (this.Messages)
            {
                this.Messages.Add(message);
                this.Truncate(50);
            }

            return true;
        }

        /// <summary>
        /// Validates whether the MQTTMessage is valid. 
        /// A MQTTMessage is valid when it is unique and when said message is not null. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Boolean based on whether the message is valid or not.</returns>
        private bool IsValidMessage(MQTTMessage message)
        {
            if (this.IsDuplicate(message) || message == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validated whether a MQTTMessage is a duplicate or not.
        /// Validation is based on topic, payload and DateTime.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Boolean based on whether the message is a duplicate or not.</returns>
        private bool IsDuplicate(MQTTMessage message)
        {
            lock (this.Messages)
            {
                foreach (MQTTMessage storedMessage in this.Messages)
                {
                    if (storedMessage != null)
                    {
                        if (storedMessage.GetFullMessageAsString().Equals(message.GetFullMessageAsString()))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Truncates the Messages list up to a given index.
        /// </summary>
        /// <param name="index"></param>
        private void Truncate(int index)
        {
            while (this.Messages.Count >= index)
            {
                this.Messages.RemoveAt(0);
            }
        }

        /// <summary>
        /// Returns all messages from a given topic.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns>null or a list of MQTTMessages.</returns>
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

        /// <summary>
        /// Returns the latest message of a given topic.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns>null or the latest MQTTMessage of the given topic.</returns>
        public MQTTMessage GetLatestMessageFromTopic(string topic)
        {
            return this.Messages.LastOrDefault(msg => msg?.Topic == topic);
        }
    }
}