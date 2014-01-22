using System;
using System.Collections.Generic;

namespace ThingModel
{
    class Wharehouse
    {
        private readonly Dictionary<string, ThingType> _thingTypes = new Dictionary<string, ThingType>();

        private readonly Dictionary<string, Thing> _things = new Dictionary<string, Thing>();
 
        public void RegisterType(ThingType type)
        {
            if (type == null)
            {
                throw new Exception("The thing type information is missing.");
            }

            if (!_thingTypes.ContainsKey(type.Name))
            {
                _thingTypes[type.Name] = type;   
            }
        }

        public void RegisterThing(Thing thing, bool alsoRegisterConnections = true, bool alsoRegisterTypes = true)
        {
            if (thing == null)
            {
                throw new Exception("Null are not allowed in the wharehouse.");
            }



            _things[thing.ID] = thing;
            if (alsoRegisterTypes)
            {
                RegisterType(thing.Type);
            }

            if (alsoRegisterConnections)
            {
                foreach (var connectedThing in thing.ConnectedThings)
                {
                    RegisterThing(connectedThing, true, alsoRegisterTypes);
                }
            }
        }


    }
}
