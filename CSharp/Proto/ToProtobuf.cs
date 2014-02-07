#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

#endregion

namespace ThingModel.Proto
{
    public class ToProtobuf
    {
        // String declarations dictionary, for the sender side
        protected readonly IDictionary<string, int> StringDeclarations = new Dictionary<string, int>();

        // Current transaction object
        // It will be filled step by step
        protected Transaction Transaction;

        // Simple binding between ThingModel property types and protocol buffer property types
        // Tested as alternative to the dynamic keyword
        private readonly Dictionary<Type, PropertyType.Type> _prototypesBinding = new Dictionary
            <Type, PropertyType.Type>
            {
                {typeof (ThingModel.Property.Location), PropertyType.Type.LOCATION},
                {typeof (ThingModel.Property.String), PropertyType.Type.STRING},
                {typeof (ThingModel.Property.Double), PropertyType.Type.DOUBLE},
                {typeof (ThingModel.Property.Int), PropertyType.Type.INT},
                {typeof (ThingModel.Property.Boolean), PropertyType.Type.BOOLEAN},
                {typeof (ThingModel.Property.DateTime), PropertyType.Type.DATETIME},
            };

        // When a string is in this collection, it should be sent
        // as StringDeclaration for the next transaction
        private readonly HashSet<string> _stringToDeclare = new HashSet<string>();

        private readonly Dictionary<int, Thing> _thingsState = new Dictionary<int, Thing>();
        private readonly Dictionary<Tuple<int, int>, Property> _propertiesState = new Dictionary<Tuple<int, int>, Property>();

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

        public Transaction Convert(IEnumerable<ThingModel.Thing> publish, IEnumerable<ThingModel.Thing> delete,
                                   IEnumerable<ThingModel.ThingType> declarations, string senderID)
        {
            Transaction = new Transaction();

            Transaction.string_sender_id = StringToKey(senderID);

            ConvertDeleteList(delete);

            foreach (var thing in publish)
            {
                ConvertThing(thing);
            }

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

        protected void ConvertThing(ThingModel.Thing thing)
        {
            var change = false;

            var publication = new Thing
                {
                    string_id = StringToKey(thing.ID),
                    string_type_name = thing.Type != null ? StringToKey(thing.Type.Name) : 0,
                    connections_change = false
                };
            
            Thing previousThing;
            _thingsState.TryGetValue(publication.string_id, out previousThing);

            if (previousThing == null)
            {
                change = true;
            }

	        IList<ThingModel.Thing> connectedThingsCache = null;

            if ((previousThing == null && thing.ConnectedThingsCount > 0)
                || (previousThing != null && previousThing.connections.Count != thing.ConnectedThingsCount))
            {
                publication.connections_change = true;
            }
            else
            {
	            connectedThingsCache = thing.ConnectedThings;
                foreach (var connectedThing in connectedThingsCache)
                {
                    var connectionKey = StringToKey(connectedThing.ID);

                    if (previousThing == null || !previousThing.connections.Contains(connectionKey))
                    {
                        publication.connections_change = true;
                    }
                }
            }
            
            // If we don't have changes on the connection list,
            // it's useless to send it
            if (publication.connections_change)
            {
                change = true;
	            if (connectedThingsCache == null)
	            {
		            connectedThingsCache = thing.ConnectedThings;
	            }

                foreach (var connectedThing in connectedThingsCache)
                {
                    var connectionKey = StringToKey(connectedThing.ID);
                    publication.connections.Add(connectionKey);
                }
                
            }

            foreach (var property in thing.GetProperties())
            {
                var proto = new Property
                    {
                        string_key = StringToKey(property.Key)
                    };

                ConvertProperty((dynamic) property, proto);

                var key = new Tuple<int, int>(publication.string_id, proto.string_key);

                if (previousThing != null)
                {
                    Property previousProto;
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
                             (previousProto.string_value != null && proto.string_value != null && ((
                              previousProto.string_value.string_value == proto.string_value.string_value &&
                              previousProto.string_value.value == proto.string_value.value) ||
                              (previousProto.string_value.value == property.ValueToString())))))
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
                _thingsState[publication.string_id] = publication;
            }
        }

        protected void ConvertDeleteList(IEnumerable<ThingModel.Thing> publish)
        {
            foreach (var thing in publish)
            {
                var key = StringToKey(thing.ID);
                Transaction.things_remove_list.Add(key);
				ManageThingSuppression(key);
            }
        }

        protected void ConvertDeclarationList(IEnumerable<ThingModel.ThingType> declarations)
        {
            foreach (var thingType in declarations)
            {
                var declaration = new ThingType
                    {
                        string_name = StringToKey(thingType.Name),
                        string_description = StringToKey(thingType.Description)
                    };

                foreach (var propertyType in thingType.GetProperties())
                {
                    declaration.properties.Add(new PropertyType
                        {
                            string_key = StringToKey(propertyType.Key),
                            string_name = StringToKey(propertyType.Name),
                            string_description = StringToKey(propertyType.Description),
                            required = propertyType.Required,
                            type = _prototypesBinding[propertyType.Type]
                        });
                }

                Transaction.thingtypes_declaration_list.Add(declaration);
            }
        }

        protected void ConvertProperty(ThingModel.Property.Location property, Property proto)
        {
            var value = property.Value;

            if (value is Location.LatLng)
            {
                proto.type = Property.Type.LOCATION_LATLNG;
            }
            else if (value is Location.Point)
            {
                proto.type = Property.Type.LOCATION_POINT;
            }
            else
            {
                proto.type = Property.Type.LOCATION_EQUATORIAL;    
            }
            
            proto.location_value = new Property.Location
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

        protected void ConvertProperty(ThingModel.Property.String property, Property proto)
        {
            var value = property.Value;

            proto.type = Property.Type.STRING;
            proto.string_value = new Property.String();
            
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

        protected void ConvertProperty(ThingModel.Property.Double property, Property proto)
        {
            proto.type = Property.Type.DOUBLE;
            proto.double_value = property.Value;
        }

        protected void ConvertProperty(ThingModel.Property.Int property, Property proto)
        {
            proto.type = Property.Type.INT;
            proto.int_value = property.Value;
        }

        protected void ConvertProperty(ThingModel.Property.Boolean property, Property proto)
        {
            proto.type = Property.Type.BOOLEAN;
            proto.boolean_value = property.Value;
        }

        protected void ConvertProperty(ThingModel.Property.DateTime property, Property proto)
        {
            proto.type = Property.Type.DATETIME;
            proto.datetime_value = property.Value.Ticks;
        }

	    protected void ManageThingSuppression(int thingId)
	    {
		    _thingsState.Remove(thingId);

		    var toRemove = new List<Tuple<int, int>>();
		    foreach (var tuple in _propertiesState.Keys)
		    {
			    if (tuple.Item1 == thingId)
			    {
				    toRemove.Add(tuple);
			    }
		    }

		    foreach (var tuple in toRemove)
		    {
			    _propertiesState.Remove(tuple);
		    }
	    }

	    public void ApplyThingSuppressions(IEnumerable<ThingModel.Thing> things)
	    {
		    foreach (var thing in things)
		    {
				int key;
				if (StringDeclarations.TryGetValue(thing.ID, out key))
				{
					ManageThingSuppression(key);
				}
		    }
	    }
    }
}