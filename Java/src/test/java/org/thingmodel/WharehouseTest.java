package org.thingmodel;

import junit.framework.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.ArrayList;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;

public class WharehouseTest {

    private ThingType _type;
    private Thing _thing;
    private Wharehouse _wharehouse;

    private class WharehouseChangeObserver implements IWharehouseObserver {

        public boolean NewThing;
        public boolean UpdatedThing;
        public boolean DeletedThing;
        public boolean DefinedType;

        public void Reset() {
            NewThing = false;
            UpdatedThing = false;
            DeletedThing = false;
            DefinedType = false;
        }

        @Override
        public void New(Thing thing) {
            NewThing = true;
        }

        @Override
        public void Deleted(Thing thing) {
            DeletedThing = true;
        }

        @Override
        public void Updated(Thing thing) {
            UpdatedThing = true;
        }

        @Override
        public void Define(ThingType thingType) {
            DefinedType = true;
        }
    }

    private WharehouseChangeObserver _wharehouseChangeObserver;

    @Before
    public void setUp() throws Exception {
        _type = new ThingType("duck");
        _type.DefineProperty(new PropertyType("name", Property.String.class));
        _type.DefineProperty(new PropertyType("age", Property.Double.class));

        _thing = new Thing("871", _type);
        _thing.setProperty(new Property.String("name", "Maurice"));
        _thing.setProperty(new Property.Double("age", 18.0));
        _thing.setProperty(new Property.Location("localization", new Location.Point(10,44)));

        _wharehouse = new Wharehouse();

        _wharehouseChangeObserver = new WharehouseChangeObserver();

        _wharehouse.RegisterObserver(_wharehouseChangeObserver);
    }

    @Test
    public void testRegisterType() throws Exception {
        _wharehouse.RegisterType(_type);
        Assert.assertTrue(_wharehouseChangeObserver.DefinedType);

        _wharehouseChangeObserver.Reset();

        try {
            _wharehouse.RegisterType(null);
            Assert.fail();
        } catch (Exception e) {}

        Assert.assertFalse(_wharehouseChangeObserver.DefinedType);
    }

    @Test
    public void testRegisterThing() throws Exception {
        _wharehouse.RegisterThing(_thing);
        Assert.assertTrue(_wharehouseChangeObserver.NewThing);

        _wharehouseChangeObserver.Reset();

        try {
            _wharehouse.RegisterThing(null);
            Assert.fail();
        } catch (Exception e) {}

        Assert.assertFalse(_wharehouseChangeObserver.NewThing);

    }

    @Test
    public void testRegisterThingWithType() throws Exception {
        _wharehouse.RegisterThing(_thing, true, true);

        Assert.assertTrue(_wharehouseChangeObserver.DefinedType);

        _wharehouseChangeObserver.Reset();

        _wharehouse.RegisterThing(new Thing("without type"), true, true);

        Assert.assertFalse(_wharehouseChangeObserver.DefinedType);
    }

    @Test
    public void testThingUpdateCallback() throws Exception {
        _wharehouse.RegisterThing(_thing);

        Assert.assertFalse(_wharehouseChangeObserver.UpdatedThing);

        _wharehouse.RegisterThing(_thing);

        Assert.assertTrue(_wharehouseChangeObserver.UpdatedThing);

    }

    @Test
    public void testUnregisterObservers() throws Exception {
        _wharehouse.UnregisterObserver(_wharehouseChangeObserver);

        _wharehouse.RegisterType(_type);
        _wharehouse.RegisterThing(_thing);

        Assert.assertFalse(_wharehouseChangeObserver.NewThing);
        Assert.assertFalse(_wharehouseChangeObserver.DefinedType);

    }

    @Test
    public void testDeleteCallback() throws Exception {
        _wharehouse.RegisterThing(_thing);
        _wharehouse.RemoveThing(_thing);

        Assert.assertTrue(_wharehouseChangeObserver.DeletedThing);

        _wharehouseChangeObserver.Reset();

        _wharehouse.RemoveThing(null);

        Assert.assertFalse(_wharehouseChangeObserver.DeletedThing);
    }

    @Test
    public void testDeleteAndCreateAgain() throws Exception {
        _wharehouse.RegisterThing(_thing);
        _wharehouse.RemoveThing(_thing);
        _wharehouseChangeObserver.Reset();
        _wharehouse.RegisterThing(_thing);
        Assert.assertTrue(_wharehouseChangeObserver.NewThing);
    }

    @Test
    public void testRecursiveRegistration() throws Exception {
        _wharehouse.RegisterThing(_thing);
        _wharehouseChangeObserver.Reset();

        Thing newThing = new Thing("test");
        newThing.Connect(_thing);

        _thing.Connect(new Thing("blabla"));
        _thing.Connect(new Thing("blabla2"));

        _wharehouse.RegisterThing(newThing);

        Assert.assertTrue(_wharehouseChangeObserver.NewThing);
        Assert.assertTrue(_wharehouseChangeObserver.UpdatedThing);
    }

    @Test
    public void testNoInfiniteLoopRegistration() throws Exception {
        Thing newThing = new Thing("loop");
        newThing.Connect(_thing);
        _thing.Connect(newThing);

        _wharehouse.RegisterThing(newThing);
        _wharehouse.RegisterThing(_thing);

        Assert.assertTrue(_wharehouseChangeObserver.NewThing);
    }

    @Test
    public void testRegisterCollection() throws Exception {
        Thing newThing = new Thing("loop");
        newThing.Connect(_thing);
        _thing.Connect(newThing);

        ArrayList<Thing> collection = new ArrayList<>();
        collection.add(newThing);
        collection.add(_thing);

        _wharehouse.RegisterCollection(collection);

        Assert.assertTrue(_wharehouseChangeObserver.NewThing);
    }

    @Test
    public void testDeleteWithConnections() throws Exception {
        Thing otherThing = new Thing("lapin");
        otherThing.Connect(_thing);

        _wharehouse.RegisterThing(otherThing);
        _wharehouseChangeObserver.Reset();
        _wharehouse.RemoveThing(_thing);

        Assert.assertTrue(_wharehouseChangeObserver.DeletedThing);
        Assert.assertTrue(_wharehouseChangeObserver.UpdatedThing);
    }

    @Test
    public void testMultiThreadCarnage() throws Exception {
        final CountDownLatch a = new CountDownLatch(1);
        final CountDownLatch b = new CountDownLatch(1);
        final CountDownLatch c = new CountDownLatch(1);

        Thread creationA = new Thread(new Runnable() {
            @Override
            public void run() {
                for (int i = 0; i < 1000; ++i) {
                    _wharehouse.RegisterThing(new Thing("banana_"+i));
                }
                a.countDown();
            }
        });

        Thread creationB = new Thread(new Runnable() {
            @Override
            public void run() {
                for (int i = 0; i < 1000; ++i) {
                    _wharehouse.RegisterThing(new Thing("lapin_"+i));
                }
                b.countDown();
            }
        });

        Thread Deletions = new Thread(new Runnable() {
            @Override
            public void run() {
                for (int i = 0; i < 1000; ++i) {
                    _wharehouse.RegisterThing(new Thing("lapin_"+i));
                }
                c.countDown();
            }
        });

        creationA.start();
        creationB.start();
        Deletions.start();

        Assert.assertTrue(a.await(5, TimeUnit.SECONDS));
        Assert.assertTrue(b.await(5, TimeUnit.SECONDS));
        Assert.assertTrue(c.await(5, TimeUnit.SECONDS));
    }
}
