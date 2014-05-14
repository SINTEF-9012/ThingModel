package org.thingmodel;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;

public class Warehouse {

	private final HashMap<String, ThingType> _thingTypes = new HashMap<>();
	private final HashMap<String, Thing> _things = new HashMap<>();
	private final HashSet<IWarehouseObserver> _observers = new HashSet<>();

	private final Object _lockThingTypes = new Object();
	private final Object _lockThings = new Object();
	private final Object _lockObservers = new Object();

	public void RegisterType(ThingType type) {
		RegisterType(type, true, null);
	}

	public void RegisterType(ThingType type, boolean force, String sender) {
		if (type == null) {
			throw new RuntimeException(
					"The ThingType object should not be null");
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

			NotifyThingTypeDefine(type, sender);
		}
	}

	public void RegisterThing(Thing thing) {
		RegisterThing(thing, true, false, null);
	}

	public void RegisterThing(Thing thing, boolean alsoRegisterConnections,
			boolean alsoRegisterTypes, String sender) {
		if (thing == null) {
			throw new RuntimeException(
					"Null things are not allowed in the warehouse.");
		}

		String id = thing.getId();
		boolean creation;

		synchronized (_lockThings) {
			creation = !_things.containsKey(id);
			_things.put(id, thing);
		}

		if (alsoRegisterTypes && thing.getType() != null) {
			RegisterType(thing.getType(), false, sender);
		}

		if (alsoRegisterConnections) {
			HashSet<String> alreadyVisitedThings = new HashSet<>();

			for (Thing connectedThing : thing.getConnectedThings()) {
				RecursiveRegisterThing(connectedThing, alsoRegisterTypes,
						alreadyVisitedThings, sender);
			}
		}

		if (creation) {
			NotifyThingCreation(thing, sender);
		} else {
			NotifyThingUpdate(thing, sender);
		}

	}

	private void RecursiveRegisterThing(Thing thing, boolean alsoRegisterTypes,
			HashSet<String> alreadyVisitedThings, String sender) {

		String id = thing.getId();

		// Avoid infinite recursions
		if (alreadyVisitedThings.contains(id)) {
			return;
		}

		alreadyVisitedThings.add(id);

		RegisterThing(thing, false, alsoRegisterTypes, sender);

		for (Thing connectedThing : thing.getConnectedThings()) {
			RecursiveRegisterThing(connectedThing, alsoRegisterTypes,
					alreadyVisitedThings, sender);
		}

	}

	public void RemoveCollection(Collection<Thing> collection) {
		RemoveCollection(collection, null);
	}

	public void RemoveCollection(Collection<Thing> collection, String sender) {
		HashSet<Thing> thingsToDisconnect = new HashSet<>();

		for (Thing thing : collection) {
			RemoveThing(thing, false, sender);

			synchronized (_lockThings) {
				for (Thing t : _things.values()) {
					if (t.IsConnectedTo(thing)) {
						thingsToDisconnect.add(t);
					}
				}
			}
		}

		for (Thing tt : thingsToDisconnect) {
			if (!collection.contains(tt)) {
				NotifyThingUpdate(tt, sender);
			}
		}
	}

	public void RegisterCollection(Collection<Thing> collection) {
		RegisterCollection(collection, false, null);
	}

	public void RegisterCollection(Collection<Thing> collection,
			boolean alsoRegisterTypes, String sender) {
		HashSet<String> alreadyVisitedThings = new HashSet<>();
		for (Thing thing : collection) {
			this.RecursiveRegisterThing(thing, alsoRegisterTypes,
					alreadyVisitedThings, sender);
		}
	}

	public void RemoveThing(Thing thing) {
		RemoveThing(thing, true, null);
	}

	public void RemoveThing(Thing thing, boolean notifyUpdates, String sender) {
		if (thing == null) {
			return;
		}

		ArrayList<Thing> thingsToDisconnect = new ArrayList<>();

		// Remove all connections
		synchronized (_lockThings) {
			for (Thing t : _things.values()) {
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
			NotifyThingDeleted(thing, sender);
		}

		if (notifyUpdates) {
			for (Thing t : thingsToDisconnect) {
				t.Disconnect(thing);
				NotifyThingUpdate(t, sender);
			}
		} else {
			for (Thing t : thingsToDisconnect) {
				t.Disconnect(thing);
			}
		}

	}

	public void RegisterObserver(IWarehouseObserver observer) {
		synchronized (_lockObservers) {
			_observers.add(observer);
		}
	}

	public void UnregisterObserver(IWarehouseObserver observer) {
		synchronized (_lockObservers) {
			_observers.remove(observer);
		}
	}

	public void NotifyThingTypeDefine(ThingType type) {
		NotifyThingTypeDefine(type, null);
	}
	
	public void NotifyThingTypeDefine(ThingType type, String sender) {
		synchronized (_lockObservers) {
			for (IWarehouseObserver observer : _observers) {
				try {
					observer.Define(type, sender);
				} catch (Exception e) {
					System.err.println(e.getLocalizedMessage());
				}
			}
		}
	}

	public void NotifyThingCreation(Thing thing) {
		NotifyThingCreation(thing, null);
	}
	
	public void NotifyThingCreation(Thing thing, String sender) {
		synchronized (_lockObservers) {
			for (IWarehouseObserver observer : _observers) {
				try {
					observer.New(thing, sender);
				} catch (Exception e) {
					System.err.println(e.getLocalizedMessage());
				}
			}
		}
	}

	public void NotifyThingUpdate(Thing thing) {
		NotifyThingUpdate(thing, null);
	}
	
	public void NotifyThingUpdate(Thing thing, String sender) {
		synchronized (_lockObservers) {
			for (IWarehouseObserver observer : _observers) {
				try {
					observer.Updated(thing, sender);
				} catch (Exception e) {
					System.err.println(e.getLocalizedMessage());
				}
			}
		}
	}

	public void NotifyThingDeleted(Thing thing) {
		NotifyThingDeleted(thing, null);
	}
	
	public void NotifyThingDeleted(Thing thing, String sender) {
		
		synchronized (_lockObservers) {
			for (IWarehouseObserver observer : _observers) {
				try {
					observer.Deleted(thing, sender);
				} catch (Exception e) {
					System.err.println(e.getLocalizedMessage());
				}
			}
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

	public List<ThingType> getThingTypes() {
		synchronized (_lockThingTypes) {
			return new ArrayList<>(_thingTypes.values());
		}
	}
}
