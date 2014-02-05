module ThingModel {
	export class ThingType {

	}

    export interface Property {
        /* get */ Key: string;

        ValueToString(): string;

        Compare(other: Property): boolean;
    }
    
	export module Property {

		export class Location implements Property {
			private _key: string;

			public get Key(): string {
				return this._key;
			}

			public ValueToString() {
				return "canard";
			}

			public Compare(other: Property) {
				return false;
			}

		}

	}
//    export class Property {
//        private _key: string;
//
//		public get Key(): string {
//		    return this._key;
//        }
//
//        constructor(key : string) {
//            this._key = key;
//        }
//	}
}