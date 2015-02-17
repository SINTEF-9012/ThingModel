using System;
using System.Threading;
using NUnit.Framework;
using ThingModel.Builders;
using ThingModel.WebSockets;

namespace ThingModel.Specs
{
    [TestFixture]
    class WebSocketServers
    {
        private Server _server;

        private Client _clientA;
        private Client _clientB;
        
        private Warehouse _warehouseA;
        private Warehouse _warehouseB;

        public class WarehouseWait : IWarehouseObserver
        {
            private readonly AutoResetEvent _newEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _deleteEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _updatedEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _defineEvent = new AutoResetEvent(false);

            public void New(Thing thing, string sender)
            {
                _newEvent.Set();
            }

            public void Deleted(Thing thing, string sender)
            {
                _deleteEvent.Set();
            }

            public void Updated(Thing thing, string sender)
            {
                _updatedEvent.Set();
            }

            public void Define(ThingType thing, string sender)
            {
                _defineEvent.Set();
            }

            public bool WaitNew(int millisecondsTimeout = 3500)
            {
	            if (!_newEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitDeleted(int millisecondsTimeout = 3500)
            {
	            if (!_deleteEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitUpdated(int millisecondsTimeout = 3500)
            {
	            if (!_updatedEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }

            public bool WaitDefine(int millisecondsTimeout = 3500)
            {
	            if (!_defineEvent.WaitOne(millisecondsTimeout)) return false;
	            Thread.Sleep(5);
	            return true;
            }
           
        }

        private WarehouseWait _warehouseWaitA;
        private WarehouseWait _warehouseWaitB;

        private const string Path = "ws://localhost:4251/";

        [SetUp]
        protected void SetUp()
        {
            _warehouseA = new Warehouse();
            _warehouseB = new Warehouse();

            _server = new Server(Path);
            

            _clientA = new Client("UnitTestA", Path, _warehouseA);
            _clientB = new Client("UnitTestB", Path, _warehouseB);

            _warehouseWaitA = new WarehouseWait();
            _warehouseA.RegisterObserver(_warehouseWaitA);

            _warehouseWaitB = new WarehouseWait();
            _warehouseB.RegisterObserver(_warehouseWaitB);

            _server.Debug();
            _clientA.DebugMode = true;
            
            _clientA.WaitConnection();
            _clientB.WaitConnection();
        }

        [TearDown]
        protected void TearDown()
        {
            _clientA.Close();
            _clientB.Close();
            _server.Close();

            Thread.Sleep(2200);
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
	
			Thread.Sleep(1500);
            Assert.That(_clientA.WaitConnection(TimeSpan.FromSeconds(16)), Is.True);
        }

	    [Test]
	    public void TestClientReconnection()
	    {
			_clientB.Close();

			_warehouseA.RegisterThing(new Thing("groaw", new ThingType("duck")), false, true);
			_clientA.Send();

			_clientB.Connect();

            Assert.That(_clientB.WaitConnection(TimeSpan.FromSeconds(8)), Is.True);

            Assert.That(_warehouseWaitB.WaitNew(5000), Is.True);
	
			var thing = _warehouseB.GetThing("groaw");
            Assert.That(thing, Is.Not.Null);
            Assert.That(thing.Type, Is.Not.Null);
	    }

        [Test]
        public void TestNew()
        {
            var thing = new Thing("lapin");
            _warehouseA.RegisterThing(thing);
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);

            Assert.That(_warehouseB.GetThing("lapin"), Is.Not.Null);
        }
    
        [Test]
        public void SaveState()
        {
            _clientB.Close();
            _warehouseA.RegisterThing(new Thing("boat"));
            _clientA.Send();
            Thread.Sleep(2000);
            _clientB.Connect();

            Assert.That(_clientB.WaitConnection(TimeSpan.FromSeconds(8)), Is.True);

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
            Assert.That(_warehouseB.GetThing("boat"), Is.Not.Null);
        }

        [Test]
        public void TestDelete()
        {
            _warehouseA.RegisterThing(new Thing("lapin"));
            _clientA.Send();

            _warehouseWaitB.WaitNew();
            _warehouseB.RemoveThing(_warehouseB.GetThing("lapin"));
            Assert.That(_warehouseB.GetThing("lapin"), Is.Null);
            _clientB.Send();

            Assert.That( _warehouseWaitA.WaitDeleted(), Is.True);
            Assert.That(_warehouseA.GetThing("lapin"), Is.Null);
            
			_warehouseA.RegisterThing(new Thing("lapin"));
            _clientA.Send();

            Assert.That( _warehouseWaitB.WaitNew(), Is.True);
            Assert.That(_warehouseB.GetThing("lapin"), Is.Not.Null);
        }

		[Test]
		public void TestDeleteAndCreateInSameTransaction()
		{
			var lapin = new Thing("lapin");
			_warehouseA.RegisterThing(lapin);
			_warehouseA.RemoveThing(lapin);

			_warehouseA.RegisterThing(new Thing("ping"));
			_clientA.Send();

			_warehouseWaitB.WaitNew();

			Assert.That(_warehouseB.GetThing("lapin"), Is.Null);

		}
	    
        [Test]
        public void TestUpdate()
        {
            var lapin = new Thing("lapin");
            lapin.SetProperty(new Property.String("name", "roger"));
            _warehouseA.RegisterThing(lapin);
            _clientA.Send();
            _warehouseWaitB.WaitNew();

            var lapinB = _warehouseB.GetThing("lapin");
            lapinB.SetProperty(new Property.String("name", "groaw"));
            _warehouseB.NotifyThingUpdate(lapinB, "op");
			Console.WriteLine(" ------ send ------ ");
            _clientB.Send();

            Assert.That(_warehouseWaitA.WaitUpdated(), Is.True);

            Assert.That(_warehouseA.GetThing("lapin")
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
            _warehouseA.RegisterThing(thing, true, true);
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitDefine(), Is.True);

            var transportedType = _warehouseB.GetThingType("rabbit");
            Assert.That(transportedType, Is.Not.Null);

            Assert.That(type.Description, Is.EqualTo(transportedType.Description));

            Assert.That(transportedType.GetPropertyDefinition("position").Description, Is.EqualTo(position.Description));

            Assert.That(_warehouseB.GetThing("lapin").Type.Name, Is.EqualTo(type.Name));
        }

        [Test]
        public void TestTypeDefinitionWithoutThing()
        {
            var type = BuildANewThingType.Named("rabbit")
                .WhichIs("Just a rabbit")
                .ContainingA.LocationLatLng();

            _warehouseA.RegisterType(type);

            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitDefine(), Is.True);
        }

        [Test]
        public void TestTypeDefinitionRegisteredBeforeWarehouseCreation()
        {

            ThingType typeRabbit = BuildANewThingType.Named("rabbit");
            ThingType typeDuck = BuildANewThingType.Named("duck");
            var warehouse = new Warehouse();
            warehouse.RegisterType(typeRabbit);

            var client = new Client("test_unconnected", Path, warehouse);
            client.WaitConnection();
            Assert.That(client.IsConnected(), Is.True);

            var thing = BuildANewThing.As(typeDuck)
                .IdentifiedBy("superduck");
            warehouse.RegisterThing(thing);
            client.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);

            Assert.That(_warehouseB.GetThing("superduck"), Is.Not.Null);
            Assert.That(_warehouseB.GetThingType("duck"), Is.Not.Null);

            //Assert.That(_warehouseB.GetThingType("rabbit"), Is.Not.Null);
        }
        
        [Test]
        public void TestTypeDefinitionRegisteredBeforeConnection()
        {
            // Stop the server
            Assert.That(_clientA.IsConnected(), Is.True);
            _server.Close();
            Assert.That(_clientA.IsConnected(), Is.False);

            ThingType typeRabbit = BuildANewThingType.Named("rabbit");
            ThingType typeDuck = BuildANewThingType.Named("duck");
            var warehouse = new Warehouse();
            var client = new Client("test_unconnected", Path, warehouse);
            warehouse.RegisterType(typeRabbit);

            _server = new Server(Path);
            Thread.Sleep(2500);
            Assert.That(client.IsConnected(), Is.True);

            var thing = BuildANewThing.As(typeDuck)
                .IdentifiedBy("superduck");
            warehouse.RegisterThing(thing);
            client.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);

            Assert.That(_warehouseB.GetThing("superduck"), Is.Not.Null);
            Assert.That(_warehouseB.GetThingType("duck"), Is.Not.Null);

            Assert.That(_warehouseB.GetThingType("rabbit"), Is.Not.Null);
        }


