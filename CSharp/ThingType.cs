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
            return (thing.Type == this ||
				(thing.Type != null && thing.Type.Name == Name)) &&
				Properties.All(propertyType => propertyType.Value.Check(thing.GetProperty<Property>(propertyType.Key)));
        }

        public void DefineProperty(PropertyType property)
        {
            Properties[property.Key] = property;
        }

        public PropertyType GetPropertyDefinition(string key)
        {
            PropertyType value;
            Properties.TryGetValue(key, out value);
            return value;
        }

        public IEnumerable<PropertyType> GetProperties()
        {
            return new List<PropertyType>(Properties.Values);
        }
    }
}
