module ThingModel {
	export class Wharehouse {
		private _thingTypes: { [key: string]: ThingType };
		private _things: { [key: string]: Thing };
		private _observers: IWharehouseObserver[];
		
		constructor() {
			this._thingTypes = {};
			this._things = {};
			this._observers = [];
		}

		public RegisterType(type: ThingType, force: boolean = true) : void {
			if (!type) {
				throw "The thing type information is missing.";
			}

			if (force || !_.has(this._thingTypes, type.Name)) {
				this._thingTypes[type.Name] = type;

				this.NotifyThingTypeDefine(type);
			}
		}

		public RegisterThing(thing: Thing, alsoRegisterConnections: boolean = true,
			alsoRegisterTypes: boolean = false): void {
			if (!thing) {
				throw "A thing should be something to be allowed in the wharehouse";
			}

			var creation = !_.has(this._things, thing.ID);
			this._things[thing.ID] = thing;

			if (alsoRegisterTypes) {
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

		public RegisterObserver(observer: IWharehouseObserver): void {
			this._observers.push(observer);
		}

		public UnregisterObserver(observer: IWharehouseObserver): void {
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

		public GetThing(key: string): Thing {
			return this._things[key];
		}

		public GetThingType(key: string): ThingType {
			return this._thingTypes[key];
		}

		public get Things() : Thing[] {
			return _.values(this._things);
		}
	}	
} 