module ThingModel.Proto {

	export class FromProtobuf {
		private _warehouse: Warehouse;

		public StringDeclarations: { [key: number]: string };

		constructor(warehouse: Warehouse) {
			this._warehouse = warehouse;
			this.StringDeclarations = {};
		}

		private KeyToString(key: number): string {
			if (key == 0) {
				return "";
			}

			var value = this.StringDeclarations[key];

			return typeof value === "undefined" ? "undefined" : value;
		}

		public Convert(data: ArrayBuffer, check: boolean = false): string {
			var transaction = ProtoTools.Builder.Transaction.decode(data);

			return this.ConvertTransaction(transaction, check);
		}

		public ConvertTransaction(transaction: Transaction, check: boolean): string {

			// Convert the string declarations
			_.each(transaction.string_declarations, (d: StringDeclaration) => {
				this.StringDeclarations[d.key] = d.value;
			});

			var senderId = this.KeyToString(transaction.string_sender_id);

			var thingsToDelete: { [id: string]: ThingModel.Thing } = {};
			// Remove the things
			_.each(transaction.things_remove_list, (key: number) => {
				var id = this.KeyToString(key);
				var thing = this._warehouse.GetThing(id);
				if (thing) {
					thingsToDelete[thing.ID] = thing;
				}
			});

			this._warehouse.RemoveCollection(thingsToDelete, true, senderId);

			// Declare the things type
			_.each(transaction.thingtypes_declaration_list, (d: ThingType) => {
				this.ConvertThingTypeDeclaration(d, senderId);
			});

			var thingsToConnect: { model: ThingModel.Thing;proto: Thing }[] = [];

			// Create the things
			_.each(transaction.things_publish_list, (t: Thing) => {
				var modelThing = this.ConvertThingPublication(t, check, senderId);

				if (t.connections_change) {
					thingsToConnect.push({ proto: t, model: modelThing });
				}
			});

			// Connect the things
			_.each(thingsToConnect, (tuple) => {
				tuple.model.DisconnectAll();

				_.each(tuple.proto.connections, (connection) => {
					var t = this._warehouse.GetThing(this.KeyToString(connection));

					if (t) {
						tuple.model.Connect(t);
					}
				});

				this._warehouse.RegisterThing(tuple.model, false, false, senderId);
			});

			return senderId;
		}

		private ConvertThingTypeDeclaration(thingType: ThingType, senderId: string): void {
			var modelType = new ThingModel.ThingType(this.KeyToString(thingType.string_name));
			modelType.Description = this.KeyToString(thingType.string_description);

			_.each(thingType.properties, (propertyType: PropertyType) => {

				var type: ThingModel.Type = null;
				switch (propertyType.type) {
				case PropertyType.Type.LOCATION:
					type = ThingModel.Type.Location;
					break;
				case PropertyType.Type.STRING:
					type = ThingModel.Type.String;
					break;
				case PropertyType.Type.DOUBLE:
					type = ThingModel.Type.Double;
					break;
				case PropertyType.Type.INT:
					type = ThingModel.Type.Int;
					break;
				case PropertyType.Type.BOOLEAN:
					type = ThingModel.Type.Boolean;
					break;
				case PropertyType.Type.DATETIME:
					type = ThingModel.Type.DateTime;
					break;
				}

				var modelProperty = new ThingModel.PropertyType(
					this.KeyToString(propertyType.string_key),
					type,
					propertyType.required);

				modelProperty.Description = this.KeyToString(propertyType.string_description);
				modelProperty.Name = this.KeyToString(propertyType.string_name);

				modelType.DefineProperty(modelProperty);
			});

			this._warehouse.RegisterType(modelType, true, senderId);
		}

		private ConvertThingPublication(thing: Thing, check: boolean, senderId: string): ThingModel.Thing {
			var type: ThingModel.ThingType = null;

			if (thing.string_type_name != 0) {
				type = this._warehouse.GetThingType(this.KeyToString(thing.string_type_name));
			}

			var id = this.KeyToString(thing.string_id);

			var modelThing = this._warehouse.GetThing(id);

			if (modelThing == null || (
				modelThing.Type == null && type != null ||
					type == null && modelThing.Type != null ||
					(modelThing.Type != null && type != null && modelThing.Type.Name != type.Name))) {
				modelThing = new ThingModel.Thing(id, type);
			}

			_.each(thing.properties, (property) => {
				this.ConvertThingProperty(property, modelThing);
			});

			if (check && type != null && !type.Check(modelThing)) {
				console.log("Object «" + id + "» from «" + senderId + "»not valid, ignored");
			} else if (!thing.connections_change) {
				this._warehouse.RegisterThing(modelThing, false, false, senderId);
			}

			return modelThing;
		}

		private ConvertLocationProperty(location: Location, property: Property) {
			if (property.location_value != null) {
				location.X = property.location_value.x;
				location.Y = property.location_value.y;

				if (!property.location_value.z_null) {
					location.Z = property.location_value.z;
				}
			}
		}

		private ConvertThingProperty(property: Property, modelThing: ThingModel.Thing): void {
			var modelProperty: ThingModel.Property = null;

			var key = this.KeyToString(property.string_key);

			switch (property.type) {
			case Property.Type.LOCATION_EQUATORIAL:
				var locEquatorial = new ThingModel.Location.Equatorial();
				this.ConvertLocationProperty(locEquatorial, property);
				modelProperty = new ThingModel.Property.Location.Equatorial(key, locEquatorial);
				break;

			case Property.Type.LOCATION_POINT:
				var locPoint = new ThingModel.Location.Point();
				this.ConvertLocationProperty(locPoint, property);
				modelProperty = new ThingModel.Property.Location.Point(key, locPoint);
				break;
			case Property.Type.LOCATION_LATLNG:
				var locLatLng = new ThingModel.Location.LatLng();
				this.ConvertLocationProperty(locLatLng, property);
				modelProperty = new ThingModel.Property.Location.LatLng(key, locLatLng);
				break;

			case Property.Type.STRING:
				var value: string;
				if (property.string_value != null) {
					value = property.string_value.value;

					if (!value && property.string_value.string_value != 0) {
						value = this.KeyToString(property.string_value.string_value);
					}
				} else {
					value = "undefined";
				}

				modelProperty = new ThingModel.Property.String(key, value);
				break;

			case Property.Type.DOUBLE:
				modelProperty = new ThingModel.Property.Double(key, property.double_value);
				break;

			case Property.Type.INT:
				modelProperty = new ThingModel.Property.Int(key, property.int_value);
				break;

			case Property.Type.BOOLEAN:
				modelProperty = new ThingModel.Property.Boolean(key, property.boolean_value);
				break;

			case Property.Type.DATETIME:
				modelProperty = new ThingModel.Property.DateTime(key,
					new Date((<any>property.datetime_value).toNumber()));
				break;
			}

			modelThing.SetProperty(modelProperty);
		}
	}

}
