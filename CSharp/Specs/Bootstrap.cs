using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThingModel.Builders;

namespace ThingModel.Specs
{
    [TestFixture]
    class Bootstrap
    {
        [Test]
        public void BootStrap()
        {
            var thing = BuildANewThingType.Named("Thing")
                .ContainingA.String("id")
                .AndA.NotRequired.String("typename");

            var thingProperties = BuildANewThingType.Named("Property")
                .ContainingA.String("key")
                .AndA.NotRequired.String("value_string")
                .AndA.NotRequired.Int("value_int")
                .AndA.NotRequired.Double("value_double")
                .AndA.NotRequired.Boolean("value_boolean")
                .AndA.NotRequired.DateTime("value_datetime")
                .AndA.NotRequired.LocationLatLng("value_location");

            var thingType = BuildANewThingType.Named("Thing type")
                .ContainingA.NotRequired.String("name")
                .AndA.NotRequired.String("description");

            var propertyType = BuildANewThingType.Named("Property type")
                .ContainingA.String("key")
                .AndA.NotRequired.String("name")
                .AndA.NotRequired.String("description")
                .AndA.Boolean("required")
                .AndA.String("type");

            // thingProperties mustt contain one of the notrequired properties
            // propertyType type should be a value in string/location/double/int/boolean/datetime
            // the properties should be connected to their things
            // the properties types should be connected to their things type
        }
    }
}
