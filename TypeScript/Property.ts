module ThingModel {

	export class Property {

		private _key: string;

		Value: any;

		public get Key(): string {
			return this._key;
		}

		public get Type(): Type {
			return Type.Unknown;
		}

		constructor(key: string, value: any) {
			if (!key) {
				throw "The Property key should not be null or empty";
			}
			this._key = key;
			this.Value = value;
		}

		ValueToString(): string {
			return this.Value != null ? this.Value.ToString() : "";
		}

		CompareValue(other: Property): boolean {
			if (other == null || (this.Value != null && other.Value == null)) {
				return false;
			}

			if (this.Value != null && other.Value != null) {
				if (this.Value.Compare && other.Value.Compare) {
					return this.Value.Compare(other.Value);
				} else {
					return this.Value === other.Value;
				}
			}	

			return false;
		}
	}

	export module Property {

		export class Location extends Property {
			constructor(key: string, value?: ThingModel.Location) {
				super(key, value);
			}

			public get Value(): ThingModel.Location {
				return this.Value;
			}

			public set Value(value: ThingModel.Location) {
				this.Value = value;
			}

			public get Type(): Type {
				return Type.Location;
			}
		}

		export class String extends Property {

			constructor(key: string, value?: string) {
				super(key, value);
			}

			public get Value(): string {
				return this.Value;
			}
	
			public set Value(value: string) {
				this.Value = value;
			}

			public get Type(): Type {
				return Type.String;
			}
		}

		export class Double extends Property {
			constructor(key: string, value?: number) {
				super(key, value);
			}

			public get Value(): number {
				return this.Value;
			}
	
			public set Value(value: number) {
				this.Value = value;
			}

			public get Type(): Type {
				return Type.Double;
			}
		}

		export class Int extends Property {
			constructor(key: string, value?: number) {
				super(key, value);
			}

			public get Value(): number {
				return this.Value;
			}
	
			public set Value(value: number) {
				this.Value = Math.round(value);
			}

			public get Type(): Type {
				return Type.Int;
			}
		}

		export class Boolean extends Property {
			constructor(key: string, value?: boolean) {
				super(key, value);
			}

			public get Value(): boolean {
				return this.Value;
			}
	
			public set Value(value: boolean) {
				this.Value = value;
			}

			public get Type(): Type {
				return Type.Boolean;
			}
		}


		export class DateTime extends Property {
			constructor(key: string, value?: Date) {
				super(key, value);
			}

			public get Value(): Date {
				return this.Value;
			}
	
			public set Value(value: Date) {
				this.Value = value;
			}

			public get Type(): Type {
				return Type.DateTime;
			}
		}

	}

}