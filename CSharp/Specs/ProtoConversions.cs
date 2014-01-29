
using NUnit.Framework;
using ThingModel.Client;

namespace ThingModel.Specs
{
    [TestFixture]
    public class ProtoConversions
    {
        private Wharehouse _wharehouse;
        private FromProtobuf _fromProtobuf;
        private ToProtobuf _toProtobuf;

        [SetUp]
        protected void SetUp()
        {
            _wharehouse = new Wharehouse();
            _fromProtobuf = new FromProtobuf(_wharehouse);
            _toProtobuf = new ToProtobuf();
        }

        [Test]
        public void HelloWorld()
        {
            
            var message = new Thing("first");
            message.SetProperty(new Property.String("content", "Hello World"));

            var transaction = _toProtobuf.Convert(new Thing[] {message}, new Thing[0], new ThingType[0], "tests");

            var senderId = _fromProtobuf.Convert(transaction);
            Assert.That(senderId, Is.EqualTo("tests"));

            var newMessage = _wharehouse.GetThing("first");
            Assert.That(newMessage, Is.Not.Null);

            Assert.That(message.ID, Is.EqualTo(newMessage.ID));

            Assert.That(newMessage.GetProperty<Property.String>("content"), Is.Not.Null);
            Assert.That(newMessage.GetProperty<Property.String>("content").Value,
                Is.EqualTo("Hello World"));
        }

        [Test]
        public void HelloWorldType()
        {
            var type = new ThingType("message");
            type.DefineProperty(PropertyType.Create<Property.String>("content"));
            type.Description = "Just a simple text message";

            var transaction = _toProtobuf.Convert(new Thing[0], new Thing[0], new []{type}, "tests");

            _fromProtobuf.Convert(transaction);

            var newType = _wharehouse.GetThingType("message");

            Assert.That(newType, Is.Not.Null);

            Assert.That(newType.Description, Is.EqualTo(type.Description));

            Assert.That(newType.GetPropertyDefinition("content"), Is.Not.Null);
            Assert.That(newType.GetPropertyDefinition("content").Type, Is.EqualTo(typeof (Property.String)));
        }
    }
}