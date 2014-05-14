package org.thingmodel;

import junit.framework.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.ArrayList;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

public class WarehouseTest {

    private ThingType _type;
    private Thing _thing;
    private Warehouse _warehouse;

    private class WarehouseChangeObserver implements IWarehouseObserver {

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
        public void New(Thing thing, String sender) {
            NewThing = true;
        }

        @Override
        public void Deleted(Thing thing, String sender) {
            DeletedThing = true;
        }

        @Override
        public void Updated(Thing thing, String sender) {
            UpdatedThing = true;
        }

        @Override
        public void Define(ThingType thingType, String sender) {
            DefinedType = true;
        }
    }

    private WarehouseChangeObserver _warehouseChangeObserver;

    @Before
    public void setUp() throws Exception {
        _type = new ThingType("duck");
        _type.DefineProperty(new PropertyType("name", Property.String.class));
        _type.DefineProperty(new PropertyType("age", Property.Double.class));

        _thing = new Thing("871", _type);
        _thing.setProperty(new Property.String("name", "Maurice"));
        _thing.setProperty(new Property.Double("age", 18.0));
        _thing.setProperty(new Property.Location.Point("localization", new Location.Point(10,44)));

        _warehouse = new Warehouse();

        _warehouseChangeObserver = new WarehouseChangeObserver();

        _warehouse.RegisterObserver(_warehouseChangeObserver);
    }

    @Test
    public void testRegisterType() throws Exception {
        _warehouse.RegisterType(_type);
        Assert.assertTrue(_warehouseChangeObserver.DefinedType);

        _warehouseChangeObserver.Reset();

        try {
            _warehouse.RegisterType(null);
            Assert.fail();
        } catch (Exception e) {}

        Assert.assertFalse(_warehouseChangeObserver.DefinedType);
    }

    @Test
    public void testRegisterThing() throws Exception {
        _warehouse.RegisterThing(_thing);
        Assert.assertTrue(_warehouseChangeObserver.NewThing);

        _warehouseChangeObserver.Reset();

        try {
            _warehouse.RegisterThing(null);
            Assert.fail();
        } catch (Exception e) {}

        Assert.assertFalse(_warehouseChangeObserver.NewThing);

    }

    @Test
    public void testRegisterThingWithType() throws Exception {
        _warehouse.RegisterThing(_thing, true, true, null);

        Assert.assertTrue(_warehouseChangeObserver.DefinedType);

        _warehouseChangeObserver.Reset();

        _warehouse.RegisterThing(new Thing("without type"), true, true, null);

        Assert.assertFalse(_warehouseChangeObserver.DefinedType);
    }

    @Test
    public void testThingUpdateCallback() throws Exception {
        _warehouse.RegisterThing(_thing);

        Assert.assertFalse(_warehouseChangeObserver.UpdatedThing);

        _warehouse.RegisterThing(_thing);

        Assert.assertTrue(_warehouseChangeObserver.UpdatedThing);

    }

    @Test
    public void testUnregisterObservers() throws Exception {
        _warehouse.UnregisterObserver(_warehouseChangeObserver);

        _warehouse.RegisterType(_type);
        _warehouse.RegisterThing(_thing);

        Assert.assertFalse(_warehouseChangeObserver.NewThing);
        Assert.assertFalse(_warehouseChangeObserver.DefinedType);

    }

    @Test
    public void testDeleteCallback() throws Exception {
        _warehouse.RegisterThing(_thing);
        _warehouse.RemoveThing(_thing);

        Assert.assertTrue(_warehouseChangeObserver.DeletedThing);

        _warehouseChangeObserver.Reset();

        _warehouse.RemoveThing(null);

        Assert.assertFalse(_warehouseChangeObserver.DeletedThing);
    }

    @Test
    public void testDeleteAndCreateAgain() throws Exception {
        _warehouse.RegisterThing(_thing);
        _warehouse.RemoveThing(_thing);
        _warehouseChangeObserver.Reset();
        _warehouse.RegisterThing(_thing);
        Assert.assertTrue(_warehouseChangeObserver.NewThing);
    }

    @Test
    public void testRecursiveRegistration() throws Exception {
        _warehouse.RegisterThing(_thing);
        _warehouseChangeObserver.Reset();

        Thing newThing = new Thing("test");
        newThing.Connect(_thing);

        _thing.Connect(new Thing("blabla"));
        _thing.Connect(new Thing("blabla2"));

        _warehouse.RegisterThing(newThing);

        Assert.assertTrue(_warehouseChangeObserver.NewThing);
        Assert.assertTrue(_warehouseChangeObserver.UpdatedThing);
    }

    @Test
    public void testNoInfiniteLoopRegistration() throws Exception {
        Thing newThing = new Thing("loop");
        newThing.Connect(_thing);
        _thing.Connect(newThing);

        _warehouse.RegisterThing(newThing);
        _warehouse.RegisterThing(_thing);

        Assert.assertTrue(_warehouseChangeObserver.NewThing);
    }

    @Test
    public void testRegisterCollection() throws Exception {
        Thing newThing = new Thing("loop");
        newThing.Connect(_thing);
        _thing.Connect(newThing);

        ArrayList<Thing> collection = new ArrayList<>();
        collection.add(newThing);
        collection.add(_thing);

        _warehouse.RegisterCollection(collection);

        Assert.assertTrue(_warehouseChangeObserver.NewThing);
    }

    @Test
    public void testDeleteWithConnections() throws Exception {
        Thing otherThing = new Thing("lapin");
        otherThing.Connect(_thing);

        _warehouse.RegisterThing(otherThing);
        _warehouseChangeObserver.Reset();
        _warehouse.RemoveThing(_thing);

        Assert.assertTrue(_warehouseChangeObserver.DeletedThing);
        Assert.assertTrue(_warehouseChangeObserver.UpdatedThing);
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
                    _warehouse.RegisterThing(new Thing("banana_"+i));
                }
                a.countDown();
            }
        });

        Thread creationB = new Thread(new Runnable() {
            @Override
            public void run() {
                for (int i = 0; i < 1000; ++i) {
                    _warehouse.RegisterThing(new Thing("lapin_"+i));
                }
                b.countDown();
            }
        });

        Thread Deletions = new Thread(new Runnable() {
            @Override
            public void run() {
                for (int i = 0; i < 1000; ++i) {
                    _warehouse.RegisterThing(new Thing("lapin_"+i));
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
