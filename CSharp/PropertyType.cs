using System;
using System.IO;

namespace ThingModel
{
    public class PropertyType
    {
        public string Key;
        public string Name;
        public string Description;

        private readonly Type _type;
        public Type Type
        {
            get { return _type; }
        }

        public bool Required;

        private PropertyType(string key, Type type, bool required)
        {
            Key = key;
            Required = required;
            _type = type;
        }

        public PropertyType(string key, Type type)
        {
            if (!typeof(Property).IsAssignableFrom(type))
            {
                throw new Exception("The type should be a subtype of Property");
            }
            Key = key;
            _type = type;
        }

        public static PropertyType Create<T>(string key,
            bool required = true) where T : Property
        {
            return new PropertyType(key, typeof(T), required);
        }

        public bool Check(Property property)
        {
            return (!Required && property == null) ||
                (property != null && property.GetType() == Type && Key.Equals(property.Key));
        }


    }
}
