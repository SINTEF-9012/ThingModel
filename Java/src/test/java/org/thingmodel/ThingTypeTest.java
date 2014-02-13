package org.thingmodel;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class ThingTypeTest {
    private ThingType _type;
    private PropertyType _propertyType;

    @Before
    public void setUp() throws Exception {
        _type = new ThingType("plane");
        _propertyType = new PropertyType("location", Property.Location.class);
        _type.DefineProperty(_propertyType);
    }

    @Test
    public void testName() throws Exception {
        // YES It's a stupid test :-)
        Assert.assertEquals(_type.getName(), "plane");

        try {
            new ThingType("");
            Assert.fail();
        } catch (Exception e) {}

        try {
            new ThingType(null);
            Assert.fail();
        } catch (Exception e) {}
    }

    @Test
    public void testCheck() throws Exception {
        Thing plane = new Thing("A380", _type);

        Assert.assertFalse(_type.Check(plane));

        _propertyType.Required = false;
        Assert.assertTrue(_type.Check(plane));

        _propertyType.Required = true;

        plane.setProperty(new Property.String("location", "CDG"));

        Assert.assertFalse(_type.Check(plane));

        plane.setProperty(new Property.Location("location", new Location.LatLng(49.010852, 2.547398)));


        Assert.assertTrue(_type.Check(plane));

    }

    @Test
    public void testGetPropertyDefinition() throws Exception {
        Assert.assertEquals(_type.getPropertyDefinition("location"), _propertyType);
    }

    @Test
    public void testGetProperties() throws Exception {
        boolean visited = false;

        for(PropertyType p : _type.getProperties()) {
            visited = true;
            // Should not throw exceptions
            _type.DefineProperty(p);
        }

        Assert.assertTrue(visited);
    }
}
