using System;
using System.Threading;
using NUnit.Framework;
using ThingModel.WebSockets;

namespace ThingModel.Specs
{
    [TestFixture]
    class ClientEntrerpriseEditionTest
    {
        private Server _server;
        private ClientEnterpriseEdition _client;

        private const string Path = "ws://localhost:4251/";

        [SetUp]
        protected void SetUp()
        {
            _server = new Server(Path);
            _client = new ClientEnterpriseEdition("UnitTestA", Path, new Warehouse());
        }
        
        [TearDown]
        protected void TearDown()
        {
            _client.Close();
            _server.Close();

            Thread.Sleep(200);
        }

        [Test]
        public void TestLive()
        {
            Assert.IsTrue(_client.IsLive);

            _client.Load(new DateTime(1980,5,22));

            Assert.IsFalse(_client.IsLive);

            _client.Live();
            Assert.IsTrue(_client.IsLive);
            
            _client.Live();
            Assert.IsTrue(_client.IsLive);
        }

        [Test]
        public void TestPlayPause()
        {
            Assert.IsFalse(_client.IsPaused);

            _client.Pause();

            Assert.IsTrue(_client.IsPaused);
            
            _client.Play();
            
            Assert.IsFalse(_client.IsPaused);
            
            _client.Pause();
            _client.Pause();
            
            _client.Live();
            Assert.IsFalse(_client.IsPaused);
        }

        [Test]
        public void TestExceptionWhenPaused()
        {
            _client.Pause();

            Assert.Throws<Exception>(() => _client.Send());
        }
        
        [Test]
        public void TestExceptionWhenPast()
        {
            _client.Send();

            _client.Load(new DateTime(1980,5,22));

            Assert.Throws<Exception>(() => _client.Send());
        }
    }
}
