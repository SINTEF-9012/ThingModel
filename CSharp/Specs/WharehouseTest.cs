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

        private class WharehouseChangeObserver : IWharehouseObserver
        {

            public bool NewThing;
            public bool UpdatedThing;
            public bool DeletedThing;
            public bool DefineType;

            public void Reset()
            {
                NewThing = false;
                UpdatedThing = false;
                DeletedThing = false;
                DefineType = false;
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

            public void Define(ThingType thing)
            {
                DefineType = true;
            }
        }

        private WharehouseChangeObserver _wharehouseChangeObserver;

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

            _wharehouseChangeObserver = new WharehouseChangeObserver();

            _wharehouse.RegisterObserver(_wharehouseChangeObserver);
        }

        [Test]
        public void RegisterType()
        {
            _wharehouse.RegisterType(_type);
            Assert.That(_wharehouseChangeObserver.DefineType, Is.True);

            _wharehouseChangeObserver.Reset();
            Assert.Throws<Exception>(() => _wharehouse.RegisterType(null));
            Assert.That(_wharehouseChangeObserver.DefineType, Is.False);

        }

        [Test]
        public void RegisterThing()
        {
            _wharehouse.RegisterThing(_thing, false);
            Assert.That(_wharehouseChangeObserver.NewThing, Is.True);

            _wharehouseChangeObserver.Reset();
            Assert.Throws<Exception>(() => _wharehouse.RegisterThing(null));
            Assert.That(_wharehouseChangeObserver.NewThing, Is.False);
        }

        [Test]
        public void RegisterThingWithTypes()
        {
            _wharehouse.RegisterThing(_thing, false, true);

            Assert.That(_wharehouseChangeObserver.DefineType, Is.True);

			_wharehouseChangeObserver.Reset();

	        var newThing = new Thing("without type");
			_wharehouse.RegisterThing(newThing, false, true);

            Assert.That(_wharehouseChangeObserver.DefineType, Is.False);
        }

        [Test]
        public void CheckThingUpdateCallback()
        {
            _wharehouse.RegisterThing(_thing);
            Assert.That(_wharehouseChangeObserver.UpdatedThing, Is.False);

            _wharehouse.RegisterThing(_thing);
            Assert.That(_wharehouseChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void UnregisterCallbacks()
        {
            _wharehouse.UnregisterObserver(_wharehouseChangeObserver);

            _wharehouse.RegisterThing(_thing);
            _wharehouse.RegisterType(_type);

            Assert.That(_wharehouseChangeObserver.NewThing, Is.False);
            Assert.That(_wharehouseChangeObserver.DefineType, Is.False);
        }

        [Test]
        public void DeleteCallback()
        {
            _wharehouse.RegisterThing(_thing);
            _wharehouse.RemoveThing(_thing);

            Assert.That(_wharehouseChangeObserver.DeletedThing, Is.True);

            _wharehouseChangeObserver.Reset();
            _wharehouse.RemoveThing(null);
            Assert.That(_wharehouseChangeObserver.DeletedThing, Is.False);
        }

	    [Test]
	    public void DeleteAndCreateAgain()
	    {
		    _wharehouse.RegisterThing(_thing);
			_wharehouse.RemoveThing(_thing);
			_wharehouseChangeObserver.Reset();
			_wharehouse.RegisterThing(_thing);
            Assert.That(_wharehouseChangeObserver.NewThing, Is.True);
	    }

        [Test]
        public void RecursiveRegistration()
        {
            _wharehouse.RegisterThing(_thing);
            _wharehouseChangeObserver.Reset();

            var newThing = new Thing("test");
            newThing.Connect(_thing);

            _thing.Connect(new Thing("blabla"));
            _thing.Connect(new Thing("blabla2"));

            _wharehouse.RegisterThing(newThing/*, true*/);

            Assert.That(_wharehouseChangeObserver.NewThing, Is.True);
            Assert.That(_wharehouseChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void NoInfiniteLoopRegistration()
        {
            var newThing = new Thing("loop");
            newThing.Connect(_thing);
            _thing.Connect(newThing);

            _wharehouse.RegisterThing(newThing);
            _wharehouse.RegisterThing(_thing);

            Assert.That(_wharehouseChangeObserver.NewThing, Is.True);
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

            Assert.That(_wharehouseChangeObserver.NewThing, Is.True);
        }

        [Test]
        public void DeleteWithConnections()
        {
            var otherThing = new Thing("lapin");
            otherThing.Connect(_thing);

            _wharehouse.RegisterThing(otherThing);
            _wharehouseChangeObserver.Reset();
            _wharehouse.RemoveThing(_thing);

            Assert.That(_wharehouseChangeObserver.DeletedThing, Is.True);
            Assert.That(_wharehouseChangeObserver.UpdatedThing, Is.True);

        }

	    [Test]
	    public void DeleteTwoConnectedThings()
	    {
		    var otherThing = new Thing("lapin");
			otherThing.Connect(_thing);
            _wharehouse.RegisterThing(otherThing);
            _wharehouse.RegisterThing(_thing);
            Assert.That(_wharehouse.Things.Count, Is.EqualTo(2));

            _wharehouseChangeObserver.Reset();
            _wharehouse.RemoveThing(_thing);
            _wharehouse.RemoveThing(otherThing);

            Assert.That(_wharehouse.Things.Count, Is.EqualTo(0));
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

					Assert.That(a.WaitOne(5000), Is.True);
					Assert.That(b.WaitOne(5000), Is.True);
					Assert.That(c.WaitOne(5000), Is.True);
//                    Assert.That(WaitHandle.WaitAll(new WaitHandle[] {a, b, c}, 5000), Is.True);
                });

        }

	    [Test]
	    public void EventsTest()
	    {
		    bool define = false;
		    _wharehouse.Events.OnDefine += (sender, args) => define = true;

			_wharehouse.RegisterType(_type);

			Assert.That(define, Is.True);

		    bool newThing = false;
		    _wharehouse.Events.OnNew += (sender, args) => newThing = true;
			_wharehouse.RegisterThing(_thing);
			Assert.That(newThing, Is.True);
		    
			bool update = false;
		    _wharehouse.Events.OnUpdate += (sender, args) => update = true;
			_wharehouse.RegisterThing(_thing);
			Assert.That(update, Is.True);
			
			bool delete = false;
		    _wharehouse.Events.OnDelete += (sender, args) => delete = true;
			_wharehouse.RemoveThing(_thing);
		    newThing = false;
			_wharehouse.RegisterThing(new Thing("canard"));
			Assert.That(newThing, Is.True);
			Assert.That(delete, Is.True);
	    }
    }

}
