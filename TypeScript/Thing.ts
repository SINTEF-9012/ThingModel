/// <reference path="./ThingType.ts" />
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
		private _properties : {[key: string] : Property };

		/**
		 *	A thing can have a list of connected things.
		 *
		 *	But a thing cannot be connected directly to itself.
		 */
		private _connections : {[key: string] : Thing };

		public constructor(id : string, type : ThingType = null) {
			if (!id) {
				throw "The thing ID should not be null or empty";
			}

			this._id = id;
			this._type = type;

			this._properties = {};
			this._connections = {};
		}

		public SetProperty(property : Property) : void {
			this._properties[property.key] = property;
		}

		public HasProperty(key : string) : boolean {
			return this.
		}

		public GetProperty<T extends Property>(key : string) : Property {
			return this._properties[key];
		}

		public Connect(thing : Thing) : void {
			if (thing === this || thing._id === this._id) {
				throw "You can't connect a thing directly to itself";
			}
			this._connections[thing._id] = thing;
		}

		public Disconnect(thing : Thing) : void {
			delete this._connections[thing._id];
		}

		public DisconnectAll() : void {
			this._connections = {};
		}

		public IsConnectedTo(thing : Thing) : boolean {
			return thing._id in this._connections;
		}

	}
}