package org.thingmodel.proto;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;

import org.thingmodel.Property.Location;
import org.thingmodel.PropertyType;
import org.thingmodel.ThingType;
import org.thingmodel.proto.ProtoProperty.Property;
import org.thingmodel.proto.ProtoStringDeclaration.StringDeclaration;
import org.thingmodel.proto.ProtoThing.Thing;
import org.thingmodel.proto.ProtoTransaction.Transaction;

public class ToProtobuf {
	// String declarations dictionary, for the sender side
	private HashMap<String, Integer> _stringDeclarations = new HashMap<>();
	
	// Current transaction object
	// It will be filled step by step
	private Transaction.Builder _transaction;
	
	private static HashMap<Class<? extends org.thingmodel.Property>, org.thingmodel.proto.ProtoPropertyType.PropertyType.Type>
		_prototypesBindings = new HashMap<>();
		
	static {
		_prototypesBindings.put(org.thingmodel.Property.Location.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.LOCATION);
		
		_prototypesBindings.put(org.thingmodel.Property.String.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.STRING);
		
		_prototypesBindings.put(org.thingmodel.Property.Double.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.DOUBLE);
		
		_prototypesBindings.put(org.thingmodel.Property.Integer.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.INT);
		
		_prototypesBindings.put(org.thingmodel.Property.Boolean.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.BOOLEAN);
		
		_prototypesBindings.put(org.thingmodel.Property.Date.class,
				org.thingmodel.proto.ProtoPropertyType.PropertyType.Type.DATETIME);
	}
	
	// When a string is in this collection, it should be sent
	// as a StringDeclaration for the next transaction
	private HashSet<String> _stringToDeclare = new HashSet<>();
	
	private HashMap<Integer, Thing.Builder> _thingsState = new HashMap<>();
	private HashMap<PropertyStateKey, Property.Builder> _propertiesState = new HashMap<>();

	private class PropertyStateKey {
		public int thingId;
		public int propertyId;
		
		@Override
		public int hashCode() {
			final int prime = 31;
			int result = 1;
			result = prime * result + propertyId;
			result = prime * result + thingId;
			return result;
		}
		
		@Override
		public boolean equals(Object obj) {
			if (this == obj)
				return true;
			if (obj == null)
				return false;
			if (getClass() != obj.getClass())
				return false;
			PropertyStateKey other = (PropertyStateKey) obj;
			if (propertyId != other.propertyId)
				return false;
			if (thingId != other.thingId)
				return false;
			return true;
		}
		
	}
	
	protected int stringToKey(String value) {
		if (value == null || value.isEmpty()) {
			return 0;
		}
		
		Integer key = _stringDeclarations.get(value);
		if (key != null) {
			return key;
		}
		
		key = _stringDeclarations.size()+1;
		
		_stringDeclarations.put(value, key);

		_transaction.addStringDeclarations(
			StringDeclaration.newBuilder()
            	.setKey(key)
                .setValue(value));
		
		return key;
	}
	
	public Transaction Convert(Collection<org.thingmodel.Thing> publish,
			Collection<org.thingmodel.Thing> delete,
			Collection<ThingType> declarations,
			String senderID) {
		
		_transaction = Transaction.newBuilder();
		
		_transaction.setStringSenderId(stringToKey(senderID));
		
		for (org.thingmodel.Thing thing : publish) {
			ConvertThing(thing);
		}
		
		ConvertDeleteList(delete);
		ConvertDeclarationList(declarations);
		
		return _transaction.build();
	}
	
