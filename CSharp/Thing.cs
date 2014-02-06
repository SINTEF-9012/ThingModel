﻿using System;
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
         * A thing contains a list of properties.
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

        public void Detach()
        {
            Connections.Clear();
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


        /**
         * Compare if two things are equals
         * 
         * Can compare also connected things, without infinite recursions.
         */
        public bool Compare(Thing other, bool compareId = true, bool deepComparaisonForConnectedThings = false)
        {
            // Optimization, when two things are the same
            if (this == other)
            {
                return true;
            }

            // If the types are not the sames
            // The type can be null so it's a bit ugly here
            if (other == null || (Type != null && other.Type != null && Type.Name != other.Type.Name) ||
                (Type == null && other.Type != null) || (Type != null && other.Type == null))
            {
                return false;
            }

            // If we just need to compare the ids, it's very fast
            if (compareId && _id != other._id)
            {
                return false;
            }

            // Check if the connection list are the same
            if (Connections.Count != other.Connections.Count ||
                Connections.Keys.Any(key => !other.Connections.ContainsKey(key)))
            {
                return false;
            }

            if (Properties.Count != other.Properties.Count)
            {
                return false;
            }

            // Check if the properties are the same
            foreach (var property in Properties)
            {
                Property otherProp;
                other.Properties.TryGetValue(property.Key, out otherProp);

                if (otherProp == null || !property.Value.CompareValue(otherProp))
                {
                    return false;
                }
            }

            // And start the start recursion if needed
            if (deepComparaisonForConnectedThings)
            {
                // The hashset is declared only once and here
                return RecursiveCompare(other, new HashSet<string>());
            }

            return true;
        }

        // Do a recursive comparaison, should be called 
        private bool RecursiveCompare(Thing other, HashSet<string> workIds) {
            // If the thing was already checked, we don't need to check it again
            if (workIds.Contains(_id))
            {
                // And it's alway true, because we are still looking for a difference
                return true;
            }

            // Made a simple comparison first
            if (!Compare(other))
            {
                return false;
            }

            // We are working on the current thing,
            // this instruction prevent infinite recursion
            workIds.Add(_id);

            // And start the recursion for connected things
            foreach (var connectedThing in Connections)
            {
                var otherThing = other.Connections[connectedThing.Key];

                if (!connectedThing.Value.RecursiveCompare(otherThing, workIds))
                {
                    return false;
                }
            }

            return true;

        }

        public IEnumerable<Property> GetProperties()
        {
            // Create a copy of this list, so the user can change the
            // things properties during a loop on this list without exceptions
            return new List<Property>(Properties.Values);
        }
    }
}
