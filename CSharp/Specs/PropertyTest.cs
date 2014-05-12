﻿#region

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
    }
}
