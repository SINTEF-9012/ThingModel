/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

import should = require('should');

describe("Thing test", ()=> {
	var thing : ThingModel.Thing;
	var type : ThingModel.ThingType;
	var otherThing : ThingModel.Thing;
	var name : ThingModel.Property;

	beforeEach(() => {
		type = new ThingModel.ThingType("Person");
		type.DefineProperty(
			new ThingModel.PropertyType("name", ThingModel.Type.String));

		thing = new ThingModel.Thing("200787", type);

		otherThing = new ThingModel.Thing("875402");
		thing.Connect(otherThing);

		name = new ThingModel.Property.String("name", "Pierre");
		thing.SetProperty(name);

	});

// ReSharper disable WrongExpressionStatement
	describe("testWrongID", () => {
		it("should fail with a null", ()=> {
			(() => { thing = new ThingModel.Thing(null); }).should.throw(/null/);
		});

		it("should fail with an empty string", () => {
			(() => { thing = new ThingModel.Thing(""); }).should.throw(/empty/);
		});
	});

	describe("Test set properties", ()=> {
		it("should contains the saved value", ()=> {
			thing.SetProperty(new ThingModel.Property.Int("speed", 12));
			thing.SetProperty(new ThingModel.Property.Double("speed", 12.5));

			thing.GetProperty<ThingModel.Property.Double>("speed",
				ThingModel.Type.Double).Value.should.be.equal(12.5);
		});
	});

	describe("Test get property", ()=> {
		it("is not null", () => {
			(thing.GetProperty<ThingModel.Property>("name", ThingModel.Type.String) !== null).should.be.ok;
			(thing.GetProperty<ThingModel.Property>("name") !== null).should.be.ok;
			(thing.GetProperty<ThingModel.Property>("name", null) !== null).should.be.ok;

			(thing.GetProperty<ThingModel.Property>("name", ThingModel.Type.Double) === null).should.be.ok;
			(thing.GetProperty<ThingModel.Property>("wrong key", ThingModel.Type.String) === null).should.be.ok;
		});
	});

	describe("Test has property", ()=> {
		it("just few checks", ()=> {
			thing.HasProperty("name").should.be.true;
			thing.HasProperty("name2").should.be.false;
		});
	});

	describe("Test connections", ()=> {
		it("should be connected to connected things", ()=> {
			thing.IsConnectedTo(otherThing).should.be.true;
			otherThing.IsConnectedTo(thing).should.be.false;
		});

		it("should handle null values", ()=> {
			thing.Connect(null);
			thing.IsConnectedTo(null).should.be.false;
			thing.Disconnect(null);
		});

		it("should not be connected to itself", ()=> {
			(()=> {
				thing.Connect(thing);
			}).should.throw(/itself/);

			(() => {
				thing.Connect(new ThingModel.Thing(thing.ID));
			}).should.throw(/itself/);
		});

		it("should disconnect a thing properly", ()=> {
			thing.Disconnect(otherThing);
			thing.IsConnectedTo(otherThing).should.be.false;
		});

		it("should disconnect all things if needed", ()=> {
			thing.DisconnectAll();
			thing.IsConnectedTo(otherThing).should.be.false;
		});

		it("should create a list of connected things", () => {
			_.each(thing.ConnectedThings, (t) => {
				thing.Disconnect(t);
			});

			thing.IsConnectedTo(otherThing).should.be.false;
		});

		it("should count connected things correctly", ()=> {
			thing.ConnectedThingsCount.should.be.equal(1);
			thing.Disconnect(otherThing);
			thing.ConnectedThingsCount.should.be.equal(0);
		});
	});

	describe("Test properties", ()=> {
		it("should contains the name property", () => {
			_.each(thing.Properties, (p: ThingModel.Property) => {
				p.Key.should.be.equal("name");
			});
		});
	});

	describe("Test comparisons", () => {
		it("should handle nulls", () => {
			thing.Compare(null).should.be.false;
		});

		it("should return true when it's the same thing", () => {
			thing.Compare(thing).should.be.true;
		});

		it("should be different when the type is different", ()=> {
			var newThing = new ThingModel.Thing(thing.ID); // default type

			newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));
			newThing.Connect(otherThing);

			newThing.Compare(thing).should.be.false;
			thing.Compare(newThing).should.be.false;
		});

		it("should be different when the connections are different", ()=> {
			var newThing = new ThingModel.Thing(thing.ID, type);
			newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));

			newThing.Compare(thing).should.be.false;
			thing.Compare(newThing).should.be.false;
		});

		it("should handle differents ids correctly", ()=> {
			var newThing = new ThingModel.Thing("banana", type);
			newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));
			newThing.Connect(otherThing);

			newThing.Compare(thing, false, false).should.be.true;
			thing.Compare(newThing, false, false).should.be.true;

			thing.Compare(newThing, true, false).should.be.false;
		});

		describe("Properties comparisons", () => {


			it("should be different when the list of properties is different", () => {

				var newThing = new ThingModel.Thing(thing.ID, type);
				newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));
				newThing.Connect(otherThing);

				// New property
				newThing.SetProperty(new ThingModel.Property.String("surname", "Lapinou"));

				newThing.Compare(thing).should.be.false;
				thing.Compare(newThing).should.be.false;

				thing.SetProperty(new ThingModel.Property.String("surname", "Lapinou"));

				newThing.Compare(thing).should.be.true;
				thing.Compare(newThing).should.be.true;

				thing.SetProperty(new ThingModel.Property.String("color", "white"));
				newThing.SetProperty(new ThingModel.Property.String("meal", "carrot"));

				newThing.Compare(thing).should.be.false;
				thing.Compare(newThing).should.be.false;
			});

			it("should be different if the properties value are different", ()=> {

				var newThing = new ThingModel.Thing(thing.ID, type);
				newThing.SetProperty(new ThingModel.Property.String("name", "Bob"));
				newThing.Connect(otherThing);

				newThing.Compare(thing).should.be.false;
				thing.Compare(newThing).should.be.false;

				newThing.SetProperty(new ThingModel.Property.Double("name", 12));

				newThing.Compare(thing).should.be.false;
				thing.Compare(newThing).should.be.false;

				newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));

				newThing.Compare(thing).should.be.true;
				thing.Compare(newThing).should.be.true;
			});
		});

		it("should handle deep comparisons", ()=> {
			var aThingForTheRoad = new ThingModel.Thing("rabbit");
			aThingForTheRoad.SetProperty(new ThingModel.Property.Double("speed", 12));
			thing.Connect(aThingForTheRoad);

			var newThing = new ThingModel.Thing(thing.ID, type);
			newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));
			newThing.Connect(otherThing);
			newThing.Connect(aThingForTheRoad);

			thing.Compare(thing, true, true).should.be.true;
			thing.Compare(newThing, true, true).should.be.true;
			newThing.Compare(thing, true, true).should.be.true;

			newThing.Disconnect(otherThing);
			var newOtherThing = new ThingModel.Thing(otherThing.ID);
			newThing.Connect(newOtherThing);

			thing.Compare(newThing, true, true).should.be.true;
			newThing.Compare(thing, true, true).should.be.true;

			newOtherThing.SetProperty(new ThingModel.Property.String("name", "Alain"));

			thing.Compare(newThing, true, true).should.be.false;
			newThing.Compare(thing, true, true).should.be.false;
		});

		it("should handle infinite loop connections", ()=> {

			var newThing = new ThingModel.Thing(thing.ID, type);
			newThing.SetProperty(new ThingModel.Property.String("name", "Pierre"));
			newThing.Connect(otherThing);

			// infinite loop
			otherThing.Connect(thing);

			thing.Compare(newThing, true, true).should.be.true;
			newThing.Compare(thing, true, true).should.be.true;

			var newOtherThing = new ThingModel.Thing(otherThing.ID);
			newThing.Connect(newOtherThing);

			thing.Compare(newThing, true, true).should.be.false;
			newThing.Compare(thing, true, true).should.be.false;

			newOtherThing.Connect(thing);

			thing.Compare(newThing, true, true).should.be.true;
			newThing.Compare(thing, true, true).should.be.true;
		});
	});


// ReSharper restore WrongExpressionStatement
});