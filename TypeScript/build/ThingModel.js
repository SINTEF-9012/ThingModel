var ThingModel;
(function (ThingModel) {
    var BuildANewThing = (function () {
        function BuildANewThing(type) {
            this.type = type;
        }
        Object.defineProperty(BuildANewThing, "WithoutType", {
            get: function () {
                return new BuildANewThing(null);
            },
            enumerable: true,
            configurable: true
        });

        BuildANewThing.As = function (type) {
            return new BuildANewThing(type);
        };

        BuildANewThing.prototype.IdentifiedBy = function (id) {
            return new ThingPropertyBuilder(new ThingModel.Thing(id, this.type));
        };
        return BuildANewThing;
    })();
    ThingModel.BuildANewThing = BuildANewThing;

    var ThingPropertyBuilder = (function () {
        function ThingPropertyBuilder(thing) {
            this.thing = thing;
            this.ContainingA = this;
            this.ContainingAn = this;
            this.AndA = this;
            this.AndAn = this;
        }
        ThingPropertyBuilder.prototype.String = function (key, value) {
            this.thing.SetProperty(new ThingModel.Property.String(key, value));
            return this;
        };

        ThingPropertyBuilder.prototype.Double = function (key, value) {
            this.thing.SetProperty(new ThingModel.Property.Double(key, value));
            return this;
        };

        ThingPropertyBuilder.prototype.Int = function (key, value) {
            this.thing.SetProperty(new ThingModel.Property.Int(key, value));
            return this;
        };

        ThingPropertyBuilder.prototype.Boolean = function (key, value) {
            this.thing.SetProperty(new ThingModel.Property.Boolean(key, value));
            return this;
        };

        ThingPropertyBuilder.prototype.DateTime = function (key, value) {
            this.thing.SetProperty(new ThingModel.Property.DateTime(key, value));
            return this;
        };

        ThingPropertyBuilder.prototype.Location = function (mixed, value) {
            var key;
            if (typeof value !== "undefined") {
                key = mixed;
            } else {
                key = "location";
                value = mixed;
            }

            if (!value)
                return this;

            switch (value.type) {
                case "equatorial":
                    this.thing.SetProperty(new ThingModel.Property.Location.Equatorial(key, value));
                    break;
                case "latlng":
                    this.thing.SetProperty(new ThingModel.Property.Location.LatLng(key, value));
                    break;
                case "point":
                default:
                    this.thing.SetProperty(new ThingModel.Property.Location.Point(key, value));
                    break;
            }
            return this;
        };

        ThingPropertyBuilder.prototype.Build = function () {
            return this.thing;
        };
        return ThingPropertyBuilder;
    })();
    ThingModel.ThingPropertyBuilder = ThingPropertyBuilder;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var BuildANewThingType = (function () {
        function BuildANewThingType() {
        }
        BuildANewThingType.Named = function (name) {
            return new ThingTypePropertyBuilder(new ThingModel.ThingType(name));
        };
        return BuildANewThingType;
    })();
    ThingModel.BuildANewThingType = BuildANewThingType;

    var ThingTypePropertyBuilder = (function () {
        function ThingTypePropertyBuilder(type) {
            this.type = type;

            this.ContainingA = this;
            this.ContainingAn = this;
            this.AndA = this;
            this.AndAn = this;
        }
        Object.defineProperty(ThingTypePropertyBuilder.prototype, "NotRequired", {
            get: function () {
                this.nextPropertyIsNotRequired = true;
                return this;
            },
            enumerable: true,
            configurable: true
        });

        ThingTypePropertyBuilder.prototype.WhichIs = function (description) {
            if (!this.lastProperty) {
                this.type.Description = description;
            } else {
                this.lastProperty.Description = description;
            }

            return this;
        };

        ThingTypePropertyBuilder.prototype._createProperty = function (key, name, type) {
            if (!this.lastPropertyAdded && this.lastProperty != null) {
                this.type.DefineProperty(this.lastProperty);
                this.lastPropertyAdded = false;
            }

            var prop = new ThingModel.PropertyType(key, type, true);
            prop.Name = name;

            if (this.nextPropertyIsNotRequired) {
                this.nextPropertyIsNotRequired = false;
                prop.Required = false;
            }

            this.lastProperty = prop;
        };

        ThingTypePropertyBuilder.prototype.String = function (key, name) {
            this._createProperty(key, name, 5 /* String */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.LocationPoint = function (key, name) {
            if (typeof key === "undefined") { key = "location"; }
            this._createProperty(key, name, 2 /* LocationPoint */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.LocationLatLng = function (key, name) {
            if (typeof key === "undefined") { key = "location"; }
            this._createProperty(key, name, 3 /* LocationLatLng */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.LocationEquatorial = function (key, name) {
            if (typeof key === "undefined") { key = "location"; }
            this._createProperty(key, name, 4 /* LocationEquatorial */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.Double = function (key, name) {
            this._createProperty(key, name, 6 /* Double */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.Int = function (key, name) {
            this._createProperty(key, name, 7 /* Int */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.Boolean = function (key, name) {
            this._createProperty(key, name, 8 /* Boolean */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.DateTime = function (key, name) {
            this._createProperty(key, name, 9 /* DateTime */);
            return this;
        };

        ThingTypePropertyBuilder.prototype.CopyOf = function (otherType) {
            var _this = this;
            _.each(otherType.Properties, function (propertyType) {
                _this.type.DefineProperty(propertyType.Clone());
            });
            return this;
        };

        ThingTypePropertyBuilder.prototype.Build = function () {
            if (!this.lastPropertyAdded && this.lastProperty != null) {
                this.type.DefineProperty(this.lastProperty);
            }

            return this.type;
        };
        return ThingTypePropertyBuilder;
    })();
    ThingModel.ThingTypePropertyBuilder = ThingTypePropertyBuilder;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (WebSockets) {
        var Client = (function () {
            function Client(senderID, path, warehouse) {
                this._connexionDelay = 2000;
                this.SenderID = senderID;
                this._path = path;

                this._warehouse = warehouse;

                this._thingModelObserver = new ThingModel.Proto.ProtoModelObserver();
                warehouse.RegisterObserver(this._thingModelObserver);

                this._fromProtobuf = new ThingModel.Proto.FromProtobuf(this._warehouse);
                this._toProtobuf = new ThingModel.Proto.ToProtobuf();

                this._closed = true;
                this._reconnection = false;

                this._observers = null;

                this._sendMessageWaitingList = [];

                this.Connect();
            }
            Client.prototype.Connect = function () {
                var _this = this;
                if (!this._closed) {
                    return;
                }

                this._ws = new WebSocket(this._path);

                this._ws.onopen = function () {
                    console.info("ThingModel: Open connection");
                    _this._closed = false;
                    _this._fromProtobuf = new ThingModel.Proto.FromProtobuf(_this._warehouse);
                    _this._toProtobuf = new ThingModel.Proto.ToProtobuf();
                    _this._connexionDelay = 2000;

                    var firstOpen = !_this._reconnection;

                    if (_this._sendMessageWaitingList.length > 0) {
                        _.each(_this._sendMessageWaitingList, function (message) {
                            _this.SendMessage(message);
                        });

                        _this._sendMessageWaitingList = [];
                    }

                    _this.Send();

                    if (firstOpen) {
                        _this.NotifyObservers(function (obs) {
                            return obs.OnFirstOpen();
                        });
                    }

                    _this.NotifyObservers(function (obs) {
                        return obs.OnOpen();
                    });
                };

                this._ws.onclose = function () {
                    if (!_this._closed) {
                        _this._closed = true;
                        console.info("ThingModel: Connection lost");

                        _this.NotifyObservers(function (obs) {
                            return obs.OnClose();
                        });
                    }
                    _this._reconnection = true;

                    setTimeout(function () {
                        return _this.Connect();
                    }, _this._connexionDelay);

                    if (_this._connexionDelay < 20000) {
                        _this._connexionDelay += 3500;
                    }
                };

                var useFileReader = typeof FileReader !== "undefined";

                this._ws.onmessage = function (message) {
                    var data = message.data;

                    if (typeof data === "string") {
                        console.info("ThingModel has received a string: " + message);
                    } else if (useFileReader) {
                        var fileReader = new FileReader();
                        fileReader.readAsArrayBuffer(data);
                        fileReader.onload = function () {
                            return _this.parseBuffer(fileReader.result);
                        };
                    } else {
                        _this.parseBuffer(data);
                    }
                };
            };

            Client.prototype.parseBuffer = function (buffer) {
                var senderName = this._fromProtobuf.Convert(buffer);
                console.debug("ThingModel: message from: " + senderName);

                this._toProtobuf.ApplyThingsSuppressions(_.values(this._thingModelObserver.Deletions));
                this._thingModelObserver.Reset();

                this.NotifyObservers(function (obs) {
                    return obs.OnTransaction(senderName);
                });
            };

            Client.prototype.Send = function () {
                if (this._closed) {
                    console.debug("ThingModel: Does not send, waiting for connexion");
                    return;
                }
                if (this._thingModelObserver.SomethingChanged) {
                    var transaction = this._thingModelObserver.GetTransaction(this._toProtobuf, this.SenderID, this._reconnection);

                    this._ws.send(this._toProtobuf.ConvertTransaction(transaction));
                    this._thingModelObserver.Reset();
                    this._reconnection = false;
                    console.debug("ThingModel: transaction sent");

                    this.NotifyObservers(function (obs) {
                        return obs.OnSend();
                    });
                }
            };

            Client.prototype.SendMessage = function (message) {
                if (this._closed) {
                    this._sendMessageWaitingList.push(message);
                } else {
                    this._ws.send(message);
                }
            };

            Client.prototype.Close = function () {
                if (!this._closed) {
                    this._ws.close();
                    this._closed = true;
                }
            };

            Client.prototype.IsConnected = function () {
                return this._closed;
            };

            Client.prototype.RegisterObserver = function (observer) {
                if (this._observers === null) {
                    this._observers = [observer];
                } else {
                    this._observers.push(observer);
                }
            };

            Client.prototype.UnregisterObserver = function (observer) {
                if (this._observers !== null) {
                    this._observers.splice(_.indexOf(this._observers, observer), 1);
                }
            };

            Client.prototype.NotifyObservers = function (callback) {
                if (this._observers !== null) {
                    _.each(this._observers, callback);
                }
            };
            return Client;
        })();
        WebSockets.Client = Client;
    })(ThingModel.WebSockets || (ThingModel.WebSockets = {}));
    var WebSockets = ThingModel.WebSockets;
})(ThingModel || (ThingModel = {}));
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ThingModel;
(function (ThingModel) {
    (function (WebSockets) {
        var ClientEnterpriseEdition = (function (_super) {
            __extends(ClientEnterpriseEdition, _super);
            function ClientEnterpriseEdition(senderID, path, warehouse) {
                _super.call(this, senderID, path, warehouse);

                this._isLive = true;
                this._isPaused = false;
            }
            Object.defineProperty(ClientEnterpriseEdition.prototype, "IsLive", {
                get: function () {
                    return this._isLive;
                },
                enumerable: true,
                configurable: true
            });

            Object.defineProperty(ClientEnterpriseEdition.prototype, "IsPaused", {
                get: function () {
                    return this._isPaused;
                },
                enumerable: true,
                configurable: true
            });

            ClientEnterpriseEdition.prototype.Live = function () {
                if (this._isLive && !this._isPaused)
                    return;

                this.SendMessage("live");

                this._isLive = true;
                this._isPaused = false;
            };

            ClientEnterpriseEdition.prototype.Play = function () {
                if (!this._isPaused)
                    return;

                this.SendMessage("play");

                this._isPaused = false;
            };

            ClientEnterpriseEdition.prototype.Pause = function () {
                if (this._isPaused)
                    return;

                this.SendMessage("pause");

                this._isPaused = true;
            };

            ClientEnterpriseEdition.prototype.Load = function (time) {
                this.SendMessage("load " + time.getTime());

                this._isLive = false;
                this._isPaused = true;
            };

            ClientEnterpriseEdition.prototype.Send = function () {
                if (!this._isLive || this._isPaused) {
                    throw new Error("ThingModelClientEnterpriseEdition cannot send data while paused or in a past situation");
                }
                _super.prototype.Send.call(this);
            };
            return ClientEnterpriseEdition;
        })(WebSockets.Client);
        WebSockets.ClientEnterpriseEdition = ClientEnterpriseEdition;
    })(ThingModel.WebSockets || (ThingModel.WebSockets = {}));
    var WebSockets = ThingModel.WebSockets;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Proto) {
        var FromProtobuf = (function () {
            function FromProtobuf(warehouse) {
                this._warehouse = warehouse;
                this.StringDeclarations = {};
            }
            FromProtobuf.prototype.KeyToString = function (key) {
                if (key == 0) {
                    return "";
                }

                var value = this.StringDeclarations[key];

                return typeof value === "undefined" ? "undefined" : value;
            };

            FromProtobuf.prototype.Convert = function (data, check) {
                if (typeof check === "undefined") { check = false; }
                var transaction = Proto.ProtoTools.Builder.Transaction.decode(data);

                return this.ConvertTransaction(transaction, check);
            };

            FromProtobuf.prototype.ConvertTransaction = function (transaction, check) {
                var _this = this;
                _.each(transaction.string_declarations, function (d) {
                    _this.StringDeclarations[d.key] = d.value;
                });

                var senderId = this.KeyToString(transaction.string_sender_id);

                var thingsToDelete = {};

                _.each(transaction.things_remove_list, function (key) {
                    var id = _this.KeyToString(key);
                    var thing = _this._warehouse.GetThing(id);
                    if (thing) {
                        thingsToDelete[thing.ID] = thing;
                    }
                });

                this._warehouse.RemoveCollection(thingsToDelete, true, senderId);

                _.each(transaction.thingtypes_declaration_list, function (d) {
                    _this.ConvertThingTypeDeclaration(d, senderId);
                });

                var thingsToConnect = [];

                _.each(transaction.things_publish_list, function (t) {
                    var modelThing = _this.ConvertThingPublication(t, check, senderId);

                    if (t.connections_change) {
                        thingsToConnect.push({ proto: t, model: modelThing });
                    }
                });

                _.each(thingsToConnect, function (tuple) {
                    tuple.model.DisconnectAll();

                    _.each(tuple.proto.connections, function (connection) {
                        var t = _this._warehouse.GetThing(_this.KeyToString(connection));

                        if (t) {
                            tuple.model.Connect(t);
                        }
                    });

                    _this._warehouse.RegisterThing(tuple.model, false, false, senderId);
                });

                return senderId;
            };

            FromProtobuf.prototype.ConvertThingTypeDeclaration = function (thingType, senderId) {
                var _this = this;
                var modelType = new ThingModel.ThingType(this.KeyToString(thingType.string_name));
                modelType.Description = this.KeyToString(thingType.string_description);

                _.each(thingType.properties, function (propertyType) {
                    var type = null;
                    switch (propertyType.type) {
                        case 0 /* LOCATION */:
                            type = 1 /* Location */;
                            break;
                        case 1 /* STRING */:
                            type = 5 /* String */;
                            break;
                        case 2 /* DOUBLE */:
                            type = 6 /* Double */;
                            break;
                        case 3 /* INT */:
                            type = 7 /* Int */;
                            break;
                        case 4 /* BOOLEAN */:
                            type = 8 /* Boolean */;
                            break;
                        case 5 /* DATETIME */:
                            type = 9 /* DateTime */;
                            break;
                    }

                    var modelProperty = new ThingModel.PropertyType(_this.KeyToString(propertyType.string_key), type, propertyType.required);

                    modelProperty.Description = _this.KeyToString(propertyType.string_description);
                    modelProperty.Name = _this.KeyToString(propertyType.string_name);

                    modelType.DefineProperty(modelProperty);
                });

                this._warehouse.RegisterType(modelType, true, senderId);
            };

            FromProtobuf.prototype.ConvertThingPublication = function (thing, check, senderId) {
                var _this = this;
                var type = null;

                if (thing.string_type_name != 0) {
                    type = this._warehouse.GetThingType(this.KeyToString(thing.string_type_name));
                }

                var id = this.KeyToString(thing.string_id);

                var modelThing = this._warehouse.GetThing(id);

                if (modelThing == null || (modelThing.Type == null && type != null || type == null && modelThing.Type != null || (modelThing.Type != null && type != null && modelThing.Type.Name != type.Name))) {
                    modelThing = new ThingModel.Thing(id, type);
                }

                _.each(thing.properties, function (property) {
                    _this.ConvertThingProperty(property, modelThing);
                });

                if (check && type != null && !type.Check(modelThing)) {
                    console.log("Object «" + id + "» from «" + senderId + "»not valid, ignored");
                } else if (!thing.connections_change) {
                    this._warehouse.RegisterThing(modelThing, false, false, senderId);
                }

                return modelThing;
            };

            FromProtobuf.prototype.ConvertLocationProperty = function (location, property) {
                if (property.location_value != null) {
                    location.X = property.location_value.x;
                    location.Y = property.location_value.y;

                    if (!property.location_value.z_null) {
                        location.Z = property.location_value.z;
                    }
                }
            };

            FromProtobuf.prototype.ConvertThingProperty = function (property, modelThing) {
                var modelProperty = null;

                var key = this.KeyToString(property.string_key);

                switch (property.type) {
                    case 2 /* LOCATION_EQUATORIAL */:
                        var locEquatorial = new ThingModel.Location.Equatorial();
                        this.ConvertLocationProperty(locEquatorial, property);
                        modelProperty = new ThingModel.Property.Location.Equatorial(key, locEquatorial);
                        break;

                    case 0 /* LOCATION_POINT */:
                        var locPoint = new ThingModel.Location.Point();
                        this.ConvertLocationProperty(locPoint, property);
                        modelProperty = new ThingModel.Property.Location.Point(key, locPoint);
                        break;
                    case 1 /* LOCATION_LATLNG */:
                        var locLatLng = new ThingModel.Location.LatLng();
                        this.ConvertLocationProperty(locLatLng, property);
                        modelProperty = new ThingModel.Property.Location.LatLng(key, locLatLng);
                        break;

                    case 3 /* STRING */:
                        var value;
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

                    case 4 /* DOUBLE */:
                        modelProperty = new ThingModel.Property.Double(key, property.double_value);
                        break;

                    case 5 /* INT */:
                        modelProperty = new ThingModel.Property.Int(key, property.int_value);
                        break;

                    case 6 /* BOOLEAN */:
                        modelProperty = new ThingModel.Property.Boolean(key, property.boolean_value);
                        break;

                    case 7 /* DATETIME */:
                        modelProperty = new ThingModel.Property.DateTime(key, new Date(property.datetime_value.toNumber()));
                        break;
                }

                modelThing.SetProperty(modelProperty);
            };
            return FromProtobuf;
        })();
        Proto.FromProtobuf = FromProtobuf;
    })(ThingModel.Proto || (ThingModel.Proto = {}));
    var Proto = ThingModel.Proto;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Proto) {
        var ProtoModelObserver = (function () {
            function ProtoModelObserver() {
                this.Updates = {};
                this.Deletions = {};
                this.Definitions = {};
                this.PermanentDefinitions = {};
                this.somethingChanged = false;
            }
            ProtoModelObserver.prototype.Reset = function () {
                this.Updates = {};
                this.Deletions = {};
                this.Definitions = {};
                this.somethingChanged = true;
            };

            ProtoModelObserver.prototype.New = function (thing) {
                this.Updates[thing.ID] = thing;
                this.somethingChanged = true;
            };

            ProtoModelObserver.prototype.Deleted = function (thing) {
                delete this.Updates[thing.ID];
                this.Deletions[thing.ID] = thing;
                this.somethingChanged = true;
            };

            ProtoModelObserver.prototype.Updated = function (thing) {
                this.Updates[thing.ID] = thing;
                this.somethingChanged = true;
            };

            ProtoModelObserver.prototype.Define = function (thingType) {
                this.Definitions[thingType.Name] = thingType;
                this.PermanentDefinitions[thingType.Name] = thingType;
                this.somethingChanged = true;
            };

            Object.defineProperty(ProtoModelObserver.prototype, "SomethingChanged", {
                get: function () {
                    return this.somethingChanged;
                },
                enumerable: true,
                configurable: true
            });

            ProtoModelObserver.prototype.GetTransaction = function (toProtobuf, senderID, allDefinitions, onlyDefinitions) {
                if (typeof allDefinitions === "undefined") { allDefinitions = false; }
                if (typeof onlyDefinitions === "undefined") { onlyDefinitions = false; }
                return toProtobuf.Convert(onlyDefinitions ? [] : _.values(this.Updates), _.values(this.Deletions), _.values(allDefinitions ? this.PermanentDefinitions : this.Definitions), senderID);
            };
            return ProtoModelObserver;
        })();
        Proto.ProtoModelObserver = ProtoModelObserver;
    })(ThingModel.Proto || (ThingModel.Proto = {}));
    var Proto = ThingModel.Proto;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Proto) {
        var ProtoTools = (function () {
            function ProtoTools() {
            }
            ProtoTools.Builder = dcodeIO.ProtoBuf.loadJson({ "package": "ThingModel.Proto", "messages": [{ "name": "Property", "fields": [{ "rule": "required", "type": "int32", "name": "string_key", "id": 1, "options": {} }, { "rule": "required", "type": "Type", "name": "type", "id": 2, "options": { "default": "STRING" } }, { "rule": "optional", "type": "Location", "name": "location_value", "id": 3, "options": {} }, { "rule": "optional", "type": "String", "name": "string_value", "id": 4, "options": {} }, { "rule": "optional", "type": "double", "name": "double_value", "id": 5, "options": {} }, { "rule": "optional", "type": "sint32", "name": "int_value", "id": 6, "options": {} }, { "rule": "optional", "type": "bool", "name": "boolean_value", "id": 7, "options": {} }, { "rule": "optional", "type": "int64", "name": "datetime_value", "id": 8, "options": {} }], "enums": [{ "name": "Type", "values": [{ "name": "LOCATION_POINT", "id": 0 }, { "name": "LOCATION_LATLNG", "id": 1 }, { "name": "LOCATION_EQUATORIAL", "id": 2 }, { "name": "STRING", "id": 3 }, { "name": "DOUBLE", "id": 4 }, { "name": "INT", "id": 5 }, { "name": "BOOLEAN", "id": 6 }, { "name": "DATETIME", "id": 7 }], "options": {} }], "messages": [{ "name": "Location", "fields": [{ "rule": "required", "type": "double", "name": "x", "id": 1, "options": { "default": 0 } }, { "rule": "required", "type": "double", "name": "y", "id": 2, "options": { "default": 0 } }, { "rule": "optional", "type": "double", "name": "z", "id": 3, "options": { "default": 0 } }, { "rule": "optional", "type": "int32", "name": "string_system", "id": 4, "options": {} }, { "rule": "optional", "type": "bool", "name": "z_null", "id": 5, "options": { "default": false } }], "enums": [], "messages": [], "options": {} }, { "name": "String", "fields": [{ "rule": "optional", "type": "string", "name": "value", "id": 1, "options": {} }, { "rule": "optional", "type": "int32", "name": "string_value", "id": 2, "options": { "default": 0 } }], "enums": [], "messages": [], "options": {} }], "options": {} }, { "name": "PropertyType", "fields": [{ "rule": "required", "type": "int32", "name": "string_key", "id": 1, "options": {} }, { "rule": "required", "type": "Type", "name": "type", "id": 2, "options": { "default": "STRING" } }, { "rule": "required", "type": "bool", "name": "required", "id": 3, "options": { "default": true } }, { "rule": "optional", "type": "int32", "name": "string_name", "id": 4, "options": {} }, { "rule": "optional", "type": "int32", "name": "string_description", "id": 5, "options": {} }], "enums": [{ "name": "Type", "values": [{ "name": "LOCATION", "id": 0 }, { "name": "STRING", "id": 1 }, { "name": "DOUBLE", "id": 2 }, { "name": "INT", "id": 3 }, { "name": "BOOLEAN", "id": 4 }, { "name": "DATETIME", "id": 5 }], "options": {} }], "messages": [], "options": {} }, { "name": "StringDeclaration", "fields": [{ "rule": "required", "type": "string", "name": "value", "id": 1, "options": {} }, { "rule": "required", "type": "int32", "name": "key", "id": 2, "options": {} }], "enums": [], "messages": [], "options": {} }, { "name": "Thing", "fields": [{ "rule": "required", "type": "int32", "name": "string_id", "id": 1 }, { "rule": "required", "type": "int32", "name": "string_type_name", "id": 2 }, { "rule": "repeated", "type": "Property", "name": "properties", "id": 3 }, { "rule": "repeated", "type": "int32", "name": "connections", "id": 4, "options": { "packed": true } }, { "rule": "optional", "type": "bool", "name": "connections_change", "id": 5, "options": { "default": false } }], "enums": [], "messages": [] }, { "name": "ThingType", "fields": [{ "rule": "required", "type": "int32", "name": "string_name", "id": 1 }, { "rule": "optional", "type": "int32", "name": "string_description", "id": 2 }, { "rule": "repeated", "type": "PropertyType", "name": "properties", "id": 3 }], "enums": [], "messages": [] }, { "name": "Transaction", "fields": [{ "rule": "repeated", "type": "StringDeclaration", "name": "string_declarations", "id": 1 }, { "rule": "repeated", "type": "Thing", "name": "things_publish_list", "id": 2 }, { "rule": "repeated", "type": "int32", "name": "things_remove_list", "id": 3, "options": { "packed": true } }, { "rule": "repeated", "type": "ThingType", "name": "thingtypes_declaration_list", "id": 4 }, { "rule": "required", "type": "int32", "name": "string_sender_id", "id": 5 }], "enums": [], "messages": [] }], "enums": [], "imports": [], "services": [] }).build("ThingModel.Proto");
            return ProtoTools;
        })();
        Proto.ProtoTools = ProtoTools;
    })(ThingModel.Proto || (ThingModel.Proto = {}));
    var Proto = ThingModel.Proto;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Proto) {
        var ToProtobuf = (function () {
            function ToProtobuf() {
                this.StringDeclarations = {};
                this.StringDeclarationsCpt = 0;
                this._stringToDeclare = {};
                this._thingsState = {};
                this._propertiesState = {};
            }
            ToProtobuf.prototype.StringToKey = function (value) {
                if (!value) {
                    return 0;
                }

                var key = this.StringDeclarations[value];

                if (key) {
                    return key;
                }

                key = ++this.StringDeclarationsCpt;
                this.StringDeclarations[value] = key;

                var stringDeclaration = new Proto.ProtoTools.Builder.StringDeclaration();

                stringDeclaration.setKey(key);
                stringDeclaration.setValue(value);

                this._transaction.string_declarations.push(stringDeclaration);

                return key;
            };

            ToProtobuf.prototype.Convert = function (publish, deletions, declarations, senderID) {
                var _this = this;
                this._transaction = new Proto.ProtoTools.Builder.Transaction();

                this._transaction.setStringSenderId(this.StringToKey(senderID));

                _.each(publish, function (thing) {
                    _this.ConvertThing(thing);
                });

                this.ConvertDeleteList(deletions);
                this.ConvertDeclarationList(declarations);

                return this._transaction;
            };

            ToProtobuf.prototype.ConvertTransaction = function (transaction) {
                return transaction.toArrayBuffer();
            };

            ToProtobuf.prototype.ConvertThing = function (thing) {
                var _this = this;
                var change = false;

                var thingID = this.StringToKey(thing.ID);

                var publication = new Proto.ProtoTools.Builder.Thing();
                publication.setStringId(thingID);
                publication.setStringTypeName(thing.Type != null ? this.StringToKey(thing.Type.Name) : 0);
                publication.setConnectionsChange(false);

                var previousThing = this._thingsState[thingID];

                if (previousThing == null || previousThing.getStringTypeName() != publication.getStringTypeName()) {
                    change = true;
                }

                var connectedThingsCache = null;

                if ((previousThing == null && thing.ConnectedThingsCount > 0) || (previousThing != null && previousThing.getConnections().length != thing.ConnectedThingsCount)) {
                    publication.setConnectionsChange(true);
                } else {
                    connectedThingsCache = thing.ConnectedThings;

                    _.any(connectedThingsCache, function (connectedThing) {
                        var connectionKey = _this.StringToKey(connectedThing.ID);

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

                    _.each(connectedThingsCache, function (connectedThing) {
                        var connectionKey = _this.StringToKey(connectedThing.ID);
                        publication.connections.push(connectionKey);
                    });
                }

                _.each(thing.Properties, function (property) {
                    var propertyId = _this.StringToKey(property.Key);

                    var proto = new Proto.ProtoTools.Builder.Property();
                    proto.setStringKey(propertyId);

                    switch (property.Type) {
                        case 3 /* LocationLatLng */:
                        case 2 /* LocationPoint */:
                        case 4 /* LocationEquatorial */:
                            _this.ConvertLocationProperty(property, proto);
                            break;
                        case 5 /* String */:
                            _this.ConvertStringProperty(property, proto);
                            break;
                        case 6 /* Double */:
                            proto.setType(4 /* DOUBLE */);
                            proto.setDoubleValue(property.Value);
                            break;
                        case 7 /* Int */:
                            proto.setType(5 /* INT */);
                            proto.setIntValue(property.Value);
                            break;
                        case 8 /* Boolean */:
                            proto.setType(6 /* BOOLEAN */);
                            proto.setBooleanValue(property.Value);
                            break;
                        case 9 /* DateTime */:
                            proto.setType(7 /* DATETIME */);
                            proto.setDatetimeValue(property.Value.getTime());
                            break;
                    }

                    var propertyStateKey = thingID + ":" + propertyId;

                    if (previousThing != null) {
                        var previousProto = _this._propertiesState[propertyStateKey];

                        if (previousProto != null && previousProto.getType() == proto.getType() && previousProto.getBooleanValue() == proto.getBooleanValue() && previousProto.getDatetimeValue() == proto.getDatetimeValue() && previousProto.getDoubleValue() == proto.getDoubleValue() && previousProto.getIntValue() == proto.getIntValue()) {
                            var previousLoc = previousProto.getLocationValue();
                            var loc = proto.getLocationValue();

                            if ((previousLoc == null && loc == null) || (previousLoc != null && loc != null && previousLoc.x == loc.x && previousLoc.y == loc.y && previousLoc.z == loc.z && previousLoc.z_null == loc.z_null && previousLoc.getStringSystem() == loc.getStringSystem())) {
                                var previousStr = previousProto.getStringValue();
                                var str = proto.getStringValue();

                                if ((previousStr == null && str == null) || (previousStr != null && str != null && ((previousStr.getStringValue() == str.getStringValue() && previousStr.getValue() == str.getValue()) || (property.ValueToString() == previousStr.getValue())))) {
                                    return;
                                }
                            }
                        }
                    }

                    change = true;
                    publication.properties.push(proto);
                    _this._propertiesState[propertyStateKey] = proto;
                });

                if (change) {
                    this._transaction.things_publish_list.push(publication);
                    this._thingsState[thingID] = publication;
                }
            };

            ToProtobuf.prototype.ConvertDeleteList = function (list) {
                var _this = this;
                _.each(list, function (thing) {
                    var key = _this.StringToKey(thing.ID);
                    _this._transaction.things_remove_list.push(key);

                    _this.ManageThingSuppression(key);
                });
            };

            ToProtobuf.prototype.ConvertDeclarationList = function (list) {
                var _this = this;
                _.each(list, function (thingType) {
                    var declaration = new Proto.ProtoTools.Builder.ThingType();

                    declaration.setStringName(_this.StringToKey(thingType.Name));
                    declaration.setStringDescription(_this.StringToKey(thingType.Description));

                    _.each(thingType.Properties, function (propertyType) {
                        var prop = new Proto.ProtoTools.Builder.PropertyType();
                        prop.setStringKey(_this.StringToKey(propertyType.Key));
                        prop.setStringName(_this.StringToKey(propertyType.Name));
                        prop.setStringDescription(_this.StringToKey(propertyType.Description));
                        prop.setRequired(propertyType.Required);

                        switch (propertyType.Type) {
                            case 3 /* LocationLatLng */:
                            case 4 /* LocationEquatorial */:
                            case 2 /* LocationPoint */:
                            case 1 /* Location */:
                                prop.setType(0 /* LOCATION */);
                                break;
                            case 5 /* String */:
                                prop.setType(1 /* STRING */);
                                break;
                            case 6 /* Double */:
                                prop.setType(2 /* DOUBLE */);
                                break;
                            case 7 /* Int */:
                                prop.setType(3 /* INT */);
                                break;
                            case 8 /* Boolean */:
                                prop.setType(4 /* BOOLEAN */);
                                break;
                            case 9 /* DateTime */:
                                prop.setType(5 /* DATETIME */);
                                break;
                        }

                        declaration.properties.push(prop);
                    });

                    _this._transaction.thingtypes_declaration_list.push(declaration);
                });
            };

            ToProtobuf.prototype.ConvertLocationProperty = function (property, proto) {
                var value = property.Value;

                switch (value.type) {
                    case "latlng":
                        proto.setType(1 /* LOCATION_LATLNG */);
                        break;
                    case "equatorial":
                        proto.setType(2 /* LOCATION_EQUATORIAL */);
                        break;
                    case "point":
                    default:
                        proto.setType(0 /* LOCATION_POINT */);
                        break;
                }

                var loc = new Proto.ProtoTools.Builder.Property.Location();

                loc.setX(value.X);
                loc.setY(value.Y);
                loc.setStringSystem(this.StringToKey(value.System));
                loc.setZNull(value.Z == null);

                if (value.Z != null) {
                    loc.setZ(value.Z);
                }

                proto.setLocationValue(loc);
            };

            ToProtobuf.prototype.ConvertStringProperty = function (property, proto) {
                var value = property.Value;
                proto.setType(3 /* STRING */);

                var st = new Proto.ProtoTools.Builder.Property.String();
                if (value && this._stringToDeclare[value]) {
                    st.setStringValue(this.StringToKey(value));
                } else {
                    st.setValue(value);

                    this._stringToDeclare[value] = true;
                }

                proto.setStringValue(st);
            };

            ToProtobuf.prototype.ManageThingSuppression = function (thingId) {
                var _this = this;
                delete this._thingsState[thingId];

                var stringId = thingId + ":";

                _.each(this._propertiesState, function (value, key) {
                    if (key.indexOf(stringId) === 0) {
                        delete _this._propertiesState[key];
                    }
                });
            };

            ToProtobuf.prototype.ApplyThingsSuppressions = function (things) {
                var _this = this;
                _.each(things, function (thing) {
                    var key = _this.StringDeclarations[thing.ID];

                    if (key) {
                        _this.ManageThingSuppression(key);
                    }
                });
            };
            return ToProtobuf;
        })();
        Proto.ToProtobuf = ToProtobuf;
    })(ThingModel.Proto || (ThingModel.Proto = {}));
    var Proto = ThingModel.Proto;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Location) {
        var Point = (function () {
            function Point(x, y, z) {
                if (typeof x === "undefined") { x = 0.0; }
                if (typeof y === "undefined") { y = 0.0; }
                if (typeof z === "undefined") { z = null; }
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.type = "point";
            }
            Point.prototype.Compare = function (other) {
                return other != null && other.X === this.X && other.Y === this.Y && other.Z == this.Z && other.System == this.System;
            };

            Point.prototype.toString = function () {
                var s = this.X + " - " + this.Y;

                if (this.Z != null) {
                    s += " - " + this.Z;
                }

                if (this.System != null) {
                    s += " -- " + this.System;
                }
                return s;
            };
            return Point;
        })();
        Location.Point = Point;

        var LatLng = (function (_super) {
            __extends(LatLng, _super);
            function LatLng(latitude, longitude, altitude) {
                if (typeof latitude === "undefined") { latitude = 0.0; }
                if (typeof longitude === "undefined") { longitude = 0.0; }
                if (typeof altitude === "undefined") { altitude = null; }
                _super.call(this, latitude, longitude, altitude);
                this.type = "latlng";
            }
            Object.defineProperty(LatLng.prototype, "Latitude", {
                get: function () {
                    return this.X;
                },
                set: function (latitude) {
                    this.X = latitude;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(LatLng.prototype, "Longitude", {
                get: function () {
                    return this.Y;
                },
                set: function (longitude) {
                    this.Y = longitude;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(LatLng.prototype, "Altitude", {
                get: function () {
                    return this.Z;
                },
                set: function (altitude) {
                    this.Z = altitude;
                },
                enumerable: true,
                configurable: true
            });

            return LatLng;
        })(Point);
        Location.LatLng = LatLng;

        var Equatorial = (function (_super) {
            __extends(Equatorial, _super);
            function Equatorial(rightAscension, declination, hourAngle) {
                if (typeof rightAscension === "undefined") { rightAscension = 0.0; }
                if (typeof declination === "undefined") { declination = 0.0; }
                if (typeof hourAngle === "undefined") { hourAngle = 0.0; }
                _super.call(this, rightAscension, declination, hourAngle);
                this.type = "equatorial";
            }
            Object.defineProperty(Equatorial.prototype, "RightAscension", {
                get: function () {
                    return this.X;
                },
                set: function (rightAscension) {
                    this.X = rightAscension;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(Equatorial.prototype, "Declination", {
                get: function () {
                    return this.Y;
                },
                set: function (declination) {
                    this.Y = declination;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(Equatorial.prototype, "HourAngle", {
                get: function () {
                    return this.Z;
                },
                set: function (hourAngle) {
                    this.Z = hourAngle;
                },
                enumerable: true,
                configurable: true
            });

            return Equatorial;
        })(Point);
        Location.Equatorial = Equatorial;
    })(ThingModel.Location || (ThingModel.Location = {}));
    var Location = ThingModel.Location;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var Property = (function () {
        function Property(key, value) {
            if (!key) {
                throw "The Property key should not be null or empty";
            }
            this._key = key;
            this._value = value;
        }
        Object.defineProperty(Property.prototype, "Key", {
            get: function () {
                return this._key;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Property.prototype, "Type", {
            get: function () {
                return 0 /* Unknown */;
            },
            enumerable: true,
            configurable: true
        });

        Property.prototype.ValueToString = function () {
            return this._value != null ? this._value.toString() : "";
        };

        Property.prototype.CompareValue = function (other) {
            if (other == null || (this._value != null && other._value == null)) {
                return false;
            }

            if (this._value == null && other._value == null) {
                return true;
            }

            if (this._value != null && other._value != null) {
                if (this._value.Compare && other._value.Compare) {
                    return this._value.Compare(other._value);
                } else {
                    return this._value === other._value;
                }
            }

            return false;
        };

        Property.prototype.Clone = function () {
            return new Property(this._key, this._value);
        };
        return Property;
    })();
    ThingModel.Property = Property;

    (function (Property) {
        (function (Location) {
            var Point = (function (_super) {
                __extends(Point, _super);
                function Point(key, value) {
                    _super.call(this, key, value);
                }
                Object.defineProperty(Point.prototype, "Value", {
                    get: function () {
                        return this._value;
                    },
                    set: function (value) {
                        this._value = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                Object.defineProperty(Point.prototype, "Type", {
                    get: function () {
                        return 2 /* LocationPoint */;
                    },
                    enumerable: true,
                    configurable: true
                });

                Point.prototype.Clone = function () {
                    var p = new ThingModel.Location.Point(this._value.X, this._value.Y, this._value.Z);

                    if (this._value.System) {
                        p.System = this._value.System;
                    }

                    return new Property.Location.Point(this._key, p);
                };
                return Point;
            })(Property);
            Location.Point = Point;

            var LatLng = (function (_super) {
                __extends(LatLng, _super);
                function LatLng(key, value) {
                    _super.call(this, key, value);
                }
                Object.defineProperty(LatLng.prototype, "Value", {
                    get: function () {
                        return this._value;
                    },
                    set: function (value) {
                        this._value = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                Object.defineProperty(LatLng.prototype, "Type", {
                    get: function () {
                        return 3 /* LocationLatLng */;
                    },
                    enumerable: true,
                    configurable: true
                });

                LatLng.prototype.Clone = function () {
                    var p = new ThingModel.Location.LatLng(this._value.X, this._value.Y, this._value.Z);

                    if (this._value.System) {
                        p.System = this._value.System;
                    }

                    return new Property.Location.LatLng(this._key, p);
                };
                return LatLng;
            })(Property);
            Location.LatLng = LatLng;

            var Equatorial = (function (_super) {
                __extends(Equatorial, _super);
                function Equatorial(key, value) {
                    _super.call(this, key, value);
                }
                Object.defineProperty(Equatorial.prototype, "Value", {
                    get: function () {
                        return this._value;
                    },
                    set: function (value) {
                        this._value = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                Object.defineProperty(Equatorial.prototype, "Type", {
                    get: function () {
                        return 4 /* LocationEquatorial */;
                    },
                    enumerable: true,
                    configurable: true
                });

                Equatorial.prototype.Clone = function () {
                    var p = new ThingModel.Location.Equatorial(this._value.X, this._value.Y, this._value.Z);

                    if (this._value.System) {
                        p.System = this._value.System;
                    }

                    return new Property.Location.Equatorial(this._key, p);
                };
                return Equatorial;
            })(Property);
            Location.Equatorial = Equatorial;
        })(Property.Location || (Property.Location = {}));
        var Location = Property.Location;

        var String = (function (_super) {
            __extends(String, _super);
            function String(key, value) {
                _super.call(this, key, value);
            }
            Object.defineProperty(String.prototype, "Value", {
                get: function () {
                    return this._value;
                },
                set: function (value) {
                    this._value = value;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(String.prototype, "Type", {
                get: function () {
                    return 5 /* String */;
                },
                enumerable: true,
                configurable: true
            });

            String.prototype.Clone = function () {
                return new Property.String(this._key, this._value);
            };
            return String;
        })(Property);
        Property.String = String;

        var Double = (function (_super) {
            __extends(Double, _super);
            function Double(key, value) {
                _super.call(this, key, value);
                if (this._value == null) {
                    this._value = 0.0;
                }
            }
            Object.defineProperty(Double.prototype, "Value", {
                get: function () {
                    return this._value;
                },
                set: function (value) {
                    this._value = value == null ? 0.0 : value;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(Double.prototype, "Type", {
                get: function () {
                    return 6 /* Double */;
                },
                enumerable: true,
                configurable: true
            });

            Double.prototype.Clone = function () {
                return new Property.Double(this._key, this._value);
            };
            return Double;
        })(Property);
        Property.Double = Double;

        var Int = (function (_super) {
            __extends(Int, _super);
            function Int(key, value) {
                _super.call(this, key, value);
                if (this._value == null) {
                    this._value = 0;
                }
            }
            Object.defineProperty(Int.prototype, "Value", {
                get: function () {
                    return this._value;
                },
                set: function (value) {
                    this._value = Math.round(value);
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(Int.prototype, "Type", {
                get: function () {
                    return 7 /* Int */;
                },
                enumerable: true,
                configurable: true
            });

            Int.prototype.Clone = function () {
                return new Property.Int(this._key, this._value);
            };
            return Int;
        })(Property);
        Property.Int = Int;

        var Boolean = (function (_super) {
            __extends(Boolean, _super);
            function Boolean(key, value) {
                _super.call(this, key, !!value);
            }
            Object.defineProperty(Boolean.prototype, "Value", {
                get: function () {
                    return this._value;
                },
                set: function (value) {
                    this._value = !!value;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(Boolean.prototype, "Type", {
                get: function () {
                    return 8 /* Boolean */;
                },
                enumerable: true,
                configurable: true
            });

            Boolean.prototype.Clone = function () {
                return new Property.Boolean(this._key, this._value);
            };
            return Boolean;
        })(Property);
        Property.Boolean = Boolean;

        var DateTime = (function (_super) {
            __extends(DateTime, _super);
            function DateTime(key, value) {
                _super.call(this, key, value);
            }
            Object.defineProperty(DateTime.prototype, "Value", {
                get: function () {
                    return this._value;
                },
                set: function (value) {
                    this._value = value;
                },
                enumerable: true,
                configurable: true
            });


            Object.defineProperty(DateTime.prototype, "Type", {
                get: function () {
                    return 9 /* DateTime */;
                },
                enumerable: true,
                configurable: true
            });

            DateTime.prototype.Clone = function () {
                return new Property.Double(this._key, this._value ? new Date(this._value.getTime()) : this._value);
            };
            return DateTime;
        })(Property);
        Property.DateTime = DateTime;
    })(ThingModel.Property || (ThingModel.Property = {}));
    var Property = ThingModel.Property;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var PropertyType = (function () {
        function PropertyType(key, type, required) {
            if (typeof required === "undefined") { required = true; }
            if (!key) {
                throw "The PropertyType key should not be null or empty";
            }
            this._key = key;
            this._type = type;
            this.Required = required;
        }
        Object.defineProperty(PropertyType.prototype, "Key", {
            get: function () {
                return this._key;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(PropertyType.prototype, "Type", {
            get: function () {
                return this._type;
            },
            enumerable: true,
            configurable: true
        });

        PropertyType.prototype.Check = function (property) {
            return (!this.Required && property == null) || (property != null && property.Type == this._type && property.Key == this.Key);
        };

        PropertyType.prototype.Clone = function () {
            var newProp = new PropertyType(this._key, this._type, this.Required);
            newProp.Name = this.Name;
            this.Description = this.Description;
            return newProp;
        };
        return PropertyType;
    })();
    ThingModel.PropertyType = PropertyType;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var Thing = (function () {
        function Thing(id, type) {
            if (typeof type === "undefined") { type = null; }
            this._type = null;
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
        Object.defineProperty(Thing.prototype, "ID", {
            get: function () {
                return this._id;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Thing.prototype, "Type", {
            get: function () {
                return this._type;
            },
            enumerable: true,
            configurable: true
        });

        Thing.prototype.SetProperty = function (property) {
            if (!this.HasProperty(property.Key)) {
                ++this._propertiesCount;
            }
            this._properties[property.Key] = property;
        };

        Thing.prototype.HasProperty = function (key) {
            return _.has(this._properties, key);
        };

        Thing.prototype.GetProperty = function (key, type) {
            var prop = this._properties[key];
            if (!prop || (type && prop.Type != type)) {
                return null;
            }

            return prop;
        };

        Thing.prototype.GetString = function (key) {
            var prop = this.GetProperty(key, 5 /* String */);

            if (!prop) {
                return null;
            }

            return prop.Value;
        };

        Thing.prototype.Connect = function (thing) {
            if (!thing)
                return;

            if (thing === this || thing._id === this._id) {
                throw new Error("You can't connect a thing directly to itself");
            }
            if (!this.IsConnectedTo(thing)) {
                ++this._connectionsCount;
            }
            this._connections[thing._id] = thing;
        };

        Thing.prototype.Disconnect = function (thing) {
            if (this.IsConnectedTo(thing)) {
                --this._connectionsCount;
                delete this._connections[thing._id];
                return true;
            }
            return false;
        };

        Thing.prototype.DisconnectAll = function () {
            this._connections = {};
            this._connectionsCount = 0;
        };

        Thing.prototype.IsConnectedTo = function (thing) {
            return !!thing && _.has(this._connections, thing._id);
        };

        Object.defineProperty(Thing.prototype, "ConnectedThings", {
            get: function () {
                return _.values(this._connections);
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Thing.prototype, "ConnectedThingsCount", {
            get: function () {
                return this._connectionsCount;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Thing.prototype, "Properties", {
            get: function () {
                return _.values(this._properties);
            },
            enumerable: true,
            configurable: true
        });

        Thing.prototype.Compare = function (other, compareId, deepComparisonForConnectedThings) {
            if (typeof compareId === "undefined") { compareId = true; }
            if (typeof deepComparisonForConnectedThings === "undefined") { deepComparisonForConnectedThings = false; }
            if (this === other) {
                return true;
            }

            if (!other || (this._type != null && other._type != null && this._type.Name != other._type.Name) || (this._type == null && other._type != null) || (this._type != null && other._type == null)) {
                return false;
            }

            if (compareId && this._id != other._id) {
                return false;
            }

            if (this._connectionsCount !== other._connectionsCount || _.any(this._connections, function (connectedThing) {
                return !_.has(other._connections, connectedThing._id);
            })) {
                return false;
            }

            if (this._propertiesCount !== other._propertiesCount || _.any(this._properties, function (property) {
                var otherProp = other._properties[property.Key];

                return otherProp == null || !otherProp.CompareValue(property);
            })) {
                return false;
            }

            if (deepComparisonForConnectedThings) {
                return this.RecursiveCompare(other, {});
            }

            return true;
        };

        Thing.prototype.RecursiveCompare = function (other, alreadyVisitedObjets) {
            if (_.has(alreadyVisitedObjets, this._id)) {
                return true;
            }

            if (!this.Compare(other)) {
                return false;
            }

            alreadyVisitedObjets[this._id] = true;

            return !_.any(this._connections, function (connectedThing) {
                var otherThing = other._connections[connectedThing._id];

                return !connectedThing.RecursiveCompare(otherThing, alreadyVisitedObjets);
            });
        };

        Object.defineProperty(Thing.prototype, "ContainingA", {
            get: function () {
                if (!this._propertyBuilder) {
                    this._propertyBuilder = new ThingModel.ThingPropertyBuilder(this);
                }
                return this._propertyBuilder;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Thing.prototype, "ContainingAn", {
            get: function () {
                return this.ContainingA;
            },
            enumerable: true,
            configurable: true
        });

        Thing.prototype.String = function (key, value) {
            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.String(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 5 /* String */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.Double = function (key, value) {
            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Double(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 6 /* Double */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.Int = function (key, value) {
            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Int(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 7 /* Int */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.Boolean = function (key, value) {
            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Boolean(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 8 /* Boolean */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.DateTime = function (key, value) {
            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.DateTime(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 9 /* DateTime */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.LocationPoint = function (mixed, value) {
            var key;

            if (typeof mixed === 'string' || mixed instanceof String) {
                key = mixed;
            } else {
                key = "location";
                value = mixed;
            }

            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Location.Point(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 2 /* LocationPoint */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.LocationLatLng = function (mixed, value) {
            var key;

            if (typeof mixed === 'string' || mixed instanceof String) {
                key = mixed;
            } else {
                key = "location";
                value = mixed;
            }

            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Location.LatLng(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 3 /* LocationLatLng */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };

        Thing.prototype.LocationEquatorial = function (mixed, value) {
            var key;

            if (typeof mixed === 'string' || mixed instanceof String) {
                key = mixed;
            } else {
                key = "location";
                value = mixed;
            }

            if (typeof value !== "undefined") {
                this.SetProperty(new ThingModel.Property.Location.Equatorial(key, value));
                return this;
            } else {
                var p = this.GetProperty(key, 4 /* LocationEquatorial */);
                if (!p) {
                    return null;
                }
                return p.Value;
            }
        };
        return Thing;
    })();
    ThingModel.Thing = Thing;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var ThingType = (function () {
        function ThingType(name) {
            if (!name) {
                throw new Error("The name should not be null or empty");
            }
            this._name = name;
            this._properties = {};
        }
        Object.defineProperty(ThingType.prototype, "Name", {
            get: function () {
                return this._name;
            },
            enumerable: true,
            configurable: true
        });

        ThingType.prototype.Check = function (thing) {
            return (thing.Type === this || (thing.Type !== null && thing.Type._name === this._name)) && _.all(this._properties, function (propertyType) {
                return propertyType.Check(thing.GetProperty(propertyType.Key));
            });
        };

        ThingType.prototype.DefineProperty = function (property) {
            this._properties[property.Key] = property;
        };

        ThingType.prototype.GetPropertyDefinition = function (key) {
            return this._properties[key];
        };

        Object.defineProperty(ThingType.prototype, "Properties", {
            get: function () {
                return _.values(this._properties);
            },
            enumerable: true,
            configurable: true
        });
        return ThingType;
    })();
    ThingModel.ThingType = ThingType;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    (function (Type) {
        Type[Type["Unknown"] = 0] = "Unknown";
        Type[Type["Location"] = 1] = "Location";
        Type[Type["LocationPoint"] = 2] = "LocationPoint";
        Type[Type["LocationLatLng"] = 3] = "LocationLatLng";
        Type[Type["LocationEquatorial"] = 4] = "LocationEquatorial";
        Type[Type["String"] = 5] = "String";
        Type[Type["Double"] = 6] = "Double";
        Type[Type["Int"] = 7] = "Int";
        Type[Type["Boolean"] = 8] = "Boolean";
        Type[Type["DateTime"] = 9] = "DateTime";
    })(ThingModel.Type || (ThingModel.Type = {}));
    var Type = ThingModel.Type;
})(ThingModel || (ThingModel = {}));
var ThingModel;
(function (ThingModel) {
    var Warehouse = (function () {
        function Warehouse() {
            this._thingTypes = {};
            this._things = {};
            this._observers = [];
        }
        Warehouse.prototype.RegisterType = function (type, force, sender) {
            if (typeof force === "undefined") { force = true; }
            if (typeof sender === "undefined") { sender = null; }
            if (!type) {
                throw new Error("The thing type information is null.");
            }

            if (force || !_.has(this._thingTypes, type.Name)) {
                this._thingTypes[type.Name] = type;

                this.NotifyThingTypeDefine(type, sender);
            }
        };

        Warehouse.prototype.RegisterThing = function (thing, alsoRegisterConnections, alsoRegisterTypes, sender) {
            var _this = this;
            if (typeof alsoRegisterConnections === "undefined") { alsoRegisterConnections = true; }
            if (typeof alsoRegisterTypes === "undefined") { alsoRegisterTypes = true; }
            if (typeof sender === "undefined") { sender = null; }
            if (!thing) {
                throw new Error("A thing should not be null if it want to be allowed in the warehouse");
            }

            var creation = !_.has(this._things, thing.ID);
            this._things[thing.ID] = thing;

            if (alsoRegisterTypes && thing.Type) {
                this.RegisterType(thing.Type, false, sender);
            }

            if (alsoRegisterConnections) {
                var alreadyVisitedObjects = {};
                _.each(thing.ConnectedThings, function (connectedThing) {
                    _this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects, sender);
                });
            }

            if (creation) {
                this.NotifyThingCreation(thing, sender);
            } else {
                this.NotifyThingUpdate(thing, sender);
            }
        };

        Warehouse.prototype.RecursiveRegisterThing = function (thing, alsoRegisterTypes, alreadyVisitedObjects, sender) {
            var _this = this;
            if (alreadyVisitedObjects.hasOwnProperty(thing.ID)) {
                return;
            }

            alreadyVisitedObjects[thing.ID] = true;

            this.RegisterThing(thing, false, alsoRegisterTypes, sender);

            _.each(thing.ConnectedThings, function (connectedThing) {
                _this.RecursiveRegisterThing(connectedThing, alsoRegisterTypes, alreadyVisitedObjects, sender);
            });
        };

        Warehouse.prototype.RegisterCollection = function (collection, alsoRegisterTypes, sender) {
            var _this = this;
            if (typeof alsoRegisterTypes === "undefined") { alsoRegisterTypes = true; }
            if (typeof sender === "undefined") { sender = null; }
            var alreadyVisitedObjects = {};
            _.each(collection, function (thing) {
                _this.RecursiveRegisterThing(thing, alsoRegisterTypes, alreadyVisitedObjects, sender);
            });
        };

        Warehouse.prototype.RemoveCollection = function (collection, notifyUpdates, sender) {
            var _this = this;
            if (typeof notifyUpdates === "undefined") { notifyUpdates = true; }
            if (typeof sender === "undefined") { sender = null; }
            var thingsToDisconnect = {};

            _.each(_.keys(collection), function (id) {
                var thing = collection[id];

                if (notifyUpdates) {
                    _.each(_this._things, function (t) {
                        if (t.IsConnectedTo(thing)) {
                            thingsToDisconnect[t.ID] = t;
                        }
                    });
                }

                _this.RemoveThing(thing, false, sender);
            });

            if (notifyUpdates) {
                _.each(_.keys(thingsToDisconnect), function (id) {
                    if (!collection.hasOwnProperty(id)) {
                        _this.NotifyThingUpdate(thingsToDisconnect[id]);
                    }
                });
            }
        };

        Warehouse.prototype.RemoveThing = function (thing, notifyUpdates, sender) {
            var _this = this;
            if (typeof notifyUpdates === "undefined") { notifyUpdates = true; }
            if (typeof sender === "undefined") { sender = null; }
            if (!thing) {
                return;
            }

            _.each(this._things, function (t) {
                if (t.IsConnectedTo(thing)) {
                    t.Disconnect(thing);
                    if (notifyUpdates) {
                        _this.NotifyThingUpdate(t, sender);
                    }
                }
            });

            if (_.has(this._things, thing.ID)) {
                delete this._things[thing.ID];
                this.NotifyThingDeleted(thing, sender);
            }
        };

        Warehouse.prototype.RegisterObserver = function (observer) {
            this._observers.push(observer);
        };

        Warehouse.prototype.UnregisterObserver = function (observer) {
            this._observers.splice(_.indexOf(this._observers, observer), 1);
        };

        Warehouse.prototype.NotifyThingTypeDefine = function (type, sender) {
            if (typeof sender === "undefined") { sender = null; }
            _.each(this._observers, function (observer) {
                observer.Define(type, sender);
            });
        };

        Warehouse.prototype.NotifyThingUpdate = function (thing, sender) {
            if (typeof sender === "undefined") { sender = null; }
            _.each(this._observers, function (observer) {
                observer.Updated(thing, sender);
            });
        };

        Warehouse.prototype.NotifyThingCreation = function (thing, sender) {
            if (typeof sender === "undefined") { sender = null; }
            _.each(this._observers, function (observer) {
                observer.New(thing, sender);
            });
        };

        Warehouse.prototype.NotifyThingDeleted = function (thing, sender) {
            if (typeof sender === "undefined") { sender = null; }
            _.each(this._observers, function (observer) {
                observer.Deleted(thing, sender);
            });
        };

        Warehouse.prototype.GetThing = function (id) {
            return this._things[id];
        };

        Warehouse.prototype.GetThingType = function (name) {
            return this._thingTypes[name];
        };

        Object.defineProperty(Warehouse.prototype, "Things", {
            get: function () {
                return _.values(this._things);
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Warehouse.prototype, "ThingsTypes", {
            get: function () {
                return _.values(this._thingTypes);
            },
            enumerable: true,
            configurable: true
        });
        return Warehouse;
    })();
    ThingModel.Warehouse = Warehouse;
})(ThingModel || (ThingModel = {}));
//# sourceMappingURL=ThingModel.js.map
