package org.thingmodel;

public class PropertyType {
	
	private String _key;
	
	public String getKey() {
		return _key;
	}
	
	public String Name;
	public String Description;

	public boolean Required;
	
	private Class<Property> _type;
	
	public Class<Property> getType() {
		return _type;
	}

	
	public PropertyType(String key, Class<Property> type) {
		this(key,type,true);
	}
	
	public PropertyType(String key, Class<Property> type, boolean required) {
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
}
