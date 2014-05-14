package org.thingmodel.builders;

import org.thingmodel.Property;
import org.thingmodel.Thing;
import org.thingmodel.ThingType;

public class BuildANewThing {

	private final ThingType _type;
	
	private BuildANewThing(ThingType type) {
		_type = type;
	}
	
	public ThingPropertyBuilder IdentifiedBy(String id) {
		return new ThingPropertyBuilder(new Thing(id, _type));
	}
	
	public static BuildANewThing As(ThingType type) {
		return new BuildANewThing(type);
	}
	
	public static BuildANewThing WithoutType() {
		return new BuildANewThing(null);
	}
	
	public static class ThingPropertyBuilder {
		private final Thing _thing;
		
		public final ThingPropertyBuilder ContainingA;
		public final ThingPropertyBuilder ContainingAn;
		public final ThingPropertyBuilder AndA;
		public final ThingPropertyBuilder AndAn;
		
		public ThingPropertyBuilder(Thing thing) {
			_thing = thing;
			ContainingA = this;
			ContainingAn = this;
			AndA = this;
			AndAn = this;
		}
		
		public ThingPropertyBuilder String(String key, String value) {
			_thing.setProperty(new Property.String(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Location(org.thingmodel.Location.Point value) {
			return Location("location", value);
		}
		
		public ThingPropertyBuilder Location(String key, org.thingmodel.Location.Point value) {
			_thing.setProperty(new Property.Location.Point(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Location(org.thingmodel.Location.LatLng value) {
			return Location("location", value);
		}
		
		public ThingPropertyBuilder Location(String key, org.thingmodel.Location.LatLng value) {
			_thing.setProperty(new Property.Location.LatLng(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Location(org.thingmodel.Location.Equatorial value) {
			return Location("location", value);
		}
		
		public ThingPropertyBuilder Location(String key, org.thingmodel.Location.Equatorial value) {
			_thing.setProperty(new Property.Location.Equatorial(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Double(String key, double value) {
			_thing.setProperty(new Property.Double(key, value));
			return this;
		}

		public ThingPropertyBuilder Int(String key, int value) {
			_thing.setProperty(new Property.Int(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Boolean(String key, boolean value) {
			_thing.setProperty(new Property.Boolean(key, value));
			return this;
		}
		
		public ThingPropertyBuilder Date(String key, java.util.Date value) {
			_thing.setProperty(new Property.DateTime(key, value));
			return this;
		}
		
		public Thing Build() {
			return _thing;
		}
	}
}
