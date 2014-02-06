package org.thingmodel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class ThingType {

	/**
	 * The name of a thing type should be unique in the system.
	 * 
	 * You can have multiple instances of things with the sane name
	 * but they will represent the same type.
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
     * Check if the given thing have all the required properties,
     * and if their values are correct.
     */
    public boolean Check(Thing thing) {
    	ThingType thingType = thing.getType();
    	
    	if (thingType == this) {
    		return true;
    	}
    	
    	if (thingType != null && thingType._name.equals(_name)) {
    		for (PropertyType propertyType : Properties.values()) {
    			if (!propertyType.Check(thing.getProperty(propertyType.getKey(), Property.class))) {
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
    
}
