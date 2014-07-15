module ThingModel {
	export interface Location {
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

	export module Location {
		/**
		 *	Simple location value.
		 *	Often used in videogames.
		 */
		export class Point implements Location {
			public X: number;
			public Y: number;
			public Z: number;
			public System: string;
			public type: string;

			// The last argument can be null, it's a feature
			constructor(x: number = 0.0, y: number = 0.0, z: number = null) {
				this.X = x;
				this.Y = y;
				this.Z = z;
				this.type = "point";
			}

			// Check if the values are the same and if the representation is the same
			public Compare(other: Location) {
				return other != null &&
					other.X === this.X &&
					other.Y === this.Y &&
					other.Z == this.Z &&
					other.System == this.System;
			}

			public toString() {
				var s = this.X + " - " + this.Y;

				if (this.Z != null) {
					s += " - " + this.Z;
				}

				if (this.System != null) {
					s += " -- " + this.System;
				}
				return s;
			}
		}

		/**
		 *	Latitude longitude representation, often used for WGS 84 GPS localizations.
		 *	And if the System property is null, it's considered by default as a WGS 84 LatLng system.
		 */
		export class LatLng extends Point {

			constructor(latitude: number = 0.0, longitude: number = 0.0, altitude: number = null) {
				super(latitude, longitude, altitude);
				this.type = "latlng";
			}

			public get Latitude() {
				return this.X;
			}

			public set Latitude(latitude: number) {
				this.X = latitude;
			}

			public get Longitude() {
				return this.Y;
			}

			public set Longitude(longitude: number) {
				this.Y = longitude;
			}

			public get Altitude() {
				return this.Z;
			}

			public set Altitude(altitude: number) {
				this.Z = altitude;
			}

		}

		/**
		 *	Representations in space.
		 */
		export class Equatorial extends Point {

			constructor(rightAscension: number = 0.0, declination: number = 0.0, hourAngle: number = 0.0) {
				super(rightAscension, declination, hourAngle);
				this.type = "equatorial";
			}

			public get RightAscension() {
				return this.X;
			}

			public set RightAscension(rightAscension: number) {
				this.X = rightAscension;
			}

			public get Declination() {
				return this.Y;
			}

			public set Declination(declination: number) {
				this.Y = declination;
			}

			public get HourAngle() {
				return this.Y;
			}

			public set HourAngle(hourAngle: number) {
				this.Z = hourAngle;
			}

		}
	}
}