﻿#region

using System;
using NUnit.Framework;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    class WharehouseTest
    {
        private ThingType _type;
        private Thing _thing;
        private Wharehouse _wharehouse;

        private class ThingChangeObserver : IThingObserver
        {

            public bool NewThing = false;
            public bool UpdatedThing = false;
            public bool DeletedThing = false;

            public void Reset()
            {
                NewThing = false;
                UpdatedThing = false;
                DeletedThing = false;
            }

            public void New(Thing thing)
            {
                NewThing = true;
            }

            public void Deleted(Thing thing)
            {
                DeletedThing = true;
            }

            public void Updated(Thing thing)
            {
                UpdatedThing = true;
            }


        }

        private class TypeChangeObserver : IThingTypeObserver
        {

            public bool DefineType = false;

            public void Reset()
            {
                DefineType = false;
            }

            public void Define(ThingType thing)
            {
                DefineType = true;
            }
        }

        private ThingChangeObserver _thingChangeObserver;
        private TypeChangeObserver _typeChangeObserver;

        [SetUp]
        protected void SetUp()
        {
            _type = new ThingType("duck");
            _type.DefineProperty(PropertyType.Create<Property.String>("name"));
            _type.DefineProperty(PropertyType.Create<Property.Double>("age"));

            _thing = new Thing("871", _type);
            _thing.SetProperty(new Property.String("name", "Maurice"));
            _thing.SetProperty(new Property.Double("age", 18));
            _thing.SetProperty(new Property.Location("localization", new Location.Point(10,44)));

            _wharehouse = new Wharehouse();

            _thingChangeObserver = new ThingChangeObserver();
            _typeChangeObserver = new TypeChangeObserver();

            _wharehouse.RegisterObserver(_thingChangeObserver);
            _wharehouse.RegisterObserver(_typeChangeObserver);
        }

        [Test]
        public void RegisterType()
        {
            _wharehouse.RegisterType(_type);
            Assert.That(_typeChangeObserver.DefineType, Is.True);

            _typeChangeObserver.Reset();
            Assert.Throws<Exception>(() => _wharehouse.RegisterType(null));
            Assert.That(_typeChangeObserver.DefineType, Is.False);

        }

        [Test]
        public void RegisterThing()
        {
            _wharehouse.RegisterThing(_thing, false, false);
            Assert.That(_thingChangeObserver.NewThing, Is.True);

            _thingChangeObserver.Reset();
            Assert.Throws<Exception>(() => _wharehouse.RegisterThing(null));
            Assert.That(_thingChangeObserver.NewThing, Is.False);
        }

        [Test]
        public void RegisterThingWithTypes()
        {
            _wharehouse.RegisterThing(_thing, false, true);

            Assert.That(_typeChangeObserver.DefineType, Is.True);
        }
    }

}
