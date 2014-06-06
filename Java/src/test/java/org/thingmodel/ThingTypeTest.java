package org.thingmodel;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import org.thingmodel.builders.BuildANewThing;
import org.thingmodel.builders.BuildANewThingType;

public class ThingTypeTest {
    private ThingType _type;
    private PropertyType _propertyType;

    @Before
    public void setUp() throws Exception {
        _type = new ThingType("plane");
        _propertyType = new PropertyType("location", Property.Location.LatLng.class);
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

        plane.setProperty(new Property.Location.LatLng("location", new Location.LatLng(49.010852, 2.547398)));


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

    @Test
    public void CheckRequiredProperties() throws Exception {
        Thing plane = new Thing("A380", _type);
        Assert.assertFalse(_type.Check(plane));

        plane.setProperty(new Property.Location.LatLng("location",
                new Location.LatLng(49.010852, 2.547398)));

        Assert.assertTrue(_type.Check(plane));
    }

    @Test
    public void CheckNotRequiredProperties() throws Exception {
        Thing plane = new Thing("A380", _type);
        _propertyType.Required = false;

        Assert.assertTrue(_type.Check(plane));

        plane.setProperty(new Property.Double("location", 27.0));
        Assert.assertFalse(_type.Check(plane));
    }

    @Test
    public void CheckEqualityComparator() throws Exception {
        Assert.assertEquals(_type, _type);

        ThingType newType = BuildANewThingType.Named("plane").Build();

        Assert.assertFalse(_type.equals(newType));

        newType = BuildANewThingType.Named("plane")
                .ContainingA.LocationLatLng().Build();

        Assert.assertEquals(_type, newType);
    }

    @Test
    public void CheckEqualityComparatorWithOrder() throws Exception {
        ThingType typeA = BuildANewThingType.Named("aircraft")
                .ContainingA.LocationLatLng()
                .AndA.String("name").Build();

        ThingType typeB = BuildANewThingType.Named("aircraft")
                .ContainingA.String("name")
                .AndA.LocationLatLng().Build();

        Assert.assertEquals(typeA, typeB);
    }
}