	protected void ConvertThing(org.thingmodel.Thing thing) {
		boolean change = false;
		
		int thingId = stringToKey(thing.getId());
		
		Thing.Builder publication = Thing.newBuilder()
			.setStringId(thingId)
			.setStringTypeName(thing.getType() != null ? stringToKey(thing.getType().getName()): 0)
			.setConnectionsChange(false);
		
		Thing.Builder previousThing = _thingsState.get(thingId);
		
		if (previousThing == null || previousThing.getStringTypeName() != publication.getStringTypeName()) {
			change = true;
		}
		
		List<org.thingmodel.Thing> connectedThingsCache = null;
		
		if ((previousThing == null && thing.getConnectedThingsCount() > 0)
			|| (previousThing != null && previousThing.getConnectionsCount() !=
				thing.getConnectedThingsCount())) {
			publication.setConnectionsChange(true);
		} else {
			connectedThingsCache = thing.getConnectedThings();
			
			for(org.thingmodel.Thing connectedThing : connectedThingsCache) {
				int connectionKey = stringToKey(connectedThing.getId());
				
				if (previousThing == null ||
						!previousThing.getConnectionsList().contains(connectionKey)) {
					publication.setConnectionsChange(true);
				}
			}
		}
		
		// If we don't have changes on the connection list
		// It's useless to send it
		if (publication.getConnectionsChange()) {
			change = true;
			
			if (connectedThingsCache == null) {
				connectedThingsCache = thing.getConnectedThings();
			}
			
			for (org.thingmodel.Thing connectedThing : connectedThingsCache) {
				int connectionKey = stringToKey(connectedThing.getId());
				
				publication.addConnections(connectionKey);
			}
		}
		
		for (org.thingmodel.Property property : thing.getProperties()) {
			int propertyId = stringToKey(property.getKey());
			
			Property.Builder proto = Property.newBuilder()
				.setStringKey(propertyId);
		
			Class<? extends org.thingmodel.Property> c = property.getClass();
			if (c == org.thingmodel.Property.Location.class) {
				ConvertLocationProperty((Location) property, proto);
			}
			else if (c == org.thingmodel.Property.String.class)
			{
				ConvertStringProperty((org.thingmodel.Property.String) property, proto);
			}
			else if (c == org.thingmodel.Property.Double.class)
			{
				proto.setType(Property.Type.DOUBLE);
				proto.setDoubleValue(((org.thingmodel.Property.Double) property).getValue());
			}
			else if (c == org.thingmodel.Property.Integer.class)
			{
				proto.setType(Property.Type.INT);
				proto.setIntValue(((org.thingmodel.Property.Integer) property).getValue());
			}
			else if (c == org.thingmodel.Property.Boolean.class)
			{
				proto.setType(Property.Type.BOOLEAN);
				proto.setBooleanValue(((org.thingmodel.Property.Boolean) property).getValue());
			}
			else if (c == org.thingmodel.Property.Date.class)
			{
				proto.setType(Property.Type.DATETIME);
				proto.setDatetimeValue(((org.thingmodel.Property.Date) property).getValue().getTime());
			}
			else if (c == org.thingmodel.Property.Double.class)
			{
				proto.setType(Property.Type.DOUBLE);
				proto.setDoubleValue(((org.thingmodel.Property.Double) property).getValue());
			}
			
			PropertyStateKey key = new PropertyStateKey();
			key.thingId = thingId;
			key.propertyId = propertyId;
			
			if (previousThing != null) {
				Property.Builder previousProto = _propertiesState.get(key);
				
				// Now, close your eyes !
				if (previousProto != null &&
					previousProto.getType().ordinal() == proto.getType().ordinal() &&
					previousProto.getBooleanValue() == proto.getBooleanValue() &&
					previousProto.getDatetimeValue() == proto.getDatetimeValue() &&
					previousProto.getDoubleValue() == proto.getDoubleValue() &&
					previousProto.getIntValue() == proto.getIntValue()
						) {
					
					Property.Location previousLoc = previousProto.getLocationValue();
					Property.Location loc = proto.getLocationValue();
					
					if ((previousLoc == null && loc == null) ||
						(previousLoc != null && loc != null &&
						previousLoc.getX() == loc.getX() &&
						previousLoc.getY() == loc.getY() &&
						previousLoc.getZ() == loc.getZ() &&
						previousLoc.getZNull() == loc.getZNull() &&
						previousLoc.getStringSystem() == loc.getStringSystem())) {
					
						Property.String previousStr = previousProto.getStringValue();
						Property.String str = proto.getStringValue();
						
						if ((previousStr == null && str == null) ||
							(previousStr != null && str != null &&
								((previousStr.getStringValue() ==  str.getStringValue() &&
								previousStr.getValue() == str.getValue()) ||
								(property.ValueToString().equals(previousStr.getValue()))))) {
							continue;
						}
					}
				}
			}
			
			change = true;
			
			publication.addProperties(proto);
			_propertiesState.put(key, proto);
			
		}
		
		if (change) {
			_transaction.addThingsPublishList(publication);
			_thingsState.put(thingId, publication);
		}
	}
	
