using System.Collections.Generic;

namespace ThingModel.Proto
{
    public class ProtoModelObserver : IThingModelObserver
    {
        public readonly HashSet<ThingModel.Thing> Updates = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.Thing> Deletions = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.ThingType> Definitions = new HashSet<ThingModel.ThingType>(); 

        public void Reset()
        {
            Updates.Clear();
            Deletions.Clear();
            Definitions.Clear();
        }

        public void New(ThingModel.Thing thing)
        {
            Updates.Add(thing);
        }

        public void Deleted(ThingModel.Thing thing)
        {
            Deletions.Add(thing);
        }

        public void Updated(ThingModel.Thing thing)
        {
            Updates.Add(thing);
        }

        public void Define(ThingModel.ThingType thing)
        {
            Definitions.Add(thing);
        }

        public Transaction GetTransaction(ToProtobuf toProtobuf, string senderID)
        {
            return toProtobuf.Convert(new List<ThingModel.Thing>(Updates), new List<ThingModel.Thing>(Deletions), new List<ThingModel.ThingType>(Definitions), senderID);
        }
    }
}