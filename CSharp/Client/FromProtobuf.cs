using System;
using System.Collections.Generic;
using ThingModel.Proto;

namespace ThingModel.Client
{
    public class FromProtobuf
    {
        private readonly Dictionary<Proto.PropertyType.Type, Type> _prototypesBinding = new Dictionary
            <Proto.PropertyType.Type, Type>
            {
                {Proto.PropertyType.Type.LOCATION, typeof (Property.Location)},
                {Proto.PropertyType.Type.STRING, typeof (Property.String)},
                {Proto.PropertyType.Type.DOUBLE, typeof (Property.Double)},
                {Proto.PropertyType.Type.INT, typeof (Property.Int)},
                {Proto.PropertyType.Type.BOOLEAN, typeof (Property.Boolean)},
                {Proto.PropertyType.Type.DATETIME, typeof (Property.DateTime)},
            };

        protected readonly IDictionary<int,string> StringDeclarations = new Dictionary<int, string>();

        protected string KeyToString(int key)
        {
            string value;
            return StringDeclarations.TryGetValue(key, out value) ? value : "undefined";
        }

        public string Convert(Transaction transaction, Wharehouse wharehouse)
        {
            ConvertDeclarationList(transaction.string_declarations);
            ConvertDeleteList(wharehouse, transaction.things_remove_list);
            ConvertThingTypesDeclarationList(wharehouse, transaction.thingtypes_declaration_list);

            var senderId = KeyToString(transaction.string_sender_id);

            return senderId;
        }

        protected void ConvertDeclarationList(IList<StringDeclaration> declarations)
        {
            foreach (var declaration in declarations)
            {
                StringDeclarations.Add(declaration.key, declaration.value);
            }
        }

        public void ConvertDeleteList(Wharehouse wharehouse, IList<int> thingKeys)
        {
            foreach (var thingKey in thingKeys)
            {
                var id = KeyToString(thingKey);
                wharehouse.RemoveThing(wharehouse.GetThing(id));
            }
        }

        public void ConvertThingTypesDeclarationList(Wharehouse wharehouse, IList<Proto.ThingType> types)
        {
            foreach (var thingType in types)
            {
                var modelType = new ThingType(KeyToString(thingType.string_name));
                modelType.Description = thingType.description;

                foreach (var propertyType in thingType.properties)
                {
                    var type = _prototypesBinding[propertyType.type];
                    var modelProperty = new PropertyType(KeyToString(propertyType.string_key),
                                                          type);
                    modelProperty.Description = propertyType.description;
                    modelProperty.Name = propertyType.name;
                    modelProperty.Required = propertyType.required;

                    modelType.DefineProperty(modelProperty);
                }
                wharehouse.RegisterType(modelType, true);
            }
        }
    }
}