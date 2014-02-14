﻿using System;

namespace ThingModel.Builders
{
	public class BuildANewThing
	{
		private ThingType _type;

		private BuildANewThing(ThingType type)
		{
			_type = type;
		}


		public ThingPropertyBuilder IdentifiedBy(String id)
		{
			return new ThingPropertyBuilder(new Thing(id, _type));
		}

		public static BuildANewThing WithoutType()
		{
			return new BuildANewThing(null);
		}

		public static BuildANewThing As(ThingType type)
		{
			return new BuildANewThing(type);
		}

		public class ThingPropertyBuilder
		{
			private readonly Thing _thing;

			public ThingPropertyBuilder(Thing thing)
			{
				_thing = thing;
			}

			public ThingPropertyBuilder ContainingA
			{
				get { return this; }
			}

			public ThingPropertyBuilder ContainingAn
			{
				get { return this; }
			}

			public ThingPropertyBuilder AndA
			{
				get { return this; }
			}

			public ThingPropertyBuilder AndAn
			{
				get { return this; }
			}

			public ThingPropertyBuilder String(String key, String value)
			{
				_thing.SetProperty(new Property.String(key, value));
				return this;
			}

			public ThingPropertyBuilder Location(String key, Location value)
			{
				_thing.SetProperty(new Property.Location(key, value));
				return this;
			}

			public ThingPropertyBuilder Double(String key, double value)
			{
				_thing.SetProperty(new Property.Double(key, value));
				return this;
			}

			public ThingPropertyBuilder Int(String key, int value)
			{
				_thing.SetProperty(new Property.Int(key, value));
				return this;
			}

			public ThingPropertyBuilder Boolean(String key, bool value)
			{
				_thing.SetProperty(new Property.Boolean(key, value));
				return this;
			}

			public ThingPropertyBuilder DateTime(String key, DateTime value)
			{
				_thing.SetProperty(new Property.DateTime(key, value));
				return this;
			}

			public static implicit operator Thing(ThingPropertyBuilder b)
			{
				return b._thing;
			}
		}
	}
}
