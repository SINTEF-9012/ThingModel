#region

using System;
using System.Collections.Generic;
using ThingModel.Proto;

#endregion

namespace ThingModel.Client
{
    public class ToProtobuf
    {
        // String declarations dictionary, in the sender side
        protected readonly IDictionary<string, int> StringDeclarations = new Dictionary<string, int>();

        // Current transaction object
        // It will be filled step by step
        protected Transaction Transaction;

        // Simple binding between ThingModel property types and protocol buffer property types
        // Tested as alternative to the dynamic keyword
        private readonly Dictionary<Type, Proto.PropertyType.Type> _prototypesBinding = new Dictionary
            <Type, Proto.PropertyType.Type>
            {
                {typeof (Property.Location), Proto.PropertyType.Type.LOCATION},
                {typeof (Property.String), Proto.PropertyType.Type.STRING},
                {typeof (Property.Double), Proto.PropertyType.Type.DOUBLE},
                {typeof (Property.Int), Proto.PropertyType.Type.INT},
                {typeof (Property.Boolean), Proto.PropertyType.Type.BOOLEAN},
                {typeof (Property.DateTime), Proto.PropertyType.Type.DATETIME},
            };

        // When a string is in this collection, it should be sended
        // as an int key with the StringToKey method
        private readonly HashSet<string> _stringToDeclare = new HashSet<string>();

        protected int StringToKey(string value)
        {
            int key;
            if (StringDeclarations.TryGetValue(value, out key))
            {
                return key;
            }

            key = StringDeclarations.Count + 1;

            StringDeclarations[value] = key;

            Transaction.string_declarations.Add(new StringDeclaration
                {
                    key = key,
                    value = value
                });

            return key;
        }

        public Transaction Convert(IEnumerable<Thing> publish, IEnumerable<Thing> delete,
                                   IEnumerable<ThingType> declarations, string senderID)
        {
            Transaction = new Transaction();

            Transaction.string_sender_id = StringToKey(senderID);

            ConvertPublishList(publish);
            ConvertDeleteList(delete);
            ConvertDeclarationList(declarations);

            return Transaction;
        }

        protected void ConvertPublishList(IEnumerable<Thing> publish)
        {
            foreach (var thing in publish)
            {
                var publication = new Proto.Thing
                    {
                        string_id = StringToKey(thing.ID),
                        string_type_name = thing.Type != null ? StringToKey(thing.Type.Name) : 0
                    };

                foreach (var connectedThing in thing.ConnectedThings)
                {
                    publication.connections.Add(StringToKey(connectedThing.ID));
                }

                foreach (var property in thing.GetProperties())
                {
                    var proto = new Proto.Property
                        {
                            string_key = StringToKey(property.Key)
                        };

                    ConvertProperty((dynamic) property, proto);
                }

                Transaction.things_publish_list.Add(publication);
            }
        }

        protected void ConvertDeleteList(IEnumerable<Thing> publish)
        {
            foreach (var thing in publish)
            {
                Transaction.things_remove_list.Add(StringToKey(thing.ID));
            }
        }

        protected void ConvertDeclarationList(IEnumerable<ThingType> declarations)
        {
            foreach (var thingType in declarations)
            {
                var declaration = new Proto.ThingType
                    {
                        string_name = StringToKey(thingType.Name),
                        description = thingType.Description
                    };

                foreach (var propertyType in thingType.GetProperties())
                {
                    declaration.properties.Add(new Proto.PropertyType
                        {
                            string_key = StringToKey(propertyType.Key),
                            name = propertyType.Name,
                            description = propertyType.Description,
                            required = propertyType.Required,
                            type = _prototypesBinding[propertyType.Type]
                        });
                }

                Transaction.thingtypes_declaration_list.Add(declaration);
            }
        }

        protected void ConvertProperty(Property.Location property, Proto.Property proto)
        {
            var value = property.Value;

            if (value is Location.LatLng)
            {
                proto.type = Proto.Property.Type.LOCATION_LATLNG;
            }
            else if (value is Location.Point)
            {
                proto.type = Proto.Property.Type.LOCATION_POINT;
            }
            else
            {
                proto.type = Proto.Property.Type.LOCATION_EQUATORIAL;    
            }
            
            proto.location_value = new Proto.Property.Location
                {
                    x = value.X,
                    y = value.Y,
                    z = value.Z == null ? 0.0 : (double) value.Z,
                    string_system = StringToKey(value.System),
                };
        }

        protected void ConvertProperty(Property.String property, Proto.Property proto)
        {
            var value = property.Value;

            proto.type = Proto.Property.Type.STRING;
            proto.string_value = new Proto.Property.String();
            
            if (_stringToDeclare.Contains(value))
            {
                proto.string_value.string_value = StringToKey(value);
            }
            else
            {
                proto.string_value.value = value;

                // Use string to key the next time
                _stringToDeclare.Add(value);
            }
        }

        protected void ConvertProperty(Property.Double property, Proto.Property proto)
        {
            proto.type = Proto.Property.Type.DOUBLE;
            proto.double_value = property.Value;
        }

        protected void ConvertProperty(Property.Int property, Proto.Property proto)
        {
            proto.type = Proto.Property.Type.INT;
            proto.int_value = property.Value;
        }

        protected void ConvertProperty(Property.Boolean property, Proto.Property proto)
        {
            proto.type = Proto.Property.Type.BOOLEAN;
            proto.boolean_value = property.Value;
        }

        protected void ConvertProperty(Property.DateTime property, Proto.Property proto)
        {
            proto.type = Proto.Property.Type.DATETIME;
            proto.datetime_value = property.Value.Ticks;
        }
    }
}