	    [Test]
	    public void TestThingTypeChange()
	    {
		    var type = new ThingType("rabbit");
		    var thing = new Thing("lapin", type);
			thing.SetProperty(new Property.String("name", "Roger"));
			_warehouseA.RegisterThing(thing,true,true);
			_clientA.Send();

            Assert.That(_warehouseWaitB.WaitDefine(), Is.True);

			thing = new Thing("lapin");
			Assert.That(thing.GetProperty<Property.String>("name"), Is.Null);
		    _warehouseA.RegisterThing(thing);
			_clientA.Send();

            Assert.That(_warehouseWaitB.WaitUpdated(), Is.True);

			Assert.That(thing.GetProperty<Property.String>("name"), Is.Null);
			Assert.That(_warehouseB.GetThing("lapin").GetProperty<Property.String>("name"), Is.Null);
	    }

        [Test]
        public void TestNoChanges()
        {
            var pc = new Thing("computer");
            pc.SetProperty(new Property.String("name", "Interstella"));
            pc.SetProperty(new Property.Double("weight", 12));
            _warehouseA.RegisterThing(pc);
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
            
            _warehouseA.RegisterThing(_warehouseA.GetThing("computer"));
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitUpdated(500), Is.False);
            _clientA.Send();
            Assert.That(_warehouseWaitB.WaitUpdated(500), Is.False);

