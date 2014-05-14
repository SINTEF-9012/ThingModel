package org.thingmodel.proto;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;

import org.thingmodel.Location;
import org.thingmodel.Property;
import org.thingmodel.Thing;
import org.thingmodel.Warehouse;
import org.thingmodel.proto.ProtoPropertyType.PropertyType;
import org.thingmodel.proto.ProtoStringDeclaration.StringDeclaration;
import org.thingmodel.proto.ProtoThingType.ThingType;
import org.thingmodel.proto.ProtoTransaction.Transaction;

public class FromProtobuf {
	private static HashMap<PropertyType.Type, Class<? extends Property>> _prototypesBinding = new HashMap<>();
	 
	static {
		_prototypesBinding.put(PropertyType.Type.LOCATION, Property.Location.class);
		_prototypesBinding.put(PropertyType.Type.STRING, Property.String.class);
		_prototypesBinding.put(PropertyType.Type.DOUBLE, Property.Double.class);
		_prototypesBinding.put(PropertyType.Type.INT, Property.Int.class);
		_prototypesBinding.put(PropertyType.Type.BOOLEAN, Property.Boolean.class);
		_prototypesBinding.put(PropertyType.Type.DATETIME, Property.DateTime.class);
	}
	 
	
	private Map<Integer, String> _stringDeclarations = new HashMap<>();
	
	protected String keyToString(int key) {
		if (key == 0) {
			return "";
		}
	
		String value = _stringDeclarations.get(key);
				
		return value != null ? value : "undefined";
	}

	private Warehouse _warehouse;
	
	public FromProtobuf(Warehouse warehouse) {
		_warehouse = warehouse;
	}
	
	public String Convert(Transaction transaction) {
		return Convert(transaction, false);
	}
	
	public String Convert(Transaction transaction, boolean check)
	{
		for (int i = 0, l = transaction.getStringDeclarationsCount(); i < l; ++i) {
			ConvertStringDeclaration(transaction.getStringDeclarations(i));
		}
		
		String senderId = keyToString(transaction.getStringSenderId());
		
		HashSet<Thing> thingsToDelete = new HashSet<>();
		
		for (int i = 0, l = transaction.getThingsRemoveListCount(); i < l; ++i) {
			int key = transaction.getThingsRemoveList(i);
			thingsToDelete.add(_warehouse.getThing(keyToString(key)));
		}
		_warehouse.RemoveCollection(thingsToDelete, senderId);
		
		
		for (int i = 0, l = transaction.getThingtypesDeclarationListCount(); i < l; ++i) {
			ConvertThingTypeDeclaration(transaction.getThingtypesDeclarationList(i), senderId);
		}
		
		class Tuple {
			Thing model;
			org.thingmodel.proto.ProtoThing.Thing proto;
		}
	
		ArrayList<Tuple> thingsToConnect = new ArrayList<>();
		
		for (int i = 0, l = transaction.getThingsPublishListCount(); i < l; ++i) {
			org.thingmodel.proto.ProtoThing.Thing protoThing = transaction.getThingsPublishList(i);
			Thing modelThing = ConvertThingPublication(protoThing, check, senderId);
			
			if (protoThing.getConnectionsChange()) {
				Tuple t = new Tuple();
				t.model = modelThing;
				t.proto = protoThing;
				thingsToConnect.add(t);
			}
		}
		
		for (Tuple tuple : thingsToConnect) {
			tuple.model.DisconnectAll();
			
			for (int i = 0, l = tuple.proto.getConnectionsCount(); i < l; ++i) {
				Thing t = _warehouse.getThing(keyToString(tuple.proto.getConnections(i)));
				
				if (t != null) {
					tuple.model.Connect(t);
				}
			}
			
			_warehouse.RegisterThing(tuple.model, false, false, senderId);
		}
		
		return senderId;
	}

	protected void ConvertStringDeclaration(StringDeclaration declaration) {
		_stringDeclarations.put(declaration.getKey(), declaration.getValue());
	}

