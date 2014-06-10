/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

import should = require('should');


describe("Thing test", () => {

	beforeEach(() => {

	});

	// ReSharper disable WrongExpressionStatement
	describe("testThingTypeBuilder", () => {
		var type = ThingModel.BuildANewThingType.Named("rabbit")
			.WhichIs("Just a rabbit")
			.ContainingA.String("name")
			.AndA.LocationLatLng()
			.AndA.NotRequired.Double("speed")
			.AndAn.Int("nbChildren", "Number of children")
			.WhichIs("The number of direct children")
			.Build();

		it('should contains the properties', () => {
			(type == null).should.be.false;

			type.GetPropertyDefinition("name").should.be.ok;
			type.GetPropertyDefinition("speed").should.be.ok;
			type.GetPropertyDefinition("speed").Required.should.be.false;
			type.GetPropertyDefinition("nbChildren").Required.should.be.true;
			type.GetPropertyDefinition("nbChildren").Description.should.match(/direct children/);
		});

		var superRappit = ThingModel.BuildANewThingType.Named("super_rabbit")
			.WhichIs("A better rabbit")
			.ContainingA.CopyOf(type)
			.AndAn.Int("power")
			.Build();

		it('should create a copy of the type', () => {
			superRappit.GetPropertyDefinition("name").should.be.ok;
			superRappit.GetPropertyDefinition("power").should.be.ok;

			superRappit.GetPropertyDefinition("nbChildren").Name = "Number of super children";
			type.GetPropertyDefinition("nbChildren").Name.should.not.match(/super/);
		});
	});


	describe("test thing builder", () => {
		var duckType = ThingModel.BuildANewThingType.Named("duck").Build();

		var duck = ThingModel.BuildANewThing.As(duckType)
			.IdentifiedBy("ab548")
			.ContainingA.String("name", "Roger")
			.AndA.Location(new ThingModel.Location.Point())
			.AndAn.Int("nbChildren", 12)
			.Build();

		it("should contains something", () => {
			duck.ID.should.be.equal("ab548");
			duck.Type.Name.should.be.equal("duck");
			duck.Int("nbChildren").should.be.equal(12);
		});
	});

	describe("BootStrap", () => {
		var thing = ThingModel.BuildANewThingType.Named("Thing")
			.ContainingA.String("id")
			.AndA.NotRequired.String("typename").Build();

		var thingPropierties = ThingModel.BuildANewThingType.Named("Property")
			.ContainingA.String("key")
			.AndA.NotRequired.String("value_string")
			.AndA.NotRequired.Int("value_int")
			.AndA.NotRequired.Double("value_double")
			.AndA.NotRequired.Boolean("value_boolean")
			.AndA.NotRequired.DateTime("value_datetime")
			.AndA.NotRequired.LocationLatLng("value_location").Build();

		var thingType = ThingModel.BuildANewThingType.Named("Thing type")
			.ContainingA.NotRequired.String("name")
			.AndA.NotRequired.String("description").Build();

		var propertyType = ThingModel.BuildANewThingType.Named("Property type")
			.ContainingA.String("key")
			.AndA.NotRequired.String("name")
			.AndA.NotRequired.String("description")
			.AndA.Boolean("required")
			.AndA.String("type").Build();

		// thingProperties mustt contain one of the notrequired properties
		// propertyType type should be a value in string/location/double/int/boolean/datetime
		// the properties should be connected to their things
		// the properties types should be connected to their things type
	});
});