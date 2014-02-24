 module ThingModel {
	 export class BuildANewThingType {
		 public static Named(name: string): ThingTypePropertyBuilder {
			 return new ThingTypePropertyBuilder(new ThingType(name));
		 }
	 }

	 export class ThingTypePropertyBuilder {
		 private type: ThingType;
		 private nextPropertyIsNotRequired: boolean;
		 private lastPropertyAdded: boolean;
		 private lastProperty: PropertyType;

		 public ContainingA : ThingTypePropertyBuilder;
		 public ContainingAn : ThingTypePropertyBuilder;
		 public AndA : ThingTypePropertyBuilder;
		 public AndAn : ThingTypePropertyBuilder;
		 
		 constructor(type: ThingType) {
			 this.type = type;

			 this.ContainingA = this;
			 this.ContainingAn = this;
			 this.AndA = this;
			 this.AndAn = this;
		 }

		 public get NotRequired(): ThingTypePropertyBuilder {
			 this.nextPropertyIsNotRequired = true;
			 return this;
		 }

		 public WhichIs(description: string) : ThingTypePropertyBuilder {
			 if (!this.lastProperty) {
				 this.type.Description = description;
			 } else {
				 this.lastProperty.Description = description;
			 }

			 return this;
		 }

		 private _createProperty(key: string, name: string, type: Type): void {

			 if (!this.lastPropertyAdded && this.lastProperty != null) {
				 this.type.DefineProperty(this.lastProperty);
				 this.lastPropertyAdded = false;
			 }

			 var prop = new PropertyType(key, type, true);
			 prop.Name = name;

			 if (this.nextPropertyIsNotRequired) {
				 this.nextPropertyIsNotRequired = false;
				 prop.Required = false;
			 }

			 this.lastProperty = prop;
		 }

		 public String(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.String);
			 return this;
		 }

		 public Location(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.Location);
			 return this;
		 }

		 public Double(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.Double);
			 return this;
		 }

		 public Int(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.Int);
			 return this;
		 }

		 public Boolean(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.Boolean);
			 return this;
		 }

		 public DateTime(key: string, name?: string): ThingTypePropertyBuilder {
			 this._createProperty(key, name, Type.DateTime);
			 return this;
		 }

		 public CopyOf(otherType: ThingType): ThingTypePropertyBuilder {
			 _.each(otherType.Properties, (propertyType : PropertyType)=> {
				 this.type.DefineProperty(propertyType.Clone());
			 });
			 return this;
		 }

		 public Build(): ThingType {
			 if (!this.lastPropertyAdded && this.lastProperty != null) {
				 this.type.DefineProperty(this.lastProperty);
			 }

			 return this.type;
		 }
	 }
 }