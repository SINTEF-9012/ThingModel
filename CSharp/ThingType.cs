using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingModel
{
    public class ThingType
    {
        /**
         * The name of a thing type should be unique in the system.
         * 
         * You can have multiple instance of things types with the same name,
         * but they will represent the same type.
         */
        private readonly string _name;
        public string Name { get { return _name; } }

        /**
         * Documentation for the user or the developer of other applications.
         */
        public string Description;

        /**
         * A thing type contains a list of properties.
         * Each property is defined by a key and a type.
         */
        protected IDictionary<string, PropertyType> Properties { get; private set; } 

        /**
         * The Properties dictionary is not thread-safe.
         * The following object will be used as a lock.
         */
        private readonly object _PropertiesLock = new object();

        /**
         * Create a new thing type.
         * The name should not be null or empty.
         */
        public ThingType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("The name should not be null or empty");
            }

            _name = name;
            Properties = new Dictionary<string, PropertyType>();
        }

        /**
         * Check if the given thing have all the required properties,
         * and if their values are correct.
         */
        public bool Check(Thing thing)
        {
            lock (_PropertiesLock)
            {
                return (thing.Type.Equals(this) ||
                    (thing.Type != null && thing.Type.Name == Name)) &&
                    Properties.All(propertyType => propertyType.Value.Check(thing.GetProperty<Property>(propertyType.Key)));
            }
        }

        public void DefineProperty(PropertyType property)
        {
            lock (_PropertiesLock)
            {
                Properties[property.Key] = property;
            }
        }

        public PropertyType GetPropertyDefinition(string key)
        {
            PropertyType value;
            lock (_PropertiesLock)
            {
                Properties.TryGetValue(key, out value);
            }
            return value;
        }

        public IEnumerable<PropertyType> GetProperties()
        {
            lock (_PropertiesLock)
            {
                return new List<PropertyType>(Properties.Values);
            }
        }

	    public bool Is(ThingType other)
		{
		    if (other == null || !string.Equals(_name, other._name))
		    {
			    return false;
		    }

	        lock (_PropertiesLock)
	        {
	            lock (other._PropertiesLock)
	            {
                    if (Properties.Count != other.Properties.Count)
                    {
                        return false;
                    }

                    foreach (var propertyType in Properties)
                    {
                        PropertyType otherPropertyType;
                        if (!other.Properties.TryGetValue(propertyType.Key, out otherPropertyType))
                        {
                            return false;
                        }

                        if (!propertyType.Value.Equals(otherPropertyType))
                        {
                            return false;
                        }
                    }
	            }
	        }

	        return true;
		}

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != GetType()) return false;
		    return Is((ThingType) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    return ((_name != null ? _name.GetHashCode() : 0)*397) ^ (Properties != null ? Properties.GetHashCode() : 0);
		    }
	    }
    }
}
