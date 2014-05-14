package org.thingmodel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class ThingType {

	/**
	 * The name of a thing type should be unique in the system.
	 * 
	 * You can have multiple instances of things with the sane name but they
	 * will represent the same type.
	 */
	private String _name;

	public String getName() {
		return _name;
	}

	/**
	 * Documentation for the user or the developer of other applications
	 * connected with the ThingModel.
	 */
	public String Description;

	/**
	 * A thing type contains a list of properties.
	 * 
	 * Each property is defined by a key and a type.
	 */
	protected HashMap<String, PropertyType> Properties;

	public ThingType(String name) {
		if (name == null || name.isEmpty()) {
			throw new RuntimeException("The name should not be null or empty");
		}

		_name = name;
		Properties = new HashMap<>();
	}

	/**
	 * Check if the given thing have all the required properties, and if their
	 * values are correct.
	 */
	public boolean Check(Thing thing) {
		ThingType thingType = thing.getType();

		if (thingType.equals(this)
				|| (thingType != null && thingType._name.equals(_name))) {
			for (PropertyType propertyType : Properties.values()) {
				if (!propertyType.Check(thing.getProperty(
						propertyType.getKey(), propertyType.getType()))) {
					return false;
				}
			}
			return true;
		}
		return false;

	}

	public void DefineProperty(PropertyType property) {
		Properties.put(property.getKey(), property);
	}

	public PropertyType getPropertyDefinition(String key) {
		return Properties.get(key);
	}

	public List<PropertyType> getProperties() {
		return new ArrayList<>(Properties.values());
	}

	public boolean Is(ThingType other) {
		if (other == null || (other._name == null && _name != null)
				|| (other._name != null && !other._name.equals(_name))) {
			return false;
		}
		
		return Properties.equals(other.Properties);
		
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result
				+ ((Properties == null) ? 0 : Properties.hashCode());
		result = prime * result + ((_name == null) ? 0 : _name.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (!(obj instanceof ThingType))
			return false;
		ThingType other = (ThingType) obj;
		if (Properties == null) {
			if (other.Properties != null)
				return false;
		} else if (!Properties.equals(other.Properties))
			return false;
		if (_name == null) {
			if (other._name != null)
				return false;
		} else if (!_name.equals(other._name))
			return false;
		return true;
	}

}
