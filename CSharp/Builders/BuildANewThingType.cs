using System;

namespace ThingModel.Builders
{
	public class BuildANewThingType
	{
		private BuildANewThingType()
		{}

		public static ThingTypePropertyBuilder Named(String name)
		{
			return new ThingTypePropertyBuilder(new ThingType(name));
		}

		public class ThingTypePropertyBuilder {
			private readonly ThingType _type;
			private bool _nextPropertyIsRequired;
			private bool _lastPropertyAdded;
			private PropertyType _lastProperty;

			internal ThingTypePropertyBuilder(ThingType type)
			{
				_type = type;
			}

			public ThingTypePropertyBuilder ContainingA
			{
				get { return this; }
			}

			public ThingTypePropertyBuilder ContainingAn
			{
				get { return this; }
			}

			public ThingTypePropertyBuilder AndA
			{
				get { return this; }
			}
			public ThingTypePropertyBuilder AndAn
			{
				get { return this; }
			}
			
			public ThingTypePropertyBuilder NotRequired
			{
				get
				{
					_nextPropertyIsRequired = true;
					return this;
				}
			}


			public ThingTypePropertyBuilder WichIs(String description)
			{
				if (_lastProperty == null)
				{
					_type.Description = description;
				}
				else
				{
					_lastProperty.Description = description;
				}
				return this;
			}

			private void _createProperty<T>(String key, String name) where T : Property
			{
				if (!_lastPropertyAdded && _lastProperty != null)
				{
					_type.DefineProperty(_lastProperty);
					_lastPropertyAdded = false;
				}

				var property = PropertyType.Create<T>(key);
				property.Name = name;

				if (_nextPropertyIsRequired)
				{
					_nextPropertyIsRequired = false;
					property.Required = false;
				}

				_lastProperty = property;
			}

			public ThingTypePropertyBuilder String(String key, String name = null)
			{
				_createProperty<Property.String>(key, name);
				return this;
			}
			
			public ThingTypePropertyBuilder Location(String key, String name = null)
			{
				_createProperty<Property.Location>(key, name);
				return this;
			}

			public ThingTypePropertyBuilder Double(String key, String name = null)
			{
				_createProperty<Property.Double>(key, name);
				return this;
			}

			public ThingTypePropertyBuilder Int(String key, String name = null)
			{
				_createProperty<Property.Int>(key, name);
				return this;
			}

			public ThingTypePropertyBuilder Boolean(String key, String name = null)
			{
				_createProperty<Property.Boolean>(key, name);
				return this;
			}

			public ThingTypePropertyBuilder DateTime(String key, String name = null)
			{
				_createProperty<Property.DateTime>(key, name);
				return this;
			}

			public ThingTypePropertyBuilder CopyOfThis(ThingType otherType)
			{
				foreach (var propertyType in otherType.GetProperties())
				{
					_type.DefineProperty(propertyType.Clone());
				}
				return this;
			}

			public static implicit operator ThingType(ThingTypePropertyBuilder b)
			{
				if (!b._lastPropertyAdded && b._lastProperty != null)
				{
					b._type.DefineProperty(b._lastProperty);
				}

				return b._type;
			}
		}

	}
}
