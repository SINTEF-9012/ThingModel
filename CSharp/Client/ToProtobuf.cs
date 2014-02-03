#region

using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
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

        private readonly Dictionary<int, Proto.Thing> _thingsState = new Dictionary<int, Proto.Thing>();
        private readonly Dictionary<Tuple<int, int>, Proto.Property> _propertiesState = new Dictionary<Tuple<int, int>, Proto.Property>();

        protected int StringToKey(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

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

            foreach (var thing in publish)
            {
                Convert(thing);
            }

            ConvertDeleteList(delete);
            ConvertDeclarationList(declarations);

            return Transaction;
        }


        private readonly MemoryStream _memoryInput = new MemoryStream();

        public byte[] Convert(Transaction transaction)
        {
            _memoryInput.SetLength(0);
            Serializer.Serialize(_memoryInput, transaction);
            return _memoryInput.ToArray();
        }

        protected Proto.Thing Convert(Thing thing)
        {
            var change = false;

            var publication = new Proto.Thing
                {
                    string_id = StringToKey(thing.ID),
                    string_type_name = thing.Type != null ? StringToKey(thing.Type.Name) : 0,
                    connections_change = false
                };
            
            Proto.Thing previousThing;
            _thingsState.TryGetValue(publication.string_id, out previousThing);

            if (previousThing == null)
            {
                change = true;
            }

            foreach (var connectedThing in thing.ConnectedThings)
            {
                var connectionKey = StringToKey(connectedThing.ID);
                publication.connections.Add(connectionKey);

                if (previousThing == null || !previousThing.connections.Contains(connectionKey))
                {
                    publication.connections_change = true;
                }
            }

            if (previousThing != null && publication.connections_change == false)
            {
                if (previousThing.connections.Count != publication.connections.Count)
                {
                    publication.connections_change = true;
                }
                else
                {
                    foreach (var key in previousThing.connections)
                    {
                        if (!publication.connections.Contains(key))
                        {
                            publication.connections_change = true;
                            break;
                        }
                    }
                }
            }
            
            // If we don't have changes on the connection list,
            // it's useless to send it
            if (!publication.connections_change)
            {
                if (publication.connections.Count > 0)
                {
                    publication.connections.Clear();    
                }
                
            }
            else
            {
                change = true;
            }

            foreach (var property in thing.GetProperties())
            {
                var proto = new Proto.Property
                    {
                        string_key = StringToKey(property.Key)
                    };

                ConvertProperty((dynamic) property, proto);

                var key = new Tuple<int, int>(publication.string_id, proto.string_key);

                if (previousThing != null)
                {
                    Proto.Property previousProto;
                    if (_propertiesState.TryGetValue(key, out previousProto))
                    {
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                        // Boring but necessary ?
                        if (previousProto.type == proto.type &&
                            previousProto.boolean_value == proto.boolean_value &&
                            previousProto.datetime_value == proto.datetime_value &&
                            previousProto.double_value == proto.double_value &&
                            previousProto.int_value == proto.int_value &&
                            ((previousProto.location_value == null && proto.location_value == null) ||
                             (previousProto.location_value != null && proto.location_value != null &&
                              previousProto.location_value.x == proto.location_value.x &&
                              previousProto.location_value.y == proto.location_value.y &&
                              previousProto.location_value.z == proto.location_value.z &&
                              previousProto.location_value.z_null == proto.location_value.z_null &&
                              previousProto.location_value.string_system == proto.location_value.string_system)) &&
                            ((previousProto.string_value == null && proto.string_value == null) ||
                             (previousProto.string_value != null && proto.string_value != null &&
                              previousProto.string_value.string_value == proto.string_value.string_value &&
                              previousProto.string_value.value == proto.string_value.value)))
                        {
                            continue;
                        }

                        // ReSharper restore CompareOfFloatsByEqualityOperator
                                    
                    }

                    
                }

                change = true;
                
                publication.properties.Add(proto);
                _propertiesState[key] = proto;
            }

            if (change)
            {
                Transaction.things_publish_list.Add(publication);
                if (previousThing == null)
                {
                    _thingsState.Add(publication.string_id, publication);
                }
            }
            

            return publication;
        }

        protected void ConvertDeleteList(IEnumerable<Thing> publish)
        {
            foreach (var thing in publish)
            {
                var key = StringToKey(thing.ID);
                Transaction.things_remove_list.Add(key);
                _thingsState.Remove(key);
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
                    string_system = StringToKey(value.System),
                    z_null = value.Z == null
                };

            if (value.Z != null)
            {
                proto.location_value.z = (double) value.Z;
            }
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