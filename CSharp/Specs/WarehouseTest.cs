#region

using System;
using System.Threading;
using NUnit.Framework;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    class WarehouseTest
    {
        private ThingType _type;
        private Thing _thing;
        private Warehouse _warehouse;

        private class WarehouseChangeObserver : IWarehouseObserver
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

        private WarehouseChangeObserver _warehouseChangeObserver;

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

            _warehouse = new Warehouse();

            _warehouseChangeObserver = new WarehouseChangeObserver();

            _warehouse.RegisterObserver(_warehouseChangeObserver);
        }

        [Test]
        public void RegisterType()
        {
            _warehouse.RegisterType(_type);
            Assert.That(_warehouseChangeObserver.DefineType, Is.True);

            _warehouseChangeObserver.Reset();
            Assert.Throws<Exception>(() => _warehouse.RegisterType(null));
            Assert.That(_warehouseChangeObserver.DefineType, Is.False);

        }

        [Test]
        public void RegisterThing()
        {
            _warehouse.RegisterThing(_thing, false);
            Assert.That(_warehouseChangeObserver.NewThing, Is.True);

            _warehouseChangeObserver.Reset();
            Assert.Throws<Exception>(() => _warehouse.RegisterThing(null));
            Assert.That(_warehouseChangeObserver.NewThing, Is.False);
        }

        [Test]
        public void RegisterThingWithTypes()
        {
            _warehouse.RegisterThing(_thing, false, true);

            Assert.That(_warehouseChangeObserver.DefineType, Is.True);

			_warehouseChangeObserver.Reset();

	        var newThing = new Thing("without type");
			_warehouse.RegisterThing(newThing, false, true);

            Assert.That(_warehouseChangeObserver.DefineType, Is.False);
        }

        [Test]
        public void CheckThingUpdateCallback()
        {
            _warehouse.RegisterThing(_thing);
            Assert.That(_warehouseChangeObserver.UpdatedThing, Is.False);

            _warehouse.RegisterThing(_thing);
            Assert.That(_warehouseChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void UnregisterCallbacks()
        {
            _warehouse.UnregisterObserver(_warehouseChangeObserver);

            _warehouse.RegisterThing(_thing);
            _warehouse.RegisterType(_type);

            Assert.That(_warehouseChangeObserver.NewThing, Is.False);
            Assert.That(_warehouseChangeObserver.DefineType, Is.False);
        }

        [Test]
        public void DeleteCallback()
        {
            _warehouse.RegisterThing(_thing);
            _warehouse.RemoveThing(_thing);

            Assert.That(_warehouseChangeObserver.DeletedThing, Is.True);

            _warehouseChangeObserver.Reset();
            _warehouse.RemoveThing(null);
            Assert.That(_warehouseChangeObserver.DeletedThing, Is.False);
        }

	    [Test]
	    public void DeleteAndCreateAgain()
	    {
		    _warehouse.RegisterThing(_thing);
			_warehouse.RemoveThing(_thing);
			_warehouseChangeObserver.Reset();
			_warehouse.RegisterThing(_thing);
            Assert.That(_warehouseChangeObserver.NewThing, Is.True);
	    }

        [Test]
        public void RecursiveRegistration()
        {
            _warehouse.RegisterThing(_thing);
            _warehouseChangeObserver.Reset();

            var newThing = new Thing("test");
            newThing.Connect(_thing);

            _thing.Connect(new Thing("blabla"));
            _thing.Connect(new Thing("blabla2"));

            _warehouse.RegisterThing(newThing/*, true*/);

            Assert.That(_warehouseChangeObserver.NewThing, Is.True);
            Assert.That(_warehouseChangeObserver.UpdatedThing, Is.True);
        }

        [Test]
        public void NoInfiniteLoopRegistration()
        {
            var newThing = new Thing("loop");
            newThing.Connect(_thing);
            _thing.Connect(newThing);

            _warehouse.RegisterThing(newThing);
            _warehouse.RegisterThing(_thing);

            Assert.That(_warehouseChangeObserver.NewThing, Is.True);
        }

        [Test]
        public void RegisterCollection()
        {
            var newThing = new Thing("loop");
            newThing.Connect(_thing);
            _thing.Connect(newThing);

            _warehouse.RegisterCollection(new[]
                {
                    newThing,
                    _thing
                });

            Assert.That(_warehouseChangeObserver.NewThing, Is.True);
        }

        [Test]
        public void DeleteWithConnections()
        {
            var otherThing = new Thing("lapin");
            otherThing.Connect(_thing);

            _warehouse.RegisterThing(otherThing);
            _warehouseChangeObserver.Reset();
            _warehouse.RemoveThing(_thing);

            Assert.That(_warehouseChangeObserver.DeletedThing, Is.True);
            Assert.That(_warehouseChangeObserver.UpdatedThing, Is.True);

        }

	    [Test]
	    public void DeleteTwoConnectedThings()
	    {
		    var otherThing = new Thing("lapin");
			otherThing.Connect(_thing);
            _warehouse.RegisterThing(otherThing);
            _warehouse.RegisterThing(_thing);
            Assert.That(_warehouse.Things.Count, Is.EqualTo(2));

            _warehouseChangeObserver.Reset();
            _warehouse.RemoveThing(_thing);
            _warehouse.RemoveThing(otherThing);

            Assert.That(_warehouse.Things.Count, Is.EqualTo(0));
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
                            _warehouse.RegisterThing(new Thing("banana_" + ++cptCreationA));
                        }
                        a.Set();
                    });

                    var cptCreationB = 0;
                    var creationB = new Thread(delegate(object sender)
                    {
                        for (var i = 0; i < 1000; ++i)
                        {
                            _warehouse.RegisterThing(new Thing("lapin_" + ++cptCreationB));
                        }
                        b.Set();
                    });

                    var cptDeletions = 0;
                    var deletions = new Thread(delegate(object sender)
                    {
                        for (var i = 0; i < 1000; ++i)
                        {
                            _warehouse.RemoveThing(_warehouse.GetThing("lapin_" + ++cptDeletions));
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
		    _warehouse.Events.OnDefine += (sender, args) => define = true;

			_warehouse.RegisterType(_type);

			Assert.That(define, Is.True);

		    bool newThing = false;
		    _warehouse.Events.OnNew += (sender, args) => newThing = true;
			_warehouse.RegisterThing(_thing);
			Assert.That(newThing, Is.True);
		    
			bool update = false;
		    _warehouse.Events.OnUpdate += (sender, args) => update = true;
			_warehouse.RegisterThing(_thing);
			Assert.That(update, Is.True);
			
			bool delete = false;
		    _warehouse.Events.OnDelete += (sender, args) => delete = true;
			_warehouse.RemoveThing(_thing);
		    newThing = false;
			_warehouse.RegisterThing(new Thing("canard"));
			Assert.That(newThing, Is.True);
			Assert.That(delete, Is.True);
	    }
    }

}
