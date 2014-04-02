module ThingModel {
	export class Warehouse {
		private _thingTypes: { [key: string]: ThingType };
		private _things: { [key: string]: Thing };
		private _observers: IWarehouseObserver[];
		
		constructor() {
			this._thingTypes = {};
			this._things = {};
			this._observers = [];
		}

		public RegisterType(type: ThingType, force: boolean = true) : void {
			if (!type) {
				throw new Error("The thing type information is null.");
			}

			if (force || !_.has(this._thingTypes, type.Name)) {
				this._thingTypes[type.Name] = type;

				this.NotifyThingTypeDefine(type);
			}
		}

		public RegisterThing(thing: Thing, alsoRegisterConnections: boolean = true,
			alsoRegisterTypes: boolean = true): void {
			if (!thing) {
				throw new Error("A thing should not be null if it want to be allowed in the warehouse");
			}

			var creation = !_.has(this._things, thing.ID);
			this._things[thing.ID] = thing;

			if (alsoRegisterTypes && thing.Type) {
				this.RegisterType(thing.Type, false);
			}

			if (alsoRegisterConnections) {
				var alreadyVisitedObjects: {[id:string]:boolean} = {};
				_.each(thing.ConnectedThings, (connectedThing) => {
					this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects);
				});
			}

			if (creation) {
				this.NotifyThingCreation(thing);
			} else {
				this.NotifyThingUpdate(thing);
			}
		}

		private RecursiveRegisterThing(thing: Thing, alsoRegisterTypes: boolean,
			alreadyVisitedObjects: { [id: string]: boolean }): void {
		
			// Avoid infinite recursions	
			if (alreadyVisitedObjects.hasOwnProperty(thing.ID)) {
				return;
			}

			alreadyVisitedObjects[thing.ID] = true;

			this.RegisterThing(thing, false, alsoRegisterTypes);

			_.each(thing.ConnectedThings, (connectedThing)=> {
				this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects);
			});
		}

		public RegisterCollection(collection: Thing[], alsoRegisterTypes: boolean=false): void {
			var alreadyVisitedObjects: { [id: string]: boolean } = {};
			_.each(collection, (thing) => {
				this.RecursiveRegisterThing(thing, alsoRegisterTypes, alreadyVisitedObjects);
			});
		}

		public RemoveCollection(collection: { [id: string]: Thing }) {
			var thingsToDisconnect: { [id: string]: Thing } = {};

			_.each(_.keys(collection), (id: string) => {
				var thing = collection[id];
				this.RemoveThing(thing, false);

				_.each(this._things, (t : Thing) => {
					if (t.IsConnectedTo(thing)) {
						thingsToDisconnect[t.ID] = t;
					}
				});
			});

			_.each(_.keys(thingsToDisconnect), (id: string) => {
				if (!collection.hasOwnProperty(id)) {
					this.NotifyThingUpdate(thingsToDisconnect[id]);
				}
			});
		}

		public RemoveThing(thing: Thing, notifyUpdates = true): void{
			if (!thing) {
				return;
			}

			// Remove all connections
			_.each(this._things, (t : Thing) => {
				if (t.IsConnectedTo(thing)) {
					t.Disconnect(thing);
					if (notifyUpdates) {
						this.NotifyThingUpdate(t);
					}
				}
			});

			if (_.has(this._things, thing.ID)) {
				delete this._things[thing.ID];
				this.NotifyThingDeleted(thing);
			}
		}

		public RegisterObserver(observer: IWarehouseObserver): void {
			this._observers.push(observer);
		}

		public UnregisterObserver(observer: IWarehouseObserver): void {
			// Array remove
			this._observers.splice(_.indexOf(this._observers, observer), 1);
		}

		public NotifyThingTypeDefine(type: ThingType): void {
			_.each(this._observers, (observer) => {
				observer.Define(type);
			});
		}

		public NotifyThingUpdate(thing: Thing): void {
			_.each(this._observers, (observer) => {
				observer.Updated(thing);
			});
		}

		public NotifyThingCreation(thing: Thing): void {
			_.each(this._observers, (observer) => {
				observer.New(thing);
			});
		}

		public NotifyThingDeleted(thing: Thing): void {
			_.each(this._observers, (observer) => {
				observer.Deleted(thing);
			});
		}

		public GetThing(id: string): Thing {
			return this._things[id];
		}

		public GetThingType(name: string): ThingType {
			return this._thingTypes[name];
		}

		public get Things() : Thing[] {
			return _.values(this._things);
		}

		public get ThingsTypes(): ThingType[]{
			return _.values(this._thingTypes);
		}
	}	
} 