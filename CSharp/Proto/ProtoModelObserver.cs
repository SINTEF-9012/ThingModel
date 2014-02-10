using System.Collections.Generic;

namespace ThingModel.Proto
{
    public class ProtoModelObserver : IWharehouseObserver
    {
	    private readonly object _lockHashSet = new object();
        public readonly HashSet<ThingModel.Thing> Updates = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.Thing> Deletions = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.ThingType> Definitions = new HashSet<ThingModel.ThingType>(); 

        public void Reset()
       { 
			lock (_lockHashSet) {
				Updates.Clear();
				Deletions.Clear();
				Definitions.Clear();
			}
        }

        public void New(ThingModel.Thing thing)
        {
	        lock (_lockHashSet)
	        {
		        Updates.Add(thing);
	        }
        }

        public void Deleted(ThingModel.Thing thing)
        {
	        lock (_lockHashSet)
	        {
		        Deletions.Add(thing);
	        }
        }

        public void Updated(ThingModel.Thing thing)
        {
	        lock (_lockHashSet)
	        {
		        Updates.Add(thing);
	        }
        }

        public void Define(ThingModel.ThingType thing)
        {
	        lock (_lockHashSet)
	        {
		        Definitions.Add(thing);
	        }
        }

        public Transaction GetTransaction(ToProtobuf toProtobuf, string senderID)
        {
	        lock (_lockHashSet)
	        {
		        return toProtobuf.Convert(new List<ThingModel.Thing>(Updates), new List<ThingModel.Thing>(Deletions),
			        new List<ThingModel.ThingType>(Definitions), senderID);
	        }
        }
    }
}