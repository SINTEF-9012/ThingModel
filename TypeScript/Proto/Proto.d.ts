declare module ThingModel.Proto {
	interface ProtoBufModel {
		toArrayBuffer(): ArrayBuffer;
		//toBuffer(): NodeBuffer;
		//encode(): ByteBuffer;
		toBase64(): string;
		toString(): string;
	}

	export interface ProtoBufBuilder {
		Property: PropertyBuilder;
		PropertyType: PropertyTypeBuilder;
		StringDeclaration: StringDeclarationBuilder;
		Thing: ThingBuilder;
		ThingType: ThingTypeBuilder;
		Transaction: TransactionBuilder;

	}
}

declare module ThingModel.Proto {

	export interface Property extends ProtoBufModel {
		string_key: number;
		getStringKey(): number;
		setStringKey(stringKey: number): void;
		type: Property.Type;
		getType(): Property.Type;
		setType(type: Property.Type): void;
		location_value?: Property.Location;
		getLocationValue(): Property.Location;
		setLocationValue(locationValue: Property.Location): void;
		string_value?: Property.String;
		getStringValue(): Property.String;
		setStringValue(stringValue: Property.String): void;
		double_value?: number;
		getDoubleValue(): number;
		setDoubleValue(doubleValue: number): void;
		int_value?: number;
		getIntValue(): number;
		setIntValue(intValue: number): void;
		boolean_value?: boolean;
		getBooleanValue(): boolean;
		setBooleanValue(booleanValue: boolean): void;
		datetime_value?: number;
		getDatetimeValue(): number;
		setDatetimeValue(datetimeValue: number): void;

	}

	export interface PropertyBuilder {
		new(): Property;
		decode(buffer: ArrayBuffer): Property;
		//decode(buffer: NodeBuffer) : Property;
		//decode(buffer: ByteArrayBuffer) : Property;
		decode64(buffer: string): Property;
		Location: Property.LocationBuilder;
		String: Property.StringBuilder;
		Type: Property.Type;

	}
}

declare module ThingModel.Proto.Property {

	export interface Location extends ProtoBufModel {
		x: number;
		getX(): number;
		setX(x: number): void;
		y: number;
		getY(): number;
		setY(y: number): void;
		z?: number;
		getZ(): number;
		setZ(z: number): void;
		string_system?: number;
		getStringSystem(): number;
		setStringSystem(stringSystem: number): void;
		z_null?: boolean;
		getZNull(): boolean;
		setZNull(zNull: boolean): void;

	}

	export interface LocationBuilder {
		new(): Location;
		decode(buffer: ArrayBuffer): Location;
		//decode(buffer: NodeBuffer) : Location;
		//decode(buffer: ByteArrayBuffer) : Location;
		decode64(buffer: string): Location;

	}
}

declare module ThingModel.Proto.Property {

	export interface String extends ProtoBufModel {
		value?: string;
		getValue(): string;
		setValue(value: string): void;
		string_value?: number;
		getStringValue(): number;
		setStringValue(stringValue: number): void;

	}

	export interface StringBuilder {
		new(): String;
		decode(buffer: ArrayBuffer): String;
		//decode(buffer: NodeBuffer) : String;
		//decode(buffer: ByteArrayBuffer) : String;
		decode64(buffer: string): String;

	}
}

declare module ThingModel.Proto.Property {

	export enum Type {
		LOCATION_POINT = 0,
		LOCATION_LATLNG = 1,
		LOCATION_EQUATORIAL = 2,
		STRING = 3,
		DOUBLE = 4,
		INT = 5,
		BOOLEAN = 6,
		DATETIME = 7,

	}
}

declare module ThingModel.Proto {

	export interface PropertyType extends ProtoBufModel {
		string_key: number;
		getStringKey(): number;
		setStringKey(stringKey: number): void;
		type: PropertyType.Type;
		getType(): PropertyType.Type;
		setType(type: PropertyType.Type): void;
		required: boolean;
		getRequired(): boolean;
		setRequired(required: boolean): void;
		string_name?: number;
		getStringName(): number;
		setStringName(stringName: number): void;
		string_description?: number;
		getStringDescription(): number;
		setStringDescription(stringDescription: number): void;

	}

