/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

import should = require('should');

describe("Thing test", ()=> {
	var thing;
	var type;
	var otherThing;
	var name;

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

	describe("testWrongID", () => {
		it("should fail with a null", ()=> {
			(() => { thing = new ThingModel.Thing(null); }).should.throw(/null/);
		});

		it("should fail with an empty string", () => {
			(() => { thing = new ThingModel.Thing(""); }).should.throw(/empty/);
		});
	});
});