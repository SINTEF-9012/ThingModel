package org.thingmodel.builders;

import org.thingmodel.Property;
import org.thingmodel.PropertyType;
import org.thingmodel.ThingType;

public class BuildANewThingType {
	private BuildANewThingType() {
	}

	public static ThingTypePropertyBuilder Named(String name) {
		return new ThingTypePropertyBuilder(new ThingType(name));
	}
	
	public static class ThingTypePropertyBuilder {
		private final ThingType _type;
		private boolean _nextPropertyIsNotRequired;
		
		private PropertyType _lastProperty;

		public final ThingTypePropertyBuilder ContainingA;
		public final ThingTypePropertyBuilder ContainingAn;
		public final ThingTypePropertyBuilder AndA;
		public final ThingTypePropertyBuilder AndAn;
		
		private ThingTypePropertyBuilder(ThingType type) {
			_type = type;
			ContainingA = this;
			ContainingAn = this;
			AndA = this;
			AndAn = this;
		}
		
		public ThingTypePropertyBuilder NotRequired() {
			_nextPropertyIsNotRequired = true;
			return this;
		}
		
		public ThingTypePropertyBuilder WhichIs(String description) {
			if (_lastProperty == null) {
				_type.Description = description;
			} else {
				_lastProperty.Description = description;
			}
			
			return this;
		}
		
		private void _createProperty(String key, String name, Class<? extends Property> type) {
			if (_lastProperty != null) {
				_type.DefineProperty(_lastProperty);
			}
			
			PropertyType property = new PropertyType(key, type);
			property.Name = name;
			
			if (_nextPropertyIsNotRequired) {
				_nextPropertyIsNotRequired = false;
				property.Required = false;
			}
			
			_lastProperty = property;
		}
		
		public ThingTypePropertyBuilder String(String key) {
			_createProperty(key, null, Property.String.class);
			return this;
		}
		
		public ThingTypePropertyBuilder String(String key, String name) {
			_createProperty(key, name, Property.String.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationPoint() {
			_createProperty("location", null, Property.Location.Point.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationPoint(String key) {
			_createProperty(key, null, Property.Location.Point.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationPoint(String key, String name) {
			_createProperty(key, name, Property.Location.Point.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationLatLng() {
			_createProperty("location", null, Property.Location.LatLng.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationLatLng(String key) {
			_createProperty(key, null, Property.Location.LatLng.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationLatLng(String key, String name) {
			_createProperty(key, name, Property.Location.LatLng.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationEquatorial() {
			_createProperty("location", null, Property.Location.Equatorial.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationEquatorial(String key) {
			_createProperty(key, null, Property.Location.Equatorial.class);
			return this;
		}
		
		public ThingTypePropertyBuilder LocationEquatorial(String key, String name) {
			_createProperty(key, name, Property.Location.Equatorial.class);
			return this;
		}
		
		
		public ThingTypePropertyBuilder Double(String key) {
			_createProperty(key, null, Property.Double.class);
			return this;
		}
		
		public ThingTypePropertyBuilder Double(String key, String name) {
			_createProperty(key, name, Property.Double.class);
			return this;
		}
		
		public ThingTypePropertyBuilder Int(String key) {
			_createProperty(key, null, Property.Int.class);
			return this;
		}
		
		public ThingTypePropertyBuilder Int(String key, String name) {
			_createProperty(key, name, Property.Int.class);
			return this;
		}	
		
		public ThingTypePropertyBuilder Boolean(String key) {
			_createProperty(key, null, Property.Boolean.class);
			return this;
		}
		
		public ThingTypePropertyBuilder Boolean(String key, String name) {
			_createProperty(key, name, Property.Boolean.class);
			return this;
		}
		
		public ThingTypePropertyBuilder DateTime(String key) {
			_createProperty(key, null, Property.DateTime.class);
			return this;
		}
		
		public ThingTypePropertyBuilder DateTime(String key, String name) {
			_createProperty(key, name, Property.DateTime.class);
			return this;
		}
		
		public ThingTypePropertyBuilder CopyOf(ThingType otherType) {
			for (PropertyType propertyType : otherType.getProperties()) {
				_type.DefineProperty(propertyType.Clone());
			}
			return this;
		}
		
		public ThingType Build() {
			if (_lastProperty != null) {
				_type.DefineProperty(_lastProperty);
			}
			
			return _type;
		}
	}
}
