/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

describe("ProtoConversions Test", () => {
	var warehouseInput: ThingModel.Warehouse;
	var warehouseOutput: ThingModel.Warehouse;

	var fromProtobuf: ThingModel.Proto.FromProtobuf;
	var toProtobuf: ThingModel.Proto.ToProtobuf;

	var observer: ThingModel.Proto.ProtoModelObserver;

	beforeEach(() => {
		warehouseInput = new ThingModel.Warehouse();
		warehouseOutput = new ThingModel.Warehouse();

		fromProtobuf = new ThingModel.Proto.FromProtobuf(warehouseInput);

		observer = new ThingModel.Proto.ProtoModelObserver();
		warehouseOutput.RegisterObserver(observer);

		toProtobuf = new ThingModel.Proto.ToProtobuf();

	});

// ReSharper disable WrongExpressionStatement
	it("should contains Hello World", () => {
		var message = ThingModel.BuildANewThing.WithoutType
			.IdentifiedBy("first")
			.ContainingA.String("content", "Hello World")
			.Build();

		warehouseOutput.RegisterThing(message);

		var transaction = observer.GetTransaction(toProtobuf, "RogerEnterpriseBroadcaster");

		var senderId = fromProtobuf.Convert(transaction.toArrayBuffer());

		senderId.should.be.equal("RogerEnterpriseBroadcaster");

		var newMessage = warehouseInput.GetThing("first");

		(newMessage == null).should.be.false;

		(newMessage.GetProperty<ThingModel.Property>("content") == null).should.be.false;

		newMessage.GetProperty<ThingModel.Property.String>("content").Value
			.should.be.equal("Hello World");

	});

	it("should work with a type", () => {
		var type = ThingModel.BuildANewThingType.Named("message")
			.WhichIs("Just a simple text message")
			.ContainingA.String("content").Build();

		warehouseOutput.RegisterType(type);

		var transaction = observer.GetTransaction(toProtobuf, "bob");

		fromProtobuf.Convert(transaction.toArrayBuffer());

		var newType = warehouseInput.GetThingType("message");

		(newType == null).should.be.false;

		newType.Description.should.be.equal(type.Description);
		(newType.GetPropertyDefinition("content") == null).should.be.false;

		newType.GetPropertyDefinition("content").Type.should.be.equal(ThingModel.Type.String);

	});

	it("should have efficient string declarations", () => {
		var firstTransaction = toProtobuf.Convert([], [], [], "canard");

		firstTransaction.string_declarations.length.should.be.equal(1);

		var secondTransaction = toProtobuf.Convert([], [], [], "canard");
		secondTransaction.string_declarations.length.should.be.equal(0);
	});

	it("should work with deletes", () => {
		var duck = new ThingModel.Thing("canard");
		warehouseInput.RegisterThing(duck);

		var transaction = toProtobuf.Convert([], [duck], [], "bob");
		transaction.things_remove_list.length.should.be.equal(1);

		fromProtobuf.ConvertTransaction(transaction, true);

		(warehouseInput.GetThing("canard") == null).should.be.true;
	});

	it("should contains good location properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("earth")
			.ContainingA.Location("point", new ThingModel.Location.Point(42, 43, 44))
			.AndA.Location("latlng", new ThingModel.Location.LatLng(-51, -52, -53))
			.AndA.Location("equatorial", new ThingModel.Location.Equatorial(27, 28, 29));

		warehouseOutput.RegisterThing(thing.Build());

		var transaction = observer.GetTransaction(toProtobuf, "bob");

		fromProtobuf.Convert(transaction.toArrayBuffer());

		var newThing = warehouseInput.GetThing("earth");

		(newThing == null).should.be.false;

		newThing.GetProperty<ThingModel.Property.Location.Point>("point").Value.X.should.be.equal(42);
		newThing.GetProperty<ThingModel.Property.Location.LatLng>("latlng").Value.Y.should.be.equal(-52);
		newThing.GetProperty<ThingModel.Property.Location.Equatorial>("equatorial").Value.Z.should.be.equal(29);
	});

	it("should has efficient string properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("computer")
			.ContainingA.String("name", "Interstella")
			.AndA.String("hostname", "Interstella");

		warehouseOutput.RegisterThing(thing.Build());

		var transaction = observer.GetTransaction(toProtobuf, "");

		transaction.string_declarations.length.should.be.equal(4);

		fromProtobuf.Convert(transaction.toArrayBuffer());

		var newThing = warehouseInput.GetThing("computer");
		newThing.GetProperty<ThingModel.Property.String>("name").Value
			.should.be.equal("Interstella");
	});

	it("should has correct double properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("twingo")
			.ContainingA.Double("speed", 45.71)
			.AndA.Double("acceleration", -5.14);

		warehouseOutput.RegisterThing(thing.Build());
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("twingo");
		newThing.GetProperty<ThingModel.Property.Double>("speed").Value.should.be.equal(45.71);
		newThing.GetProperty<ThingModel.Property.Double>("acceleration").Value.should.be.equal(-5.14);
	});

	it("should has correct int properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("twingo")
			.ContainingAn.Int("doors", 3)
			.AndAn.Int("altitude", -5);

		warehouseOutput.RegisterThing(thing.Build());
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("twingo");
		newThing.GetProperty<ThingModel.Property.Int>("doors").Value.should.be.equal(3);
		newThing.GetProperty<ThingModel.Property.Int>("altitude").Value.should.be.equal(-5);
	});

	it("should has correct boolean properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("twingo")
			.ContainingA.Boolean("moving", true);

		warehouseOutput.RegisterThing(thing.Build());
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("twingo");
		newThing.GetProperty<ThingModel.Property.Boolean>("moving").Value.should.be.true;
	});

	it("should has correct datetime properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("twingo")
			.ContainingA.DateTime("birthdate", new Date(Date.UTC(1998, 6, 24)))
			.AndA.DateTime("now", new Date());

		warehouseOutput.RegisterThing(thing.Build());
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("twingo");
		newThing.GetProperty<ThingModel.Property.DateTime>("birthdate").Value.should.be.eql(
			new Date(Date.UTC(1998, 6, 24)));
		(+newThing.GetProperty<ThingModel.Property.DateTime>("now").Value).should.be.lessThan(
			(+new Date())+1);
	});

	it("should has correct javascript false properties", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("twingo").Build();

		thing.Double("speed", 0.0);
		thing.Boolean("accelerating", false);
		thing.Int("nbElectricalEngines", 0);

		warehouseOutput.RegisterThing(thing);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("twingo");
		newThing.Double("speed").should.be.equal(0.0);
		newThing.Boolean("accelerating").should.be.false;
		newThing.Int("nbElectricalEngines").should.be.equal(0);
	});

	it("works with connexions", () => {
		var group = new ThingModel.Thing("family"),
			roger = new ThingModel.Thing("roger"),
			alain = new ThingModel.Thing("alain");

		group.Connect(roger);
		group.Connect(alain);

		warehouseOutput.RegisterThing(group);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		warehouseInput.GetThing("family").ConnectedThingsCount.should.be.equal(2);
	});

	it("has independent instances", () => {
		var location = new ThingModel.Location.LatLng(25, 2);

		var thing = new ThingModel.Thing("8712C");
		thing.SetProperty(new ThingModel.Property.Location.LatLng("position", location));

		warehouseOutput.RegisterThing(thing);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newLocation = warehouseInput.GetThing("8712C").GetProperty<ThingModel.Property.Location.LatLng>("position").Value;

		location.Compare(newLocation).should.be.true;

		newLocation.Y = 27;
		location.Compare(newLocation).should.be.false;
	});

	it("has incremental properties updates", () => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("rocket")
			.ContainingA.Double("speed", 1500.0)
			.AndA.String("name", "Ariane").Build();

		warehouseOutput.RegisterThing(thing);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("rocket")
			.ContainingA.Double("speed", 1200.0)
			.AndA.Boolean("space", true).Build();

		warehouseOutput.NotifyThingUpdate(thing);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		var newThing = warehouseInput.GetThing("rocket");

		newThing.GetProperty<ThingModel.Property.String>("name").Value.should.be.equal("Ariane");
		newThing.GetProperty<ThingModel.Property.Double>("speed").Value.should.be.equal(1200);
		newThing.GetProperty<ThingModel.Property.Boolean>("space").Value.should.be.true;
	});

	it("has incremental updates and disconnection", () => {
		var couple = new ThingModel.Thing("couple"),
			a = new ThingModel.Thing("James"),
			b = new ThingModel.Thing("Germaine");

		couple.Connect(a);
		couple.Connect(b);

		warehouseOutput.RegisterThing(couple);
		fromProtobuf.Convert(observer.GetTransaction(toProtobuf, null).toArrayBuffer());

		warehouseInput.GetThing("couple").ConnectedThingsCount.should.be.equal(2);

		couple.Disconnect(b);
		var transaction = toProtobuf.Convert([couple], [], [], "yeah");
		fromProtobuf.ConvertTransaction(transaction, true);

		warehouseInput.GetThing("couple").ConnectedThingsCount.should.be.equal(1);
	});

	it("doesn't change un unchanged object", () => {
		var thing = new ThingModel.Thing("café");
		thing.SetProperty(new ThingModel.Property.Double("temperature", 40.0));

		var transaction = toProtobuf.Convert([thing], [], [], "yeah");
		transaction.things_publish_list.length.should.be.equal(1);

		var transaction = toProtobuf.Convert([thing], [], [], "yeah");
		transaction.things_publish_list.length.should.be.equal(0);
	});

	it("send sender informations", () => {
		var type = ThingModel.BuildANewThingType.Named("message")
			.ContainingA.String("content").Build();

		var message = ThingModel.BuildANewThing.As(type)
			.IdentifiedBy("first")
			.ContainingA.String("content", "Hello World").Build();


		warehouseInput.RegisterObserver({
			New: (thingType: ThingModel.Thing, sender: string) => {
				sender.should.be.equal("niceSenderIdNew");
			},
			Define: (thingType: ThingModel.ThingType, sender: string) => {
				sender.should.be.equal("niceSenderIdNew");
			},
			Updated: (thingType: ThingModel.Thing, sender: string) => {
				sender.should.be.equal("niceSenderIdUpdate");
			},
			Deleted: (thingType: ThingModel.Thing, sender: string) => {
				sender.should.be.equal("niceSenderIdDelete");
			}
		});

		warehouseOutput.RegisterThing(message);

		var transaction = observer.GetTransaction(toProtobuf, "niceSenderIdNew");
		fromProtobuf.Convert(transaction.toArrayBuffer());

		message.SetProperty(new ThingModel.Property.String("content", "Hello world 2"));
		warehouseOutput.NotifyThingUpdate(message);

		observer.Reset();
		transaction = observer.GetTransaction(toProtobuf, "niceSenderIdUpdate");
		fromProtobuf.Convert(transaction.toArrayBuffer());

		warehouseOutput.RemoveThing(message);
		observer.Reset();
		transaction = observer.GetTransaction(toProtobuf, "niceSenderIdDelete");
		fromProtobuf.Convert(transaction.toArrayBuffer());
	});

	it("should handle null and undefined strings",() => {
		var thing = ThingModel.BuildANewThing.WithoutType.IdentifiedBy("computer")
			.ContainingA.String("poisson", undefined)
			.AndA.String("hostname", undefined);

		warehouseOutput.RegisterThing(thing.Build());

		var transaction = observer.GetTransaction(toProtobuf, "");

		transaction.string_declarations.length.should.be.equal(3);

		fromProtobuf.Convert(transaction.toArrayBuffer());

		var newThing = warehouseInput.GetThing("computer");
		newThing.GetProperty<ThingModel.Property.String>("poisson").Value
			.should.be.equal("");
	});

	it("should send update when updated string is empty", () => {

		var thing = new ThingModel.Thing("computer");
		thing.String("name", "Interstella");

		warehouseOutput.RegisterThing(thing);

		var transaction = observer.GetTransaction(toProtobuf, "");
		fromProtobuf.Convert(transaction.toArrayBuffer());

		var newThing = warehouseInput.GetThing("computer");
		newThing.String("name").should.be.equal("Interstella");

		thing.String("name", "");

		observer.Reset();
		warehouseOutput.NotifyThingUpdate(thing);
		transaction = observer.GetTransaction(toProtobuf, "");
		fromProtobuf.Convert(transaction.toArrayBuffer());

		newThing.String("name").should.be.equal("");
	});
// ReSharper restore WrongExpressionStatement
});
