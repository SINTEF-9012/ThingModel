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

        private Client _clientA;
        private Client _clientB;
        
        private Wharehouse _wharehouseA;
        private Wharehouse _wharehouseB;

        private class WharehouseWait : IWharehouseObserver
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

            public bool WaitNew(int millisecondsTimeout = 2500)
            {
	            if (!_newEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitDeleted(int millisecondsTimeout = 2500)
            {
	            if (!_deleteEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitUpdated(int millisecondsTimeout = 2500)
            {
	            if (!_updatedEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitDefine(int millisecondsTimeout = 2500)
            {
	            if (!_defineEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }
           
        }

        private WharehouseWait _wharehouseWaitA;
        private WharehouseWait _wharehouseWaitB;

        private const string Path = "ws://localhost:4251/";

        [SetUp]
        protected void SetUp()
        {
            _wharehouseA = new Wharehouse();
            _wharehouseB = new Wharehouse();

            _server = new Server(Path);
            

            _clientA = new Client("UnitTestA", Path, _wharehouseA);
            _clientB = new Client("UnitTestB", Path, _wharehouseB);

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

            Thread.Sleep(200);
        }

        [Test]
        public void TestConnection()
        {
            Assert.That(_clientA.IsConnected(), Is.True);
        }

        [Test]
        public void TestServerReconnection()
        {
            Assert.That(_clientA.IsConnected(), Is.True);
            _server.Close();
            Assert.That(_clientA.IsConnected(), Is.False);
            _server = new Server(Path);
	
			Thread.Sleep(2500);
            Assert.That(_clientA.IsConnected(), Is.True);
        }

	    [Test]
	    public void TestClientReconnection()
	    {
			_clientB.Close();

			_wharehouseA.RegisterThing(new Thing("groaw", new ThingType("duck")), false, true);
			_clientA.Send();

			_clientB.Connect();


            Assert.That(_wharehouseWaitB.WaitNew(5000), Is.True);
	
			var thing = _wharehouseB.GetThing("groaw");
            Assert.That(thing, Is.Not.Null);
            Assert.That(thing.Type, Is.Not.Null);
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

            Assert.That( _wharehouseWaitA.WaitDeleted(), Is.True);
            Assert.That(_wharehouseA.GetThing("lapin"), Is.Null);
            
			_wharehouseA.RegisterThing(new Thing("lapin"));
            _clientA.Send();

            Assert.That( _wharehouseWaitB.WaitNew(), Is.True);
            Assert.That(_wharehouseB.GetThing("lapin"), Is.Not.Null);
        }

		[Test]
		public void TestDeleteAndCreateInSameTransaction()
		{
			var lapin = new Thing("lapin");
			_wharehouseA.RegisterThing(lapin);
			_wharehouseA.RemoveThing(lapin);

			_wharehouseA.RegisterThing(new Thing("ping"));
			_clientA.Send();

			_wharehouseWaitB.WaitNew();

			Assert.That(_wharehouseB.GetThing("lapin"), Is.Null);

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
            _wharehouseB.NotifyThingUpdate(lapinB);
			Console.WriteLine(" ------ send ------ ");
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
            thing.SetProperty(new Property.String("name", "Roger"));
            _wharehouseA.RegisterThing(thing, true, true);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitDefine(), Is.True);

            var transportedType = _wharehouseB.GetThingType("rabbit");
            Assert.That(transportedType, Is.Not.Null);

            Assert.That(type.Description, Is.EqualTo(transportedType.Description));

            Assert.That(transportedType.GetPropertyDefinition("position").Description, Is.EqualTo(position.Description));

            Assert.That(_wharehouseB.GetThing("lapin").Type.Name, Is.EqualTo(type.Name));
        }

	    [Test]
	    public void TestThingTypeChange()
	    {
		    var type = new ThingType("rabbit");
		    var thing = new Thing("lapin", type);
			thing.SetProperty(new Property.String("name", "Roger"));
			_wharehouseA.RegisterThing(thing,true,true);
			_clientA.Send();

            Assert.That(_wharehouseWaitB.WaitDefine(), Is.True);

			thing = new Thing("lapin");
			Assert.That(thing.GetProperty<Property.String>("name"), Is.Null);
		    _wharehouseA.RegisterThing(thing);
			_clientA.Send();

            Assert.That(_wharehouseWaitB.WaitUpdated(), Is.True);

			Assert.That(thing.GetProperty<Property.String>("name"), Is.Null);
			Assert.That(_wharehouseB.GetThing("lapin").GetProperty<Property.String>("name"), Is.Null);
	    }

        [Test]
        public void TestNoChanges()
        {
            var pc = new Thing("computer");
            pc.SetProperty(new Property.String("name", "Interstella"));
            pc.SetProperty(new Property.Double("weight", 12));
            _wharehouseA.RegisterThing(pc);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitNew(), Is.True);
            
            _wharehouseA.RegisterThing(_wharehouseA.GetThing("computer"));
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitUpdated(500), Is.False);
            _clientA.Send();
            Assert.That(_wharehouseWaitB.WaitUpdated(500), Is.False);

            pc.SetProperty(new Property.String("name", "Interstella2"));
            _wharehouseA.NotifyThingUpdate(pc);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitUpdated(), Is.True);
        }

        [Test]
        public void TestThingsConnections()
        {
            var family = new Thing("family");
            var parentA = new Thing("Patrick");
            var parentB = new Thing("Bob");

            family.Connect(parentA);

            _wharehouseA.RegisterThing(family);
            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitNew(), Is.True);
            Assert.That(_wharehouseB.GetThing("family").IsConnectedTo(_wharehouseB.GetThing("Patrick")), Is.True);

            family.Connect(parentB);
            _wharehouseA.RegisterThing(parentB);
            _wharehouseA.NotifyThingUpdate(family);
            _clientA.Send();


            Assert.That(_wharehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_wharehouseB.GetThing("family").IsConnectedTo(_wharehouseB.GetThing("Bob")), Is.True);

            family.Disconnect(parentB);

            _wharehouseA.NotifyThingUpdate(family);

            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_wharehouseB.GetThing("family").IsConnectedTo(_wharehouseB.GetThing("Bob")), Is.False);

            _wharehouseA.NotifyThingUpdate(family);
            _clientA.Send();
            Assert.That(_wharehouseWaitB.WaitUpdated(500), Is.False);

            family.Disconnect(parentA);
            family.Connect(parentB);
            _wharehouseA.NotifyThingUpdate(family);

            _clientA.Send();

            Assert.That(_wharehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_wharehouseB.GetThing("family").IsConnectedTo(_wharehouseB.GetThing("Bob")), Is.True);
        }

        [Test]
        public void TestInvalidObject()
        {
            // Missing property
            var type = new ThingType("rabbit");
            type.DefineProperty(PropertyType.Create<Property.String>("name"));

            var thing = new Thing("a", type);
            _wharehouseA.RegisterThing(thing,true,true);
            _clientA.Send();

            // Not sent
            Assert.That(_wharehouseWaitB.WaitNew(500), Is.False);

            // Wrong property
            thing.SetProperty(new Property.Double("name", 42));
            _wharehouseA.NotifyThingUpdate(thing);
            _clientA.Send();

            // Still wrong
            Assert.That(_wharehouseWaitB.WaitNew(500), Is.False);

            thing.SetProperty(new Property.String("name", "Roger"));
            _wharehouseA.NotifyThingUpdate(thing);
            _clientA.Send();

            // Now, it's OK
            Assert.That(_wharehouseWaitB.WaitNew(), Is.True);
        }
    }
}
