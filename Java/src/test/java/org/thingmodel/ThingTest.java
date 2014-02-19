package org.thingmodel;

import org.junit.Before;
import org.junit.Test;
import org.junit.Assert;

public class ThingTest {

    private Thing _thing;
    private ThingType _type;
    private Thing _otherThing;
    private Property _name;

    @Before
    public void setUp() throws Exception {
        _type = new ThingType("rabbit");
        _type.DefineProperty(new PropertyType("name", Property.String.class));

        _thing = new Thing("Groaw", _type);

        _otherThing = new Thing("Gobiwa");
        _thing.Connect(_otherThing);

        _name = new Property.String("name", "Alphonse");
        _thing.setProperty(_name);
    }

    @Test
    public void testWrongID() throws Exception {

        try {

            new Thing(null);
            Assert.fail();
        } catch (Exception e) {

        }

        try {

            new Thing("");
            Assert.fail();
        } catch (Exception e) {

        }


    }

    @Test
    public void testSetProperties() throws Exception {
        _thing.setProperty(new Property.Integer("speed", 15));
        _thing.setProperty(new Property.Double("speed", 15.0));
        Assert.assertNotNull(_thing.getProperty("speed"));
    }

    @Test
    public void testGetProperty() throws Exception {
        Assert.assertNotNull(_thing.getProperty("name", Property.String.class));
        Assert.assertNotNull(_thing.getProperty("name"));
        Assert.assertNotNull(_thing.getProperty("name", null));

        Assert.assertNull(_thing.getProperty("name", Property.class));
        Assert.assertNull(_thing.getProperty("name", Property.Double.class));
        Assert.assertNull(_thing.getProperty("this is a wrong key", Property.String.class));
    }

    @Test
    public void testHasProperty() throws Exception {
        Assert.assertTrue(_thing.hasProperty("name"));
        Assert.assertFalse(_thing.hasProperty("name2"));
    }

    @Test
    public void testConnect() throws Exception {
        Assert.assertTrue(_thing.IsConnectedTo(_otherThing));
        Assert.assertFalse(_otherThing.IsConnectedTo(_thing));

        _thing.Connect(null);
        Assert.assertFalse(_thing.IsConnectedTo(null));

        _thing.Disconnect(null);
    }

    @Test
    public void testDisconnect() throws Exception {
        _thing.Disconnect(_otherThing);
        Assert.assertFalse(_thing.IsConnectedTo(_otherThing));
    }

    @Test
    public void testDisconnectAll() throws Exception {
        _thing.DisconnectAll();
        Assert.assertFalse(_thing.IsConnectedTo(_otherThing));
    }

    @Test
    public void testGetConnectedThings() throws Exception {
        for (Thing t : _thing.getConnectedThings()) {
            _thing.Disconnect(t);
        }

        Assert.assertFalse(_thing.IsConnectedTo(_otherThing));
    }

    @Test
    public void testGetConnectedThingsCount() throws Exception {
        Assert.assertEquals(_thing.getConnectedThingsCount(), 1);
        _thing.Disconnect(_otherThing);
        Assert.assertEquals(_thing.getConnectedThingsCount(), 0);
    }

    @Test
    public void testGetProperties() throws Exception {
        for (Property p : _thing.getProperties()) {
            Assert.assertEquals(p.getKey(), "name");
        }
    }

    @Test
    public void testConnectingThingtoItself() throws Exception {
        try {
            _thing.Connect(_thing);
            Assert.fail();
        } catch (Exception e) {}

        try {
            _thing.Connect(new Thing(_thing.getId()));
            Assert.fail();
        } catch (Exception e) {}
    }

    @Test
    public void testCompareSameThing() throws Exception {
        Assert.assertTrue(_thing.Compare(_thing));
    }

    @Test
    public void testCompareWithWrongType() throws Exception {
        Thing newThing = new Thing(_thing.getId()); // default type

        newThing.setProperty(new Property.String("name", "Alphonse"));
        newThing.Connect(_otherThing);

        Assert.assertFalse(newThing.Compare(_thing));
        Assert.assertFalse(_thing.Compare(newThing));

    }

