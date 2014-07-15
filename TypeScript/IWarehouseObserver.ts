module ThingModel {
	export interface IWarehouseObserver {
		New(thing: Thing, sender: string): void;
		Deleted(thing: Thing, sender: string): void;
		Updated(thing: Thing, sender: string): void;
		Define(thing: ThingType, sender: string): void;
	}
}
