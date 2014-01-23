using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingModel
{
    public class Thing
    {
        /**
         * String ID, unique in the world.
         * 
         * Is never null.
         */
        private readonly string _id;
        public string ID { get { return _id; } }

        /**
         * Reference to the type of the object.
         * 
         * Can be null if the thing is related to the default type.
         */
        private readonly ThingType _type;
        public ThingType Type { get { return _type; } }

        /**
         * A thing have a liste of properties.
         * A property is defined by a key and a value.
         */
        protected IDictionary<string, Property> Properties { get; private set; }

        /**
         * A thing can be connected to other things,
         * but never directly to itself.
         */
        protected IDictionary<string, Thing> Connections { get; private set; } 

        /**
         * Create a new thing. The id cannot be null and should
         * be unique in the world. If the type is null, a default
         * thing type is used.
         */
        public Thing(string id, ThingType type = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("The ID should not be null or empty");
            }

            _id = id;
            _type = type;
            Properties = new Dictionary<string, Property>();
            Connections = new Dictionary<string, Thing>();
        }

        public void SetProperty(Property property)
        {
            Properties[property.Key] = property;
        }

        public T GetProperty<T>(string key) where T : Property
        {
            Property value;
            Properties.TryGetValue(key, out value);
            return value as T;
        }

        public bool HasProperty(string key)
        {
            return Properties.ContainsKey(key);
        }

        public void Connect(Thing thing)
        {
            if (thing == this || thing._id == _id)
            {
                throw new Exception("Cannot connect a thing to itself");
            }
            Connections.Add(thing._id, thing);
        }

        public bool Disconnect(Thing thing)
        {
            return Connections.Remove(thing._id);
        }

        public bool IsConnectedTo(Thing thing)
        {
            return Connections.ContainsKey(thing._id);
        }

        /**
         * Get a list of connected things.
         * The list is a copy so you can connect and disconnect
         * things when you are enumating the list.
         */
        public IList<Thing> ConnectedThings
        {
            get { return new List<Thing>(Connections.Values); }
        }



        public bool Compare(Thing other, bool compareId = true, bool deepComparaisonForConnectedThings = false)
        {
            // Optimization
            if (this == other)
            {
                return true;
            }

            if (other == null || (Type != null && other.Type != null && Type.Name != other.Type.Name) ||
                (Type == null && other.Type != null) || (Type != null && other.Type == null))
            {
                return false;
            }

            if (compareId && _id != other._id)
            {
                return false;
            }

            if (Connections.Keys.Any(key => !other.Connections.ContainsKey(key)))
            {
                return false;
            }

            foreach (var property in Properties)
            {
                Property otherProp;
                other.Properties.TryGetValue(property.Key, out otherProp);

                if (otherProp == null || !property.Value.Compare(otherProp))
                {
                    return false;
                }
            }

            foreach (var property in other.Properties)
            {
                Property ownProp;
                Properties.TryGetValue(property.Key, out ownProp);

                if (ownProp == null || !property.Value.Compare(ownProp))
                {
                    return false;
                }
            }

            if (deepComparaisonForConnectedThings)
            {
                return RecursiveCompare(other, new HashSet<string>());
            }

            return true;
        }

        private bool RecursiveCompare(Thing other, HashSet<string> workIds) {
            if (workIds.Contains(_id))
            {
                return true;
            }

            if (!Compare(other))
            {
                return false;
            }

            workIds.Add(_id);

            foreach (var connectedThing in Connections)
            {
                Thing otherThing;
                other.Connections.TryGetValue(connectedThing.Key, out otherThing);

                if (otherThing == null || !connectedThing.Value.RecursiveCompare(otherThing, workIds))
                {
                    return false;
                }
            }

            foreach (var connectedThing in other.Connections)
            {
                Thing ownThing;
                Connections.TryGetValue(connectedThing.Key, out ownThing);

                if (ownThing == null || !connectedThing.Value.RecursiveCompare(ownThing, workIds))
                {
                    return false;
                }
            }

            return true;

        }

        public IEnumerable<Property> GetProperties()
        {
            return new List<Property>(Properties.Values);/*.AsReadOnly()*/;
        }
    }
}
