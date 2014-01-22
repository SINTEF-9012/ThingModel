#region

using System;
using NUnit.Framework;

#endregion

namespace ThingModel.Specs
{
    [TestFixture]
    class WharehouseTest
    {
        private ThingType _type;
        private Thing _thing;
        private Wharehouse _wharehouse;

        [SetUp]
        protected void SetUp()
        {
            _type = new ThingType("duck");
            _type.DefineProperty(PropertyType.Create<Property.String>("name"));
            _type.DefineProperty(PropertyType.Create<Property.Number>("age"));

            _thing = new Thing("871", _type);
            _thing.SetProperty(new Property.String("name", "Maurice"));
            _thing.SetProperty(new Property.Number("age", 18));
            _thing.SetProperty(new Property.Location("localization", new Location.Point(10,44)));

            _wharehouse = new Wharehouse();
        }

        [Test]
        public void RegisterType()
        {
            _wharehouse.RegisterType(_type);
            Assert.Throws<Exception>(() => _wharehouse.RegisterType(null));

        }

        [Test]
        public void RegisterThing()
        {
            _wharehouse.RegisterThing(_thing, false, false);
            Assert.Throws<Exception>(() => _wharehouse.RegisterThing(null));
        }
    }

}
