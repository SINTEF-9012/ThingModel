/// <reference path="../bower_components/DefinitelyTyped/mocha/mocha.d.ts" />
/// <reference path="../bower_components/DefinitelyTyped/should/should.d.ts" />

describe("ThingType Test", () => {
	var type : ThingModel.ThingType;
	var propertyType: ThingModel.PropertyType;

	beforeEach(()=> {
		type = new ThingModel.ThingType("plane");
		propertyType = new ThingModel.PropertyType("location", ThingModel.Type.Location);

		type.DefineProperty(propertyType);
	});

// ReSharper disable WrongExpressionStatement
	it("should handle names correctly", ()=> {
		type.Name.should.equal("plane");

		(()=> {
			new ThingModel.ThingType("");
		}).should.throw(/empty/);

		(() => {
			new ThingModel.ThingType(null);
		}).should.throw(/null/);
	});

	it("should return the properties", ()=> {
		type.GetPropertyDefinition("location").should.equal(propertyType);
		(type.GetPropertyDefinition("canard") == null).should.be.ok;
	});

	it("should return a good list of properties", () => {
		var visited = false;

		_.each(type.Properties, (p: ThingModel.PropertyType) => {
			visited = true;

			// Should work (like everything else)
			type.DefineProperty(p);
		});

		// ReSharper disable once ExpressionIsAlwaysConst
		visited.should.be.true;
	});

	it("should check the values correctly", ()=> {
		var plane = new ThingModel.Thing("A380", type);

		type.Check(plane).should.be.false;

		propertyType.Required = false;
		type.Check(plane).should.be.true;

		propertyType.Required = true;

		plane.SetProperty(new ThingModel.Property.String("location", "CDG"));
		type.Check(plane).should.be.false;

		plane.SetProperty(new ThingModel.Property.Location("location",
			new ThingModel.Location.LatLng(49.010852, 2.547398)));

		type.Check(plane).should.be.true;
	});
// ReSharper restore WrongExpressionStatement
});