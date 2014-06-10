/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

class WarehouseChangeObserver implements ThingModel.IWarehouseObserver {
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

describe("Warehouse Test", () => {
	var type: ThingModel.ThingType;
	var thing: ThingModel.Thing;
	var warehouse: ThingModel.Warehouse;
	var warehouseChangeObserver: WarehouseChangeObserver;

	beforeEach(() => {
		type = new ThingModel.ThingType("duck");
		type.DefineProperty(new ThingModel.PropertyType("name", ThingModel.Type.String));
		type.DefineProperty(new ThingModel.PropertyType("age", ThingModel.Type.Double));

		thing = new ThingModel.Thing("871", type);
		thing.SetProperty(new ThingModel.Property.String("name", "Maurice"));
		thing.SetProperty(new ThingModel.Property.Double("age", 18.0));
		thing.SetProperty(new ThingModel.Property.Location.Point("localization",
			new ThingModel.Location.Point(10, 44)));

		warehouse = new ThingModel.Warehouse();
		warehouseChangeObserver = new WarehouseChangeObserver();
		warehouse.RegisterObserver(warehouseChangeObserver);
	});

// ReSharper disable WrongExpressionStatement
	it("should register types", () => {
		warehouse.RegisterType(type);
		warehouseChangeObserver.DefinedType.should.be.true;

		warehouseChangeObserver.Reset();

		(()=> {
			warehouse.RegisterType(null);
		}).should.throw(/null/);

		warehouseChangeObserver.DefinedType.should.be.false;
	});

	it("should register things", ()=> {
		warehouse.RegisterThing(thing);

		warehouseChangeObserver.NewThing.should.be.true;

		warehouseChangeObserver.Reset();

		(()=> {
			warehouse.RegisterThing(null);
		}).should.throw(/null/);

		warehouseChangeObserver.NewThing.should.be.false;
	});

	it("should register things and types in one instruction", ()=> {
		warehouse.RegisterThing(thing, true, true);

		warehouseChangeObserver.NewThing.should.be.true;
		warehouseChangeObserver.DefinedType.should.be.true;

		warehouseChangeObserver.Reset();

		warehouse.RegisterThing(new ThingModel.Thing("without type"), true, true);

		warehouseChangeObserver.DefinedType.should.be.false;
	});

	it("should understand when it's an update", ()=> {
		warehouse.RegisterThing(thing);
		warehouseChangeObserver.UpdatedThing.should.be.false;

		warehouse.RegisterThing(thing);
		warehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should remove the observers", () => {
		warehouse.UnregisterObserver(warehouseChangeObserver);

		warehouse.RegisterType(type);
		warehouse.RegisterThing(thing);

		warehouseChangeObserver.DefinedType.should.be.false;
		warehouseChangeObserver.NewThing.should.be.false;
	});

	it("should delete the things", ()=> {
		warehouse.RegisterThing(thing);
		warehouse.RemoveThing(thing);

		warehouseChangeObserver.DeletedThing.should.be.true;

		warehouseChangeObserver.Reset();
		warehouse.RemoveThing(null);
		warehouseChangeObserver.DeletedThing.should.be.false;
	});


	it("should handle deletions and creations of the same things", () => {
		warehouse.RegisterThing(thing);
		warehouse.RemoveThing(thing);
		warehouseChangeObserver.Reset();
		warehouse.RegisterThing(thing);
		warehouseChangeObserver.NewThing.should.be.true;
	});

	it("should register things recursively", ()=> {
		warehouse.RegisterThing(thing);
		warehouseChangeObserver.Reset();

		var newThing = new ThingModel.Thing("test");
		newThing.Connect(thing);

		thing.Connect(new ThingModel.Thing("blabla"));
		thing.Connect(new ThingModel.Thing("blabla2"));

		warehouse.RegisterThing(thing);

		warehouseChangeObserver.NewThing.should.be.true;
		warehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should detect loops in connections", () => {
		var newThing = new ThingModel.Thing("loop");
		newThing.Connect(thing);
		thing.Connect(newThing);

		warehouse.RegisterThing(newThing);
		warehouse.RegisterThing(thing);

		warehouseChangeObserver.NewThing.should.be.true;
	});

	it("should handle deletes and connections", ()=> {
		var otherThing = new ThingModel.Thing("lapin");
		otherThing.Connect(thing);

		warehouse.RegisterThing(otherThing);
		warehouseChangeObserver.Reset();
		warehouse.RemoveThing(thing);

		warehouseChangeObserver.DeletedThing.should.be.true;
		warehouseChangeObserver.UpdatedThing.should.be.true;
	});

	it("should delete two connected things", () => {
		var otherThing = new ThingModel.Thing("lapin");
		otherThing.Connect(thing);

		warehouse.RegisterThing(otherThing);
		warehouse.RegisterThing(thing);
		warehouse.Things.length.should.be.equal(2);

		warehouseChangeObserver.Reset();
		warehouse.RemoveThing(thing);
		warehouse.RemoveThing(otherThing);

		warehouse.Things.length.should.be.equal(0);
	});
// ReSharper restore WrongExpressionStatement
});