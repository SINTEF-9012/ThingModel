package org.thingmodel;

import org.junit.Assert;
import org.junit.Test;
import org.thingmodel.builders.BuildANewThingType;

public class BuildersTest {
    @Test
    public void TestThingTypeBuilder() {
        ThingType type = BuildANewThingType.Named("rabbit")
                .WhichIs("Just a rabbit")
                .ContainingA.String("name")
                .AndA.LocationLatLng("Localization")
                .AndA.NotRequired().Double("speed")
                .AndAn.Int("nbChildren", "Number of children")
                    .WhichIs("The number of direct children for the rabbit")
                .Build();

        Assert.assertNotNull(type);

        Assert.assertEquals(type.Description, "Just a rabbit");

        Assert.assertNotNull(type.getPropertyDefinition("name"));
        Assert.assertNotNull(type.getPropertyDefinition("speed"));

        Assert.assertFalse(type.getPropertyDefinition("speed").Required);
        Assert.assertTrue(type.getPropertyDefinition("nbChildren").Required);

        Assert.assertTrue(type.getPropertyDefinition("nbChildren").Description
            .contains("direct children"));

        ThingType superRabbit = BuildANewThingType.Named("super_rabbit")
                .WhichIs("A better rabbit")
                .ContainingA.CopyOf(type)
                .AndAn.Int("power")
                .Build();

        Assert.assertNotNull(superRabbit.getPropertyDefinition("name"));
        Assert.assertNotNull(superRabbit.getPropertyDefinition("power"));

        superRabbit.getPropertyDefinition("name").Name = "Name of super children";

        Assert.assertNull(type.getPropertyDefinition("name").Name);
    }
}
