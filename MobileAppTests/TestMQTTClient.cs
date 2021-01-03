using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileApp.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobileAppTests
{
    [TestClass]
    public class TestMQTTClient
    {
        private string IP { get; } = "192.168.1.71";
        private int PORT { get; } = 1883;
        private string TESTCHANNEL { get; } = "unittest";

        [TestMethod]
        public async Task ConnectToClient()
        {
            MQTTClient client = new MQTTClient();
            bool isConnected = await client.Connect(this.IP, this.PORT);
            Assert.IsTrue(isConnected);
        }

        [TestMethod]
        public async Task FailedConnectionToClient()
        {
            MQTTClient client = new MQTTClient();
            bool isConnected = await client.Connect(this.IP, this.PORT);
            Assert.IsTrue(isConnected);
        }

        [TestMethod]
        public async Task DisconnectFromClient()
        {
            MQTTClient client = new MQTTClient();
            bool isConnected = await client.Connect(this.IP, this.PORT);
            Assert.IsTrue(isConnected);
            isConnected = await client.Disconnect();
            Assert.IsFalse(isConnected);
        }

        [TestMethod]
        public async Task ConnectoToMockup()
        {
            MQTTMockClient client = new MQTTMockClient();
            bool isConnected = await client.Connect(this.IP, this.PORT);
            Assert.IsTrue(isConnected);
        }

        [TestMethod]
        public async Task SubscribeAndSendMessage()
        {
            MQTTClient client = new MQTTClient();
            _ = await client.Connect(this.IP, this.PORT);
            client.Subscribe(this.TESTCHANNEL);
            client.Publish(this.TESTCHANNEL, "hello world");
            Thread.Sleep(100);
            MQTTMessage message = client.MQTTMessageStore.GetLatestMessageFromTopic(this.TESTCHANNEL);
            Assert.AreEqual("hello world", message.Message);
        }

        [TestMethod]
        public async Task SubscribeAndSendMessageMockup()
        {
            MQTTMockClient client = new MQTTMockClient();
            _ = await client.Connect(this.IP, this.PORT);
            client.Subscribe(this.TESTCHANNEL);
            client.Publish(this.TESTCHANNEL, "hello world");
            MQTTMessage message = client.MQTTMessageStore.GetLatestMessageFromTopic(this.TESTCHANNEL);
            Assert.AreEqual("hello world", message.Message);
        }

        [TestMethod]
        public async Task PreventDuplicateMessage()
        {
            MQTTMockClient client = new MQTTMockClient();
            _ = await client.Connect(this.IP, this.PORT);
            MQTTMessage duplicateMessage = new MQTTMessage(this.TESTCHANNEL, "hello world", DateTime.Now);
            client.MQTTMessageStore.AddMessage(duplicateMessage);
            Assert.IsFalse(client.MQTTMessageStore.AddMessage(duplicateMessage));
        }
    }
}
