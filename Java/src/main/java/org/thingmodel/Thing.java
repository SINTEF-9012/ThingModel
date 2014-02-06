package org.thingmodel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class Thing {
	
	/**
	 * String ID, unique in the world.
	 * 
	 * It can't be null.
	 * Two things instances with the same ID represent the same thing.
	 */
	private String _id;
	
	public String getId() {
		return _id;
	}

	/**
	 * Reference to the object type.
	 * 
	 * The type is null by default.
	 * If the type is null, a default type is applied.
	 */
	private ThingType _type;
	
	public ThingType getType() {
		return _type;
	}
	
	/**
	 *  A thing contains a list of properties.
	 *  
	 *  A property is defined by his key and his value.
	 */
	protected HashMap<String, Property> Properties;
	
	/**
	 * A thing can be connected to other things.
	 * 
	 * But a thing cannot be connected directly to itself.
	 */
	protected HashMap<String, Thing> Connections;
	
	public Thing(String id) {
		this(id, null);
	}
	
	public Thing(String id, ThingType type) {
		if (id == null || id.isEmpty()) {
			throw new RuntimeException("The thing ID should not be null or empty");
		}
		
		_id = id;
		_type = type;
		
		Properties = new HashMap<>();
		Connections = new HashMap<>();
	}
	
	public void setProperty(Property property) {
		Properties.put(property.getKey(), property);
	}
	
	@SuppressWarnings("unchecked") // I check but the compiler is rustic
	public <T extends Property> T getProperty(String key, Class<T> type) {
		Property value = Properties.get(key);
		
		// TypeErasure is definitely a great idea
		if (type == null || type.equals(value.getClass())) {
			return (T) value;
		}
	
		return null;
	}
	
	public boolean hasProperty(String key) {
		return Properties.containsKey(key);
	}
	
	public void Connect(Thing thing) {
		if (thing == this || _id.equals(thing._id)) {
			throw new RuntimeException("You can't connect a thing directly to itself");
		}
		
		Connections.put(thing._id, thing);
	}

	public boolean Disconnect(Thing thing) {
		return Connections.remove(thing._id) != null;
	}
	
	public void DisconnectAll() {
		Connections.clear();
	}
	
	public boolean IsConnectedTo(Thing thing) {
		return Connections.containsKey(thing._id);
	}
	
	public List<Thing> getConnectedThings() {
		return new ArrayList<>(Connections.values());
	}
	
	public List<Property> getProperties() {
		return new ArrayList<>(Properties.values());
	}
	
	public boolean Compare(Thing other, boolean compareId, boolean deepComparisonForConnectThings) {
		// TODO
		return false;
	}
}
