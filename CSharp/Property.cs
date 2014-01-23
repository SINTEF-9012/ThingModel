using System;
using System.Globalization;

namespace ThingModel
{
    public abstract class Property
    {
        private readonly string _key;

        public string Key
        {
            get { return _key; }
        }

        protected Property(string key)
        {
            _key = key;
        }

        public string ValueToString()
        {
            return "undefined";
        }

        public abstract bool Compare(Property other);
        /*{
            if (other != null)
            {
                Type t = GetType();
                if (t == other.GetType())
                {
                    return (bool) t.GetMethod("Compare", new [] {t}).Invoke(this, new object[]{other});
                }
            }

            return false;
        }*/

        public class Location : Property
        {
            public ThingModel.Location Value;

            public Location(string key, ThingModel.Location value = null) : base(key)
            {
                Value = value;
            }

            public new string ValueToString()
            {
                return Value.ToString();
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

            public new string ValueToString()
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

            public new string ValueToString()
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

            public new string ValueToString()
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

            public new string ValueToString()
            {
                return Value.ToString();
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

            public new string ValueToString()
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
