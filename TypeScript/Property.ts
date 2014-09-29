module ThingModel {

	export class Property {

		/* protected */ _key: string;

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

		public Clone(): Property {
			return new Property(this._key, this._value);
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

				public Clone(): Property.Location.Point {
					var p = new ThingModel.Location.Point(
						this._value.X, this._value.Y, this._value.Z);

					if (this._value.System) {
						p.System = this._value.System;
					}

					return new Property.Location.Point(
						this._key,p);
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

				public Clone(): Property.Location.LatLng {
					var p = new ThingModel.Location.LatLng(
						this._value.X, this._value.Y, this._value.Z);

					if (this._value.System) {
						p.System = this._value.System;
					}

					return new Property.Location.LatLng(
						this._key,p);
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

				public Clone(): Property.Location.Equatorial {
					var p = new ThingModel.Location.Equatorial(
						this._value.X, this._value.Y, this._value.Z);

					if (this._value.System) {
						p.System = this._value.System;
					}

					return new Property.Location.Equatorial(
						this._key,p);
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

			public Clone(): Property.String {
				return new Property.String(this._key, this._value);
			}
		}

		export class Double extends Property {
			constructor(key: string, value?: number) {
				super(key, value);
				if (this._value == null) {
					this._value = 0.0;
				}
			}

			public get Value(): number {
				return this._value;
			}

			public set Value(value: number) {
				this._value = value == null ? 0.0 : value;
			}

			public get Type(): Type {
				return Type.Double;
			}

			public Clone(): Property.Double {
				return new Property.Double(this._key, this._value);
			}
		}

		export class Int extends Property {
			constructor(key: string, value?: number) {
				super(key, value);
				if (this._value == null) {
					this._value = 0;
				}
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

			public Clone(): Property.Int {
				return new Property.Int(this._key, this._value);
			}
		}

		export class Boolean extends Property {
			constructor(key: string, value?: boolean) {
				super(key, !!value);
			}

			public get Value(): boolean {
				return this._value;
			}

			public set Value(value: boolean) {
				this._value = !!value;
			}

			public get Type(): Type {
				return Type.Boolean;
			}

			public Clone(): Property.Boolean {
				return new Property.Boolean(this._key, this._value);
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

			public Clone(): Property.Double {
				return new Property.Double(this._key, this._value ?
					new Date(this._value.getTime()) : this._value);
			}
		}

	}

}
