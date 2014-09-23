#region

using System;
using NUnit.Framework;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    class PropertyTest
    {
        private Property _property;

        [SetUp]
        protected void SetUp()
        {
            _property = new Property.Double("speed");
        }

        [Test]
        public void CheckKey()
        {
            // Stupid test
            Assert.That(_property.Key, Is.EqualTo("speed"));
        }

        [Test]
        public void CheckBoringGetterAndSetter()
        {
            Assert.That(new Property.Boolean("good", true).Value, Is.True);

            Assert.That(new Property.String("name", "Alphonse").Value, Is.EqualTo("Alphonse"));

            Assert.That(new Property.Double("answer", 42).Value, Is.InRange(41, 43));
            Assert.That(new Property.Int("rounded answer", 42).Value, Is.InRange(41, 43));

            Assert.That(new Property.DateTime("time").Value, Is.LessThan(DateTime.Now));

            Assert.That(new Property.Location.LatLng("localization", new Location.LatLng(42.0,19.6,100)).Value, Is.Not.Null);

        }

        [Test]
        public void CheckToString()
        {
            Assert.That(new Property.Boolean("lapin", true).ValueToString().Length, Is.GreaterThan(0));

            Assert.That(new Property.String("name", "Alphonse").ValueToString(), Is.StringContaining("Alphonse"));

            Assert.That(new Property.Double("answer", 42).ValueToString(), Is.StringContaining("42"));
            Assert.That(new Property.Int("rounded answer", 42).ValueToString(), Is.StringContaining("42"));

            Assert.That(new Property.DateTime("time").ValueToString().Length, Is.GreaterThan(0));

            Assert.That(new Property.Location.LatLng("localization",
                new Location.LatLng(42.0, 19.6, 100)).ValueToString(), Is.StringContaining("42"));
        }

        [Test]
        public void LocationChange()
        {
            var a = new Property.Location.Equatorial("the key is not important", new Location.Equatorial(1, 2, 3));
            var b = new Property.Location.Equatorial("not the same key", new Location.Equatorial(1, 2, 3));
            var c = new Property.Location.LatLng("the key is not important", new Location.LatLng(1, 2, 3));
            var d = new Property.Location.LatLng("the key is not important");

            Assert.That(a.CompareValue(b), Is.True);

            
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(null), Is.False);
            Assert.That(d.CompareValue(a), Is.False);
            Assert.That(d.CompareValue(d), Is.True);

        }

        [Test]
        public void StringChange()
        {
            var a = new Property.String("hobbies", "hei");
            var b = new Property.String("no", "hei");
            var c = new Property.String(":-)", "salut");
            var d = new Property.String(":-)");

            Assert.That(a.CompareValue(a), Is.True);
            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(d), Is.False);
            Assert.That(d.CompareValue(d), Is.True);
            Assert.That(d.CompareValue(null), Is.False);
        }

        [Test]
        public void DoubleChange()
        {
            var a = new Property.Double("a", 1.0);
            var b = new Property.Double("b", 1.0);
            var c = new Property.Double("98", 98);
            var d = new Property.Double("a");

            Assert.That(a.CompareValue(a), Is.True);
            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(null), Is.False);
            Assert.That(d.CompareValue(a), Is.False);
            Assert.That(d.CompareValue(null), Is.False);
        }

        [Test]
        public void IntChange()
        {
            var a = new Property.Int("a", 1);
            var b = new Property.Int("b", 1);
            var c = new Property.Int("98", 98);
            var d = new Property.Int("a");

            Assert.That(a.CompareValue(a), Is.True);
            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(null), Is.False);
            Assert.That(d.CompareValue(a), Is.False);
            Assert.That(d.CompareValue(null), Is.False);
        }

        [Test]
        public void BooleanChange()
        {
            var a = new Property.Boolean("a", true);
            var b = new Property.Boolean("b", true);
            var c = new Property.Boolean("98");

            Assert.That(a.CompareValue(a), Is.True);
            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(null), Is.False);
            Assert.That(c.CompareValue(null), Is.False);
        }

        [Test]
        public void DateTimeChange()
        {
            var a = new Property.DateTime("a", DateTime.MaxValue);
            var b = new Property.DateTime("b", DateTime.MaxValue);
            var c = new Property.DateTime("98", DateTime.Now);
            var d = new Property.DateTime("a");

            Assert.That(a.CompareValue(a), Is.True);
            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(a.CompareValue(c), Is.False);
            Assert.That(a.CompareValue(null), Is.False);
            Assert.That(d.CompareValue(a), Is.False);
            Assert.That(d.CompareValue(null), Is.False);
        }

        [Test]
        public void CompareDifferentThings()
        {
            var a = new Property.Double("test");
            var b = new Property.Boolean("test");

            Assert.That(a.CompareValue(b), Is.False);
        }

        [Test]
        public void CompareWithGenericType()
        {
            var a = new Property.Double("test");
            Property b = new Property.Double("test");

            Assert.That(a.CompareValue(b), Is.True);
            Assert.That(b.CompareValue(a), Is.True);
        }

        [Test]
        public void LocationGetterAndSetter()
        {
            var a = new Location.Equatorial
                {
                    RightAscension = 42,
                    Declination = 42,
                    HourAngle = 42
                };
            Assert.That(a.RightAscension, Is.EqualTo(42));
            Assert.That(a.Declination, Is.EqualTo(42));
            Assert.That(a.HourAngle, Is.EqualTo(42));

            var b = new Location.LatLng
            {
                Latitude = 42,
                Longitude = 42,
                Altitude = 42
            };
            Assert.That(b.Latitude, Is.EqualTo(42));
            Assert.That(b.Longitude, Is.EqualTo(42));
            Assert.That(b.Altitude, Is.EqualTo(42));
        }

        [Test]
        public void TestPropertyTypeHashCode()
        {
            var a = new PropertyType("location", typeof (Property.Location.Equatorial));
            var b = new PropertyType("location", typeof (Property.Location.LatLng));

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));

            b = new PropertyType("location2", typeof (Property.Location.Equatorial));
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
            
            b = new PropertyType("location", typeof (Property.Location.Equatorial));
            b.Required = false;
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
            
            b = new PropertyType("location", typeof (Property.Location.Equatorial));
            b.Description = "this is a location";
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
            
            b = new PropertyType("location", typeof (Property.Location.Equatorial));
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void TestLocationHashCode()
        {
            var a = new Location.LatLng();
            var b = new Location.LatLng(1, 2, 3);

            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));

            // Same even it the type is not exactly the same
            var c = new Location.Point();
            Assert.That(a.GetHashCode(), Is.EqualTo(c.GetHashCode()));

            b = new Location.LatLng();
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void TestEmptyPropertyKey()
        {
            Assert.Throws<Exception>(() => new Property.DateTime(""));
        }

        [Test]
        public void TestEmptyPropertyTestkey()
        {
            Assert.Throws<Exception>(() => new PropertyType("", typeof (Property.String)));
        }

        [Test]
        public void TestClonePoint()
        {
            var a = new Property.Location.Point("p");
            a.Value = new Location.Point();
            var b = (Property.Location.Point) a.Clone();

            a.Value.X += 2;

            Assert.That(a.Value.X, Is.Not.EqualTo(b.Value.X));
        }
        
        [Test]
        public void TestCloneLatLng()
        {
            var a = new Property.Location.LatLng("p");
            a.Value = new Location.LatLng();
            var b = (Property.Location.LatLng) a.Clone();

            a.Value.Latitude += 2;

            Assert.That(a.Value.Latitude, Is.Not.EqualTo(b.Value.Latitude));
        }
        
        [Test]
        public void TestCloneEquatorial()
        {
            var a = new Property.Location.Equatorial("p");
            a.Value = new Location.Equatorial();
            var b = (Property.Location.Equatorial) a.Clone();

            a.Value.Declination += 2;

            Assert.That(a.Value.Declination, Is.Not.EqualTo(b.Value.Declination));
        }
        
        [Test]
        public void TestCloneString()
        {
            var a = new Property.String("p");
            a.Value = "a";
            var b = (Property.String) a.Clone();
            a.Value = "b";
            Assert.That(a.Value, Is.Not.EqualTo(b.Value));
        }
        
        [Test]
        public void TestCloneInt()
        {
            var a = new Property.Int("p");
            a.Value = 1;
            var b = (Property.Int) a.Clone();
            a.Value = 2;
            Assert.That(a.Value, Is.Not.EqualTo(b.Value));
        }

        [Test]
        public void TestCloneBoolean()
        {
            var a = new Property.Boolean("p");
            a.Value = true;
            var b = (Property.Boolean) a.Clone();
            a.Value = false;
            Assert.That(a.Value, Is.Not.EqualTo(b.Value));
        }

        [Test]
        public void TestCloneDateTime()
        {
            var a = new Property.DateTime("p");
            a.Value = new DateTime(1,2,3);
            var b = (Property.DateTime) a.Clone();
            a.Value = new DateTime(4,5,6);
            Assert.That(a.Value, Is.Not.EqualTo(b.Value));
        }
    }
}
