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
            _key = key;
        }

        // Used for simple debug and displays
        public abstract string ValueToString();
        
        // After some tests, a generic version is complex and not very performant
        public abstract bool Compare(Property other);

        public class Location : Property
        {
            public ThingModel.Location Value;

            public Location(string key, ThingModel.Location value = null) : base(key)
            {
                Value = value;
            }

            public override string ValueToString()
            {
                var sb = new StringBuilder();
                sb.Append(Value.X);
                sb.Append(" - ");
                sb.Append(Value.Y);
                sb.Append(" - ");
                sb.Append(Value.Z);
                return sb.ToString();
            }

            public override bool Compare(Property prop)
            {
                var other = prop as Location;
                return other != null && ((Value == null && other.Value == null) || (Value!= null && other.Value != null && Value.GetType() == other.Value.GetType() && Value.Compare(other.Value)));
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

            public override bool Compare(Property prop)
            {
                var other = prop as String;
                return other != null && ((Value == null && other.Value == null) || (Value != null && Value.Equals(other.Value)));
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

            public override bool Compare(Property prop)
            {
                var other = prop as Double;
                // ReSharper disable CompareOfFloatsByEqualityOperator
                return other != null && Value == other.Value;
                // ReSharper restore CompareOfFloatsByEqualityOperator
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

            public override bool Compare(Property prop)
            {
                var other = prop as Int;
                return other != null && Value == other.Value;
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

            public override bool Compare(Property prop)
            {
                var other = prop as Boolean;
                return other != null && Value == other.Value;
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

            public override bool Compare(Property prop)
            {
                var other = prop as DateTime;
                return other != null && Value == other.Value;
            }
        }
    }
}
