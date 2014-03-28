/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

describe("ProtoConversions Test", ()=> {
	var warehouseInput: ThingModel.Warehouse;
	var warehouseOutput: ThingModel.Warehouse;

	var fromProtobuf: ThingModel.Proto.FromProtobuf;
	var toProtobuf: ThingModel.Proto.ToProtobuf;

	var observer: ThingModel.Proto.ProtoModelObserver;

	beforeEach(()=> {
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
// ReSharper restore WrongExpressionStatement
});