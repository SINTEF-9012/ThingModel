package org.thingmodel;

public class PropertyType {
	
	private String _key;
	
	public String getKey() {
		return _key;
	}
	
	public String Name;
	public String Description;

	public boolean Required;
	
	private Class<? extends Property> _type;
	
	public Class<? extends Property> getType() {
		return _type;
	}

	
	public PropertyType(String key, Class<? extends Property> type) {
		this(key,type,true);
	}
	
	public PropertyType(String key, Class<? extends Property> type, boolean required) {
		if (key == null || key.isEmpty()) {
			throw new RuntimeException("The PropertyType key should not be null or empty");
		}
		
		_key = key;
		_type = type;
		Required = required;
	}

	public boolean Check(Property property) {
		return (!Required && property == null) ||
			(property != null && property.getClass().equals(_type) &&
			property.getKey().equals(_key));
	}
	
	public PropertyType Clone() {
		PropertyType p = new PropertyType(_key, _type);
		p.Required = Required;
		p.Name = Name;
		p.Description = Description;
		
		return p;
	}


	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result
				+ ((Description == null) ? 0 : Description.hashCode());
		result = prime * result + ((Name == null) ? 0 : Name.hashCode());
		result = prime * result + (Required ? 1231 : 1237);
		result = prime * result + ((_key == null) ? 0 : _key.hashCode());
		result = prime * result + ((_type == null) ? 0 : _type.hashCode());
		return result;
	}


	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (!(obj instanceof PropertyType))
			return false;
		PropertyType other = (PropertyType) obj;
		if (Description == null) {
			if (other.Description != null)
				return false;
		} else if (!Description.equals(other.Description))
			return false;
		if (Name == null) {
			if (other.Name != null)
				return false;
		} else if (!Name.equals(other.Name))
			return false;
		if (Required != other.Required)
			return false;
		if (_key == null) {
			if (other._key != null)
				return false;
		} else if (!_key.equals(other._key))
			return false;
		if (_type == null) {
			if (other._type != null)
				return false;
		} else if (!_type.equals(other._type))
			return false;
		return true;
	}
	
	
}
