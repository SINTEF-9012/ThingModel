﻿using System.Collections.Generic;

namespace ThingModel.Proto
{
    internal class ProtoModelObserver : IWarehouseObserver
    {
	    private readonly object _lockHashSet = new object();
        public readonly HashSet<ThingModel.Thing> Updates = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.Thing> Deletions = new HashSet<ThingModel.Thing>();
        public readonly HashSet<ThingModel.ThingType> Definitions = new HashSet<ThingModel.ThingType>(); 
        public readonly HashSet<ThingModel.ThingType> PermanentDefinitions = new HashSet<ThingModel.ThingType>(); 

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
		        Updates.Remove(thing);
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

	    public bool SomethingChanged()
	    {
		    lock (_lockHashSet)
		    {
				return Updates.Count != 0 || Deletions.Count != 0 || Definitions.Count != 0;
		    }
	    }

        public Transaction GetTransaction(ToProtobuf toProtobuf, string senderID,
			bool allDefinitions = false)
        {
	        List<ThingModel.Thing> copyUpdates;
	        List<ThingModel.Thing> copyDeletions;
	        List<ThingModel.ThingType> copyDefinitions;

	        lock (_lockHashSet)
	        {
				copyUpdates = new List<ThingModel.Thing>(Updates);
				copyDeletions = new List<ThingModel.Thing>(Deletions);
				copyDefinitions = new List<ThingModel.ThingType>(allDefinitions ?
					PermanentDefinitions : Definitions);
	        }

	        return toProtobuf.Convert(copyUpdates, copyDeletions, copyDefinitions, senderID);
        }
    }
}