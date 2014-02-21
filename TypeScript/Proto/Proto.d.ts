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
		type: Property.Type;
		location_value?: Property.Location;
		string_value?: Property.String;
		double_value?: number;
		int_value?: number;
		boolean_value?: boolean;
		datetime_value?: number;
		
	}
	
	export interface PropertyBuilder {
		new(): Property;
		decode(buffer: ArrayBuffer) : Property;
		//decode(buffer: NodeBuffer) : Property;
		//decode(buffer: ByteArrayBuffer) : Property;
		decode64(buffer: string) : Property;
		Location: Property.LocationBuilder;
		String: Property.StringBuilder;
		Type: Property.Type;
		
	}	
}

declare module ThingModel.Proto.Property {

	export interface Location extends ProtoBufModel {
		x: number;
		y: number;
		z?: number;
		string_system?: number;
		z_null?: boolean;
		
	}
	
	export interface LocationBuilder {
		new(): Location;
		decode(buffer: ArrayBuffer) : Location;
		//decode(buffer: NodeBuffer) : Location;
		//decode(buffer: ByteArrayBuffer) : Location;
		decode64(buffer: string) : Location;
		
	}	
}

declare module ThingModel.Proto.Property {

	export interface String extends ProtoBufModel {
		value?: string;
		string_value?: number;
		
	}
	
	export interface StringBuilder {
		new(): String;
		decode(buffer: ArrayBuffer) : String;
		//decode(buffer: NodeBuffer) : String;
		//decode(buffer: ByteArrayBuffer) : String;
		decode64(buffer: string) : String;
		
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
		type: PropertyType.Type;
		required: boolean;
		string_name?: number;
		string_description?: number;
		
	}
	
	export interface PropertyTypeBuilder {
		new(): PropertyType;
		decode(buffer: ArrayBuffer) : PropertyType;
		//decode(buffer: NodeBuffer) : PropertyType;
		//decode(buffer: ByteArrayBuffer) : PropertyType;
		decode64(buffer: string) : PropertyType;
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
		key: number;
		
	}
	
	export interface StringDeclarationBuilder {
		new(): StringDeclaration;
		decode(buffer: ArrayBuffer) : StringDeclaration;
		//decode(buffer: NodeBuffer) : StringDeclaration;
		//decode(buffer: ByteArrayBuffer) : StringDeclaration;
		decode64(buffer: string) : StringDeclaration;
		
	}	
}

declare module ThingModel.Proto {

	export interface Thing extends ProtoBufModel {
		string_id: number;
		string_type_name: number;
		properties: Property[];
		connections: number[];
		connections_change?: boolean;
		
	}
	
	export interface ThingBuilder {
		new(): Thing;
		decode(buffer: ArrayBuffer) : Thing;
		//decode(buffer: NodeBuffer) : Thing;
		//decode(buffer: ByteArrayBuffer) : Thing;
		decode64(buffer: string) : Thing;
		
	}	
}

declare module ThingModel.Proto {

	export interface ThingType extends ProtoBufModel {
		string_name: number;
		string_description?: number;
		properties: PropertyType[];
		
	}
	
	export interface ThingTypeBuilder {
		new(): ThingType;
		decode(buffer: ArrayBuffer) : ThingType;
		//decode(buffer: NodeBuffer) : ThingType;
		//decode(buffer: ByteArrayBuffer) : ThingType;
		decode64(buffer: string) : ThingType;
		
	}	
}

declare module ThingModel.Proto {

	export interface Transaction extends ProtoBufModel {
		string_declarations: StringDeclaration[];
		things_publish_list: Thing[];
		things_remove_list: number[];
		thingtypes_declaration_list: ThingType[];
		string_sender_id: number;
		
	}
	
	export interface TransactionBuilder {
		new(): Transaction;
		decode(buffer: ArrayBuffer) : Transaction;
		//decode(buffer: NodeBuffer) : Transaction;
		//decode(buffer: ByteArrayBuffer) : Transaction;
		decode64(buffer: string) : Transaction;
		
	}	
}

