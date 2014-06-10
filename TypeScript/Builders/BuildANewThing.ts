 module ThingModel {
	 export class BuildANewThing {
		 private type: ThingType;

		 /* private */ constructor(type: ThingType) {
			 this.type = type;
		 }

		 public static get WithoutType(): BuildANewThing {
			 return new BuildANewThing(null);
		 }

		 public static As(type : ThingType): BuildANewThing {
			 return new BuildANewThing(type);
		 }

		 public IdentifiedBy(id: string) : ThingPropertyBuilder {
			 return new ThingPropertyBuilder(new Thing(id, this.type));
		 }
	 }

	 export class ThingPropertyBuilder {
		 private thing: Thing;
		 public ContainingA : ThingPropertyBuilder;
		 public ContainingAn : ThingPropertyBuilder;
		 public AndA : ThingPropertyBuilder;
		 public AndAn : ThingPropertyBuilder;

		 constructor(thing: Thing) {
			 this.thing = thing;
			 this.ContainingA = this;
			 this.ContainingAn = this;
			 this.AndA = this;
			 this.AndAn = this;
		 }

		 public String(key: string, value: string): ThingPropertyBuilder {
			 this.thing.SetProperty(new Property.String(key, value));
			 return this;
		 }

		 public Double(key: string, value: number): ThingPropertyBuilder {
			 this.thing.SetProperty(new Property.Double(key, value));
			 return this;
		 }

		 public Int(key: string, value: number): ThingPropertyBuilder {
			 this.thing.SetProperty(new Property.Int(key, value));
			 return this;
		 }

		 public Boolean(key: string, value: boolean): ThingPropertyBuilder {
			 this.thing.SetProperty(new Property.Boolean(key, value));
			 return this;
		 }

		 public DateTime(key: string, value: Date): ThingPropertyBuilder {
			 this.thing.SetProperty(new Property.DateTime(key, value));
			 return this;
		 }

		 public Location(value: Location):	 ThingPropertyBuilder;
		 public Location(key: string, value: Location): ThingPropertyBuilder;
		 public Location(mixed: any, value?: Location): ThingPropertyBuilder {

			 var key;
			 if (value) {
				 key = mixed;
			 } else {
				 key = "location";
				 value = mixed;
			 }

			 switch(value.type) {
				 case "equatorial":
					this.thing.SetProperty(new Property.Location.Equatorial(key, <Location.Equatorial>value));
					break;
				 case "latlng":
					this.thing.SetProperty(new Property.Location.LatLng(key,<Location.LatLng>value));
					 break;
				 case "point":
				default :
					this.thing.SetProperty(new Property.Location.Point(key, <Location.Point>value));
					break;

			 }
			 return this;
		 }

		 public Build(): Thing {
			 return this.thing;
		 }
	 }
 }