/// <reference path="../Proto/Proto.d.ts" />

declare module ThingModel {
    class BuildANewThing {
        constructor(type: ThingType);
        static WithoutType : BuildANewThing;
        static As(type: ThingType): BuildANewThing;
        public IdentifiedBy(id: string): ThingPropertyBuilder;
    }
    class ThingPropertyBuilder {
        public ContainingA: ThingPropertyBuilder;
        public ContainingAn: ThingPropertyBuilder;
        public AndA: ThingPropertyBuilder;
        public AndAn: ThingPropertyBuilder;
        constructor(thing: Thing);
        public String(key: string, value: string): ThingPropertyBuilder;
        public Double(key: string, value: number): ThingPropertyBuilder;
        public Int(key: string, value: number): ThingPropertyBuilder;
        public Boolean(key: string, value: boolean): ThingPropertyBuilder;
        public DateTime(key: string, value: Date): ThingPropertyBuilder;
        public Location(value: Location): ThingPropertyBuilder;
        public Location(key: string, value: Location): ThingPropertyBuilder;
        public Build(): Thing;
    }
}
declare module ThingModel {
    class BuildANewThingType {
        static Named(name: string): ThingTypePropertyBuilder;
    }
    class ThingTypePropertyBuilder {
        public ContainingA: ThingTypePropertyBuilder;
        public ContainingAn: ThingTypePropertyBuilder;
        public AndA: ThingTypePropertyBuilder;
        public AndAn: ThingTypePropertyBuilder;
        constructor(type: ThingType);
        public NotRequired : ThingTypePropertyBuilder;
        public WhichIs(description: string): ThingTypePropertyBuilder;
        public String(key: string, name?: string): ThingTypePropertyBuilder;
        public LocationPoint(key?: string, name?: string): ThingTypePropertyBuilder;
        public LocationLatLng(key?: string, name?: string): ThingTypePropertyBuilder;
        public LocationEquatorial(key?: string, name?: string): ThingTypePropertyBuilder;
        public Double(key: string, name?: string): ThingTypePropertyBuilder;
        public Int(key: string, name?: string): ThingTypePropertyBuilder;
        public Boolean(key: string, name?: string): ThingTypePropertyBuilder;
        public DateTime(key: string, name?: string): ThingTypePropertyBuilder;
        public CopyOf(otherType: ThingType): ThingTypePropertyBuilder;
        public Build(): ThingType;
    }
}
declare module ThingModel.WebSockets {
    class Client {
        public SenderID: string;
        constructor(senderID: string, path: string, warehouse: Warehouse);
        public Connect(): void;
        public Send(): void;
        public SendMessage(message: string): void;
        public Close(): void;
        public IsConnected(): boolean;
        public RegisterObserver(observer: IClientObserver): void;
        public UnregisterObserver(observer: IClientObserver): void;
    }
}
declare module ThingModel.WebSockets {
    class ClientEnterpriseEdition extends Client {
        public IsLive : boolean;
        public IsPaused : boolean;
        constructor(senderID: string, path: string, warehouse: Warehouse);
        public Live(): void;
        public Play(): void;
        public Pause(): void;
        public Load(time: Date): void;
        public Send(): void;
    }
}
declare module ThingModel.WebSockets {
    interface IClientObserver {
        OnFirstOpen(): void;
        OnOpen(): void;
        OnClose(): void;
        OnTransaction(senderName: string): void;
        OnSend(): void;
    }
}
declare module ThingModel.Proto {
    class FromProtobuf {
        public StringDeclarations: {
            [key: number]: string;
        };
        constructor(warehouse: Warehouse);
        public Convert(data: ArrayBuffer, check?: boolean): string;
        public ConvertTransaction(transaction: Transaction, check: boolean): string;
    }
}
declare module ThingModel.Proto {
    class ProtoModelObserver implements IWarehouseObserver {
        public Updates: {
            [id: string]: ThingModel.Thing;
        };
        public Deletions: {
            [id: string]: ThingModel.Thing;
        };
        public Definitions: {
            [name: string]: ThingModel.ThingType;
        };
        public PermanentDefinitions: {
            [name: string]: ThingModel.ThingType;
        };
        public Reset(): void;
        public New(thing: ThingModel.Thing): void;
        public Deleted(thing: ThingModel.Thing): void;
        public Updated(thing: ThingModel.Thing): void;
        public Define(thingType: ThingModel.ThingType): void;
        public SomethingChanged : boolean;
        public GetTransaction(toProtobuf: ToProtobuf, senderID: string, allDefinitions?: boolean, onlyDefinitions?: boolean): Transaction;
    }
}
declare module ThingModel.Proto {
    class ProtoTools {
        static Builder: ProtoBufBuilder;
        constructor();
    }
}
declare module ThingModel.Proto {
    class ToProtobuf {
        public StringDeclarations: {
            [value: string]: number;
        };
        public StringDeclarationsCpt: number;
        constructor();
        public Convert(publish: ThingModel.Thing[], deletions: ThingModel.Thing[], declarations: ThingModel.ThingType[], senderID: string): Transaction;
        public ConvertTransaction(transaction: Transaction): ArrayBuffer;
        public ApplyThingsSuppressions(things: ThingModel.Thing[]): void;
    }
}
declare module ThingModel {
    interface IWarehouseObserver {
        New(thing: Thing, sender: string): void;
        Deleted(thing: Thing, sender: string): void;
        Updated(thing: Thing, sender: string): void;
        Define(thing: ThingType, sender: string): void;
    }
}
declare module ThingModel {
    interface Location {
        X: number;
        Y: number;
        Z: number;
        /**
        *	Name of the location system.
        *
        *	Examples : WGS84, ETRS89...
        */
        System: string;
        /**
        *	Return true if the locations are the same.
        */
        Compare(other: Location): boolean;
        toString(): string;
        type: string;
    }
    module Location {
        /**
        *	Simple location value.
        *	Often used in videogames.
        */
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
        /**
        *	Latitude longitude representation, often used for WGS 84 GPS localizations.
        *	And if the System property is null, it's considered by default as a WGS 84 LatLng system.
        */
        class LatLng extends Point {
            constructor(latitude?: number, longitude?: number, altitude?: number);
            public Latitude : number;
            public Longitude : number;
            public Altitude : number;
        }
        /**
        *	Representations in space.
        */
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
        public _key: string;
        public _value: any;
        public Key : string;
        public Type : Type;
        constructor(key: string, value: any);
        public ValueToString(): string;
        public CompareValue(other: Property): boolean;
        public Clone(): Property;
    }
    module Property {
        module Location {
            class Point extends Property {
                constructor(key: string, value?: ThingModel.Location.Point);
                public Value : ThingModel.Location.Point;
                public Type : Type;
                public Clone(): Point;
            }
            class LatLng extends Property {
                constructor(key: string, value?: ThingModel.Location.LatLng);
                public Value : ThingModel.Location.LatLng;
                public Type : Type;
                public Clone(): LatLng;
            }
            class Equatorial extends Property {
                constructor(key: string, value?: ThingModel.Location.Equatorial);
                public Value : ThingModel.Location.Equatorial;
                public Type : Type;
                public Clone(): Equatorial;
            }
        }
        class String extends Property {
            constructor(key: string, value?: string);
            public Value : string;
            public Type : Type;
            public Clone(): String;
        }
        class Double extends Property {
            constructor(key: string, value?: number);
            public Value : number;
            public Type : Type;
            public Clone(): Double;
        }
        class Int extends Property {
            constructor(key: string, value?: number);
            public Value : number;
            public Type : Type;
            public Clone(): Int;
        }
        class Boolean extends Property {
            constructor(key: string, value?: boolean);
            public Value : boolean;
            public Type : Type;
            public Clone(): Boolean;
        }
        class DateTime extends Property {
            constructor(key: string, value?: Date);
            public Value : Date;
            public Type : Type;
            public Clone(): Double;
        }
    }
}
declare module ThingModel {
    class PropertyType {
        public Key : string;
        public Name: string;
        public Description: string;
        public Required: boolean;
        public Type : Type;
        constructor(key: string, type: Type, required?: boolean);
        public Check(property: Property): boolean;
        public Clone(): PropertyType;
    }
}
declare module ThingModel {
    class Thing {
        /**
        *	String ID, unique in the world.
        *
        *	It can't be null.
        *	Two things objets with the same ID represent the same thing.
        */
        public ID : string;
        /**
        *	Reference to the object type.
        *
        *	The type is null by default, and the default type is applied.
        */
        public Type : ThingType;
        constructor(id: string, type?: ThingType);
        public SetProperty(property: Property): void;
        public HasProperty(key: string): boolean;
        public GetProperty<T extends Property>(key: string, type?: Type): T;
        public GetString(key: string): string;
        public Connect(thing: Thing): void;
        public Disconnect(thing: Thing): boolean;
        public DisconnectAll(): void;
        public IsConnectedTo(thing: Thing): boolean;
        public ConnectedThings : Thing[];
        public ConnectedThingsCount : number;
        public Properties : Property[];
        public Compare(other: Thing, compareId?: boolean, deepComparisonForConnectedThings?: boolean): boolean;
        public ContainingA : ThingPropertyBuilder;
        public ContainingAn : ThingPropertyBuilder;
        public String(key: string): string;
        public String(key: string, value: string): Thing;
        public Double(key: string): number;
        public Double(key: string, value: number): Thing;
        public Int(key: string): number;
        public Int(key: string, value: number): Thing;
        public Boolean(key: string): boolean;
        public Boolean(key: string, value: boolean): Thing;
        public DateTime(key: string): Date;
        public DateTime(key: string, value: Date): Thing;
        public LocationPoint(): Location.Point;
        public LocationPoint(key: string): Location.Point;
        public LocationPoint(value: Location.Point): Thing;
        public LocationPoint(key: string, value: Location.Point): Thing;
        public LocationLatLng(): Location.LatLng;
        public LocationLatLng(key: string): Location.LatLng;
        public LocationLatLng(value: Location.LatLng): Thing;
        public LocationLatLng(key: string, value: Location.LatLng): Thing;
        public LocationEquatorial(): Location.Equatorial;
        public LocationEquatorial(key: string): Location.Equatorial;
        public LocationEquatorial(value: Location.Equatorial): Thing;
        public LocationEquatorial(key: string, value: Location.Equatorial): Thing;
    }
}
declare module ThingModel {
    class ThingType {
        public Name : string;
        public Description: string;
        constructor(name: string);
        public Check(thing: Thing): boolean;
        public DefineProperty(property: PropertyType): void;
        public GetPropertyDefinition(key: string): PropertyType;
        public Properties : PropertyType[];
    }
}
declare module ThingModel {
    enum Type {
        Unknown = 0,
        Location = 1,
        LocationPoint = 2,
        LocationLatLng = 3,
        LocationEquatorial = 4,
        String = 5,
        Double = 6,
        Int = 7,
        Boolean = 8,
        DateTime = 9,
    }
}
declare module ThingModel {
    class Warehouse {
        constructor();
        public RegisterType(type: ThingType, force?: boolean, sender?: string): void;
        public RegisterThing(thing: Thing, alsoRegisterConnections?: boolean, alsoRegisterTypes?: boolean, sender?: string): void;
        public RegisterCollection(collection: Thing[], alsoRegisterTypes?: boolean, sender?: string): void;
        public RemoveCollection(collection: {
            [id: string]: Thing;
        }, notifyUpdates?: boolean, sender?: string): void;
        public RemoveThing(thing: Thing, notifyUpdates?: boolean, sender?: string): void;
        public RegisterObserver(observer: IWarehouseObserver): void;
        public UnregisterObserver(observer: IWarehouseObserver): void;
        public NotifyThingTypeDefine(type: ThingType, sender?: string): void;
        public NotifyThingUpdate(thing: Thing, sender?: string): void;
        public NotifyThingCreation(thing: Thing, sender?: string): void;
        public NotifyThingDeleted(thing: Thing, sender?: string): void;
        public GetThing(id: string): Thing;
        public GetThingType(name: string): ThingType;
        public Things : Thing[];
        public ThingsTypes : ThingType[];
    }
}
