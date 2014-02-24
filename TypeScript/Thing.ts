/// <reference path="./bower_components/DefinitelyTyped/underscore/underscore.d.ts" />

module ThingModel {
	export class Thing {

		/**
		 *	String ID, unique in the world.
		 *
		 *	It can't be null.
		 *	Two things objets with the same ID represent the same thing.
		 */
		private _id : string;

		public get ID() : string {
			return this._id;
		}


		/**
		 *	Reference to the object type.
		 *
		 *	The type is null by default, and the default type is applied.
		 */
		private _type : ThingType = null;

		public get Type() : ThingType {
			return this._type;
		}

		/**
		 *	A thing contains a list of properties.
		 *
		 *	A property is defined by his key and his value.
		 */
		private _properties: { [key: string]: Property };
		private _propertiesCount: number;

		/**
		 *	A thing can have a list of connected things.
		 *
		 *	But a thing cannot be connected directly to itself.
		 */
		private _connections: { [key: string]: Thing };
		private _connectionsCount: number;

		constructor(id : string, type : ThingType = null) {
			if (!id) {
				throw new Error("The thing ID should not be null or empty");
			}

			this._id = id;
			this._type = type;

			this._properties = {};
			this._propertiesCount = 0;
			this._connections = {};
			this._connectionsCount = 0;
		}

		public SetProperty(property: Property): void {
			if (!this.HasProperty(property.Key)) {
				++this._propertiesCount;
			}
			this._properties[property.Key] = property;
		}

		public HasProperty(key : string) : boolean {
			return _.has(this._properties, key);
		}

		public GetProperty<T extends Property>(key: string, type?: Type): T {

			var prop = this._properties[key];
			if (!prop || (type && prop.Type != type)) {
				return null;
			}

			return prop;
		}

		public GetString(key: string) {

			var prop = this.GetProperty<ThingModel.Property.String>(key, Type.String);

			if (!prop) {
				return null;
			}

			return prop.Value;
		}

		public Connect(thing: Thing): void {
			if (!thing) return;

			if (thing === this || thing._id === this._id) {
				throw new Error("You can't connect a thing directly to itself");
			}
			if (!this.IsConnectedTo(thing)) {
				++this._connectionsCount;
			}
			this._connections[thing._id] = thing;
		}

		public Disconnect(thing: Thing): boolean {
			if (this.IsConnectedTo(thing)) {
				--this._connectionsCount;	
				delete this._connections[thing._id];
				return true;
			}
			return false;
		}

		public DisconnectAll() : void {
			this._connections = {};
			this._connectionsCount = 0;
		}

		public IsConnectedTo(thing: Thing): boolean {
			// !!thing in order will convert the assertion to a boolean
			return !!thing && _.has(this._connections, thing._id);
		}

		public get ConnectedThings(): Thing[] {
			return _.values(this._connections);
		}

		public get ConnectedThingsCount(): number {
			return this._connectionsCount;
		}

		public get Properties(): Property[] {
			return _.values(this._properties);
		}

		public Compare(other: Thing, compareId: boolean = true,
			deepComparisonForConnectedThings: boolean = false): boolean {
			// Optimization, when two things are the same instance
			if (this === other) {
				return true;
			}

			// If the types are not the same
			if (!other || (this._type != null && other._type != null &&
				this._type.Name != other._type.Name) ||
				(this._type == null && other._type != null) ||
				(this._type != null && other._type == null)) {
				return false;
			}

			// If we need to compare the ids, and they are not the same
			if (compareId && this._id != other._id) {
				return false;
			}

			// Check if the connections are the same
			if (this._connectionsCount !== other._connectionsCount || 
				_.any(this._connections, (connectedThing:Thing)=> {
					return !_.has(other._connections, connectedThing._id);
				})) {
				return false;
			}

			// Check if the properties are the sames
			if (this._propertiesCount !== other._propertiesCount ||
				_.any(this._properties, (property: Property) => {
					var otherProp = other._properties[property.Key];

					return otherProp == null || !otherProp.CompareValue(property);
				})) {
				return false;
			}

			if (deepComparisonForConnectedThings) {
				return this.RecursiveCompare(other, {});
			}

			return true;
		}

		private RecursiveCompare(other: Thing,
			alreadyVisitedObjets: { [id: string]: boolean }): boolean {

			// If the thing was already checked,
			// we don't need to check it again
			if (_.has(alreadyVisitedObjets, this._id)) {

				// It's true because we are still looking for a difference
				return true;
			}

			// Made a simple comparison first
			if (!this.Compare(other)) {
				return false;
			}

			// Register the thing now, prevent infinite recursions
			alreadyVisitedObjets[this._id] = true;

			return !_.any(this._connections, (connectedThing: Thing)=> {
				var otherThing = other._connections[connectedThing._id];

				return !connectedThing.RecursiveCompare(otherThing, alreadyVisitedObjets);
			});
		}

	}	
}