	protected void ConvertDeleteList(Collection<org.thingmodel.Thing> list) {
		for (org.thingmodel.Thing thing : list) {
			int key = stringToKey(thing.getId());
			
			_transaction.addThingsRemoveList(key);
			ManageThingSuppression(key);
		}
	}
	
	protected void ConvertDeclarationList(Collection<org.thingmodel.ThingType> declarations) {
		for (ThingType thingType : declarations) {
			org.thingmodel.proto.ProtoThingType.ThingType.Builder declaration =
					org.thingmodel.proto.ProtoThingType.ThingType.newBuilder()
						.setStringName(stringToKey(thingType.getName()))
						.setStringDescription(stringToKey(thingType.Description));
			
			for (PropertyType propertyType : thingType.getProperties()) {
				declaration.addProperties(org.thingmodel.proto.ProtoPropertyType.PropertyType.newBuilder()
						.setStringKey(stringToKey(propertyType.getKey()))
						.setStringName(stringToKey(propertyType.Name))
						.setStringDescription(stringToKey(propertyType.Description))
						.setRequired(propertyType.Required)
						.setType(_prototypesBindings.get(propertyType.getType())));
			}
			
			_transaction.addThingtypesDeclarationList(declaration);
		}
	}
	
	protected void ConvertLocationProperty(org.thingmodel.Property.Location property,
			Property.Builder proto) {
		
		org.thingmodel.Location value = property.getValue();
		Class<? extends org.thingmodel.Location> c = value.getClass();
	
		if (c == org.thingmodel.Location.LatLng.class) {
			proto.setType(Property.Type.LOCATION_LATLNG);
		}
		else if (c == org.thingmodel.Location.Point.class) {
			proto.setType(Property.Type.LOCATION_POINT);
		}
		else {
			proto.setType(Property.Type.LOCATION_EQUATORIAL);
		}
	
		Property.Location.Builder loc = Property.Location.newBuilder()
				.setX(value.X)
				.setY(value.Y)
				.setStringSystem(stringToKey(value.System))
				.setZNull(value.Z == null);
		
		if (value.Z != null) {
			loc.setZ(value.Z);
		}
		
		proto.setLocationValue(loc);
	}
	
	protected void ConvertStringProperty(org.thingmodel.Property.String property,
			Property.Builder proto) {
		
		String value = property.getValue();
		proto.setType(Property.Type.STRING);
		
		Property.String.Builder st = Property.String.newBuilder();
		
		if (_stringToDeclare.contains(value)) {
			st.setStringValue(stringToKey(value));
		} else {
			st.setValue(value);
			
			// Use a string declaration next time
			_stringToDeclare.add(value);
		}

        proto.setStringValue(st);
		
	}
	
	protected void ManageThingSuppression(int thingId) {
		_thingsState.remove(thingId);
		
		ArrayList<PropertyStateKey> toRemove = new ArrayList<>();
		
		for (PropertyStateKey key : _propertiesState.keySet()) {
			if (key.thingId == thingId) {
				toRemove.add(key);
			}
		}
		
		for (PropertyStateKey key : toRemove) {
			_propertiesState.remove(key);
		}
	}
	
	public void ApplyThingsSuppressions(Collection<org.thingmodel.Thing> things) {
		for (org.thingmodel.Thing thing : things) {
			Integer key = _stringDeclarations.get(thing.getId());
			if (key != null){
				ManageThingSuppression(key);
			}
		}
	}
}
