#region

using System;
using System.Threading;
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

            public bool NewThing;
            public bool UpdatedThing;
            public bool DeletedThing;

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

            public bool DefineType;

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
            _wharehouse.RegisterThing(_thing, false);
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

        [Test]
        public void CheckThingUpdateCallback()
        {
            _wharehouse.RegisterThing(_thing);
            Assert.That(_thingChangeObserver.UpdatedThing, Is.False);

            _wharehouse.RegisterThing(_thing);
            Assert.That(_thingChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void UnregisterCallbacks()
        {
            _wharehouse.Unregister(_thingChangeObserver);
            _wharehouse.Unregister(_typeChangeObserver);

            _wharehouse.RegisterThing(_thing);
            _wharehouse.RegisterType(_type);

            Assert.That(_thingChangeObserver.NewThing, Is.False);
            Assert.That(_typeChangeObserver.DefineType, Is.False);
        }

        [Test]
        public void DeleteCallback()
        {
            _wharehouse.RegisterThing(_thing);
            _wharehouse.RemoveThing(_thing);

            Assert.That(_thingChangeObserver.DeletedThing, Is.True);
        }

        [Test]
        public void RecursiveRegistration()
        {
            _wharehouse.RegisterThing(_thing);
            _thingChangeObserver.Reset();

            var newThing = new Thing("test");
            newThing.Connect(_thing);

            _thing.Connect(new Thing("blabla"));
            _thing.Connect(new Thing("blabla2"));

            _wharehouse.RegisterThing(newThing/*, true*/);

            Assert.That(_thingChangeObserver.NewThing, Is.True);
            Assert.That(_thingChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void NoInfiniteLoopRegistration()
        {
            var newThing = new Thing("loop");
            newThing.Connect(_thing);
            _thing.Connect(newThing);

            _wharehouse.RegisterThing(newThing);
            _wharehouse.RegisterThing(_thing);

            Assert.That(_thingChangeObserver.NewThing, Is.True);
        }

        [Test]
        public void RegisterCollection()
        {
            var newThing = new Thing("loop");
            newThing.Connect(_thing);
            _thing.Connect(newThing);

            _wharehouse.RegisterCollection(new[]
                {
                    newThing,
                    _thing
                });

            Assert.That(_thingChangeObserver.NewThing, Is.True);
        }

        [Test]
        public void DeleteWithConnections()
        {
            var otherThing = new Thing("lapin");
            otherThing.Connect(_thing);

            _wharehouse.RegisterThing(otherThing);
            _thingChangeObserver.Reset();
            _wharehouse.RemoveThing(_thing);

            Assert.That(_thingChangeObserver.DeletedThing, Is.True);
            Assert.That(_thingChangeObserver.UpdatedThing, Is.True);

        }

        [Test]
        public void MultiThreadCarnage()
        {
            Assert.DoesNotThrow(delegate
                {
                    var a = new ManualResetEvent(false);
                    var b = new ManualResetEvent(false);
                    var c = new ManualResetEvent(false);

                    var cptCreationA = 0;
                    var creationA = new Thread(delegate(object sender)
                    {
                        for (var i = 0; i < 1000; ++i)
                        {
                            _wharehouse.RegisterThing(new Thing("banana_" + ++cptCreationA));
                        }
                        a.Set();
                    });

                    var cptCreationB = 0;
                    var creationB = new Thread(delegate(object sender)
                    {
                        for (var i = 0; i < 1000; ++i)
                        {
                            _wharehouse.RegisterThing(new Thing("lapin_" + ++cptCreationB));
                        }
                        b.Set();
                    });

                    var cptDeletions = 0;
                    var deletions = new Thread(delegate(object sender)
                    {
                        for (var i = 0; i < 1000; ++i)
                        {
                            _wharehouse.RemoveThing(_wharehouse.GetThing("lapin_" + ++cptDeletions));
                        }
                        c.Set();
                    });


                    creationA.Start();
                    creationB.Start();
                    deletions.Start();

                    Assert.That(WaitHandle.WaitAll(new WaitHandle[] {a, b, c}, 5000), Is.True);
                });

        }
    }

}
