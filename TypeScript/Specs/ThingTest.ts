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
	});

// ReSharper restore WrongExpressionStatement
});