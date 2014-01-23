#region

using System;
using System.Collections.Generic;
using ThingModel.Proto;

#endregion

namespace ThingModel.Client
{
    public class ToProtobuf
    {
        // String declarations dictionary, for the sender side
        protected readonly IDictionary<string, int> StringDeclarations = new Dictionary<string, int>();

        // Current transaction object
        // It will be filled step by step
        protected Transaction Transaction = new Transaction();

        private readonly Dictionary<Type, Proto.PropertyType.Type> _prototypesBinding = new Dictionary<Type, Proto.PropertyType.Type>
            {
                {typeof(Property.Location), Proto.PropertyType.Type.LOCATION},
                {typeof(Property.String), Proto.PropertyType.Type.STRING},
                {typeof(Property.Double), Proto.PropertyType.Type.DOUBLE},
                {typeof(Property.Int), Proto.PropertyType.Type.INT},
                {typeof(Property.Boolean), Proto.PropertyType.Type.BOOLEAN},
                {typeof(Property.DateTime), Proto.PropertyType.Type.DATETIME},
            }; 

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
            Transaction.string_sender_id = StringToKey(senderID);

            ConvertPublishList(publish);
            ConvertDeleteList(delete);
            ConvertDeclarationList(declarations);

            var tmpTransaction = Transaction;

            Transaction = new Transaction();

            return tmpTransaction;
        }

        protected void ConvertPublishList(IEnumerable<Thing> publish)
        {
            
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
    }
}