module ThingModel {

	export class Property {

		private _key: string;

		/* protected */
		_value: any;

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
			this._value = value;
		}

		ValueToString(): string {
			return this._value != null ? this._value.toString() : "";
		}

		CompareValue(other: Property): boolean {
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
		}
	}

	export module Property {

		export module Location {

			export class Point extends Property {
				constructor(key: string, value?: ThingModel.Location.Point) {
					super(key, value);
				}

				public get Value(): ThingModel.Location.Point {
					return this._value;
				}

				public set Value(value: ThingModel.Location.Point) {
					this._value = value;
				}

				public get Type(): Type {
					return Type.LocationPoint;
				}
			}

			export class LatLng extends Property {
				constructor(key: string, value?: ThingModel.Location.LatLng) {
					super(key, value);
				}

				public get Value(): ThingModel.Location.LatLng {
					return this._value;
				}

				public set Value(value: ThingModel.Location.LatLng) {
					this._value = value;
				}

				public get Type(): Type {
					return Type.LocationLatLng;
				}
			}

			export class Equatorial extends Property {
				constructor(key: string, value?: ThingModel.Location.Equatorial) {
					super(key, value);
				}

				public get Value(): ThingModel.Location.Equatorial {
					return this._value;
				}

				public set Value(value: ThingModel.Location.Equatorial) {
					this._value = value;
				}

				public get Type(): Type {
					return Type.LocationEquatorial;
				}
			}
		}

		export class String extends Property {

			constructor(key: string, value?: string) {
				super(key, value);
			}

			public get Value(): string {
				return this._value;
			}

			public set Value(value: string) {
				this._value = value;
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
				return this._value;
			}

			public set Value(value: number) {
				this._value = value;
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
				return this._value;
			}

			public set Value(value: number) {
				this._value = Math.round(value);
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
				return this._value;
			}

			public set Value(value: boolean) {
				this._value = value;
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
				return this._value;
			}

			public set Value(value: Date) {
				this._value = value;
			}

			public get Type(): Type {
				return Type.DateTime;
			}
		}

	}

}