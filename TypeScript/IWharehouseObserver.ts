 module ThingModel {
	 export interface IWharehouseObserver {
		 New(thing: Thing): void;
		 Deleted(thing: Thing): void;
		 Updated(thing: Thing): void;
		 Define(thing:ThingType): void;
	 }
 }