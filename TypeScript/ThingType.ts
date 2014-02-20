module ThingModel {

	export class ThingType {
		private _name: string;

		public get Name(): string {
			return this._name;
		}

		public Description: string;

		private _properties: { [key: string]: PropertyType };

		constructor(name: string) {
			if (!name) {
				throw new Error("The name should not be null or empty");
			}
			this._name = name;
			this._properties = {};
		}

		public Check(thing: Thing): boolean {
			return (thing.Type === this ||
				(thing.Type !== null && thing.Type._name === this._name)) &&
				_.all(this._properties, (propertyType: PropertyType) => {
						return propertyType.Check(thing.GetProperty<Property>(propertyType.Key));
					}
				);
		}

		public DefineProperty(property: PropertyType): void {
			this._properties[property.Key] = property;
		}

		public GetPropertyDefinition(key: string): PropertyType {
			return this._properties[key];
		}

		public get Properties(): PropertyType[]{
			return _.values(this._properties);
		}
	}
}