/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

describe("Property Test", ()=> {

// ReSharper disable WrongExpressionStatement
	describe("Test values", ()=> {
		it("should work with a boolean", () => {
			new ThingModel.Property.Boolean("good", true).Value
			.should.be.true;
		});

		it("should work with a string", () => {
			new ThingModel.Property.String("name", "Alphonse").Value
				.should.be.equal("Alphonse");
		});

		it("should work with a double", () => {
			new ThingModel.Property.Double("speed", 12.0).Value
				.should.be.equal(12.0);
		});

		it("should work with a integer", () => {
			new ThingModel.Property.Int("speed", 12).Value
				.should.be.equal(12);
		});

		it("should work with a date", () => {
			var t = new Date();
			new ThingModel.Property.DateTime("date", t).Value
				.should.be.equal(t);
		});

		it("should work with a location", () => {
			new ThingModel.Property.Location.Point("location",
				new ThingModel.Location.Point(1.0, 2.0, 3.0)).Value
					.should.be.ok;
			new ThingModel.Property.Location.LatLng("location",
				new ThingModel.Location.LatLng(1.0, 2.0, 3.0)).Value
					.should.be.ok;
			new ThingModel.Property.Location.Equatorial("location",
				new ThingModel.Location.Equatorial(1.0, 2.0, 3.0)).Value
					.should.be.ok;
		});
	});

	describe("Test ValueToString()", ()=> {
		it("should work with a boolean", () => {
			new ThingModel.Property.Boolean("good", true).ValueToString()
				.should.not.be.empty;
		});

		it("should work with a string", () => {
			new ThingModel.Property.String("name", "Alphonse").ValueToString()
				.should.match("Alphonse");
		});

		it("should work with a double", () => {
			new ThingModel.Property.Double("speed", 12.0).ValueToString()
				.should.match("12");
		});

		it("should work with a integer", () => {
			new ThingModel.Property.Int("speed", 12).ValueToString()
				.should.match("12");
		});

		it("should work with a date", () => {
			var t = new Date();
			new ThingModel.Property.DateTime("date", t).ValueToString()
				.should.not.be.empty;
		});

		it("should work with a location", () => {
			new ThingModel.Property.Location.LatLng("location",
				new ThingModel.Location.LatLng(1.0, 2.0, 3.0)).ValueToString()
					.should.match(/2/);
		});
	});

	describe("Properties comparison", ()=> {

		var property: ThingModel.Property;

		beforeEach(()=> {
			property = new ThingModel.Property.Double("speed");
		});

		it("should handle null", ()=> {
			property.CompareValue(null).should.be.false;
		});

		it("should return false when the property is totally different", () => {
			property.CompareValue(new ThingModel.Property.String("poisson", "roger"))
			.should.be.false;
		});

		it("should return false if the value is different", ()=> {
			property.CompareValue(new ThingModel.Property.Double("speed2", 45.0))
				.should.be.false;
		});

		it("should return true if the value is the same", () => {
			property.CompareValue(new ThingModel.Property.Double("speed2"))
				.should.be.true;
		});
	});
// ReSharper restore WrongExpressionStatement
});