using System;

namespace ThingModel
{
    public class PropertyType
    {
	    private readonly string _key;

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
	        if (String.IsNullOrEmpty(key))
	        {
		        throw new Exception("The PropertyType key should not be null or empty");
	        }
            _key = key;
            Required = required;
            _type = type;
        }

        // Slow constructor, but you can give directly a Type object
        // The static Create method can be used instead
        public PropertyType(string key, Type type) : this(key, type, true)
        {
            if (!typeof(Property).IsAssignableFrom(type))
            {
                throw new Exception("The type should be a subtype of Property");
            }
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

	    public PropertyType Clone()
	    {
			return new PropertyType(_key, _type, Required)
			{
				Name = Name,
				Description = Description
			};
	    }

	    protected bool Equals(PropertyType other)
	    {
		    return string.Equals(_key, other._key) &&
		           string.Equals(Name, other.Name) &&
		           _type == other._type &&
		           Required.Equals(other.Required);
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != GetType()) return false;
		    return Equals((PropertyType) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    { 
// ReSharper disable NonReadonlyFieldInGetHashCode
			    int hashCode = (_key != null ? _key.GetHashCode() : 0);
			    hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
			    hashCode = (hashCode*397) ^ (_type != null ? _type.GetHashCode() : 0);
			    hashCode = (hashCode*397) ^ Required.GetHashCode();
			    hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
// ReSharper restore NonReadonlyFieldInGetHashCode
			    return hashCode;
		    }
	    }
    }
}
