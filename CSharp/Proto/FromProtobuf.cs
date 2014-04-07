using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace ThingModel.Proto
{
    internal class FromProtobuf
    {
        private static readonly Dictionary<PropertyType.Type, Type> PrototypesBinding = new Dictionary
            <PropertyType.Type, Type>
            {
                {PropertyType.Type.LOCATION, typeof (ThingModel.Property.Location)},
                {PropertyType.Type.STRING, typeof (ThingModel.Property.String)},
                {PropertyType.Type.DOUBLE, typeof (ThingModel.Property.Double)},
                {PropertyType.Type.INT, typeof (ThingModel.Property.Int)},
                {PropertyType.Type.BOOLEAN, typeof (ThingModel.Property.Boolean)},
                {PropertyType.Type.DATETIME, typeof (ThingModel.Property.DateTime)},
            };

        protected readonly IDictionary<int,string> StringDeclarations = new Dictionary<int, string>();

        protected string KeyToString(int key)
        {
            if (key == 0)
            {
                return "";
            }

            string value;
            return StringDeclarations.TryGetValue(key, out value) ? value : "undefined";
        }

        protected Warehouse Warehouse;

        public FromProtobuf(Warehouse warehouse)
        {
            Warehouse = warehouse;
        }

        private readonly MemoryStream _memoryInput = new MemoryStream();

        public string Convert(byte[] data, bool check = false)
        {
            _memoryInput.Write(data, 0, data.Length);
            _memoryInput.Position = 0;
            var transaction = Serializer.Deserialize<Transaction>(_memoryInput);
            
            _memoryInput.SetLength(0);

            return Convert(transaction, check);
        }

        public string Convert(Transaction transaction, bool check = false)
        {
            transaction.string_declarations.ForEach(ConvertStringDeclaration);

			var thingsToDelete = new HashSet<ThingModel.Thing>();

			foreach (var thingKey in transaction.things_remove_list)
			{
				var id = KeyToString(thingKey);
				thingsToDelete.Add(Warehouse.GetThing(id));
			}
			Warehouse.RemoveCollection(thingsToDelete);

            transaction.thingtypes_declaration_list.ForEach(ConvertThingTypeDeclaration);

            var thingsToConnect = new List<Tuple<ThingModel.Thing, Thing>>();
            
            foreach (var thing in transaction.things_publish_list)
            {
                var modelThing = ConvertThingPublication(thing, check);
                
                if (thing.connections_change)
                {
                    thingsToConnect.Add(new Tuple<ThingModel.Thing, Thing>(modelThing, thing));
                }
            }

            foreach (var tuple in thingsToConnect)
            {
                tuple.Item1.Detach();

                foreach (var connection in tuple.Item2.connections)
                {
                    var t = Warehouse.GetThing(KeyToString(connection));
                    if (t != null)
                    {
                        tuple.Item1.Connect(t);    
                    }
                    
                }

                Warehouse.RegisterThing(tuple.Item1, false);    
            }

            var senderId = KeyToString(transaction.string_sender_id);

            return senderId;
        }

        protected void ConvertStringDeclaration(StringDeclaration declaration)
        {
            StringDeclarations[declaration.key] = declaration.value;
        }

        protected void ConvertThingTypeDeclaration(ThingType thingType)
        {
            var modelType = new ThingModel.ThingType(KeyToString(thingType.string_name));
            modelType.Description = KeyToString(thingType.string_description);

            foreach (var propertyType in thingType.properties)
            {
                var type = PrototypesBinding[propertyType.type];
                var modelProperty = new ThingModel.PropertyType(KeyToString(propertyType.string_key),
                                                        type);
                modelProperty.Description = KeyToString(propertyType.string_description);
                modelProperty.Name = KeyToString(propertyType.string_name);
                modelProperty.Required = propertyType.required;

                modelType.DefineProperty(modelProperty);
            }
            Warehouse.RegisterType(modelType);
        }

        protected ThingModel.Thing ConvertThingPublication(Thing thing, bool check)
        {
            ThingModel.ThingType type = null;
            if (thing.string_type_name != 0)
            {
                // Here the type can be null if the string type name is not registered
                // in the Warehouse (can be an error from a client)

                // So, if the type is null, it rolls back to the default type
                // It's a robust model
                type = Warehouse.GetThingType(KeyToString(thing.string_type_name));
            }

            var id = KeyToString(thing.string_id);
            
            var modelThing = Warehouse.GetThing(id);
               
            if (modelThing == null || (
				modelThing.Type == null && type != null ||
				type == null && modelThing.Type != null ||
				(modelThing.Type != null && type != null && modelThing.Type.Name != type.Name)))
            {
                modelThing = new ThingModel.Thing(id, type);
            }

            foreach (var property in thing.properties)
            {
                ThingModel.Property modelProperty = null;
                string key = KeyToString(property.string_key);
                Location location = null;

                switch (property.type)
                {
                    case Property.Type.LOCATION_POINT:
                        location = new Location.Point();

                        // C# imposes a break statements when a case is not empty
                        // So in 2014 I am using a goto instruction
                        // for the first time in my life
                        goto case Property.Type.LOCATION_EQUATORIAL;
                        
                    case Property.Type.LOCATION_LATLNG:
                        location = new Location.LatLng();
                        goto case Property.Type.LOCATION_EQUATORIAL;

                    case Property.Type.LOCATION_EQUATORIAL:
                        if (location == null)
                        {
                            location = new Location.Equatorial();
                        }
                        
                        if (property.location_value != null)
                        {

                            location.X = property.location_value.x;
                            location.Y = property.location_value.y;
                            
                            if (!property.location_value.z_null)
                            {
                                location.Z = property.location_value.z;
                            }
                            
                        }

                        modelProperty = new ThingModel.Property.Location(key, location);
                        break;

                    case Property.Type.STRING:
		                string value;
		                if (property.string_value != null)
		                {
			                value = property.string_value.value;
			                if (String.IsNullOrEmpty(value) && property.string_value.string_value != 0)
			                {
				                value = KeyToString(property.string_value.string_value);
			                }
		                }
		                else
		                {
			                value = "undefined";
		                }
                        modelProperty = new ThingModel.Property.String(key, value);
                        break;

                    case Property.Type.DOUBLE:
                        modelProperty = new ThingModel.Property.Double(key, property.double_value);
                        break;

                    case Property.Type.INT:
                        modelProperty = new ThingModel.Property.Int(key, property.int_value);
                        break;

                    case Property.Type.BOOLEAN:
                        modelProperty = new ThingModel.Property.Boolean(key, property.boolean_value);
                        break;

                    case Property.Type.DATETIME:
		                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		                modelProperty = new ThingModel.Property.DateTime(key,
			                dtDateTime.AddMilliseconds(property.datetime_value));
                        break;
                }
                
                modelThing.SetProperty(modelProperty);
                
            }

            if (check && type != null && !type.Check(modelThing))
            {
                Console.WriteLine("Object «" + id + "» not valid, ignored");
            }
            else if (!thing.connections_change)
            {
                Warehouse.RegisterThing(modelThing, false);    
            }
            
            return modelThing;
        }
    }
}