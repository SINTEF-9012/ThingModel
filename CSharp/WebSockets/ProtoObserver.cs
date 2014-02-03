using System.Collections.Generic;
using ThingModel.Client;
using ThingModel.Proto;

namespace ThingModel.WebSockets
{
    public class ProtoObserver : IThingObserver
    {
        public readonly HashSet<Thing> Updates = new HashSet<Thing>();
        public readonly HashSet<Thing> Deletions = new HashSet<Thing>();

        public void Reset()
        {
            Updates.Clear();
            Deletions.Clear();
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

        public Transaction GetTransaction(ToProtobuf toProtobuf, string senderID)
        {
            return toProtobuf.Convert(Updates, Deletions, new ThingType[0], senderID);
        }
    }
}