package org.thingmodel;

public abstract class Property {

	/**
	 * The property key is fixed. If you want to change the key
	 * the best solution is to create a new property and delete this one.
	 */
	private java.lang.String _key;
	
	public java.lang.String getKey() {
		return _key;
	}

	
	protected Property(java.lang.String key, Object value) {
		
		if (key == null || key.isEmpty()) {
			throw new RuntimeException("The Property key should not be null or empty");
		}
		
		_key = key;
		_value = value;
	}
	
	protected Object _value;
	
	public Object getValue() {
		return _value;
	}
	
	public void setValue(Object value) {
		_value = value;
	}
	
	public java.lang.String ValueToString() {
		return _value != null ? _value.toString() : "";
	}
	
	
	public boolean CompareValue(Property other) {
		return other != null && (_value == other._value ||
				(_value != null && _value.equals(other._value)));
	}
	
	public class Location extends Property {
		
		public Location(java.lang.String key) {
			super(key, null);
		}
		
		public Location(java.lang.String key, Location value) {
			super(key, value);
		}
		
		public Location getValue() {
			return (Location)_value;
		}
		
		public void setValue(Location value) {
			_value = value;
		}
	}

	public class String extends Property {
		
		public String(java.lang.String key) {
			super(key, null);
		}
		
		public String(java.lang.String key, java.lang.String value) {
			super(key, value);
		}
		
		public java.lang.String getValue() {
			return (java.lang.String)_value;
		}
		
		public void setValue(java.lang.String value) {
			_value = value;
		}
	}
	
	public class Double extends Property {
		
		public Double(java.lang.String key) {
			super(key, null);
		}
		
		public Double(java.lang.String key, Double value) {
			super(key, value);
		}
		
		public Double getValue() {
			return (Double)_value;
		}
		
		public void setValue(Double value) {
			_value = value;
		}
	}

	public class Integer extends Property {
		
		public Integer(java.lang.String key) {
			super(key, null);
		}
		
		public Integer(java.lang.String key, Integer value) {
			super(key, value);
		}
		
		public Integer getValue() {
			return (Integer)_value;
		}
		
		public void setValue(Integer value) {
			_value = value;
		}
	}

	public class Boolean extends Property {
		
		public Boolean(java.lang.String key) {
			super(key, null);
		}
		
		public Boolean(java.lang.String key, Boolean value) {
			super(key, value);
		}
		
		public Boolean getValue() {
			return (Boolean)_value;
		}
		
		public void setValue(Boolean value) {
			_value = value;
		}
	}

	public class Date extends Property {
		
		public Date(java.lang.String key) {
			super(key, null);
		}
		
		public Date(java.lang.String key, Date value) {
			super(key, value);
		}
		
		public Date getValue() {
			return (Date)_value;
		}
		
		public void setValue(Date value) {
			_value = value;
		}
	}
	
}
