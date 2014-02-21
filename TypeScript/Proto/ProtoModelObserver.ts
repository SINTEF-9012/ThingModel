 module ThingModel.Proto {
	export class ProtoModelObserver implements IWharehouseObserver {
		 
		public New(thing: ThingModel.Thing): void {
		}

		public Deleted(thing: ThingModel.Thing): void {
		}

		public Updated(thing: ThingModel.Thing): void {
		}	

		public Define(thingType: ThingModel.ThingType): void {
		}
	}
 }