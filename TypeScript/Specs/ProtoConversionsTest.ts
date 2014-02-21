/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

describe("Property Test", ()=> {
	var wharehouseInput: ThingModel.Wharehouse;
	var wharehouseOutput: ThingModel.Wharehouse;

	var fromProtobuf: ThingModel.Proto.FromProtobuf;
	var toProtobuf: ThingModel.Proto.ToProtobuf;

	var observer: ThingModel.Proto.ProtoModelObserver;

	beforeEach(()=> {
		wharehouseInput = new ThingModel.Wharehouse();
		wharehouseOutput = new ThingModel.Wharehouse();

		fromProtobuf = new ThingModel.Proto.FromProtobuf(wharehouseInput);

		observer = new ThingModel.Proto.ProtoModelObserver();
		wharehouseOutput.RegisterObserver(observer);

		toProtobuf = new ThingModel.Proto.ToProtobuf();

	});

	it("test", () => {
		new ThingModel.Proto.ProtoTools();
	});
});