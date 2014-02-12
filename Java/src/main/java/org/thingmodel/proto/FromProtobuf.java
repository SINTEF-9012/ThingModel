package org.thingmodel.proto;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import org.thingmodel.Location;
import org.thingmodel.Property;
import org.thingmodel.Thing;
import org.thingmodel.Wharehouse;
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
		_prototypesBinding.put(PropertyType.Type.INT, Property.Integer.class);
		_prototypesBinding.put(PropertyType.Type.BOOLEAN, Property.Boolean.class);
		_prototypesBinding.put(PropertyType.Type.DATETIME, Property.Date.class);
	}
	 
	
	private Map<Integer, String> _stringDeclarations = new HashMap<>();
	
	protected String keyToString(int key) {
		if (key == 0) {
			return "";
		}
	
		String value = _stringDeclarations.get(key);
				
		return value != null ? value : "undefined";
	}

	private Wharehouse _wharehouse;
	
	public FromProtobuf(Wharehouse wharehouse) {
		_wharehouse = wharehouse;
	}
	
	public String Convert(Transaction transaction) {
		return Convert(transaction, false);
	}
	
	public String Convert(Transaction transaction, boolean check)
	{
		for (int i = 0, l = transaction.getStringDeclarationsCount(); i < l; ++i) {
			ConvertStringDeclaration(transaction.getStringDeclarations(i));
		}
		
		for (int i = 0, l = transaction.getThingsRemoveListCount(); i < l; ++i) {
			ConvertDelete(transaction.getThingsRemoveList(i));
		}
		
		for (int i = 0, l = transaction.getThingtypesDeclarationListCount(); i < l; ++i) {
			ConvertThingTypeDeclaration(transaction.getThingtypesDeclarationList(i));
		}
		
		class Tuple {
			Thing model;
			org.thingmodel.proto.ProtoThing.Thing proto;
		}
	
		ArrayList<Tuple> thingsToConnect = new ArrayList<>();
		
		for (int i = 0, l = transaction.getThingsPublishListCount(); i < l; ++i) {
			org.thingmodel.proto.ProtoThing.Thing protoThing = transaction.getThingsPublishList(i);
			Thing modelThing = ConvertThingPublication(protoThing, check);
			
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
				Thing t = _wharehouse.getThing(keyToString(tuple.proto.getConnections(i)));
				
				if (t != null) {
					tuple.model.Connect(t);
				}
			}
			
			_wharehouse.RegisterThing(tuple.model, false, false);
		}
	
		String senderId = keyToString(transaction.getStringSenderId());
		
		return senderId;
	}

	protected void ConvertStringDeclaration(StringDeclaration declaration) {
		_stringDeclarations.put(declaration.getKey(), declaration.getValue());
	}
	
	protected void ConvertDelete(int thinkKey) {
		_wharehouse.RemoveThing(_wharehouse.getThing(keyToString(thinkKey)));
	}

	protected void ConvertThingTypeDeclaration(ThingType thingType) {
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
		
		_wharehouse.RegisterType(modelType);
		
	}

	private Thing ConvertThingPublication(
			org.thingmodel.proto.ProtoThing.Thing thing, boolean check) {
		org.thingmodel.ThingType type = null;
	
		if (thing.getStringTypeName() != 0) {
			type = _wharehouse.getThingType(keyToString(thing.getStringTypeName()));
		}
		
		String id = keyToString(thing.getStringId());
		
		Thing modelThing = _wharehouse.getThing(id);
		
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
			case LOCATION_LATLNG:
				if (location == null) {
					location = new Location.LatLng();
				}
			case LOCATION_EQUATORIAL:
				if (location == null) {
					location = new Location.Equatorial();
				}
			
				org.thingmodel.proto.ProtoProperty.Property.Location loc =
						property.getLocationValue();
				if (loc != null) {
					location.X = loc.getX();
					location.Y = loc.getY();
					
					if (!loc.getZNull()) {
						location.Z = loc.getZ();
					}
				}
				
				modelProperty = new Property.Location(key, location);
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
				modelProperty = new Property.Date(key, new Date(property.getDatetimeValue()));
				break;
			case DOUBLE:
				modelProperty = new Property.Double(key, property.getDoubleValue());
				break;
			case INT:
				modelProperty = new Property.Integer(key, property.getIntValue());
				break;
			}
			
			
			modelThing.setProperty(modelProperty);
		}
	
		if (check && type != null && !type.Check(modelThing)) {
			System.out.println("Object "+id+" not valid, ignored");
		} else if (!thing.getConnectionsChange()) {
			_wharehouse.RegisterThing(modelThing, false, false);
		}
		
		return modelThing;
		
	}

}
