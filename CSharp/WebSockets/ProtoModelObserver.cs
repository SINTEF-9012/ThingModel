using System.Collections.Generic;
using ThingModel.Client;
using ThingModel.Proto;

namespace ThingModel.WebSockets
{
    public class ProtoModelObserver : IThingModelObserver
    {
        public readonly HashSet<Thing> Updates = new HashSet<Thing>();
        public readonly HashSet<Thing> Deletions = new HashSet<Thing>();
        public readonly HashSet<ThingType> Definitions = new HashSet<ThingType>(); 

        public void Reset()
        {
            Updates.Clear();
            Deletions.Clear();
            Definitions.Clear();
        }

        public void New(Thing thing)
        {
            Updates.Add(thing);
        }

        public void Deleted(Thing thing)
        {
            Deletions.Add(thing);
        }

        public void Updated(Thing thing)
        {
            Updates.Add(thing);
        }

        public void Define(ThingType thing)
        {
            Definitions.Add(thing);
        }

        public Transaction GetTransaction(ToProtobuf toProtobuf, string senderID)
        {
            return toProtobuf.Convert(new List<Thing>(Updates), new List<Thing>(Deletions), new List<ThingType>(Definitions), senderID);
        }
    }
}