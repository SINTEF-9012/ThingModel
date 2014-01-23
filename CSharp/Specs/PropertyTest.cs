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
        public void CodeCoverage()
        {
            Assert.That(new Property.Boolean("good", true).Value, Is.True);

            Assert.That(new Property.String("name", "Alphonse").Value, Is.EqualTo("Alphonse"));

            Assert.That(new Property.Double("answer", 42).Value, Is.InRange(41, 43));

            Assert.That(new Property.DateTime("time").Value, Is.LessThan(DateTime.Now));

            Assert.That(new Property.Location("localization", new Location.LatLng(42.0,19.6,100)).Value, Is.Not.Null);
        }

        [Test]
        public void LocationChange()
        {
            var a = new Property.Location("the key is not important", new Location.Equatorial(1, 2, 3));
            var b = new Property.Location("not the same key", new Location.Equatorial(1, 2, 3));
            var c = new Property.Location("the key is not important", new Location.LatLng(1, 2, 3));
            var d = new Property.Location("the key is not important", null);

            Assert.That(a.Compare(b), Is.True);

            
            Assert.That(a.Compare(c), Is.False);
            Assert.That(a.Compare(null), Is.False);
            Assert.That(d.Compare(a), Is.False);
            Assert.That(d.Compare(d), Is.True);

        }

        [Test]
        public void StringChange()
        {
            var a = new Property.String("hobbies", "hei");
            var b = new Property.String("no", "hei");
            var c = new Property.String(":-)", "salut");
            var d = new Property.String(":-)");

            Assert.That(a.Compare(a), Is.True);
            Assert.That(a.Compare(b), Is.True);
            Assert.That(a.Compare(c), Is.False);
            Assert.That(a.Compare(d), Is.False);
            Assert.That(d.Compare(d), Is.True);
            Assert.That(d.Compare(null), Is.False);
        }

        [Test]
        public void NumberChange()
        {
            var a = new Property.Double("a", 1.0);
            var b = new Property.Double("b", 1.0);
            var c = new Property.Double("98", 98);
            var d = new Property.Double("a");

            Assert.That(a.Compare(a), Is.True);
            Assert.That(a.Compare(b), Is.True);
            Assert.That(a.Compare(c), Is.False);
            Assert.That(a.Compare(null), Is.False);
            Assert.That(d.Compare(a), Is.False);
            Assert.That(d.Compare(null), Is.False);
        }

        [Test]
        public void BooleanChange()
        {
            var a = new Property.Boolean("a", true);
            var b = new Property.Boolean("b", true);
            var c = new Property.Boolean("98");

            Assert.That(a.Compare(a), Is.True);
            Assert.That(a.Compare(b), Is.True);
            Assert.That(a.Compare(c), Is.False);
            Assert.That(a.Compare(null), Is.False);
            Assert.That(c.Compare(null), Is.False);
        }

        [Test]
        public void DateTimeChange()
        {
            var a = new Property.DateTime("a", DateTime.MaxValue);
            var b = new Property.DateTime("b", DateTime.MaxValue);
            var c = new Property.DateTime("98", DateTime.Now);
            var d = new Property.DateTime("a");

            Assert.That(a.Compare(a), Is.True);
            Assert.That(a.Compare(b), Is.True);
            Assert.That(a.Compare(c), Is.False);
            Assert.That(a.Compare(null), Is.False);
            Assert.That(d.Compare(a), Is.False);
            Assert.That(d.Compare(null), Is.False);
        }

        [Test]
        public void CompareDifferentThings()
        {
            var a = new Property.Double("test");
            var b = new Property.Boolean("test");

            Assert.That(a.Compare(b), Is.False);
        }

        [Test]
        public void CompareWithGenericType()
        {
            var a = new Property.Double("test");
            Property b = new Property.Double("test");

            Assert.That(a.Compare(b), Is.True);
            Assert.That(b.Compare(a), Is.True);
        }
    }
}
