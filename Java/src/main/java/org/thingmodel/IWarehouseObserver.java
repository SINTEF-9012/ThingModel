package org.thingmodel;

public interface IWarehouseObserver {
	void New(Thing thing, String sender);
	void Deleted(Thing thing, String sender);
	void Updated(Thing thing, String sender);
	void Define(ThingType thingType, String sender);
}
