
using System;
using NUnit.Framework;
using ThingModel.Proto;

namespace ThingModel.Specs
{
    [TestFixture]
    class ProtoConversions
    {
        private Warehouse _warehouse;
        private FromProtobuf _fromProtobuf;
        private ToProtobuf _toProtobuf;

        [SetUp]
        protected void SetUp()
        {
            _warehouse = new Warehouse();
            _fromProtobuf = new FromProtobuf(_warehouse);
            _toProtobuf = new ToProtobuf();
        }

        [Test]
        public void HelloWorld()
        {
            
            var message = new Thing("first");
            message.SetProperty(new Property.String("content", "Hello World"));

            var transaction = _toProtobuf.Convert(new [] {message}, new Thing[0], new ThingType[0], "tests");

            var senderId = _fromProtobuf.Convert(transaction);
            Assert.That(senderId, Is.EqualTo("tests"));

            var newMessage = _warehouse.GetThing("first");
            Assert.That(newMessage, Is.Not.Null);

            Assert.That(message.ID, Is.EqualTo(newMessage.ID));

            Assert.That(newMessage.GetProperty<Property.String>("content"), Is.Not.Null);
            Assert.That(newMessage.GetProperty<Property.String>("content").Value,
                Is.EqualTo("Hello World"));
        }

        [Test]
        public void HelloWorldType()
        {
            var type = new ThingType("message");
            type.DefineProperty(PropertyType.Create<Property.String>("content"));
            type.Description = "Just a simple text message";

            var transaction = _toProtobuf.Convert(new Thing[0], new Thing[0], new []{type}, "tests");

            _fromProtobuf.Convert(transaction);

            var newType = _warehouse.GetThingType("message");

            Assert.That(newType, Is.Not.Null);

            Assert.That(newType.Description, Is.EqualTo(type.Description));

            Assert.That(newType.GetPropertyDefinition("content"), Is.Not.Null);
            Assert.That(newType.GetPropertyDefinition("content").Type, Is.EqualTo(typeof (Property.String)));
        }

        [Test]
        public void CompleteHelloWorld()
        {
            var type = new ThingType("message");
            type.DefineProperty(PropertyType.Create<Property.String>("content"));

            var message = new Thing("first", type);
            message.SetProperty(new Property.String("content", "Hello World"));

            _fromProtobuf.Convert(_toProtobuf.Convert(new [] {message}, new Thing[0], new [] {type}, "bob"));
            
            Assert.That(_warehouse.GetThing("first"), Is.Not.Null);
            Assert.That(_warehouse.GetThingType("message"), Is.Not.Null);
            Assert.That(_warehouse.GetThing("first").Type, Is.EqualTo(_warehouse.GetThingType("message")));
        }

        [Test]
        public void EfficientStringDeclarations()
        {
            var firstTransaction = _toProtobuf.Convert(new Thing[0], new Thing[0], new ThingType[0], "canard");

            Assert.That(firstTransaction.string_declarations.Count, Is.EqualTo(1));

            var secondTransaction = _toProtobuf.Convert(new Thing[0], new Thing[0], new ThingType[0], "canard");

            Assert.That(secondTransaction.string_declarations.Count, Is.EqualTo(0));
        }

        [Test]
        public void CheckDeletes()
        {
            var duck = new Thing("canard");
            _warehouse.RegisterThing(duck);

            var transaction = _toProtobuf.Convert(new Thing[0], new [] {duck}, new ThingType[0], "bob");

            Assert.That(transaction.things_remove_list.Count, Is.EqualTo(1));

            _fromProtobuf.Convert(transaction);

            Assert.That(_warehouse.GetThing("canard"), Is.Null);
        }

        [Test]
        public void CheckLocationProperty()
        {
            var thing = new Thing("earth");
            thing.SetProperty(new Property.Location("point", new Location.Point(42,43,44)));
            thing.SetProperty(new Property.Location("latlng", new Location.LatLng(51, 52, 53)));
            thing.SetProperty(new Property.Location("equatorial", new Location.Equatorial(27, 28, 29)));

            _fromProtobuf.Convert(_toProtobuf.Convert(new [] {thing}, new Thing[0], new ThingType[0], "earth"));

            var newThing = _warehouse.GetThing("earth");
            
            Assert.That(newThing, Is.Not.Null);
            Assert.That(newThing.GetProperty<Property.Location>("point").Value.X, Is.EqualTo(42));
            Assert.That(newThing.GetProperty<Property.Location>("latlng").Value.Y, Is.EqualTo(52));
            Assert.That(newThing.GetProperty<Property.Location>("equatorial").Value.Z, Is.EqualTo(29));

        }

        [Test]
        public void EfficientStringProperties()
        {
            var thing = new Thing("computer");
            thing.SetProperty(new Property.String("name", "Interstella"));
            thing.SetProperty(new Property.String("hostname", "Interstella"));

            var transaction = _toProtobuf.Convert(new[] {thing}, new Thing[0], new ThingType[0], "");

            // Just 4 because the sender ID is an empty string (empty strings doesn't need to be declared)
            Assert.That(transaction.string_declarations.Count, Is.EqualTo(4));

            _fromProtobuf.Convert(transaction);

            Assert.That(_warehouse.GetThing("computer").GetProperty<Property.String>("name").Value, Is.EqualTo("Interstella"));
        }

        [Test]
        public void CheckDoubleProperty()
        {
            var thing = new Thing("twingo");
            thing.SetProperty(new Property.Double("speed", 45.71));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("twingo").GetProperty<Property.Double>("speed").Value, Is.EqualTo(45.71));
        }
        
