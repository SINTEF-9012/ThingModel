package org.thingmodel;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.Date;

public class PropertyTest {
    private Property _property;

    @Before
    public void setUp() throws Exception {
        _property = new Property.Double("speed");
    }

    @Test
    public void testValues() throws Exception {
        Assert.assertEquals(new Property.Boolean("good", true).getValue(), true);

        Assert.assertEquals(new Property.String("name", "Alphonse").getValue(), "Alphonse");

        Assert.assertEquals((double) new Property.Double("speed", 12.0).getValue(), 12.0, 0.001);
        Assert.assertEquals((int) new Property.Integer("speed", 12).getValue(), 12);

        Assert.assertTrue(new Property.Date("date", new Date()).getValue().getTime() <= new Date().getTime());

        Assert.assertNotNull(new Property.Location("location", new Location.LatLng(1.0, 2.0, 3.0)).getValue());
    }


    @Test
    public void testValueToString() throws Exception {
        Assert.assertFalse(new Property.Boolean("good", true).ValueToString().isEmpty());

        Assert.assertTrue(new Property.String("name", "Alphonse").ValueToString().contains("Alphonse"));
        Assert.assertTrue(new Property.Double("speed", 42.0).ValueToString().contains("42"));
        Assert.assertTrue(new Property.Integer("speed", 42).ValueToString().contains("42"));

        Assert.assertFalse(new Property.Date("date", new Date()).ValueToString().isEmpty());

        Assert.assertTrue(new Property.Location("location",
                new Location.LatLng(42.0, 2.0, 3.0)).ValueToString().contains("42"));

    }

    @Test
    public void testCompareValue() throws Exception {
        Assert.assertFalse(_property.CompareValue(null));

        Assert.assertFalse(_property.CompareValue(new Property.String("poisson", "roger")));

        Assert.assertFalse(_property.CompareValue(new Property.Double("speed2", 45.0)));

        Assert.assertTrue(_property.CompareValue(new Property.Double("speed2")));
    }
}
