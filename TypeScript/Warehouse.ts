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

		public RegisterType(type: ThingType, force: boolean = true, sender:string=null) : void {
			if (!type) {
				throw new Error("The thing type information is null.");
			}

			if (force || !_.has(this._thingTypes, type.Name)) {
				this._thingTypes[type.Name] = type;

				this.NotifyThingTypeDefine(type, sender);
			}
		}

		public RegisterThing(thing: Thing, alsoRegisterConnections: boolean = true,
			alsoRegisterTypes: boolean = true, sender:string=null): void {
			if (!thing) {
				throw new Error("A thing should not be null if it want to be allowed in the warehouse");
			}

			var creation = !_.has(this._things, thing.ID);
			this._things[thing.ID] = thing;

			if (alsoRegisterTypes && thing.Type) {
				this.RegisterType(thing.Type, false, sender);
			}

			if (alsoRegisterConnections) {
				var alreadyVisitedObjects: {[id:string]:boolean} = {};
				_.each(thing.ConnectedThings, (connectedThing) => {
					this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects,sender);
				});
			}

			if (creation) {
				this.NotifyThingCreation(thing, sender);
			} else {
				this.NotifyThingUpdate(thing, sender);
			}
		}

		private RecursiveRegisterThing(thing: Thing, alsoRegisterTypes: boolean,
			alreadyVisitedObjects: { [id: string]: boolean }, sender:string): void {
		
			// Avoid infinite recursions	
			if (alreadyVisitedObjects.hasOwnProperty(thing.ID)) {
				return;
			}

			alreadyVisitedObjects[thing.ID] = true;

			this.RegisterThing(thing, false, alsoRegisterTypes, sender);

			_.each(thing.ConnectedThings, (connectedThing)=> {
				this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects, sender);
			});
		}

		public RegisterCollection(collection: Thing[], alsoRegisterTypes: boolean=true, sender:string=null): void {
			var alreadyVisitedObjects: { [id: string]: boolean } = {};
			_.each(collection, (thing) => {
				this.RecursiveRegisterThing(thing, alsoRegisterTypes, alreadyVisitedObjects, sender);
			});
		}

		public RemoveCollection(collection: { [id: string]: Thing }, sender:string=null) {
			var thingsToDisconnect: { [id: string]: Thing } = {};

			_.each(_.keys(collection), (id: string) => {
				var thing = collection[id];
				this.RemoveThing(thing, false, sender);

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

		public RemoveThing(thing: Thing, notifyUpdates = true, sender:string=null): void{
			if (!thing) {
				return;
			}

			// Remove all connections
			_.each(this._things, (t : Thing) => {
				if (t.IsConnectedTo(thing)) {
					t.Disconnect(thing);
					if (notifyUpdates) {
						this.NotifyThingUpdate(t, sender);
					}
				}
			});

			if (_.has(this._things, thing.ID)) {
				delete this._things[thing.ID];
				this.NotifyThingDeleted(thing, sender);
			}
		}

		public RegisterObserver(observer: IWarehouseObserver): void {
			this._observers.push(observer);
		}

		public UnregisterObserver(observer: IWarehouseObserver): void {
			// Array remove
			this._observers.splice(_.indexOf(this._observers, observer), 1);
		}

		public NotifyThingTypeDefine(type: ThingType, sender:string=null): void {
			_.each(this._observers, (observer) => {
				observer.Define(type, sender);
			});
		}

		public NotifyThingUpdate(thing: Thing, sender:string=null): void {
			_.each(this._observers, (observer) => {
				observer.Updated(thing, sender);
			});
		}

		public NotifyThingCreation(thing: Thing, sender:string=null): void {
			_.each(this._observers, (observer) => {
				observer.New(thing, sender);
			});
		}

		public NotifyThingDeleted(thing: Thing, sender: string= null): void {
			_.each(this._observers, (observer) => {
				observer.Deleted(thing, sender);
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