package org.thingmodel;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;

public class Wharehouse {

	private ConcurrentHashMap<String, ThingType> _thingTypes;
	private ConcurrentHashMap<String, Thing> _things;
	private HashSet<IWharehouseObserver> _observers;
	
	public Wharehouse() {
		_thingTypes = new ConcurrentHashMap<>();
		_things = new ConcurrentHashMap<>();
		_observers = new HashSet<>();
	}

	public void RegisterType(ThingType type) {
		RegisterType(type, true);
	}
	
	public void RegisterType(ThingType type, boolean force) {
		if (type == null) {
			throw new RuntimeException("The ThingType object should not be null");
		}
	
		String name = type.getName();
		if (force || !_thingTypes.containsKey(name)) {
			_thingTypes.put(name, type);
			
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
		boolean creation = !_things.containsKey(id);
		_things.put(id, thing);
		
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
		
		// Remove all connections
		for(Thing t : _things.values()) {
			if (t.IsConnectedTo(thing)) {
				t.Disconnect(thing);
				NotifyThingUpdate(t);
			}
		}
	
		String id = thing.getId();
		if (_things.containsKey(id)) {
			_things.remove(id);
			NotifyThingDeleted(thing);
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
		return _things.get(id);
	}
	
	public ThingType getThingType(String name) {
		return _thingTypes.get(name);
	}
	
	public List<Thing> getThings() {
		return new ArrayList<>(_things.values());
	}
}
