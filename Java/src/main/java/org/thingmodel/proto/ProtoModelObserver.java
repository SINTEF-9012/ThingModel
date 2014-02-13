package org.thingmodel.proto;

import org.thingmodel.IWharehouseObserver;
import org.thingmodel.Thing;
import org.thingmodel.ThingType;
import org.thingmodel.proto.ProtoTransaction.Transaction;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class ProtoModelObserver implements IWharehouseObserver {
	
	private final Object _lock = new Object();
	public HashMap<String,Thing> Updates = new HashMap<>();
	public HashMap<String,Thing> Deletions = new HashMap<>();
	public HashMap<String,ThingType> Definitions = new HashMap<>();

	public void Reset() {
		synchronized (_lock) {
			Updates.clear();
			Deletions.clear();
			Definitions.clear();
		}
	}
	
	@Override
	public void New(Thing thing) {
		synchronized (_lock) {
			Updates.put(thing.getId(), thing);
		}
	}

	@Override
	public void Deleted(Thing thing) {
		synchronized (_lock) {
            Updates.remove(thing.getId());
            // We delete it even it's a new thing because this case is not often
            // and it's more efficient to use only one HashSet for updates and creations
			Deletions.put(thing.getId(), thing);
		}
	}

	@Override
	public void Updated(Thing thing) {
		synchronized (_lock) {
			Updates.put(thing.getId(), thing);
		}
	}

	@Override
	public void Define(ThingType thingType) {
		synchronized (_lock) {
			Definitions.put(thingType.getName(), thingType);
		}
	}

	public boolean somethingChanged() {
		synchronized (_lock) {
			return !Updates.isEmpty() || !Definitions.isEmpty() || !Deletions.isEmpty();
		}
	}
	
	public Transaction getTransaction(ToProtobuf toProtobuf, String senderID) {
		List<Thing> copyUpdates;
		List<Thing> copyDeletions;
		List<ThingType> copyDefinitions;
		
		synchronized (_lock) {
			copyUpdates = new ArrayList<>(Updates.values());
			copyDeletions = new ArrayList<>(Deletions.values());
			copyDefinitions = new ArrayList<>(Definitions.values());
		}
		
		return toProtobuf.Convert(copyUpdates, copyDeletions, copyDefinitions, senderID);
	}
}
