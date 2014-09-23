package org.thingmodel;

public abstract class Property {

	/**
	 * The property key is fixed. If you want to change the key
	 * the best solution is to create a new property and delete this one.
	 */
	protected java.lang.String _key;
	
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
	
	public java.lang.String ValueToString() {
		return _value != null ? _value.toString() : "";
	}
	
	
	public boolean CompareValue(Property other) {
		return other != null && (_value == other._value ||
				(_value != null && _value.equals(other._value)));
	}

    public abstract Property Clone();
	
	public static abstract class Location extends Property {
		
		public Location(java.lang.String key, org.thingmodel.Location value) {
			super(key, value);
		}
		
		public org.thingmodel.Location getValue() {
			return (org.thingmodel.Location)_value;
		}
		
		public static class Point extends Location {

			public Point(java.lang.String key) {
				super(key, null);
			}
			
			public Point(java.lang.String key, org.thingmodel.Location.Point value) {
				super(key, value);
			}
			
			public org.thingmodel.Location.Point getValue() {
				return (org.thingmodel.Location.Point)_value;
			}
			
			public void setValue(org.thingmodel.Location.Point value) {
				_value = value;
			}

            public Point Clone() {
                org.thingmodel.Location.Point o = getValue();
                org.thingmodel.Location.Point p = new org.thingmodel.Location.Point(
                        o.X, o.Y, o.Z
                );
                if (o.System != null) {
                    p.System = o.System;
                }

                return new Point(_key, p);
            }
		}

		public static class LatLng extends Location {

			public LatLng(java.lang.String key) {
				super(key, null);
			}
			
			public LatLng(java.lang.String key, org.thingmodel.Location.LatLng value) {
				super(key, value);
			}
			
			public org.thingmodel.Location.LatLng getValue() {
				return (org.thingmodel.Location.LatLng)_value;
			}
			
			public void setValue(org.thingmodel.Location.LatLng value) {
				_value = value;
			}

            public LatLng Clone() {
                org.thingmodel.Location.LatLng o = getValue();
                org.thingmodel.Location.LatLng p = new org.thingmodel.Location.LatLng(
                        o.X, o.Y, o.Z
                );
                if (o.System != null) {
                    p.System = o.System;
                }

                return new LatLng(_key, p);
            }
		}
		
		public static class Equatorial extends Location {

			public Equatorial(java.lang.String key) {
				super(key, null);
			}
			
			public Equatorial(java.lang.String key, org.thingmodel.Location.Equatorial value) {
				super(key, value);
			}
			
			public org.thingmodel.Location.Equatorial getValue() {
				return (org.thingmodel.Location.Equatorial)_value;
			}
			
			public void setValue(org.thingmodel.Location.Equatorial value) {
				_value = value;
			}

            public Equatorial Clone() {
                org.thingmodel.Location.Equatorial o = getValue();
                org.thingmodel.Location.Equatorial p = new org.thingmodel.Location.Equatorial(
                        o.X, o.Y, o.Z
                );
                if (o.System != null) {
                    p.System = o.System;
                }

                return new Equatorial(_key, p);
            }
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

        public String Clone() {
            return new String(_key, (java.lang.String) _value);
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

        public Double Clone() {
            return new Double(_key, (java.lang.Double) _value);
        }
	}

	public static class Int extends Property {
		
		public Int(java.lang.String key) {
			super(key, null);
		}
		
		public Int(java.lang.String key, java.lang.Integer value) {
			super(key, value);
		}
		
		public java.lang.Integer getValue() {
			return (java.lang.Integer)_value;
		}
		
		public void setValue(java.lang.Integer value) {
			_value = value;
		}

        public Int Clone() {
            return new Int(_key, (java.lang.Integer) _value);
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

        public Boolean Clone() {
            return new Boolean(_key, (java.lang.Boolean) _value);
        }
	}

	public static class DateTime extends Property {
		
		public DateTime(java.lang.String key) {
			super(key, null);
		}
		
		public DateTime(java.lang.String key, java.util.Date value) {
			super(key, value);
		}
		
		public java.util.Date getValue() {
			return (java.util.Date)_value;
		}
		
		public void setValue(java.util.Date value) {
			_value = value;
		}

        public DateTime Clone() {
            return new DateTime(_key, (java.util.Date)((java.util.Date) _value).clone());
        }
	}
	
}
