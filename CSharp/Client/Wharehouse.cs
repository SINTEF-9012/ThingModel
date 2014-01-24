using System;
using System.Collections.Generic;

namespace ThingModel
{
    public class Wharehouse
    {
        private readonly Dictionary<string, ThingType> _thingTypes = new Dictionary<string, ThingType>();

        private readonly Dictionary<string, Thing> _things = new Dictionary<string, Thing>();

        private readonly HashSet<IThingObserver> _thingObservers = new HashSet<IThingObserver>();
 
        private readonly HashSet<IThingTypeObserver> _thingTypeObservers = new HashSet<IThingTypeObserver>();
 
        public void RegisterType(ThingType type, bool force = true)
        {
            if (type == null)
            {
                throw new Exception("The thing type information is missing.");
            }

            if (force || !_thingTypes.ContainsKey(type.Name))
            {
                _thingTypes[type.Name] = type;

                NotifyThingTypeDefine(type);
            }
        }

        public void RegisterThing(Thing thing, bool alsoRegisterConnections = true, bool alsoRegisterTypes = false)
        {
            if (thing == null)
            {
                throw new Exception("Null are not allowed in the wharehouse.");
            }


            var creation = !_things.ContainsKey(thing.ID);
            _things[thing.ID] = thing;

            if (alsoRegisterTypes)
            {
                RegisterType(thing.Type, false);
            }


            if (alsoRegisterConnections)
            {
                var set = new HashSet<Thing>();
                foreach (var connectedThing in thing.ConnectedThings)
                {
                    RecursiveRegisterThing(connectedThing, alsoRegisterTypes, set);
                }
            }

            if (creation)
            {
                NotifyThingCreation(thing);
            }
            else
            {
                NotifyThingUpdate(thing);
            }
            
        }

        private void RecursiveRegisterThing(Thing thing, bool alsoRegisterTypes, HashSet<Thing> alreadyRegisteredThings)
        {
            
            // Avoid infinite recursions
            if (alreadyRegisteredThings.Contains(thing))
            {
                return;
            }

            alreadyRegisteredThings.Add(thing);
            
            RegisterThing(thing, false, alsoRegisterTypes);
            
            foreach (var connectedThing in thing.ConnectedThings)
            {
                RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyRegisteredThings);
            }
            
        }

        public void RegisterCollection(IEnumerable<Thing> collection, bool alsoRegisterTypes = false)
        {
            var set = new HashSet<Thing>();
            foreach (var thing in collection)
            {
                RecursiveRegisterThing(thing, alsoRegisterTypes, set);
            }
        }

        public void RegisterObserver(IThingObserver observer)
        {
            _thingObservers.Add(observer);
        }

        public void RegisterObserver(IThingTypeObserver observer)
        {
            _thingTypeObservers.Add(observer);
        }

        public void Unregister(IThingObserver observer)
        {
            _thingObservers.Remove(observer);
        }

        public void Unregister(IThingTypeObserver observer)
        {
            _thingTypeObservers.Remove(observer);
        }

        public void NotifyThingTypeDefine(ThingType type)
        {
            foreach (var observer in _thingTypeObservers)
            {
                observer.Define(type);
            }
        }

        public void NotifyThingUpdate(Thing thing)
        {
            foreach (var observer in _thingObservers)
            {
                observer.Updated(thing);
            }
        }

        public void NotifyThingCreation(Thing thing)
        {
            foreach (var observer in _thingObservers)
            {
                observer.New(thing);
            }
        }
    }
}
