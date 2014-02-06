using System;

namespace ThingModel
{
    public class PropertyType
    {
	    private string _key;

	    public string Key
	    {
		    get { return _key; }
	    }

        public string Name;
        public string Description;

        private readonly Type _type;
        public Type Type
        {
            get { return _type; }
        }

        public bool Required;

        // Performant constructor, but it's insecure
        // You should use the static Create method instead
        private PropertyType(string key, Type type, bool required)
        {
            _key = key;
            Required = required;
            _type = type;
        }

        // Slow constructor, but you can give directly a Type object
        // The static Create method can be used instead
        public PropertyType(string key, Type type)
        {
            if (!typeof(Property).IsAssignableFrom(type))
            {
                throw new Exception("The type should be a subtype of Property");
            }
            _key = key;
            _type = type;
        }

        // Tool for create PropertyType objets using C# genericity
        // It's performant and no so complex
        public static PropertyType Create<T>(string key,
            bool required = true) where T : Property
        {
            return new PropertyType(key, typeof(T), required);
        }

        // Compare if the key and the type are equals
        public bool Check(Property property)
        {
            return (!Required && property == null) ||
                (property != null && property.GetType() == Type && Key.Equals(property.Key));
        }


    }
}
