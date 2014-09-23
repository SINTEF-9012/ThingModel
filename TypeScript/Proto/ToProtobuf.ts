module ThingModel.Proto {

	export class ToProtobuf {
		// Current transaction object
		// it will be filled step by step
		private _transaction: Transaction;

		// String declaration dictionary
		public StringDeclarations: { [value: string]: number };
		public StringDeclarationsCpt: number;

		// String to declares
		private _stringToDeclare: { [value: string]: boolean };

		private _thingsState: { [id: number]: Thing };
		private _propertiesState: { [key: string]: Property };

		constructor() {
			this.StringDeclarations = {};
			this.StringDeclarationsCpt = 0;
			this._stringToDeclare = {};
			this._thingsState = {};
			this._propertiesState = {};
		}

		private StringToKey(value: string): number {
			if (!value) {
				return 0;
			}

			var key = this.StringDeclarations[value];

			if (key) {
				return key;
			}

			key = ++this.StringDeclarationsCpt;
			this.StringDeclarations[value] = key;

			var stringDeclaration = new ProtoTools.Builder.StringDeclaration();

			stringDeclaration.setKey(key);
			stringDeclaration.setValue(value);

			this._transaction.string_declarations.push(stringDeclaration);

			return key;
		}

		public Convert(publish: ThingModel.Thing[],
			deletions: ThingModel.Thing[],
			declarations: ThingModel.ThingType[],
			senderID: string): Transaction {

			this._transaction = new ProtoTools.Builder.Transaction();

			this._transaction.setStringSenderId(this.StringToKey(senderID));

			_.each(publish, (thing) => {
				this.ConvertThing(thing);
			});

			this.ConvertDeleteList(deletions);
			this.ConvertDeclarationList(declarations);

			return this._transaction;

		}

		public ConvertTransaction(transaction: Transaction): ArrayBuffer {
			return transaction.toArrayBuffer();
		}

		private ConvertThing(thing: ThingModel.Thing): void {
			var change = false;

			var thingID = this.StringToKey(thing.ID);

			var publication = new ProtoTools.Builder.Thing();
			publication.setStringId(thingID);
			publication.setStringTypeName(thing.Type != null ?
				this.StringToKey(thing.Type.Name) : 0);
			publication.setConnectionsChange(false);

			var previousThing = this._thingsState[thingID];

			if (previousThing == null || previousThing.getStringTypeName()
				!= publication.getStringTypeName()) {
				change = true;
			}

			var connectedThingsCache: ThingModel.Thing[] = null;

			if ((previousThing == null && thing.ConnectedThingsCount > 0)
				|| (previousThing != null && previousThing.getConnections().length !=
					thing.ConnectedThingsCount)) {
				publication.setConnectionsChange(true);
			} else {
				connectedThingsCache = thing.ConnectedThings;

				_.any(connectedThingsCache, (connectedThing: ThingModel.Thing) => {
					var connectionKey = this.StringToKey(connectedThing.ID);

					if (previousThing == null || !_.contains(previousThing.getConnections(), connectionKey)) {
						publication.setConnectionsChange(true);
						return true;
					}
					return false;
				});
			}

			if (publication.getConnectionsChange()) {
				change = true;

				if (!connectedThingsCache) {
					connectedThingsCache = thing.ConnectedThings;
				}

				_.each(connectedThingsCache, (connectedThing: ThingModel.Thing) => {
					var connectionKey = this.StringToKey(connectedThing.ID);
					publication.connections.push(connectionKey);
				});
			}

			_.each(thing.Properties, (property: ThingModel.Property) => {
				var propertyId = this.StringToKey(property.Key);

				var proto = new ProtoTools.Builder.Property();
				proto.setStringKey(propertyId);

				switch (property.Type) {
				case Type.LocationLatLng:
				case Type.LocationPoint:
				case Type.LocationEquatorial:
					this.ConvertLocationProperty(property, proto);
					break;
				case Type.String:
					this.ConvertStringProperty(
						<ThingModel.Property.String> property, proto);
					break;
				case Type.Double:
					proto.setType(Property.Type.DOUBLE);
					proto.setDoubleValue(
					(<ThingModel.Property.Double> property).Value);
					break;
				case Type.Int:
					proto.setType(Property.Type.INT);
					proto.setIntValue(
					(<ThingModel.Property.Int> property).Value);
					break;
				case Type.Boolean:
					proto.setType(Property.Type.BOOLEAN);
					proto.setBooleanValue(
					(<ThingModel.Property.Boolean> property).Value);
					break;
				case Type.DateTime:
					proto.setType(Property.Type.DATETIME);
					proto.setDatetimeValue(
					(<ThingModel.Property.DateTime> property).Value.getTime());
					break;
				}

				// We are in a JavaScript world :-)
				var propertyStateKey = thingID + ":" + propertyId;

				if (previousThing != null) {
					var previousProto = this._propertiesState[propertyStateKey];

					// Terrible part :)
					if (previousProto != null &&
						previousProto.getType() == proto.getType() &&
						previousProto.getBooleanValue() == proto.getBooleanValue() &&
						previousProto.getDatetimeValue() == proto.getDatetimeValue() &&
						previousProto.getDoubleValue() == proto.getDoubleValue() &&
						previousProto.getIntValue() == proto.getIntValue()) {
						var previousLoc = previousProto.getLocationValue();
						var loc = proto.getLocationValue();

						if ((previousLoc == null && loc == null) ||
						(previousLoc != null && loc != null &&
							previousLoc.x == loc.x &&
							previousLoc.y == loc.y &&
							previousLoc.z == loc.z &&
							previousLoc.z_null == loc.z_null &&
							previousLoc.getStringSystem() == loc.getStringSystem())) {
							var previousStr = previousProto.getStringValue();
							var str = proto.getStringValue();

							if ((previousStr == null && str == null) ||
							(previousStr != null && str != null &&
							((previousStr.getStringValue() == str.getStringValue() &&
								previousStr.getValue() == str.getValue()) ||
							(property.ValueToString() == previousStr.getValue())))) {
								return;
							}
						}
					}
				}

				change = true;
				publication.properties.push(proto);
				this._propertiesState[propertyStateKey] = proto;
			});

			if (change) {
				this._transaction.things_publish_list.push(publication);
				this._thingsState[thingID] = publication;
			}
		}

		private ConvertDeleteList(list: ThingModel.Thing[]): void {
			_.each(list, (thing: ThingModel.Thing) => {
				var key = this.StringToKey(thing.ID);
				this._transaction.things_remove_list.push(key);

				this.ManageThingSuppression(key);
			});
		}

		private ConvertDeclarationList(list: ThingModel.ThingType[]): void {
			_.each(list, (thingType: ThingModel.ThingType) => {
				var declaration = new ProtoTools.Builder.ThingType();

				declaration.setStringName(this.StringToKey(thingType.Name));
				declaration.setStringDescription(this.StringToKey(thingType.Description));

				_.each(thingType.Properties, (propertyType: ThingModel.PropertyType) => {
					var prop = new ProtoTools.Builder.PropertyType();
					prop.setStringKey(this.StringToKey(propertyType.Key));
					prop.setStringName(this.StringToKey(propertyType.Name));
					prop.setStringDescription(this.StringToKey(propertyType.Description));
					prop.setRequired(propertyType.Required);

					switch (propertyType.Type) {
					case Type.LocationLatLng:
					case Type.LocationEquatorial:
					case Type.LocationPoint:
					case Type.Location:
						prop.setType(PropertyType.Type.LOCATION);
						break;
					case Type.String:
						prop.setType(PropertyType.Type.STRING);
						break;
					case Type.Double:
						prop.setType(PropertyType.Type.DOUBLE);
						break;
					case Type.Int:
						prop.setType(PropertyType.Type.INT);
						break;
					case Type.Boolean:
						prop.setType(PropertyType.Type.BOOLEAN);
						break;
					case Type.DateTime:
						prop.setType(PropertyType.Type.DATETIME);
						break;
					}

					declaration.properties.push(prop);
				});

				this._transaction.thingtypes_declaration_list.push(declaration);
			});
		}

		private ConvertLocationProperty(property: ThingModel.Property,
			proto: Property) {

			var value = (<any>property).Value;

			switch (value.type) {
			case "latlng":
				proto.setType(Property.Type.LOCATION_LATLNG);
				break;
			case "equatorial":
				proto.setType(Property.Type.LOCATION_EQUATORIAL);
				break;
			case "point":
			default:
				proto.setType(Property.Type.LOCATION_POINT);
				break;

			}

			var loc = new ProtoTools.Builder.Property.Location();

			loc.setX(value.X);
			loc.setY(value.Y);
			loc.setStringSystem(this.StringToKey(value.System));
			loc.setZNull(value.Z == null);

			if (value.Z != null) {
				loc.setZ(value.Z);
			}

			proto.setLocationValue(loc);
		}

		private ConvertStringProperty(property: ThingModel.Property.String,
			proto: Property) {
			var value = property.Value;
			proto.setType(Property.Type.STRING);

			var st = new ProtoTools.Builder.Property.String();
			if (value && this._stringToDeclare[value]) {
				st.setStringValue(this.StringToKey(value));
			} else {
				st.setValue(value);

				// Use a string decleration next time
				this._stringToDeclare[value] = true;
			}

			proto.setStringValue(st);
		}

		private ManageThingSuppression(thingId: number): void {
			delete this._thingsState[thingId];

			var stringId = thingId + ":";

			_.each(this._propertiesState, (value, key) => {
				if (key.indexOf(stringId) === 0) {
					delete this._propertiesState[key];
				}
			});
		}

		public ApplyThingsSuppressions(things: ThingModel.Thing[]): void {
			_.each(things, (thing: ThingModel.Thing) => {
				var key = this.StringDeclarations[thing.ID];

				if (key) {
					this.ManageThingSuppression(key);
				}
			});
		}
	}

}
