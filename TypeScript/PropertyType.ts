 module ThingModel {
	 export class PropertyType {
		 public Key: string;
		 public Name: string;
		 public Description: string;

		 public Required: boolean;

		 private _type: Type;

		 public get Type() {
			 return this._type;
		 }

		constructor(key: string, type: Type, required: boolean = true) {
			this.Key = key;
			this._type = type;
			this.Required = required;
		}

		public Check(property: Property): boolean {
			return (!this.Required && property == null) ||
				(property != null && property.Type == this._type &&
				property.Key == this.Key);
		}
	 }
 }