    @Test
    public void testCompareNotConnected() throws Exception {
        Thing newThing = new Thing(_thing.getId(), _type);
        newThing.setProperty(new Property.String("name", "Alphonse"));

        Assert.assertFalse(newThing.Compare(_thing));
        Assert.assertFalse(_thing.Compare(newThing));
    }

    @Test
    public void testCompareWithDifferentID() throws Exception {
        Thing newThing = new Thing("banana", _type);
        newThing.setProperty(new Property.String("name", "Alphonse"));
        newThing.Connect(_otherThing);

        Assert.assertTrue(newThing.Compare(_thing, false, false));
        Assert.assertTrue(_thing.Compare(newThing, false, false));


        Assert.assertFalse(_thing.Compare(newThing, true, false));
    }

    @Test
    public void testCompareProperties() throws Exception {
        Thing newThing = new Thing(_thing.getId(), _type);
        newThing.setProperty(new Property.String("name", "Alphonse"));
        newThing.setProperty(new Property.String("surname", "Lapinou"));
        newThing.Connect(_otherThing);

        Assert.assertFalse(_thing.Compare(newThing));
        Assert.assertFalse(newThing.Compare(_thing));

        _thing.setProperty(new Property.String("surname", "Lapinou"));
        Assert.assertTrue(_thing.Compare(newThing));
        Assert.assertTrue(newThing.Compare(_thing));

        _thing.setProperty(new Property.String("surname", "Roger"));
        Assert.assertFalse(_thing.Compare(newThing));
        Assert.assertFalse(newThing.Compare(_thing));

        _thing.setProperty(new Property.String("surname", "Lapinou"));
        _thing.setProperty(new Property.String("color", "white"));
        newThing.setProperty(new Property.String("meal", "nothing"));
        Assert.assertFalse(_thing.Compare(newThing));
        Assert.assertFalse(newThing.Compare(_thing));
    }

    @Test
    public void testDeepComparison() throws Exception {
        Thing aThingForTheRoad = new Thing("rabbit");

        aThingForTheRoad.setProperty(new Property.Double("speed", 12.0));
        _thing.Connect(aThingForTheRoad);

        Thing newThing = new Thing(_thing.getId(), _type);
        newThing.setProperty(new Property.String("name", "Alphonse"));
        newThing.Connect(aThingForTheRoad);
        newThing.Connect(_otherThing);

        Assert.assertTrue(_thing.Compare(_thing, true, true));
        Assert.assertTrue(_thing.Compare(newThing, true, true));
        Assert.assertTrue(newThing.Compare(_thing, true, true));

        newThing.Disconnect(_otherThing);
        Thing newOtherThing = new Thing(_otherThing.getId());
        newThing.Connect(newOtherThing);


        Assert.assertTrue(_thing.Compare(newThing, true, true));
        Assert.assertTrue(newThing.Compare(_thing, true, true));

        newOtherThing.setProperty(new Property.String("name", "Alain"));

        Assert.assertFalse(_thing.Compare(newThing, true, true));
        Assert.assertFalse(newThing.Compare(_thing, true, true));
    }

    @Test
    public void testInfiniteLoopComparison() throws Exception {
        Thing newThing = new Thing(_thing.getId(), _type);
        newThing.setProperty(new Property.String("name", "Alphonse"));
        newThing.Connect(_otherThing);

        // Magic
        _otherThing.Connect(_thing);

        Assert.assertTrue(_thing.Compare(newThing, true, true));
        Assert.assertTrue(newThing.Compare(_thing, true, true));

        Thing newOtherThing = new Thing(_otherThing.getId());
        newThing.Connect(newOtherThing);

        Assert.assertFalse(_thing.Compare(newThing, true, true));
        Assert.assertFalse(newThing.Compare(_thing, true, true));

        newOtherThing.Connect(_thing);

        Assert.assertTrue(_thing.Compare(newThing, true, true));
        Assert.assertTrue(newThing.Compare(_thing, true, true));
    }

    @Test
    public void testCompareNull() throws Exception {

        Assert.assertFalse(_thing.Compare(null));
    }
}
