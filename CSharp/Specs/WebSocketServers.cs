using System;
using System.Threading;
using NUnit.Framework;
using ThingModel.WebSockets;

namespace ThingModel.Specs
{
    [TestFixture]
    class WebSocketServers
    {
        private Server _server;

        private WebSockets.Client _clientA;
        private WebSockets.Client _clientB;
        
        private Wharehouse _wharehouseA;
        private Wharehouse _wharehouseB;

        private class WharehouseWait : IThingModelObserver
        {
            private readonly AutoResetEvent _newEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _deleteEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _updatedEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _defineEvent = new AutoResetEvent(false);

            public void New(Thing thing)
            {
                _newEvent.Set();
            }

            public void Deleted(Thing thing)
            {
                _deleteEvent.Set();
            }

            public void Updated(Thing thing)
            {
                _updatedEvent.Set();
            }

            public void Define(ThingType thing)
            {
                _defineEvent.Set();
            }

            public bool WaitNew(int millisecondsTimeout = 800)
            {
                return _newEvent.WaitOne(millisecondsTimeout);
            }

            public bool WaitDeleted(int millisecondsTimeout = 800)
            {
                return _deleteEvent.WaitOne(millisecondsTimeout);
            }

            public bool WaitUpdated(int millisecondsTimeout = 800)
            {
                return _updatedEvent.WaitOne(millisecondsTimeout);
            }

            public bool WaitDefine(int millisecondsTimeout = 800)
            {
                return _defineEvent.WaitOne(millisecondsTimeout);
            }
           
        }

        private WharehouseWait _wharehouseWaitA;
        private WharehouseWait _wharehouseWaitB;

        [SetUp]
        protected void SetUp()
        {
            _wharehouseA = new Wharehouse();
            _wharehouseB = new Wharehouse();

            const string path = "ws://localhost:4251/";

            _server = new Server(path);
            

            _clientA = new WebSockets.Client("UnitTestA", path, _wharehouseA);
            _clientB = new WebSockets.Client("UnitTestB", path, _wharehouseB);

            _wharehouseWaitA = new WharehouseWait();
            _wharehouseA.RegisterObserver(_wharehouseWaitA);

            _wharehouseWaitB = new WharehouseWait();
            _wharehouseB.RegisterObserver(_wharehouseWaitB);

            _server.Debug();
            _clientA.Debug();
        }

        [TearDown]
        protected void TearDown()
        {
            _clientA.Close();
            _clientB.Close();
            _server.Close();
        }

        [Test]
        public void TestConnection()
        {
            Assert.That(_clientA.IsConnected(), Is.True);
        }

        [Test]
        public void TestReconnection()
        {
            Assert.That(_clientA.IsConnected(), Is.True);
            _server.Close();
            Assert.That(_clientA.IsConnected(), Is.False);
            _server = new Server("ws://localhost:4251/");

            Thread.Sleep(2000);
            Assert.That(_clientA.IsConnected(), Is.True);
        }

        [Test]
        public void TestNew()
        {
            var thing = new Thing("lapin");
            _wharehouseA.RegisterThing(thing);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitNew(), Is.True);

            Assert.That(_wharehouseB.GetThing("lapin"), Is.Not.Null);
        }
    
        [Test]
        public void SaveState()
        {
            _clientB.Close();
            _wharehouseA.RegisterThing(new Thing("boat"));
            _clientA.Send();
            _clientB.Connect();

            Assert.That(_wharehouseWaitB.WaitNew(), Is.True);
            Assert.That(_wharehouseB.GetThing("boat"), Is.Not.Null);
        }

        [Test]
        public void TestDelete()
        {
            _wharehouseA.RegisterThing(new Thing("lapin"));
            _clientA.Send();

            _wharehouseWaitB.WaitNew();
            _wharehouseB.RemoveThing(_wharehouseB.GetThing("lapin"));
            Assert.That(_wharehouseB.GetThing("lapin"), Is.Null);
            _clientB.Send();

            Assert.That( _wharehouseWaitA.WaitDeleted(1200), Is.True);
            Assert.That(_wharehouseA.GetThing("lapin"), Is.Null);
        }

        [Test]
        public void TestUpdate()
        {
            var lapin = new Thing("lapin");
            lapin.SetProperty(new Property.String("name", "roger"));
            _wharehouseA.RegisterThing(lapin);
            _clientA.Send();
            _wharehouseWaitB.WaitNew();

            var lapinB = _wharehouseB.GetThing("lapin");
            lapinB.SetProperty(new Property.String("name", "groaw"));
            _clientB.Send();

            Assert.That(_wharehouseWaitA.WaitUpdated(), Is.True);

            Assert.That(_wharehouseA.GetThing("lapin")
                .GetProperty<Property.String>("name").Value, Is.EqualTo("groaw"));
        }

        [Test]
        public void TestTypeDefinition()
        {
            var type = new ThingType("rabbit");
            type.Description = "Just a rabbit";

            var name = PropertyType.Create<Property.String>("name");
            name.Required = true;
            name.Name = "Name";
            name.Description = "Rabbit's name";
            type.DefineProperty(name);

            var position = PropertyType.Create<Property.Location>("position");
            position.Required = false;
            position.Description = "Localization of the rabbit";
            type.DefineProperty(position);

            var thing = new Thing("lapin", type);
            _wharehouseA.RegisterThing(thing, true, true);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitDefine(), Is.True);

            var transportedType = _wharehouseB.GetThingType("rabbit");
            Assert.That(transportedType, Is.Not.Null);

            Assert.That(type.Description, Is.EqualTo(transportedType.Description));

            Assert.That(transportedType.GetPropertyDefinition("position").Description, Is.EqualTo(position.Description));

            Assert.That(_wharehouseB.GetThing("lapin").Type.Name, Is.EqualTo(type.Name));
        }
    }
}
