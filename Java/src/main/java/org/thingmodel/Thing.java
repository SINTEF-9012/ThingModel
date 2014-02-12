package org.thingmodel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
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
	private HashMap<String, Property> Properties;
	
	/**
	 * A thing can be connected to other things.
	 * 
	 * But a thing cannot be connected directly to itself.
	 */
	private HashMap<String, Thing> Connections;
	
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

    public Property getProperty(String key) {
        return Properties.get(key);
    }

	@SuppressWarnings("unchecked") // I check but the compiler is rustic
	public <T extends Property> T getProperty(String key, Class<T> type) {
		Property value = Properties.get(key);
		
		// TypeErasure is definitely a great idea
		if (type == null || value != null && type.equals(value.getClass())) {
			return (T) value;
		}
	
		return null;
	}
	
	public boolean hasProperty(String key) {
		return Properties.containsKey(key);
	}
	
	public void Connect(Thing thing) {
        if (thing == null) {
            return;
        }

		if (thing == this || _id.equals(thing._id)) {
			throw new RuntimeException("You can't connect a thing directly to itself");
		}
		
		Connections.put(thing._id, thing);
	}

	public boolean Disconnect(Thing thing) {
		return thing != null && Connections.remove(thing._id) != null;
	}
	
	public void DisconnectAll() {
		Connections.clear();
	}
	
	public boolean IsConnectedTo(Thing thing) {
		return thing != null && Connections.containsKey(thing._id);
	}
	
	public List<Thing> getConnectedThings() {
		return new ArrayList<>(Connections.values());
	}
	
	public int getConnectedThingsCount() {
		return Connections.size();
	}
	
	public List<Property> getProperties() {
		return new ArrayList<>(Properties.values());
	}

    // No default parameters lol
    public boolean Compare(Thing other) {
        return Compare(other, true, false);
    }

	public boolean Compare(Thing other, boolean compareId,
			boolean deepComparisonForConnectThings) {
		// Optimization, when two things are the same instance
        if (this == other) {
            return true;
        }

        // If the types are not the same
        if (other == null || (_type != null && other._type != null &&
                !_type.getName().equals(other._type.getName())) ||
                (_type == null && other._type != null) ||
                (_type != null && other._type == null)) {
            return false;
        }

        // If we need to compare the ids, and they are not the same
        if (compareId && !_id.equals(other._id)) {
            return false;
        }

        // Check if the connections are the same
        if (Connections.size() != other.Connections.size() ||
            !Connections.keySet().containsAll(other.Connections.keySet())) {
            return false;
        }

        // Check if the properties are the same
        if (Properties.size() != other.Properties.size()) {
            return false;
        }

        for(Property property: Properties.values()) {
            Property otherProp = other.Properties.get(property.getKey());

            if (otherProp == null || !otherProp.CompareValue(property)) {
            	return false;
            }
        }
        
        if (deepComparisonForConnectThings) {
        	return RecursiveCompare(other, new HashSet<String>());
        }

		return true;
	}
	
	private boolean RecursiveCompare(Thing other,
			HashSet<String> alreadyVisitedThings) {
		
		// If the thing was already checked
		// We don't need to check it again
		
		if (alreadyVisitedThings.contains(other._id)) {
			// It's true because we are still looking for a difference
			return true;
		}
		
		// Made a simple comparison first
		if (!this.Compare(other)) {
			return false;
		}
		
		// Register the thing now, prevent infinite recursions
		alreadyVisitedThings.add(_id);
		
		// And start the recursion for connected things
		for (Thing connectedThing : Connections.values()) {
			Thing otherThing = other.Connections.get(connectedThing);
			
			if (!connectedThing.RecursiveCompare(otherThing, alreadyVisitedThings)) {
				return false;
			}
		}
		
		return true;
	}
}
