package org.thingmodel;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import org.thingmodel.proto.FromProtobuf;
import org.thingmodel.proto.ProtoModelObserver;
import org.thingmodel.proto.ProtoTransaction;
import org.thingmodel.proto.ToProtobuf;

import java.util.Calendar;
import java.util.Date;

public class ProtoConversionsTest {
    private Wharehouse _wharehouseInput;
    private Wharehouse _wharehouseOutput;
    private FromProtobuf _fromProtobuf;
    private ToProtobuf _toProtobuf;
    private ProtoModelObserver _observer;

    @Before
    public void setUp() throws Exception {
        _wharehouseInput = new Wharehouse();
        _wharehouseOutput = new Wharehouse();

        _fromProtobuf = new FromProtobuf(_wharehouseInput);

        _observer = new ProtoModelObserver();
        _wharehouseOutput.RegisterObserver(_observer);

        _toProtobuf = new ToProtobuf();
    }

    @Test
    public void testHelloWorld() throws Exception {
        Thing message = new Thing("first");
        message.setProperty(new Property.String("content", "Hello World"));

        _wharehouseOutput.RegisterThing(message);

        ProtoTransaction.Transaction transaction = _observer.getTransaction(_toProtobuf, "RogerEnterpriseBroadcaster");

        String senderId = _fromProtobuf.Convert(transaction);

        Assert.assertEquals(senderId, "RogerEnterpriseBroadcaster");

        Thing newMessage = _wharehouseInput.getThing("first");

        Assert.assertNotNull(newMessage);

        Assert.assertNotNull(newMessage.getProperty("content"));
        Assert.assertEquals(newMessage.getProperty("content", Property.String.class).getValue(), "Hello World");

    }

