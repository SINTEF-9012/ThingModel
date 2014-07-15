module ThingModel {
	export class PropertyType {
		private _key: string;

		public get Key() {
			return this._key;
		}

		public Name: string;
		public Description: string;

		public Required: boolean;

		private _type: Type;

		public get Type() {
			return this._type;
		}

		constructor(key: string, type: Type, required: boolean = true) {
			if (!key) {
				throw "The PropertyType key should not be null or empty";
			}
			this._key = key;
			this._type = type;
			this.Required = required;
		}

		public Check(property: Property): boolean {
			return (!this.Required && property == null) ||
			(property != null && property.Type == this._type &&
				property.Key == this.Key);
		}

		public Clone(): PropertyType {
			var newProp = new PropertyType(this._key, this._type, this.Required);
			newProp.Name = this.Name;
			this.Description = this.Description;
			return newProp;
		}
	}
}