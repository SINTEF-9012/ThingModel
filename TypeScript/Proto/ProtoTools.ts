/// <reference path="./Proto.d.ts"/>

module ThingModel.Proto {

	declare var dcodeIO: any;

	export class ProtoTools {
		public static Builder : ProtoBufBuilder
		= dcodeIO.ProtoBuf.loadJson({ "package": "ThingModel.Proto", "messages": [{ "name": "Property", "fields": [{ "rule": "required", "type": "int32", "name": "string_key", "id": 1, "options": {} }, { "rule": "required", "type": "Type", "name": "type", "id": 2, "options": { "default": "STRING" } }, { "rule": "optional", "type": "Location", "name": "location_value", "id": 3, "options": {} }, { "rule": "optional", "type": "String", "name": "string_value", "id": 4, "options": {} }, { "rule": "optional", "type": "double", "name": "double_value", "id": 5, "options": {} }, { "rule": "optional", "type": "sint32", "name": "int_value", "id": 6, "options": {} }, { "rule": "optional", "type": "bool", "name": "boolean_value", "id": 7, "options": {} }, { "rule": "optional", "type": "int64", "name": "datetime_value", "id": 8, "options": {} }], "enums": [{ "name": "Type", "values": [{ "name": "LOCATION_POINT", "id": 0 }, { "name": "LOCATION_LATLNG", "id": 1 }, { "name": "LOCATION_EQUATORIAL", "id": 2 }, { "name": "STRING", "id": 3 }, { "name": "DOUBLE", "id": 4 }, { "name": "INT", "id": 5 }, { "name": "BOOLEAN", "id": 6 }, { "name": "DATETIME", "id": 7 }], "options": {} }], "messages": [{ "name": "Location", "fields": [{ "rule": "required", "type": "double", "name": "x", "id": 1, "options": { "default": 0 } }, { "rule": "required", "type": "double", "name": "y", "id": 2, "options": { "default": 0 } }, { "rule": "optional", "type": "double", "name": "z", "id": 3, "options": { "default": 0 } }, { "rule": "optional", "type": "int32", "name": "string_system", "id": 4, "options": {} }, { "rule": "optional", "type": "bool", "name": "z_null", "id": 5, "options": { "default": false } }], "enums": [], "messages": [], "options": {} }, { "name": "String", "fields": [{ "rule": "optional", "type": "string", "name": "value", "id": 1, "options": {} }, { "rule": "optional", "type": "int32", "name": "string_value", "id": 2, "options": { "default": 0 } }], "enums": [], "messages": [], "options": {} }], "options": {} }, { "name": "PropertyType", "fields": [{ "rule": "required", "type": "int32", "name": "string_key", "id": 1, "options": {} }, { "rule": "required", "type": "Type", "name": "type", "id": 2, "options": { "default": "STRING" } }, { "rule": "required", "type": "bool", "name": "required", "id": 3, "options": { "default": true } }, { "rule": "optional", "type": "int32", "name": "string_name", "id": 4, "options": {} }, { "rule": "optional", "type": "int32", "name": "string_description", "id": 5, "options": {} }], "enums": [{ "name": "Type", "values": [{ "name": "LOCATION", "id": 0 }, { "name": "STRING", "id": 1 }, { "name": "DOUBLE", "id": 2 }, { "name": "INT", "id": 3 }, { "name": "BOOLEAN", "id": 4 }, { "name": "DATETIME", "id": 5 }], "options": {} }], "messages": [], "options": {} }, { "name": "StringDeclaration", "fields": [{ "rule": "required", "type": "string", "name": "value", "id": 1, "options": {} }, { "rule": "required", "type": "int32", "name": "key", "id": 2, "options": {} }], "enums": [], "messages": [], "options": {} }, { "name": "Thing", "fields": [{ "rule": "required", "type": "int32", "name": "string_id", "id": 1 }, { "rule": "required", "type": "int32", "name": "string_type_name", "id": 2 }, { "rule": "repeated", "type": "Property", "name": "properties", "id": 3 }, { "rule": "repeated", "type": "int32", "name": "connections", "id": 4, "options": { "packed": true } }, { "rule": "optional", "type": "bool", "name": "connections_change", "id": 5, "options": { "default": false } }], "enums": [], "messages": [] }, { "name": "ThingType", "fields": [{ "rule": "required", "type": "int32", "name": "string_name", "id": 1 }, { "rule": "optional", "type": "int32", "name": "string_description", "id": 2 }, { "rule": "repeated", "type": "PropertyType", "name": "properties", "id": 3 }], "enums": [], "messages": [] }, { "name": "Transaction", "fields": [{ "rule": "repeated", "type": "StringDeclaration", "name": "string_declarations", "id": 1 }, { "rule": "repeated", "type": "Thing", "name": "things_publish_list", "id": 2 }, { "rule": "repeated", "type": "int32", "name": "things_remove_list", "id": 3, "options": { "packed": true } }, { "rule": "repeated", "type": "ThingType", "name": "thingtypes_declaration_list", "id": 4 }, { "rule": "required", "type": "int32", "name": "string_sender_id", "id": 5 }], "enums": [], "messages": [] }], "enums": [], "imports": [], "services": [] })
			.build("ThingModel.Proto");

		constructor() {
		}
	 }

}