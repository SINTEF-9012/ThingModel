using System;
using System.Collections.Generic;

namespace ThingModel
{
    public class Warehouse
    {
        private readonly Dictionary<string, ThingType> _thingTypes = new Dictionary<string, ThingType>();

        private readonly Dictionary<string, Thing> _things = new Dictionary<string, Thing>();

        private readonly HashSet<IWarehouseObserver> _observers = new HashSet<IWarehouseObserver>();

		private readonly Object _lockDictionaryThingTypes = new object();
		private readonly Object _lockDictionaryThings = new object();
		private readonly Object _lockHashSetObservers = new object();
 
        public void RegisterType(ThingType type, bool force = true, string sender = null)
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

                NotifyThingTypeDefine(type, sender);
            }
            
        }

        public void RegisterThing(Thing thing, bool alsoRegisterConnections = true, bool alsoRegisterTypes = true, string sender = null)
        {
            if (thing == null)
            {
                throw new Exception("Null are not allowed in the warehouse.");
            }

	        bool creation;
	        lock (_lockDictionaryThings)
	        {
				creation = !_things.ContainsKey(thing.ID);
	            _things[thing.ID] = thing;
	        }

            if (alsoRegisterTypes && thing.Type != null)
            {
                RegisterType(thing.Type, false, sender);
            }


            if (alsoRegisterConnections)
            {
                var set = new HashSet<Thing>();
                foreach (var connectedThing in thing.ConnectedThings)
                {
                    RecursiveRegisterThing(connectedThing, alsoRegisterTypes, set, sender);
                }
            }

            if (creation)
            {
                NotifyThingCreation(thing, sender);
            }
            else
            {
                NotifyThingUpdate(thing, sender);
            }
            
        }

        private void RecursiveRegisterThing(Thing thing, bool alsoRegisterTypes, HashSet<Thing> alreadyRegisteredThings, string sender)
        {
            
            // Avoid infinite recursions
            if (alreadyRegisteredThings.Contains(thing))
            {
                return;
            }

            alreadyRegisteredThings.Add(thing);
            
            RegisterThing(thing, false, alsoRegisterTypes, sender);
            
            foreach (var connectedThing in thing.ConnectedThings)
            {
                RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyRegisteredThings, sender);
            }
            
        }

		public void RemoveCollection(ISet<Thing> collection, string sender = null)
		{
			var thingsToDisconnect = new HashSet<Thing>();

			foreach (var thing in collection)
			{
				RemoveThing(thing, false, sender);

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
			}

			foreach (var t in thingsToDisconnect)
			{
				if (!collection.Contains(t))
				{
					NotifyThingUpdate(t, sender);
				}
			}
		}

		public void RegisterCollection(IEnumerable<Thing> collection, bool alsoRegisterTypes = false, string sender = null)
		{
			var set = new HashSet<Thing>();
			foreach (var thing in collection)
			{
				RecursiveRegisterThing(thing, alsoRegisterTypes, set, sender);
			}
		}

		public void RemoveThing(Thing thing, bool notifyUpdates = true, string sender = null)
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
				NotifyThingDeleted(thing, sender);
			}

			if (notifyUpdates)
			{
				foreach (var t in thingsToDisconnect)
				{
					t.Disconnect(thing);
					NotifyThingUpdate(t, sender);
				}
			}
			else
			{
				foreach (var t in thingsToDisconnect)
				{
					t.Disconnect(thing);
				}
			}
		}

        public void RegisterObserver(IWarehouseObserver modelObserver)
        {
	        lock (_lockHashSetObservers)
	        {
			    _observers.Add(modelObserver);    
	        }
            
        }

        public void UnregisterObserver(IWarehouseObserver modelObserver)
        {
	        lock (_lockHashSetObservers)
	        {
		        _observers.Remove(modelObserver);
	        }
        }
        
        public void NotifyThingTypeDefine(ThingType type, string sender)
        {

	        IList<IWarehouseObserver> observers = null;
			lock (_lockHashSetObservers)
			{
				observers = new List<IWarehouseObserver>(_observers);
			}
            foreach (var observer in observers)
            {
                observer.Define(type, sender);
            }
        }

        public void NotifyThingUpdate(Thing thing, string sender)
        {
	        IList<IWarehouseObserver> observers = null;
			lock (_lockHashSetObservers)
			{
				observers = new List<IWarehouseObserver>(_observers);
			}
            foreach (var observer in observers)
            {
                observer.Updated(thing, sender);
            }
        }

        public void NotifyThingCreation(Thing thing, string sender)
        {
	        IList<IWarehouseObserver> observers = null;
			lock (_lockHashSetObservers)
			{
				observers = new List<IWarehouseObserver>(_observers);
			}
            foreach (var observer in observers)
            {
                observer.New(thing, sender);
            }
        }

        public void NotifyThingDeleted(Thing thing, string sender)
        {
	        IList<IWarehouseObserver> observers = null;
			lock (_lockHashSetObservers)
			{
				observers = new List<IWarehouseObserver>(_observers);
			}
            foreach (var observer in observers)
            {
                observer.Deleted(thing, sender);
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

	    private WarehouseEvents _eventsObserver;

	    public WarehouseEvents Events
	    {
		    get
		    {
			    // Lazy loading
			    if (_eventsObserver == null)
			    {
					_eventsObserver = new WarehouseEvents();
					RegisterObserver(_eventsObserver);
			    }

			    return _eventsObserver;
		    }
	    }

    }
}
