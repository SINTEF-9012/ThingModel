using System;
using System.Globalization;
using System.Text;

namespace ThingModel
{
    /**
     * A property represent a value attached to a key
     */
    public abstract class Property
    {
        // The key is fixed, if you want to change the key,
        // you have to create a new Property and delete the previous one
        private readonly string _key;

        public string Key
        {
            get { return _key; }
        }

        protected Property(string key)
        {
	        if (string.IsNullOrEmpty(key))
	        {
		        throw new Exception("The Property key should not be null or empty");
	        }
            _key = key;
        }

        // Used for simple debug and displays
        public abstract string ValueToString();
        
        // After some tests, a generic version is complex and not very performant
        public abstract bool CompareValue(Property other);

		// This is clonable
	    public abstract Property Clone();

        public abstract class Location : Property
        {
            private ThingModel.Location _value;

	        public ThingModel.Location Value
	        {
		        get { return _value; }
	        }

	        private Location(string key, ThingModel.Location value = null) : base(key)
            {
                _value = value;
            }

            public override string ValueToString()
            {
                var sb = new StringBuilder();
                sb.Append(_value.X);
                sb.Append(" - ");
                sb.Append(_value.Y);
                sb.Append(" - ");
                sb.Append(_value.Z);
                return sb.ToString();
            }


	        public override bool CompareValue(Property prop)
            {
                var other = prop as Location;
                return other != null && ((_value == null && other._value == null) || (_value!= null && other._value != null && _value.GetType() == other._value.GetType() && _value.Compare(other._value)));
            }

	        public class Point : Location
	        {
				public new ThingModel.Location.Point Value
				{
					get { return (ThingModel.Location.Point) _value; }
					set { _value = value; }
				}

		        public Point(string key, ThingModel.Location.Point value = null) : base(key, value)
		        {
		        }

				public override Property Clone()
				{
                    var p = new Point(_key, new ThingModel.Location.Point(
                        Value.X, Value.Y, Value.Z));

                    if (Value.System != null)
				    {
				        p.Value.System = Value.System;
				    }

				    return p;
				}
	        }

	        public class LatLng : Location
	        {
				public new ThingModel.Location.LatLng Value
				{
					get { return (ThingModel.Location.LatLng) _value; }
					set { _value = value; }
				}

		        public LatLng(string key, ThingModel.Location.LatLng value = null) : base(key, value)
		        {
		        }
				
				public override Property Clone()
				{
                    var p = new LatLng(_key, new ThingModel.Location.LatLng(
                        Value.Latitude, Value.Longitude, Value.Altitude));

                    if (Value.System != null)
				    {
				        p.Value.System = Value.System;
				    }

				    return p;
				}
	        }

	        public class Equatorial : Location
	        {
				public new ThingModel.Location.Equatorial Value
				{
					get { return (ThingModel.Location.Equatorial) _value; }
					set { _value = value; }
				}

		        public Equatorial(string key, ThingModel.Location.Equatorial value = null) : base(key, value)
		        {
		        }
				
				public override Property Clone()
				{
                    var p = new Equatorial(_key, new ThingModel.Location.Equatorial(
                        Value.RightAscension, Value.Declination, Value.HourAngle));

                    if (Value.System != null)
				    {
				        p.Value.System = Value.System;
				    }

				    return p;
				}
	        }
        }

        public class String : Property
        {
            public string Value;

            public String(string key, string value = null)
                : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                return Value;
            }

            public override bool CompareValue(Property prop)
            {
                var other = prop as String;
                return other != null && ((Value == null && other.Value == null) || (Value != null && Value.Equals(other.Value)));
            }
				
			public override Property Clone()
			{
                // String are immutable, providing Value directly is enough
				return new String(_key, Value);
			}
        }

        public class Double : Property
        {
            public double Value;

            public Double(string key, double value = 0.0) : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }

            public override bool CompareValue(Property prop)
            {
                var other = prop as Double;
                // ReSharper disable CompareOfFloatsByEqualityOperator
                return other != null && Value == other.Value;
                // ReSharper restore CompareOfFloatsByEqualityOperator
            }
			
			public override Property Clone()
			{
				return new Double(_key, Value);
			}
        }

        public class Int : Property
        {
            public int Value;

            public Int(string key, int value = 0)
                : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }

            public override bool CompareValue(Property prop)
            {
                var other = prop as Int;
                return other != null && Value == other.Value;
            }
			
			public override Property Clone()
			{
				return new Int(_key, Value);
			}
        }

        public class Boolean : Property
        {
            public bool Value;

            public Boolean(string key, bool value = false) : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                return Value ? "true" : "false";
            }

            public override bool CompareValue(Property prop)
            {
                var other = prop as Boolean;
                return other != null && Value == other.Value;
            }
			
			public override Property Clone()
			{
				return new Boolean(_key, Value);
			}
        }

        public class DateTime : Property
        {
            public System.DateTime Value;

            public DateTime(string key, System.DateTime value = new System.DateTime()) : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }

            public override bool CompareValue(Property prop)
            {
                var other = prop as DateTime;
                return other != null && Value == other.Value;
            }
			
			public override Property Clone()
			{
				return new DateTime(_key, Value);
			}
        }
    }
}
