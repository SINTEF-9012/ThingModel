#region

using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using ThingModel.Builders;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    class ThingTypeTest
    {
        private ThingType _type;
        private PropertyType _propertyType;

        [SetUp]
        protected void SetUp()
        {
            _type = new ThingType("Plane");
            _propertyType = PropertyType.Create<Property.Location.LatLng>("location");
            _type.DefineProperty(_propertyType);
        }

        [Test]
        public void CheckName()
        {
            Assert.That(_type.Name, Is.EqualTo("Plane"));
        }
        
        [Test]
        public void WrongName()
        {
            Assert.Throws<Exception>(() => new ThingType(""));
            Assert.Throws<Exception>(() => new ThingType(null));
        }

        [Test]
        public void CheckSlowPorpertyTypeConstructor()
        {
            var prop = new PropertyType("location", typeof (Property.Location));
            _type.DefineProperty(prop);

            Assert.Throws<Exception>(delegate
                {
                    prop = new PropertyType("location", typeof (string));
                });
        }

        [Test]
        public void SetProperty()
        {
            Assert.That(_type.GetPropertyDefinition("location"), Is.EqualTo(_propertyType));
        }

        [Test]
        public void CheckGetProperties()
        {

            var visited = false;
            foreach (var property in _type.GetProperties())
            {
                visited = true;
                // Should not throw exceptions
                _type.DefineProperty(property);
            }

            Assert.That(visited, Is.True);
        }

        [Test]
        public void CheckRequiredProperties()
        {
            var plane = new Thing("A380", _type);
            Assert.That(_type.Check(plane), Is.False);

            plane.SetProperty(new Property.Location.LatLng("location", new Location.LatLng(49.010852, 2.547398)));

            Assert.That(_type.Check(plane), Is.True);
        }

        [Test]
        public void CheckNotRequiredProperties()
        {
            var plane = new Thing("A380", _type);
            _propertyType.Required = false;
            Assert.That(_type.Check(plane), Is.True);

            // wrong type
            plane.SetProperty(new Property.Double("location", 27));
            Assert.That(_type.Check(plane), Is.False);
        }

	    [Test]
	    public void CheckEqualityComparator()
	    {
		    Assert.That(_type, Is.EqualTo(_type));
		    Assert.That(_type, Is.Not.EqualTo(null));
		    Assert.IsFalse(_type.Is(null));
		    Assert.That(_type, Is.Not.EqualTo(BuildANewThingType.Named("pony")));

		    var newType = BuildANewThingType.Named("Plane");
		    
			Assert.That(_type, Is.Not.EqualTo((ThingType) newType));

		    newType = BuildANewThingType.Named("Plane")
                .ContainingA.String("canard");
			Assert.That(_type, Is.Not.EqualTo((ThingType) newType));
	        
		    newType = BuildANewThingType.Named("Plane")
                .ContainingA.String("location");
			Assert.That(_type, Is.Not.EqualTo((ThingType) newType));

		    newType.ContainingA.LocationLatLng("location");
			Assert.That(_type, Is.EqualTo((ThingType) newType));
	    }

        [Test]
        public void CheckEqualityComparatorWithOrder()
        {
            ThingType typeA = BuildANewThingType.Named("aircraft")
                .ContainingA.LocationLatLng()
                .AndA.String("name");

            ThingType typeB = BuildANewThingType.Named("aircraft")
                .ContainingA.String("name")
                .AndA.LocationLatLng();

            Assert.That(typeA, Is.EqualTo(typeB));
            Assert.That(typeB, Is.EqualTo(typeA));
        }
        
        [Test]
        public void MultiThreadProperties()
        {

            ThingType thing = BuildANewThingType.Named("canard");

            const int nbLoop = 100000;
            var waitA = new AutoResetEvent(false);
            var waitB = new AutoResetEvent(false);

            new Thread(() =>
            {
                for (var i = 0; i < nbLoop; ++i)
                {
                    thing.DefineProperty(new PropertyType("a"+i, typeof (Property.Boolean)));
                }
	            waitA.Set();
	        }).Start();
            new Thread(() =>
            {
                for (var i = 0; i < nbLoop; ++i)
                {
                    thing.DefineProperty(new PropertyType("b"+i, typeof (Property.Boolean)));
                }
	            waitB.Set();
	        }).Start();

            waitA.WaitOne();
            waitB.WaitOne();

            Assert.That(thing.GetProperties().Count(), Is.EqualTo(nbLoop * 2));
        }
    }
}
