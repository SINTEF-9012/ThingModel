#region

using System;
using NUnit.Framework;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    public class ThingTest
    {
        private Thing _thing;
        private ThingType _type;
        private Thing _otherThing;
        private Property _name;

        [SetUp]
        protected void SetUp()
        {
            _type = new ThingType("Person");
            _type.DefineProperty(PropertyType.Create<Property.String>("name"));

            _thing = new Thing("200787", _type);
            

            _otherThing = new Thing("875402");
            _thing.Connect(_otherThing);

            _name = new Property.String("name", "Pierre");
            _thing.SetProperty(_name);
        }

        [Test]
        public void CheckID()
        {
            Assert.That(_thing.ID, Is.EqualTo("200787"));
        }

        [Test]
        public void WrongID()
        {
            Assert.Throws<Exception>(() => new Thing(""));
        }

        [Test]
        public void Connect()
        {
            Assert.That(_thing.IsConnectedTo(_otherThing), Is.True);
        }

        [Test]
        public void Disconnect()
        {
            _thing.Disconnect(_otherThing);
            Assert.That(_thing.IsConnectedTo(_otherThing), Is.False);
        }

        [Test]
        public void CheckName()
        {
            Assert.That(_thing.GetProperty<Property.String>("name").Value, Is.EqualTo("Pierre"));
        }

        [Test]
        public void HasProperty()
        {
            Assert.That(_thing.HasProperty("name"), Is.True);
            Assert.That(_thing.HasProperty("lapin"), Is.False);
        }

        [Test]
        public void GetPropertyWithWrongType()
        {
            Assert.That(_thing.GetProperty<Property.Number>("name"), Is.Null);
        }
        
        [Test]
        public void GetNonexistentProperty()
        {
            Assert.That(_thing.GetProperty<Property.Boolean>("abcdef"), Is.Null);
            Assert.That(_thing.GetProperty<Property.String>("abcdef"), Is.Null);
        }

        [Test]
        public void CheckObjet()
        {
            Assert.That(_type.Check(_thing), Is.True);

            _type.DefineProperty(PropertyType.Create<Property.Boolean>("beautiful"));
            Assert.That(_type.Check(_thing), Is.False);

            _thing.SetProperty(new Property.Location("beautiful", new Location.Equatorial()));
            Assert.That(_type.Check(_thing), Is.False);

            _type.DefineProperty(PropertyType.Create<Property.Location>("beautiful"));
            Assert.That(_type.Check(_thing), Is.True);
        }

        [Test]
        public void ConnectThingToItself()
        {
            Assert.Throws<Exception>(() => _thing.Connect(_thing));

            Assert.Throws<Exception>(() => _thing.Connect(new Thing(_thing.ID)));
        }

        [Test]
        public void CheckEnumerator()
        {
            Assert.That(_thing.ConnectedThings, Is.Not.Empty);

            // The connectedThing should be a copy of Connections if we delete or add things
            // during this loop
            foreach (var connectedThing in _thing.ConnectedThings)
            {
                _thing.Disconnect(connectedThing);
            }

            Assert.That(_thing.ConnectedThings, Is.Empty);
        }

        [Test]
        public void CompareSameThing()
        {
            Assert.That(_thing.Compare(_thing), Is.True);
        }

        [Test]
        public void CompareWithWrongType()
        {
            var newThing = new Thing(_thing.ID); // default type

            newThing.SetProperty(new Property.String("name", "Pierre"));
            newThing.Connect(_otherThing);

            Assert.That(_thing.Compare(newThing), Is.False);
        }

        [Test]
        public void CompareNotConnected()
        {
            var newThing = new Thing(_thing.ID, _type);

            newThing.SetProperty(new Property.String("name", "Pierre"));
            
            Assert.That(_thing.Compare(newThing), Is.False);
        }

        [Test]
        public void CompareWithDifferentID()
        {
            var newThing = new Thing("banana", _type);
            newThing.SetProperty(new Property.String("name", "Pierre"));
            newThing.Connect(_otherThing);

            Assert.That(_thing.Compare(newThing, false), Is.True);
            Assert.That(newThing.Compare(_thing, false), Is.True);

            // Different ID
            Assert.That(_thing.Compare(newThing), Is.False);
        }
        
        [Test]
        public void CompareProperties()
        {
            var newThing = new Thing(_thing.ID, _type);
            newThing.SetProperty(new Property.String("name", "Pierre"));
            newThing.SetProperty(new Property.String("surname", "Lapinou"));
            newThing.Connect(_otherThing);
            
            Assert.That(_thing.Compare(newThing), Is.False);
            Assert.That(newThing.Compare(_thing), Is.False);

            _thing.SetProperty(new Property.String("surname", "Lapinou"));
            Assert.That(_thing.Compare(newThing), Is.True);
            Assert.That(newThing.Compare(_thing), Is.True);

            _thing.SetProperty(new Property.String("surname", "Roger"));
            Assert.That(_thing.Compare(newThing), Is.False);
            Assert.That(newThing.Compare(_thing), Is.False);
        }

        [Test]
        public void DeepCompairaison()
        {
            var aThingForTheRoad = new Thing("rabbit");
            aThingForTheRoad.SetProperty(new Property.Number("speed", 12));
            _thing.Connect(aThingForTheRoad);

            var newThing = new Thing(_thing.ID, _type);
            newThing.SetProperty(new Property.String("name", "Pierre"));
            newThing.Connect(_otherThing);
            newThing.Connect(aThingForTheRoad);

            Assert.That(_thing.Compare(_thing, true, true), Is.True);
            Assert.That(_thing.Compare(newThing, true, true), Is.True);
            Assert.That(newThing.Compare(_thing, true, true), Is.True);

            newThing.Disconnect(_otherThing);
            var newOtherThing = new Thing(_otherThing.ID);
            newOtherThing.SetProperty(new Property.String("name", "Alain"));

            Assert.That(_thing.Compare(newThing, true, true), Is.False);
            Assert.That(newThing.Compare(_thing, true, true), Is.False);
        }
        
        [Test]
        public void InfiniteLoopDeepCompairaison()
        {
            var newThing = new Thing(_thing.ID, _type);
            newThing.SetProperty(new Property.String("name", "Pierre"));
            newThing.Connect(_otherThing);

            // Magic
            _otherThing.Connect(_thing);

            Assert.That(_thing.Compare(newThing, true, true), Is.True);
            Assert.That(newThing.Compare(_thing, true, true), Is.True);

            newThing.Disconnect(_otherThing);

            var newOtherThing = new Thing(_otherThing.ID);
            newThing.Connect(newOtherThing);

            Assert.That(_thing.Compare(newThing, true, true), Is.False);
            Assert.That(newThing.Compare(_thing, true, true), Is.False);

            newOtherThing.Connect(newThing);

            Assert.That(_thing.Compare(newThing, true, true), Is.True);
            Assert.That(newThing.Compare(_thing, true, true), Is.True);
        }

        [Test]
        public void CompareSomethingNull()
        {
            Assert.That(_thing.Compare(null), Is.False);
        }
    }
}