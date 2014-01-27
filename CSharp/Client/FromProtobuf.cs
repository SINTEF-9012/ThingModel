using System.Collections.Generic;
using ThingModel.Proto;

namespace ThingModel.Client
{
    public class FromProtobuf
    {
        protected readonly IDictionary<int,string> StringDeclarations = new Dictionary<int, string>();

        protected string KeyToString(int key)
        {
            string value;
            return StringDeclarations.TryGetValue(key, out value) ? value : "undefined";
        }

        public string Convert(Transaction transaction, Wharehouse wharehouse)
        {
            ConvertDeclarationList(transaction.string_declarations);

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
    }
}