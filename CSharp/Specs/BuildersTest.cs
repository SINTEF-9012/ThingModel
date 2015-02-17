using System;
using NUnit.Framework;
using ThingModel.Builders;

namespace ThingModel.Specs
{
	[TestFixture]
	class BuildersTest
	{
		[Test]
		public void TestThingTypeBuilder()
		{
			ThingType type = BuildANewThingType.Named("rabbit")
				.WhichIs("Just a rabbit")
				.ContainingA.String("name")
				.AndA.LocationLatLng("localization")
				.AndA.NotRequired.Double("speed")
				.AndAn.Int("nbChildren", "Number of children")
					.WhichIs("The number of direct children for the rabbit");

			Assert.That(type, Is.Not.Null);
			Assert.That(type.Description, Is.EqualTo("Just a rabbit"));

			Assert.That(type.GetPropertyDefinition("name"), Is.Not.Null);
			Assert.That(type.GetPropertyDefinition("speed"), Is.Not.Null);
			Assert.That(type.GetPropertyDefinition("speed").Required, Is.False);
			Assert.That(type.GetPropertyDefinition("nbChildren").Required, Is.True);
			Assert.That(type.GetPropertyDefinition("nbChildren").Description, Contains.Substring("direct children"));

			ThingType superRabbit = BuildANewThingType.Named("super_rabbit")
				.WhichIs("A better rabbit")
				.ContainingAn.Int("power")
				.AndA.CopyOf(type);

			Assert.That(superRabbit.GetPropertyDefinition("name"), Is.Not.Null);
			Assert.That(superRabbit.GetPropertyDefinition("power"), Is.Not.Null);

			superRabbit.GetPropertyDefinition("nbChildren").Name = "Number of super children";
			
			Assert.That(type.GetPropertyDefinition("nbChildren").Name, Does.Not.Contain("super"));
		}

		[Test]
		public void TestThingBuilder()
		{
			ThingType duckType = BuildANewThingType.Named("duck");

			Thing duck = BuildANewThing.As(duckType)
				.IdentifiedBy("ab548")
				.ContainingA.String("name", "Roger")
                .AndA.Location(new Location.Point())
				.AndA.Location("destination", new Location.Point())
				.AndAn.Int("nbChildren", 12);

			Assert.That(duck.ID, Is.EqualTo("ab548"));
			Assert.That(duck.Type.Name, Is.EqualTo("duck"));
			Assert.That(duck.GetProperty<Property.Int>("nbChildren").Value,
				Is.EqualTo(12));

            Thing spaceShip = BuildANewThing.WithoutType()
                .IdentifiedBy("spaceShip")
                .ContainingAn.Int("speed", 12)
                .AndA.Location(new Location.LatLng(1, 2, 3))
                .AndA.Location(new Location.Equatorial(1, 2, 3)) // change type
                .AndA.Location("destination", new Location.Equatorial(4, 5, 6))
                .AndA.Location("from", new Location.LatLng(7,8))
                .AndA.DateTime("arrivalTime", new DateTime(2087, 8, 14))
                .AndA.Double("averageSpeed", Double.NaN);
			
            Assert.That(spaceShip.ID, Is.EqualTo("spaceShip"));
		    Assert.That(spaceShip.Type, Is.Null);
			Assert.That(spaceShip.LocationEquatorial().Declination, Is.EqualTo(2));
			Assert.That(spaceShip.LocationLatLng("from").Latitude, Is.EqualTo(7));
			Assert.That(spaceShip.DateTime("arrivalTime"), Is.Not.Null);
		}

	    [Test]
	    public void TestBuilderLikeThingSyntax()
	    {
			ThingType duckType = new ThingType("duck"); 

			Thing duck = new Thing("ab548", duckType);
            duck.String("name", "Roger");
            duck.LocationPoint("location", new Location.Point());
            duck.LocationPoint("destination", new Location.Point());
            duck.Int("nbChildren", 12);

			Assert.That(duck.ID, Is.EqualTo("ab548"));
			Assert.That(duck.Type.Name, Is.EqualTo("duck"));
			Assert.That(duck.Int("nbChildren"),	Is.EqualTo(12));
			Assert.That(duck.LocationPoint().X, Is.EqualTo(0));
			Assert.That(duck.LocationPoint("destination").X, Is.EqualTo(0));

            Thing spaceShip = new Thing("spaceShip");
            spaceShip.ContainingAn.Int("speed", 12);
            spaceShip.LocationLatLng("location", new Location.LatLng(1, 2, 3));
            spaceShip.LocationEquatorial("location", new Location.Equatorial(1, 2, 3)); // change type
            spaceShip.LocationEquatorial("destination", new Location.Equatorial(4, 5, 6));
            
            spaceShip.ContainingA.Location("from", new Location.LatLng(7,8))
                .AndA.DateTime("arrivalTime", new DateTime(2087, 8, 14))
                .AndA.Double("averageSpeed", Double.NaN);
            
            Assert.That(spaceShip.ID, Is.EqualTo("spaceShip"));
		    Assert.That(spaceShip.Type, Is.Null);
			Assert.That(spaceShip.LocationEquatorial().Declination, Is.EqualTo(2));
			Assert.That(spaceShip.LocationLatLng("from").Latitude, Is.EqualTo(7));
			Assert.That(spaceShip.DateTime("arrivalTime"), Is.Not.Null);
	    }
	}
}
