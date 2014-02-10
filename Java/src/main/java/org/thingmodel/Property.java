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
	
	public static class Location extends Property {
		
		public Location(java.lang.String key) {
			super(key, null);
		}
		
		public Location(java.lang.String key, org.thingmodel.Location value) {
			super(key, value);
		}
		
		public org.thingmodel.Location getValue() {
			return (org.thingmodel.Location)_value;
		}
		
		public void setValue(org.thingmodel.Location value) {
			_value = value;
		}
	}

	public static class String extends Property {
		
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
	
	public static class Double extends Property {
		
		public Double(java.lang.String key) {
			super(key, null);
		}
		
		public Double(java.lang.String key, java.lang.Double value) {
			super(key, value);
		}
		
		public java.lang.Double getValue() {
			return (java.lang.Double)_value;
		}
		
		public void setValue(java.lang.Double value) {
			_value = value;
		}
	}

	public static class Integer extends Property {
		
		public Integer(java.lang.String key) {
			super(key, null);
		}
		
		public Integer(java.lang.String key, java.lang.Integer value) {
			super(key, value);
		}
		
		public java.lang.Integer getValue() {
			return (java.lang.Integer)_value;
		}
		
		public void setValue(java.lang.Integer value) {
			_value = value;
		}
	}

	public static class Boolean extends Property {
		
		public Boolean(java.lang.String key) {
			super(key, null);
		}
		
		public Boolean(java.lang.String key, java.lang.Boolean value) {
			super(key, value);
		}
		
		public java.lang.Boolean getValue() {
			return (java.lang.Boolean)_value;
		}
		
		public void setValue(java.lang.Boolean value) {
			_value = value;
		}
	}

	public static class Date extends Property {
		
		public Date(java.lang.String key) {
			super(key, null);
		}
		
		public Date(java.lang.String key, java.util.Date value) {
			super(key, value);
		}
		
		public java.util.Date getValue() {
			return (java.util.Date)_value;
		}
		
		public void setValue(java.util.Date value) {
			_value = value;
		}
	}
	
}
