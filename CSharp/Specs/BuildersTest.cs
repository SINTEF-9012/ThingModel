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
				.WichIs("Just a rabbit")
				.ContainingA.String("name")
				.AndA.Location("localization")
				.AndA.NotRequired.Double("speed")
				.AndAn.Int("nbChildren", "Number of children")
					.WichIs("The number of direct children for the rabbit");

			Assert.That(type, Is.Not.Null);
			Assert.That(type.Description, Is.EqualTo("Just a rabbit"));

			Assert.That(type.GetPropertyDefinition("name"), Is.Not.Null);
			Assert.That(type.GetPropertyDefinition("speed"), Is.Not.Null);
			Assert.That(type.GetPropertyDefinition("speed").Required, Is.False);
			Assert.That(type.GetPropertyDefinition("nbChildren").Required, Is.True);
			Assert.That(type.GetPropertyDefinition("nbChildren").Description, Contains.Substring("direct children"));

			ThingType superRabbit = BuildANewThingType.Named("super_rabbit")
				.WichIs("A better rabbit")
				.ContainingA.CopyOfThis(type)
				.AndAn.Int("power");

			Assert.That(superRabbit.GetPropertyDefinition("name"), Is.Not.Null);
			Assert.That(superRabbit.GetPropertyDefinition("power"), Is.Not.Null);

			superRabbit.GetPropertyDefinition("nbChildren").Name = "Number of super children";
			
			Assert.That(type.GetPropertyDefinition("nbChildren").Name, Is.Not.StringContaining("Super"));
		}

		[Test]
		public void TestThingBuilder()
		{
			ThingType duckType = BuildANewThingType.Named("duck");

			Thing duck = BuildANewThing.As(duckType)
				.IdentifiedBy("ab548")
				.ContainingA.String("name", "Roger")
				.AndA.Location("localization", new Location.Point())
				.AndAn.Int("nbChildren", 12);

			Assert.That(duck.ID, Is.EqualTo("ab548"));
			Assert.That(duck.Type.Name, Is.EqualTo("duck"));
			Assert.That(duck.GetProperty<Property.Int>("nbChildren").Value,
				Is.EqualTo(12));
		}
	}
}