            pc.SetProperty(new Property.String("name", "Interstella2"));
            _warehouseA.NotifyThingUpdate(pc, "op");
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitUpdated(), Is.True);
        }

        [Test]
        public void TestThingsConnections()
        {
            var family = new Thing("family");
            var parentA = new Thing("Patrick");
            var parentB = new Thing("Bob");

            family.Connect(parentA);

            _warehouseA.RegisterThing(family);
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
            Assert.That(_warehouseB.GetThing("family").IsConnectedTo(_warehouseB.GetThing("Patrick")), Is.True);

            family.Connect(parentB);
            _warehouseA.RegisterThing(parentB);
            _warehouseA.NotifyThingUpdate(family, "op");
            _clientA.Send();


            Assert.That(_warehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_warehouseB.GetThing("family").IsConnectedTo(_warehouseB.GetThing("Bob")), Is.True);

            family.Disconnect(parentB);

            _warehouseA.NotifyThingUpdate(family, "op");

            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_warehouseB.GetThing("family").IsConnectedTo(_warehouseB.GetThing("Bob")), Is.False);

            _warehouseA.NotifyThingUpdate(family, "op");
            _clientA.Send();
            Assert.That(_warehouseWaitB.WaitUpdated(500), Is.False);

            family.Disconnect(parentA);
            family.Connect(parentB);
            _warehouseA.NotifyThingUpdate(family, "op");

            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitUpdated(), Is.True);
            Assert.That(_warehouseB.GetThing("family").IsConnectedTo(_warehouseB.GetThing("Bob")), Is.True);
        }
		
		[Test]
	    public void TestDeleteTwoConnectedThings()
	    {
		    var lapin = new Thing("lapin");
		    var canard = new Thing("canard");
			lapin.Connect(canard);

            _warehouseA.RegisterThing(lapin);
            _warehouseA.RegisterThing(canard);
            Assert.That(_warehouseWaitA.WaitUpdated(), Is.True);
            
			_clientA.Send();
            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
            Assert.That(_warehouseWaitA.WaitNew(), Is.True);

			_warehouseB.RemoveThing(_warehouseB.GetThing("lapin"));
			_warehouseB.RemoveThing(_warehouseB.GetThing("canard"));
            Assert.That(_warehouseA.Things.Count, Is.EqualTo(2));
			_clientB.Send();
            
            Assert.That(_warehouseWaitA.WaitDeleted(), Is.True);
            Assert.That(_warehouseWaitA.WaitUpdated(500), Is.False);

            Assert.That(_warehouseWaitA.WaitNew(500), Is.False);
			
            Assert.That(_warehouseA.Things.Count, Is.EqualTo(0));
            Assert.That(_warehouseB.Things.Count, Is.EqualTo(0));
	    }



        [Test]
        public void TestInvalidObject()
        {
            // Missing property
            var type = new ThingType("rabbit");
            type.DefineProperty(PropertyType.Create<Property.String>("name"));

            var thing = new Thing("a", type);
            _warehouseA.RegisterThing(thing,true,true);
            _clientA.Send();

            // Not sent
            Assert.That(_warehouseWaitB.WaitNew(500), Is.False);

            // Wrong property
            thing.SetProperty(new Property.Double("name", 42));
            _warehouseA.NotifyThingUpdate(thing, "op");
            _clientA.Send();

            // Still wrong
            Assert.That(_warehouseWaitB.WaitNew(500), Is.False);

            thing.SetProperty(new Property.String("name", "Roger"));
            _warehouseA.NotifyThingUpdate(thing, "op");
            _clientA.Send();

            // Now, it's OK
            Assert.That(_warehouseWaitB.WaitNew(), Is.True);
        }

	    [Test]
	    public void TestStressLoop()
	    {
		    var canard = Builders.BuildANewThing.As(new ThingType("rabbit"))
			    .IdentifiedBy("canard")
			    .ContainingA.String("name", "canard_0");

			_warehouseA.RegisterThing(canard);
			_clientA.Send();

		    for (var i = 0; i < 50; ++i)
		    {
			    canard.String("name", "canard_" + i);
				_warehouseA.RegisterThing(canard);
				_clientA.Send();
			Thread.Sleep(5);
		    }

		    var j = 0;
		    _warehouseB.Events.OnReceivedUpdate += (sender, args) =>
		    {
			    _warehouseB.RegisterType(Builders.BuildANewThingType.Named("lapin_" + ++j));
			    _clientB.Send();
		    };

			Thread.Sleep(500);
	    }

        [Test]
        public void TestPotentiallyNullProperties()
        {
            
            var thingA = new Thing("twingo");
            thingA.Double("speed", 0.0);
            thingA.Boolean("accelerating", false);
            thingA.Int("nbElectricalEngines", 0);
            
            _warehouseA.RegisterThing(thingA);
            _clientA.Send();

            Assert.That(_warehouseWaitB.WaitNew(), Is.True);

            var thingB = _warehouseB.GetThing("twingo");
            Assert.That(thingB, Is.Not.Null);

            Assert.That(thingB.Double("speed"), Is.EqualTo(0.0));
            Assert.That(thingB.Boolean("accelerating"), Is.False);
            Assert.That(thingB.Int("nbElectricalEngines"), Is.EqualTo(0));

            var warehouseC = new Warehouse();
            var clientC = new Client("UnitTestC", Path, warehouseC);
            clientC.WaitConnection();

            var warehouseWaitC = new WarehouseWait();
            warehouseC.RegisterObserver(warehouseWaitC);

            Assert.That(warehouseWaitC.WaitNew(), Is.True);

            var thingC = warehouseC.GetThing("twingo");
            Assert.That(thingC, Is.Not.Null);

            Assert.That(thingC.Double("speed"), Is.EqualTo(0.0));
            Assert.That(thingC.Boolean("accelerating"), Is.False);
            Assert.That(thingC.Int("nbElectricalEngines"), Is.EqualTo(0));
        }
    }
}