		[Test]
        public void CheckNegativeDoubleProperty()
        {
            var thing = new Thing("twingo");
            thing.SetProperty(new Property.Double("speed", -45.71));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("twingo").GetProperty<Property.Double>("speed").Value, Is.EqualTo(-45.71));
        }

        [Test]
        public void CheckIntProperty()
        {
            var thing = new Thing("twingo");
            thing.SetProperty(new Property.Int("doors", 3));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("twingo").GetProperty<Property.Int>("doors").Value, Is.EqualTo(3));
        }
        
		[Test]
        public void CheckNegativeIntProperty()
        {
            var thing = new Thing("submarine");
            thing.SetProperty(new Property.Int("sea_altitude", -100));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("submarine").GetProperty<Property.Int>("sea_altitude").Value, Is.EqualTo(-100));
        }

        [Test]
        public void CheckBooleanProperty()
        {
            var thing = new Thing("twingo");
            thing.SetProperty(new Property.Boolean("moving", true));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("twingo").GetProperty<Property.Boolean>("moving").Value, Is.True);
        }

        [Test]
        public void CheckDateTimeProperty()
        {
            var thing = new Thing("twingo");
            var birthdate = new DateTime(1998, 6, 24);
            thing.SetProperty(new Property.DateTime("birthdate", birthdate));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("twingo").GetProperty<Property.DateTime>("birthdate").Value, Is.EqualTo(birthdate));
        }

        [Test]
        public void CheckConnectedThings()
        {
            var group = new Thing("family");
            var roger = new Thing("roger");
            var alain = new Thing("alain");

            group.Connect(roger);
            group.Connect(alain);

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { group, roger, alain }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("family").ConnectedThings.Count, Is.EqualTo(2));
        }

        [Test]
        public void IndependentInstances()
        {
            var location = new Location.LatLng(25, 2);

            var thing = new Thing("8712C");
            thing.SetProperty(new Property.Location("position", location));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            var newLocation = _warehouse.GetThing("8712C").GetProperty<Property.Location>("position").Value as Location.LatLng;

            Assert.That(location.Compare(newLocation), Is.True);

            newLocation.Latitude = 27;

            Assert.That(location.Compare(newLocation), Is.False);

        }

        [Test]
        public void IncrementalPropertiesUpdates()
        {
            var thing = new Thing("rocket");
            thing.SetProperty(new Property.Double("speed", 1500.0));
            thing.SetProperty(new Property.String("name", "Ariane"));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            thing = new Thing("rocket");
            thing.SetProperty(new Property.Double("speed", 1200.0));
            thing.SetProperty(new Property.Boolean("space", true));

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null));

            var newThing = _warehouse.GetThing("rocket");

            Assert.That(newThing.GetProperty<Property.String>("name").Value, Is.EqualTo("Ariane"));
            Assert.That(newThing.GetProperty<Property.Double>("speed").Value, Is.EqualTo(1200.0));
            Assert.That(newThing.GetProperty<Property.Boolean>("space").Value, Is.True);

        }

        [Test]
        public void IncrementalUpdatesAndDisconnection()
        {
            var couple = new Thing("couple");
            var a = new Thing("James");
            var b = new Thing("Germaine");

            couple.Connect(a);
            couple.Connect(b);

            _fromProtobuf.Convert(_toProtobuf.Convert(new[] { couple,a,b }, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("couple").ConnectedThings.Count, Is.EqualTo(2));

            // Germaine doesn't want to live with James anymore
            couple.Disconnect(b);
            _fromProtobuf.Convert(_toProtobuf.Convert(new[] {couple}, new Thing[0], new ThingType[0], null));

            Assert.That(_warehouse.GetThing("couple").ConnectedThings.Count, Is.EqualTo(1));


        }

        [Test]
        public void DontSendAnUnchangedObject()
        {
            var thing = new Thing("café");
            thing.SetProperty(new Property.Double("temperature", 40.0));
            thing.SetProperty(new Property.Location("location", new Location.Equatorial(48,454,2)));

            var transaction = _toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null);

            Assert.That(transaction.things_publish_list.Count, Is.EqualTo(1));

            transaction = _toProtobuf.Convert(new[] { thing }, new Thing[0], new ThingType[0], null);

            Assert.That(transaction.things_publish_list.Count, Is.EqualTo(0));
        }

	    [Test]
	    public void SenderIdInformation()
	    {
		    
            var type = new ThingType("message");
            type.DefineProperty(PropertyType.Create<Property.String>("content"));

            var message = new Thing("first", type);
            message.SetProperty(new Property.String("content", "Hello World"));

		    _warehouse.Events.OnReceivedNew += (sender, args) => Assert.That(args.Sender, Is.EqualTo("niceSenderIdNew")); 
		    _warehouse.Events.OnDefine += (sender, args) => Assert.That(args.Sender, Is.EqualTo("niceSenderIdNew"));

		    _warehouse.Events.OnUpdate += (sender, args) => Assert.That(args.Sender, Is.EqualTo("niceSenderIdUpdate")); 
		    _warehouse.Events.OnDelete += (sender, args) => Assert.That(args.Sender, Is.EqualTo("niceSenderIdDelete")); 

            _fromProtobuf.Convert(_toProtobuf.Convert(new [] {message}, new Thing[0], new [] {type}, "niceSenderIdNew"));

            message.SetProperty(new Property.String("content", "Hello World2"));
            _fromProtobuf.Convert(_toProtobuf.Convert(new [] {message}, new Thing[0], new ThingType[0], "niceSenderIdUpdate"));

			var duck = new Thing("canard");
		    _warehouse.RegisterThing(duck);

            var transaction = _toProtobuf.Convert(new Thing[0], new [] {duck}, new ThingType[0], "niceSenderIdDelete");
            _fromProtobuf.Convert(transaction);
	    }
    }
}