    @Test
    public void testHelloWorldWithType() throws Exception {

        ThingType type = new ThingType("message");
        type.DefineProperty(new PropertyType("content", Property.String.class));
        type.Description = "Just a simple text message";

        _wharehouseOutput.RegisterType(type);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, "bob"));

        ThingType newType = _wharehouseInput.getThingType("message");

        Assert.assertNotNull(newType);

        Assert.assertEquals(newType.Description, type.Description);

        Assert.assertNotNull(newType.getPropertyDefinition("content"));
        Assert.assertEquals(newType.getPropertyDefinition("content").getType(), Property.String.class);
    }

    @Test
    public void testEfficientStringDeclaration() throws Exception {
        ProtoTransaction.Transaction transaction = _observer.getTransaction(_toProtobuf, "canard");

        Assert.assertEquals(transaction.getStringDeclarationsCount(), 1);

        transaction = _observer.getTransaction(_toProtobuf, "canard"); // same canard

        Assert.assertEquals(transaction.getStringDeclarationsCount(), 0);
    }

    @Test
    public void testDeletions() throws Exception {
        Thing duck = new Thing("canard");
        _wharehouseOutput.RegisterThing(duck);
        _wharehouseOutput.RemoveThing(duck);

        ProtoTransaction.Transaction transaction = _observer.getTransaction(_toProtobuf, "bob");

        Assert.assertEquals(transaction.getThingsPublishListCount(),0);
        Assert.assertEquals(transaction.getThingsRemoveListCount(),1);
    }

    @Test
    public void CheckLocationProperty() throws Exception
    {
        Thing thing = new Thing("earth");
        thing.setProperty(new Property.Location("point", new Location.Point(42.0, 43.0, 44.0)));
        thing.setProperty(new Property.Location("latlng", new Location.LatLng(51.0, 52.0, 53.0)));
        thing.setProperty(new Property.Location("equatorial", new Location.Equatorial(27, 28, 29)));

        _wharehouseOutput.RegisterThing(thing);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, "earth"));

        Thing newThing = _wharehouseInput.getThing("earth");

        Assert.assertNotNull(newThing);
        Assert.assertEquals(newThing.getProperty("point",
                Property.Location.class).getValue().X, 42, 0.0001);
        Assert.assertEquals(newThing.getProperty("latlng",
                Property.Location.class).getValue().Y, 52, 0.0001);
        Assert.assertEquals(newThing.getProperty("equatorial",
                Property.Location.class).getValue().Z, 29, 0.001);

    }

    @Test
    public void EfficientStringProperties() throws Exception
    {
        Thing thing = new Thing("computer");
        thing.setProperty(new Property.String("name", "Interstella"));
        thing.setProperty(new Property.String("hostname", "Interstella"));

        _wharehouseOutput.RegisterThing(thing);

        ProtoTransaction.Transaction transaction = _observer.getTransaction(_toProtobuf, "");

        // Just 4 because the sender ID is an empty string (empty strings doesn't need to be declared)

        Assert.assertEquals(transaction.getStringDeclarationsCount(), 4);

        _fromProtobuf.Convert(transaction);

        Assert.assertEquals(_wharehouseInput.getThing("computer").getProperty("name",
                Property.String.class).getValue(), "Interstella");
    }

    @Test
    public void CheckDoubleProperty() throws Exception
    {
        Thing thing = new Thing("twingo");
        thing.setProperty(new Property.Double("speed", 45.71));

        _wharehouseOutput.RegisterThing(thing);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals(_wharehouseInput.getThing("twingo").getProperty("speed",
                Property.Double.class).getValue(), 45.71, 0.0001);
    }

    @Test
    public void CheckIntProperty() throws Exception
    {
        Thing thing = new Thing("twingo");
        thing.setProperty(new Property.Integer("doors", 3));

        _wharehouseOutput.RegisterThing(thing);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals((int) _wharehouseInput.getThing("twingo").getProperty("doors",
                Property.Integer.class).getValue(), 3);
    }

    @Test
    public void CheckBooleanProperty() throws Exception
    {
        Thing thing = new Thing("twingo");
        thing.setProperty(new Property.Boolean("moving", true));

        _wharehouseOutput.RegisterThing(thing);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertTrue(_wharehouseInput.getThing("twingo").getProperty("moving",
                Property.Boolean.class).getValue());
    }

    @Test
    public void CheckDateTimeProperty() throws Exception
    {
        Thing thing = new Thing("twingo");

        // ThingModel is ugly, but java too
        Calendar c = Calendar.getInstance();
        c.set(1998, Calendar.JUNE, 24);

        Date birthdate = c.getTime();

        thing.setProperty(new Property.Date("birthdate", birthdate));

        _wharehouseOutput.RegisterThing(thing);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals(_wharehouseInput.getThing("twingo").getProperty("birthdate",
                Property.Date.class).getValue(), birthdate);
    }

    @Test
    public void CheckConnectedThings() throws Exception
    {
        Thing group = new Thing("family");
        Thing roger = new Thing("roger");
        Thing alain = new Thing("alain");

        group.Connect(roger);
        group.Connect(alain);

        _wharehouseOutput.RegisterThing(group);
        _wharehouseOutput.RegisterThing(roger);
        _wharehouseOutput.RegisterThing(alain);

        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals(_wharehouseInput.getThing("family").getConnectedThingsCount(), 2);
        Assert.assertEquals(_wharehouseInput.getThing("family").getConnectedThings().size(), 2);
    }

    @Test
    public void IndependentInstances() throws Exception
    {
        Location location = new Location.LatLng(25, 2);

        Thing thing = new Thing("8712C");
        thing.setProperty(new Property.Location("position", location));

        _wharehouseOutput.RegisterThing(thing);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Location newLocation = _wharehouseInput.getThing("8712C").getProperty("position", Property.Location.class).getValue();

        Assert.assertEquals(location, newLocation);

        newLocation.Y = 27;

        Assert.assertNotSame(location, newLocation);

    }

    @Test
    public void IncrementalPropertiesUpdates() throws Exception
    {
        Thing thing = new Thing("rocket");
        thing.setProperty(new Property.Double("speed", 1500.0));
        thing.setProperty(new Property.String("name", "Ariane"));

        _wharehouseOutput.RegisterThing(thing);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        thing = new Thing("rocket");
        thing.setProperty(new Property.Double("speed", 1200.0));
        thing.setProperty(new Property.Boolean("space", true));

        _wharehouseOutput.NotifyThingUpdate(thing);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Thing newThing = _wharehouseInput.getThing("rocket");

        Assert.assertEquals(newThing.getProperty("name", Property.String.class).getValue(), "Ariane");
        Assert.assertEquals(newThing.getProperty("speed", Property.Double.class).getValue(), 1200.0, 0.0001);
        Assert.assertTrue(newThing.getProperty("space", Property.Boolean.class).getValue());

    }

    @Test
    public void IncrementalUpdatesAndDisconnection() throws Exception
    {
        Thing couple = new Thing("couple");
        Thing a = new Thing("James");
        Thing b = new Thing("Germaine");

        couple.Connect(a);
        couple.Connect(b);

        _wharehouseOutput.RegisterThing(couple, true, true);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals(_wharehouseInput.getThing("couple").getConnectedThings().size(),2);

        // Germaine doesn't want to live with James anymore
        couple.Disconnect(b);
        _wharehouseOutput.RegisterThing(couple);
        _fromProtobuf.Convert(_observer.getTransaction(_toProtobuf, null));

        Assert.assertEquals(_wharehouseInput.getThing("couple").getConnectedThings().size(),1);


    }

    @Test
    public void DontSendAnUnchangedObject() throws Exception
    {
        Thing thing = new Thing("café");
        thing.setProperty(new Property.Double("temperature", 40.0));
        thing.setProperty(new Property.Location("location", new Location.Equatorial(48, 454, 2)));

        _wharehouseOutput.RegisterThing(thing);

        ProtoTransaction.Transaction transaction = _observer.getTransaction(_toProtobuf, null);

        Assert.assertEquals(transaction.getThingsPublishListCount(),1);

        transaction = _observer.getTransaction(_toProtobuf, null);

        Assert.assertEquals(transaction.getThingsPublishListCount(), 0);
    }
}
