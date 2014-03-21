/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

class WharehouseChangeObserver implements ThingModel.IWharehouseObserver {
	public NewThing: boolean = false;
	public UpdatedThing: boolean = false;
	public DeletedThing: boolean = false;
	public DefinedType: boolean = false;
	
	public Reset() : void {
		this.NewThing = false;
		this.UpdatedThing = false;
		this.DefinedType = false;
		this.DeletedThing = false;
	}

	public New(thing: ThingModel.Thing): void {
		this.NewThing = true;
	}
	
	public Deleted(thing: ThingModel.Thing): void {
		this.DeletedThing = true;
	}
	
	public Updated(thing: ThingModel.Thing): void {
		this.UpdatedThing = true;
	}	

	public Define(thingType: ThingModel.ThingType): void {
		this.DefinedType = true;
	}
}

describe("Wharehouse Test", () => {
	var type: ThingModel.ThingType;
	var thing: ThingModel.Thing;
	var wharehouse: ThingModel.Wharehouse;
	var wharehouseChangeObserver: WharehouseChangeObserver;

	beforeEach(() => {
		type = new ThingModel.ThingType("duck");
		type.DefineProperty(new ThingModel.PropertyType("name", ThingModel.Type.String));
		type.DefineProperty(new ThingModel.PropertyType("age", ThingModel.Type.Double));

		thing = new ThingModel.Thing("871", type);
		thing.SetProperty(new ThingModel.Property.String("name", "Maurice"));
		thing.SetProperty(new ThingModel.Property.Double("age", 18.0));
		thing.SetProperty(new ThingModel.Property.Location("localization",
			new ThingModel.Location.Point(10, 44)));

		wharehouse = new ThingModel.Wharehouse();
		wharehouseChangeObserver = new WharehouseChangeObserver();
		wharehouse.RegisterObserver(wharehouseChangeObserver);
	});

// ReSharper disable WrongExpressionStatement
	it("should register types", () => {
		wharehouse.RegisterType(type);
		wharehouseChangeObserver.DefinedType.should.be.true;

		wharehouseChangeObserver.Reset();

		(()=> {
			wharehouse.RegisterType(null);
		}).should.throw(/null/);

		wharehouseChangeObserver.DefinedType.should.be.false;
	});

	it("should register things", ()=> {
		wharehouse.RegisterThing(thing);

		wharehouseChangeObserver.NewThing.should.be.true;

		wharehouseChangeObserver.Reset();

		(()=> {
			wharehouse.RegisterThing(null);
		}).should.throw(/null/);

		wharehouseChangeObserver.NewThing.should.be.false;
	});

	it("should register things and types in one instruction", ()=> {
		wharehouse.RegisterThing(thing, true, true);

		wharehouseChangeObserver.NewThing.should.be.true;
		wharehouseChangeObserver.DefinedType.should.be.true;

		wharehouseChangeObserver.Reset();

		wharehouse.RegisterThing(new ThingModel.Thing("without type"), true, true);

		wharehouseChangeObserver.DefinedType.should.be.false;
	});

	it("should understand when it's an update", ()=> {
		wharehouse.RegisterThing(thing);
		wharehouseChangeObserver.UpdatedThing.should.be.false;

		wharehouse.RegisterThing(thing);
		wharehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should remove the observers", () => {
		wharehouse.UnregisterObserver(wharehouseChangeObserver);

		wharehouse.RegisterType(type);
		wharehouse.RegisterThing(thing);

		wharehouseChangeObserver.DefinedType.should.be.false;
		wharehouseChangeObserver.NewThing.should.be.false;
	});

	it("should delete the things", ()=> {
		wharehouse.RegisterThing(thing);
		wharehouse.RemoveThing(thing);

		wharehouseChangeObserver.DeletedThing.should.be.true;

		wharehouseChangeObserver.Reset();
		wharehouse.RemoveThing(null);
		wharehouseChangeObserver.DeletedThing.should.be.false;
	});


	it("should handle deletions and creations of the same things", () => {
		wharehouse.RegisterThing(thing);
		wharehouse.RemoveThing(thing);
		wharehouseChangeObserver.Reset();
		wharehouse.RegisterThing(thing);
		wharehouseChangeObserver.NewThing.should.be.true;
	});

	it("should register things recursively", ()=> {
		wharehouse.RegisterThing(thing);
		wharehouseChangeObserver.Reset();

		var newThing = new ThingModel.Thing("test");
		newThing.Connect(thing);

		thing.Connect(new ThingModel.Thing("blabla"));
		thing.Connect(new ThingModel.Thing("blabla2"));

		wharehouse.RegisterThing(thing);

		wharehouseChangeObserver.NewThing.should.be.true;
		wharehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should detect loops in connections", () => {
		var newThing = new ThingModel.Thing("loop");
		newThing.Connect(thing);
		thing.Connect(newThing);

		wharehouse.RegisterThing(newThing);
		wharehouse.RegisterThing(thing);

		wharehouseChangeObserver.NewThing.should.be.true;
	});

	it("should handle deletes and connections", ()=> {
		var otherThing = new ThingModel.Thing("lapin");
		otherThing.Connect(thing);

		wharehouse.RegisterThing(otherThing);
		wharehouseChangeObserver.Reset();
		wharehouse.RemoveThing(thing);

		wharehouseChangeObserver.DeletedThing.should.be.true;
		wharehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should delete two connected things", () => {
		var otherThing = new ThingModel.Thing("lapin");
		otherThing.Connect(thing);

		wharehouse.RegisterThing(otherThing);
		wharehouse.RegisterThing(thing);
		wharehouse.Things.length.should.be.equal(2);

		wharehouseChangeObserver.Reset();
		wharehouse.RemoveThing(thing);
		wharehouse.RemoveThing(otherThing);

		wharehouse.Things.length.should.be.equal(0);
	});
// ReSharper restore WrongExpressionStatement
});