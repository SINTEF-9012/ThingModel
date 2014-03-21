/// <reference path="../Proto/Proto.d.ts" />
declare module ThingModel {
	class BuildANewThing {
		private type;
		constructor(type: ThingModel.ThingType);
		static WithoutType : BuildANewThing;
		static As(type: ThingModel.ThingType): BuildANewThing;
		public IdentifiedBy(id: string): ThingPropertyBuilder;
	}
	class ThingPropertyBuilder {
		private thing;
		public ContainingA: ThingPropertyBuilder;
		public ContainingAn: ThingPropertyBuilder;
		public AndA: ThingPropertyBuilder;
		public AndAn: ThingPropertyBuilder;
		constructor(thing: ThingModel.Thing);
		public String(key: string, value: string): ThingPropertyBuilder;
		public Location(key: string, value: ThingModel.Location): ThingPropertyBuilder;
		public Double(key: string, value: number): ThingPropertyBuilder;
		public Int(key: string, value: number): ThingPropertyBuilder;
		public Boolean(key: string, value: boolean): ThingPropertyBuilder;
		public DateTime(key: string, value: Date): ThingPropertyBuilder;
		public Build(): ThingModel.Thing;
	}
}
declare module ThingModel {
	class BuildANewThingType {
		static Named(name: string): ThingTypePropertyBuilder;
	}
	class ThingTypePropertyBuilder {
		private type;
		private nextPropertyIsNotRequired;
		private lastPropertyAdded;
		private lastProperty;
		public ContainingA: ThingTypePropertyBuilder;
		public ContainingAn: ThingTypePropertyBuilder;
		public AndA: ThingTypePropertyBuilder;
		public AndAn: ThingTypePropertyBuilder;
		constructor(type: ThingModel.ThingType);
		public NotRequired : ThingTypePropertyBuilder;
		public WhichIs(description: string): ThingTypePropertyBuilder;
		private _createProperty(key, name, type);
		public String(key: string, name?: string): ThingTypePropertyBuilder;
		public Location(key: string, name?: string): ThingTypePropertyBuilder;
		public Double(key: string, name?: string): ThingTypePropertyBuilder;
		public Int(key: string, name?: string): ThingTypePropertyBuilder;
		public Boolean(key: string, name?: string): ThingTypePropertyBuilder;
		public DateTime(key: string, name?: string): ThingTypePropertyBuilder;
		public CopyOf(otherType: ThingModel.ThingType): ThingTypePropertyBuilder;
		public Build(): ThingModel.ThingType;
	}
}
declare module ThingModel.WebSockets {
	class Client {
		public SenderID: string;
		private ws;
		private path;
		private _wharehouse;
		private _toProtobuf;
		private _fromProtobuf;
		private _thingModelObserver;
		constructor(senderID: string, path: string, wharehouse: ThingModel.Wharehouse);
		private Connect();
	}
}
declare module ThingModel.Proto {
	class FromProtobuf {
		private _wharehouse;
		private _stringDeclarations;
		constructor(wharehouse: ThingModel.Wharehouse);
		private KeyToString(key);
		public Convert(data: ArrayBuffer, check?: boolean): string;
		public ConvertTransaction(transaction: Proto.Transaction, check: boolean): string;
		private ConvertThingTypeDeclaration(thingType);
		private ConvertThingPublication(thing, check);
		private ConvertThingProperty(property, modelThing);
	}
}
declare module ThingModel.Proto {
	class ProtoModelObserver implements ThingModel.IWharehouseObserver {
		public Updates: {
			[id: string]: ThingModel.Thing;
		};
		public Deletions: {
			[id: string]: ThingModel.Thing;
		};
		public Definitions: {
			[name: string]: ThingModel.ThingType;
		};
		private somethingChanged;
		public Reset(): void;
		public New(thing: ThingModel.Thing): void;
		public Deleted(thing: ThingModel.Thing): void;
		public Updated(thing: ThingModel.Thing): void;
		public Define(thingType: ThingModel.ThingType): void;
		public SomethingChanged : boolean;
		public GetTransaction(toProtobuf: Proto.ToProtobuf, senderID: string): Proto.Transaction;
	}
}
declare module ThingModel.Proto {
	class ProtoTools {
		static Builder: Proto.ProtoBufBuilder;
		constructor();
	}
}
declare module ThingModel.Proto {
	class ToProtobuf {
		private _transaction;
		private _stringDeclarations;
		private _stringDeclarationsCpt;
		private _stringToDeclare;
		private _thingsState;
		private _propertiesState;
		constructor();
		private StringToKey(value);
		public Convert(publish: ThingModel.Thing[], deletions: ThingModel.Thing[], declarations: ThingModel.ThingType[], senderID: string): Proto.Transaction;
		private ConvertThing(thing);
		private ConvertDeleteList(list);
		private ConvertDeclarationList(list);
		private ConvertLocationProperty(property, proto);
		private ConvertStringProperty(property, proto);
		private ManageThingSuppression(thingId);
		public ApplyThingsSuppressions(things: ThingModel.Thing[]): void;
	}
}
declare module ThingModel {
	interface IWharehouseObserver {
		New(thing: ThingModel.Thing): void;
		Deleted(thing: ThingModel.Thing): void;
		Updated(thing: ThingModel.Thing): void;
		Define(thing: ThingModel.ThingType): void;
	}
}
declare module ThingModel {
	interface Location {
		X: number;
		Y: number;
		Z: number;
		System: string;
		Compare(other: Location): boolean;
		toString(): string;
		type: string;
	}
	module Location {
		class Point implements Location {
			public X: number;
			public Y: number;
			public Z: number;
			public System: string;
			public type: string;
			constructor(x?: number, y?: number, z?: number);
			public Compare(other: Location): boolean;
			public toString(): string;
		}
		class LatLng extends Point {
			constructor(latitude?: number, longitude?: number, altitude?: number);
			public Latitude : number;
			public Longitude : number;
			public Altitude : number;
		}
		class Equatorial extends Point {
			constructor(rightAscension?: number, declination?: number, hourAngle?: number);
			public RightAscension : number;
			public Declination : number;
			public HourAngle : number;
		}
	}
}
declare module ThingModel {
	class Property {
		private _key;
		public _value: any;
		public Key : string;
		public Type : ThingModel.Type;
		constructor(key: string, value: any);
		public ValueToString(): string;
		public CompareValue(other: Property): boolean;
	}
	module Property {
		class Location extends Property {
			constructor(key: string, value?: ThingModel.Location);
			public Value : ThingModel.Location;
			public Type : ThingModel.Type;
		}
		class String extends Property {
			constructor(key: string, value?: string);
			public Value : string;
			public Type : ThingModel.Type;
		}
		class Double extends Property {
			constructor(key: string, value?: number);
			public Value : number;
			public Type : ThingModel.Type;
		}
		class Int extends Property {
			constructor(key: string, value?: number);
			public Value : number;
			public Type : ThingModel.Type;
		}
		class Boolean extends Property {
			constructor(key: string, value?: boolean);
			public Value : boolean;
			public Type : ThingModel.Type;
		}
		class DateTime extends Property {
			constructor(key: string, value?: Date);
			public Value : Date;
			public Type : ThingModel.Type;
		}
	}
}
declare module ThingModel {
	class PropertyType {
		private _key;
		public Key : string;
		public Name: string;
		public Description: string;
		public Required: boolean;
		private _type;
		public Type : ThingModel.Type;
		constructor(key: string, type: ThingModel.Type, required?: boolean);
		public Check(property: ThingModel.Property): boolean;
		public Clone(): PropertyType;
	}
}
declare module ThingModel {
	class Thing {
		private _id;
		public ID : string;
		private _type;
		public Type : ThingModel.ThingType;
		private _properties;
		private _propertiesCount;
		private _connections;
		private _connectionsCount;
		constructor(id: string, type?: ThingModel.ThingType);
		public SetProperty(property: ThingModel.Property): void;
		public HasProperty(key: string): boolean;
		public GetProperty<T extends ThingModel.Property>(key: string, type?: ThingModel.Type): T;
		public GetString(key: string): string;
		public Connect(thing: Thing): void;
		public Disconnect(thing: Thing): boolean;
		public DisconnectAll(): void;
		public IsConnectedTo(thing: Thing): boolean;
		public ConnectedThings : Thing[];
		public ConnectedThingsCount : number;
		public Properties : ThingModel.Property[];
		public Compare(other: Thing, compareId?: boolean, deepComparisonForConnectedThings?: boolean): boolean;
		private RecursiveCompare(other, alreadyVisitedObjets);
	}
}
declare module ThingModel {
	class ThingType {
		private _name;
		public Name : string;
		public Description: string;
		private _properties;
		constructor(name: string);
		public Check(thing: ThingModel.Thing): boolean;
		public DefineProperty(property: ThingModel.PropertyType): void;
		public GetPropertyDefinition(key: string): ThingModel.PropertyType;
		public Properties : ThingModel.PropertyType[];
	}
}
declare module ThingModel {
	enum Type {
		Unknown = 0,
		Location = 1,
		String = 2,
		Double = 3,
		Int = 4,
		Boolean = 5,
		DateTime = 6,
	}
}
declare module ThingModel {
	class Wharehouse {
		private _thingTypes;
		private _things;
		private _observers;
		constructor();
		public RegisterType(type: ThingModel.ThingType, force?: boolean): void;
		public RegisterThing(thing: ThingModel.Thing, alsoRegisterConnections?: boolean, alsoRegisterTypes?: boolean): void;
		private RecursiveRegisterThing(thing, alsoRegisterTypes, alreadyVisitedObjects);
		public RegisterCollection(collection: ThingModel.Thing[], alsoRegisterTypes?: boolean): void;
		public RemoveThing(thing: ThingModel.Thing): void;
		public RegisterObserver(observer: ThingModel.IWharehouseObserver): void;
		public UnregisterObserver(observer: ThingModel.IWharehouseObserver): void;
		public NotifyThingTypeDefine(type: ThingModel.ThingType): void;
		public NotifyThingUpdate(thing: ThingModel.Thing): void;
		public NotifyThingCreation(thing: ThingModel.Thing): void;
		public NotifyThingDeleted(thing: ThingModel.Thing): void;
		public GetThing(id: string): ThingModel.Thing;
		public GetThingType(name: string): ThingModel.ThingType;
		public Things : ThingModel.Thing[];
	}
}

