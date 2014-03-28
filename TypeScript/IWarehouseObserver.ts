 module ThingModel {
	 export interface IWarehouseObserver {
		 New(thing: Thing): void;
		 Deleted(thing: Thing): void;
		 Updated(thing: Thing): void;
		 Define(thing:ThingType): void;
	 }
 }