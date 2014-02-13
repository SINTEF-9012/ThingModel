package org.thingmodel;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;

public class Wharehouse {

	private HashMap<String, ThingType> _thingTypes = new HashMap<>();
	private HashMap<String, Thing> _things = new HashMap<>();
	private HashSet<IWharehouseObserver> _observers = new HashSet<>();
	
	private final Object _lockThingTypes = new Object();
	private final Object _lockThings = new Object();
	
	
	public void RegisterType(ThingType type) {
		RegisterType(type, true);
	}
	
	public void RegisterType(ThingType type, boolean force) {
		if (type == null) {
			throw new RuntimeException("The ThingType object should not be null");
		}
	
		String name = type.getName();
		boolean register = force;
		
		if (!register) {
			synchronized (_lockThingTypes) {
				register = !_thingTypes.containsKey(name);
			}
		}
		
		if (register) {
			
			synchronized (_lockThingTypes) {
				_thingTypes.put(name, type);
			}
			
			NotifyThingTypeDefine(type);
		}
	}

	public void RegisterThing(Thing thing) {
		RegisterThing(thing, true, false);
	}
	
	public void RegisterThing(Thing thing, boolean alsoRegisterConnections, boolean alsoRegisterTypes) {
		if (thing == null) {
			throw new RuntimeException("Null things are not allowed in the warehouse.");
		}
		
		String id = thing.getId();
		boolean creation;
	
		synchronized (_lockThings) {
			creation = !_things.containsKey(id);
			_things.put(id, thing);
		}
		
		if (alsoRegisterTypes && thing.getType() != null) {
			RegisterType(thing.getType(), false);
		}
		
		if (alsoRegisterConnections) {
			HashSet<String> alreadyVisitedThings = new HashSet<>();
			
			for (Thing connectedThing : thing.getConnectedThings()) {
				RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedThings);
			}
		}
		
		if (creation) {
			NotifyThingCreation(thing);
		} else {
			NotifyThingUpdate(thing);
		}
		
	}
	
	
	private void RecursiveRegisterThing(Thing thing, boolean alsoRegisterTypes,
			HashSet<String> alreadyVisitedThings) {
	
		String id = thing.getId();
		
		// Avoid infinite recursions
		if (alreadyVisitedThings.contains(id)) {
			return;
		}
		
		alreadyVisitedThings.add(id);
		
		RegisterThing(thing, false, alsoRegisterTypes);
		
		for(Thing connectedThing : thing.getConnectedThings()) {
			RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedThings);
		}
		
	}
	
	public void RegisterCollection(Collection<Thing> collection) {
		RegisterCollection(collection, false);
	}
	
	public void RegisterCollection(Collection<Thing> collection, boolean alsoRegisterTypes) {
		HashSet<String> alreadyVisitedThings = new HashSet<>();
		for (Thing thing : collection) {
			this.RecursiveRegisterThing(thing, alsoRegisterTypes, alreadyVisitedThings);
		}
	}
	
	public void RemoveThing(Thing thing) {
		if (thing == null) {
			return;
		}
		
		ArrayList<Thing> thingsToDisconnect = new ArrayList<>();
	
		// Remove all connections
		synchronized (_lockThings) {
			for(Thing t : _things.values()) {
				if (t.IsConnectedTo(thing)) {
                    thingsToDisconnect.add(t);
				}
			}
		}

		boolean removed;
		String id = thing.getId();
		
		synchronized (_lockThings) {
			removed = _things.containsKey(id);
			if (removed) {
				_things.remove(id);
			}
		}
		
		if (removed) {
			NotifyThingDeleted(thing);
		}
		
		for (Thing t : thingsToDisconnect) {
			t.Disconnect(thing);
			NotifyThingUpdate(t);
		}
	}
	
	public void RegisterObserver(IWharehouseObserver observer) {
		_observers.add(observer);
	}
	
	public void UnregisterObserver(IWharehouseObserver observer) {
		_observers.remove(observer);
	}
	
	public void NotifyThingTypeDefine(ThingType type) {
		for (IWharehouseObserver observer : _observers) {
			observer.Define(type);
		}
	}
	
	public void NotifyThingCreation(Thing thing) {
		for (IWharehouseObserver observer : _observers) {
			observer.New(thing);
		}
	}
	
	public void NotifyThingUpdate(Thing thing) {
		for (IWharehouseObserver observer : _observers) {
			observer.Updated(thing);
		}
	}

	public void NotifyThingDeleted(Thing thing) {
		for (IWharehouseObserver observer : _observers) {
			observer.Deleted(thing);
		}
	}
	
	public Thing getThing(String id) {
		synchronized (_lockThings) {
			return _things.get(id);
		}
	}
	
	public ThingType getThingType(String name) {
		synchronized (_lockThingTypes) {
			return _thingTypes.get(name);
		}
	}
	
	public List<Thing> getThings() {
		synchronized (_lockThings) {
			return new ArrayList<>(_things.values());
		}
	}
}
