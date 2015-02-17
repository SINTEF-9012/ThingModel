using System;
using System.Threading;
using NUnit.Framework;
using ThingModel.Builders;
using ThingModel.WebSockets;

namespace ThingModel.Specs
{
    [TestFixture]
    class ClientEntrerpriseEditionTest
    {
        private Server _server;
        private ClientEnterpriseEdition _client;
        private Warehouse _warehouse;

        private const string Path = "ws://localhost:4251/";

        [SetUp]
        protected void SetUp()
        {
            _server = new Server(Path);
            _warehouse = new Warehouse();
            _client = new ClientEnterpriseEdition("UnitTestA", Path, _warehouse);
            _client.WaitConnection();
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

        [Test]
        public void TestTypeDeclaration()
        {
            var _warehouseB = new Warehouse();
            var _clientB = new Client("ClientB", Path, _warehouseB);

            var _warehouseWaitB = new WebSocketServers.WarehouseWait();
            _warehouseB.RegisterObserver(_warehouseWaitB);

            _clientB.WaitConnection();

            Assert.That(_client.IsConnected(), Is.True);
            Assert.That(_clientB.IsConnected(), Is.True);

            ThingType type = BuildANewThingType.Named("rabbit")
                .ContainingA.LocationLatLng();

            Thing thing = BuildANewThing.As(type).IdentifiedBy("lapin")
                .ContainingA.Location(new Location.LatLng(1,2));

            Assert.That(type.Check(thing), Is.True);

            _warehouse.RegisterThing(thing);
            _client.Send();
            
            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
            Assert.That(_warehouseWaitB.WaitDefine(), Is.True);

            Assert.That(_warehouseB.GetThing("lapin").Type, Is.Not.Null);
            Assert.That(_warehouseB.GetThing("lapin").Type.Name, Is.EqualTo("rabbit"));
        }
    }
}
