using System;
using System.Collections.Generic;

namespace ThingModel
{
    public class Wharehouse
    {
        private readonly Dictionary<string, ThingType> _thingTypes = new Dictionary<string, ThingType>();

        private readonly Dictionary<string, Thing> _things = new Dictionary<string, Thing>();

        private readonly HashSet<IWharehouseObserver> _observers = new HashSet<IWharehouseObserver>();

		private readonly Object _lockDictionaryThingTypes = new object();
		private readonly Object _lockDictionaryThings = new object();
 
        public void RegisterType(ThingType type, bool force = true)
        {
            if (type == null)
            {
                throw new Exception("The thing type information is missing.");
            }

	        var register = force;

	        if (!register)
	        {
		        lock (_lockDictionaryThingTypes)
		        {
			        register = !_thingTypes.ContainsKey(type.Name);
		        }
	        }
            if (register)
            {
	            lock (_lockDictionaryThingTypes)
	            {
	                _thingTypes[type.Name] = type;
	            }

                NotifyThingTypeDefine(type);
            }
            
        }

        public void RegisterThing(Thing thing, bool alsoRegisterConnections = true, bool alsoRegisterTypes = false)
        {
            if (thing == null)
            {
                throw new Exception("Null are not allowed in the wharehouse.");
            }

	        bool creation;
	        lock (_lockDictionaryThings)
	        {
				creation = !_things.ContainsKey(thing.ID);
	            _things[thing.ID] = thing;
	        }

            if (alsoRegisterTypes && thing.Type != null)
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

        public void RemoveThing(Thing thing)
        {
            if (thing == null)
            {
                return;
            }

	        var thingsToDisconnect = new List<Thing>();

            // Remove all the connections
	        lock (_lockDictionaryThings)
	        {
				foreach (var t in _things)
				{
					if (t.Value.IsConnectedTo(thing))
					{
						thingsToDisconnect.Add(t.Value);
					}
				}
	        }


            // Remove the thing
	        bool removed;

	        lock (_lockDictionaryThings)
	        {
		        removed = _things.Remove(thing.ID);
	        }

            if (removed)
            {
                NotifyThingDeleted(thing);
            }
	        
			foreach (var t in thingsToDisconnect)
	        {
				t.Disconnect(thing);
				NotifyThingUpdate(t);
	        }
        }

        public void RegisterObserver(IWharehouseObserver modelObserver)
        {
            _observers.Add(modelObserver);
        }

        public void UnregisterObserver(IWharehouseObserver modelObserver)
        {
            _observers.Remove(modelObserver);
        }
        
        public void NotifyThingTypeDefine(ThingType type)
        {
            foreach (var observer in _observers)
            {
                observer.Define(type);
            }
        }

        public void NotifyThingUpdate(Thing thing)
        {
            foreach (var observer in _observers)
            {
                observer.Updated(thing);
            }
        }

        public void NotifyThingCreation(Thing thing)
        {
            foreach (var observer in _observers)
            {
                observer.New(thing);
            }
        }

        public void NotifyThingDeleted(Thing thing)
        {
            foreach (var observer in _observers)
            {
                observer.Deleted(thing);
            }
        }

        public Thing GetThing(string id)
        {
            Thing value;
	        lock (_lockDictionaryThings)
	        {
	            _things.TryGetValue(id, out value);
	        }
            return value;
        }

        public ThingType GetThingType(string name)
        {
            ThingType value;
	        lock (_lockDictionaryThingTypes)
	        {
	            _thingTypes.TryGetValue(name, out value);
	        }
            return value;
        }

	    public IList<Thing> Things
	    {
		    get
		    {
			    lock (_lockDictionaryThings)
			    {
				    return new List<Thing>(_things.Values);
			    }
		    }
	    }

	    public IList<ThingType> ThingTypes
	    {
		    get
		    {
			    lock (_lockDictionaryThingTypes)
			    {
				    return new List<ThingType>(_thingTypes.Values);
			    }
			}
	    } 
    }
}
