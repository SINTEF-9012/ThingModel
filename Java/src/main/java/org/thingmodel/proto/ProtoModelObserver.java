package org.thingmodel.proto;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

import org.thingmodel.IWharehouseObserver;
import org.thingmodel.Thing;
import org.thingmodel.ThingType;
import org.thingmodel.proto.ProtoTransaction.Transaction;

public class ProtoModelObserver implements IWharehouseObserver {
	
	private Object _lock = new Object();
	public HashSet<Thing> Updates = new HashSet<>();
	public HashSet<Thing> Deletions = new HashSet<>();
	public HashSet<ThingType> Definitions = new HashSet<>();

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
			Updates.add(thing);
		}
	}

	@Override
	public void Deleted(Thing thing) {
		synchronized (_lock) {
			Deletions.add(thing);
		}
	}

	@Override
	public void Updated(Thing thing) {
		synchronized (_lock) {
			Updates.add(thing);
		}
	}

	@Override
	public void Define(ThingType thingType) {
		synchronized (_lock) {
			Definitions.add(thingType);
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
			copyUpdates = new ArrayList<>(Updates);
			copyDeletions = new ArrayList<>(Deletions);
			copyDefinitions = new ArrayList<>(Definitions);
		}
		
		return toProtobuf.Convert(copyUpdates, copyDeletions, copyDefinitions, senderID);
	}
}