	export interface PropertyTypeBuilder {
		new(): PropertyType;
		decode(buffer: ArrayBuffer): PropertyType;
		//decode(buffer: NodeBuffer) : PropertyType;
		//decode(buffer: ByteArrayBuffer) : PropertyType;
		decode64(buffer: string): PropertyType;
		Type: PropertyType.Type;

	}
}

declare module ThingModel.Proto.PropertyType {

	export enum Type {
		LOCATION = 0,
		STRING = 1,
		DOUBLE = 2,
		INT = 3,
		BOOLEAN = 4,
		DATETIME = 5,

	}
}

declare module ThingModel.Proto {

	export interface StringDeclaration extends ProtoBufModel {
		value: string;
		getValue(): string;
		setValue(value: string): void;
		key: number;
		getKey(): number;
		setKey(key: number): void;

	}

	export interface StringDeclarationBuilder {
		new(): StringDeclaration;
		decode(buffer: ArrayBuffer): StringDeclaration;
		//decode(buffer: NodeBuffer) : StringDeclaration;
		//decode(buffer: ByteArrayBuffer) : StringDeclaration;
		decode64(buffer: string): StringDeclaration;

	}
}

declare module ThingModel.Proto {

	export interface Thing extends ProtoBufModel {
		string_id: number;
		getStringId(): number;
		setStringId(stringId: number): void;
		string_type_name: number;
		getStringTypeName(): number;
		setStringTypeName(stringTypeName: number): void;
		properties: Property[];
		getProperties(): Property[];
		setProperties(properties: Property[]): void;
		connections: number[];
		getConnections(): number[];
		setConnections(connections: number[]): void;
		connections_change?: boolean;
		getConnectionsChange(): boolean;
		setConnectionsChange(connectionsChange: boolean): void;

	}

	export interface ThingBuilder {
		new(): Thing;
		decode(buffer: ArrayBuffer): Thing;
		//decode(buffer: NodeBuffer) : Thing;
		//decode(buffer: ByteArrayBuffer) : Thing;
		decode64(buffer: string): Thing;

	}
}

declare module ThingModel.Proto {

	export interface ThingType extends ProtoBufModel {
		string_name: number;
		getStringName(): number;
		setStringName(stringName: number): void;
		string_description?: number;
		getStringDescription(): number;
		setStringDescription(stringDescription: number): void;
		properties: PropertyType[];
		getProperties(): PropertyType[];
		setProperties(properties: PropertyType[]): void;

	}

	export interface ThingTypeBuilder {
		new(): ThingType;
		decode(buffer: ArrayBuffer): ThingType;
		//decode(buffer: NodeBuffer) : ThingType;
		//decode(buffer: ByteArrayBuffer) : ThingType;
		decode64(buffer: string): ThingType;

	}
}

declare module ThingModel.Proto {

	export interface Transaction extends ProtoBufModel {
		string_declarations: StringDeclaration[];
		getStringDeclarations(): StringDeclaration[];
		setStringDeclarations(stringDeclarations: StringDeclaration[]): void;
		things_publish_list: Thing[];
		getThingsPublishList(): Thing[];
		setThingsPublishList(thingsPublishList: Thing[]): void;
		things_remove_list: number[];
		getThingsRemoveList(): number[];
		setThingsRemoveList(thingsRemoveList: number[]): void;
		thingtypes_declaration_list: ThingType[];
		getThingtypesDeclarationList(): ThingType[];
		setThingtypesDeclarationList(thingtypesDeclarationList: ThingType[]): void;
		string_sender_id: number;
		getStringSenderId(): number;
		setStringSenderId(stringSenderId: number): void;

	}

	export interface TransactionBuilder {
		new(): Transaction;
		decode(buffer: ArrayBuffer): Transaction;
		//decode(buffer: NodeBuffer) : Transaction;
		//decode(buffer: ByteArrayBuffer) : Transaction;
		decode64(buffer: string): Transaction;

	}
}