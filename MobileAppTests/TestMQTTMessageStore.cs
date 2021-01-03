using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobileAppTests
{
    [TestClass]
    public class TestMQTTMessageStore
    {
        [TestMethod]
        public void AddMessage()
        {
            List<MQTTMessage> expected = new List<MQTTMessage>();
            MQTTMessageStore messageStore = new MQTTMessageStore();
            MQTTMessage msg = new MQTTMessage("test", "test", DateTime.Now);
            messageStore.AddMessage(msg);
            expected.Add(msg);
            Assert.AreEqual(expected.Count, messageStore.Messages.Count);
        }

        [TestMethod]
        public void GetLastAddedItemByTopicName()
        {
            MQTTMessageStore messageStore = new MQTTMessageStore();
            MQTTMessage msg = new MQTTMessage("test", "test", DateTime.Now);
            messageStore.AddMessage(msg);

            MQTTMessage result = messageStore.GetLatestMessageFromTopic("test");
            Assert.AreEqual(result, msg);
        }

        [TestMethod]
        public void GetAllMessagesByTopicName()
        {
            MQTTMessageStore messageStore = new MQTTMessageStore();
            messageStore.AddMessage(new MQTTMessage("test", "test", DateTime.Now));
            Thread.Sleep(100);
            messageStore.AddMessage(new MQTTMessage("test", "test", DateTime.Now));
            List<MQTTMessage> result = messageStore.GetAllMessagesFromTopic("test");
            Assert.IsTrue(result.Count == 2);
        }

    }
}
