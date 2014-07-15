module ThingModel.Proto {
	export class ProtoModelObserver implements IWarehouseObserver {

		public Updates: { [id: string]: ThingModel.Thing } = {};
		public Deletions: { [id: string]: ThingModel.Thing } = {};
		public Definitions: { [name: string]: ThingModel.ThingType } = {};
		public PermanentDefinitions: { [name: string]: ThingModel.ThingType } = {};
		private somethingChanged = false;

		public Reset(): void {
			this.Updates = {};
			this.Deletions = {};
			this.Definitions = {};
			this.somethingChanged = true;
		}

		public New(thing: ThingModel.Thing): void {
			this.Updates[thing.ID] = thing;
			this.somethingChanged = true;
		}

		public Deleted(thing: ThingModel.Thing): void {
			delete this.Updates[thing.ID];
			this.Deletions[thing.ID] = thing;
			this.somethingChanged = true;
		}

		public Updated(thing: ThingModel.Thing): void {
			this.Updates[thing.ID] = thing;
			this.somethingChanged = true;
		}

		public Define(thingType: ThingModel.ThingType): void {
			this.Definitions[thingType.Name] = thingType;
			this.PermanentDefinitions[thingType.Name] = thingType;
			this.somethingChanged = true;
		}

		public get SomethingChanged(): boolean {
			return this.somethingChanged;
		}

		public GetTransaction(toProtobuf: ToProtobuf, senderID: string,
			allDefinitions: boolean = false): Transaction {
			return toProtobuf.Convert(
				_.values(this.Updates),
				_.values(this.Deletions),
				_.values(allDefinitions ? this.PermanentDefinitions : this.Definitions),
				senderID);
		}
	}
}