	protected void ConvertThingTypeDeclaration(ThingType thingType, String senderId) {
		org.thingmodel.ThingType modelType = new org.thingmodel.ThingType(
				keyToString(thingType.getStringName()));
		modelType.Description = keyToString(thingType.getStringDescription());
		
		for (PropertyType propertyType : thingType.getPropertiesList()) {
			Class<? extends Property> type = _prototypesBinding.get(propertyType.getType());
			String key = keyToString(propertyType.getStringKey());
			
			
			org.thingmodel.PropertyType modelProperty = new org.thingmodel.PropertyType(key, type);
			
			modelProperty.Name = keyToString(propertyType.getStringName());
			modelProperty.Description = keyToString(propertyType.getStringDescription());
			modelProperty.Required = propertyType.getRequired();
		
			modelType.DefineProperty(modelProperty);
		}
		
		_warehouse.RegisterType(modelType, true, senderId);
		
	}

	private Thing ConvertThingPublication(
			org.thingmodel.proto.ProtoThing.Thing thing, boolean check, String senderId) {
		org.thingmodel.ThingType type = null;
	
		if (thing.getStringTypeName() != 0) {
			type = _warehouse.getThingType(keyToString(thing.getStringTypeName()));
		}
		
		String id = keyToString(thing.getStringId());
		
		Thing modelThing = _warehouse.getThing(id);
		
		if (modelThing == null || (
			modelThing.getType() == null && type != null ||
			type == null && modelThing.getType() != null ||
			(modelThing.getType() != null && type != null &&
				modelThing.getType().getName().equals(type.getName())))) {
			modelThing = new Thing(id, type);
		}
		
		for (int i = 0, l = thing.getPropertiesCount(); i < l; ++i) {
			org.thingmodel.proto.ProtoProperty.Property property = thing.getProperties(i);
			
			String key = keyToString(property.getStringKey());
			Property modelProperty = null;
			
			Location location = null;
			
			switch (property.getType()) {
			case LOCATION_POINT:
				location = new Location.Point();
				modelProperty = new Property.Location.Point(key, (Location.Point) location);
			case LOCATION_LATLNG:
				if (location == null) {
					location = new Location.LatLng();
					modelProperty = new Property.Location.LatLng(key, (Location.LatLng) location);
				}
			case LOCATION_EQUATORIAL:
				if (location == null) {
					location = new Location.Equatorial();
					modelProperty = new Property.Location.Equatorial(key, (Location.Equatorial) location);
				}
			
				org.thingmodel.proto.ProtoProperty.Property.Location loc =
						property.getLocationValue();
				
				if (loc != null) {
					location.X = loc.getX();
					location.Y = loc.getY();
					
					if (!loc.getZNull()) {
						location.Z = loc.getZ();
					}
					if (loc.getStringSystem() != 0) {
						location.System = keyToString(loc.getStringSystem());
					}
				}
				
				break;
			case STRING:
				org.thingmodel.proto.ProtoProperty.Property.String sv = property.getStringValue();

                String value;

                if (sv != null) {
                    value = sv.getValue();

                    if (value == null || value.length() == 0 && sv.getStringValue() != 0) {
                        value = keyToString(sv.getStringValue());
                    }
                } else {
                    value = "undefined";
                }

				
				modelProperty = new Property.String(key, value);
				break;
			case BOOLEAN:
				modelProperty = new Property.Boolean(key, property.getBooleanValue());
				break;
			case DATETIME:
				modelProperty = new Property.DateTime(key, new Date(property.getDatetimeValue()));
				break;
			case DOUBLE:
				modelProperty = new Property.Double(key, property.getDoubleValue());
				break;
			case INT:
				modelProperty = new Property.Int(key, property.getIntValue());
				break;
			}
			
			
			modelThing.setProperty(modelProperty);
		}
	
		if (check && type != null && !type.Check(modelThing)) {
			System.out.println("Object «"+id+"» from «"+senderId+"» is not valid, ignored");
		} else if (!thing.getConnectionsChange()) {
			_warehouse.RegisterThing(modelThing, false, false, senderId);
		}
		
		return modelThing;
		
	}

}
