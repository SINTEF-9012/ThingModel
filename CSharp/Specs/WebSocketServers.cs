using System;
using System.Threading;
using NUnit.Framework;
using ThingModel.WebSockets;

namespace ThingModel.Specs
{
    [TestFixture]
    class WebSocketServers
    {
        private Server _server;

        private WebSockets.Client _clientA;
        private WebSockets.Client _clientB;
        
        private Wharehouse _wharehouseA;
        private Wharehouse _wharehouseB;

        private class WharehouseWait : IThingObserver
        {
            private readonly AutoResetEvent _newEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _deleteEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _updatedEvent = new AutoResetEvent(false);

            public void New(Thing thing)
            {
                _newEvent.Set();
            }

            public void Deleted(Thing thing)
            {
                _deleteEvent.Set();
            }

            public void Updated(Thing thing)
            {
                _updatedEvent.Set();
            }

            public bool WaitNew(int millisecondsTimeout = 500)
            {
                return _newEvent.WaitOne(millisecondsTimeout);
            }

            public bool WaitDeleted(int millisecondsTimeout = 500)
            {
                return _deleteEvent.WaitOne(millisecondsTimeout);
            }

            public bool WaitUpdated(int millisecondsTimeout = 500)
            {
                return _updatedEvent.WaitOne(millisecondsTimeout);
            }

            public void Reset()
            {
                _newEvent.Reset();
                _deleteEvent.Reset();
                _updatedEvent.Reset();
            }
        }

        private WharehouseWait _wharehouseWait;

        [SetUp]
        protected void SetUp()
        {
            _wharehouseA = new Wharehouse();
            _wharehouseB = new Wharehouse();

            const string path = "ws://localhost:4251/";

            _server = new Server(path);
            

            _clientA = new WebSockets.Client("UnitTestA", path, _wharehouseA);
            _clientB = new WebSockets.Client("UnitTestB", path, _wharehouseB);

            _wharehouseWait = new WharehouseWait();
            _wharehouseB.RegisterObserver(_wharehouseWait);

            //_server.Debug();
            //_clientA.Debug();
        }

        [TearDown]
        protected void TearDown()
        {
            _clientA.Close();
            _clientB.Close();
            _server.Close();
        }

        [Test]
        public void TestConnection()
        {
            Assert.That(_clientA.IsConnected(), Is.True);
        }

        [Test]
        public void TestNew()
        {
            var thing = new Thing("lapin");
            _wharehouseA.RegisterThing(thing);
            _clientA.Send();

            Assert.That(_wharehouseWait.WaitNew(), Is.True);

            Assert.That(_wharehouseB.GetThing("lapin"), Is.Not.Null);
        }
    
        [Test]
        public void SaveState()
        {
            _clientB.Close();
            _wharehouseA.RegisterThing(new Thing("boat"));
            _clientA.Send();
            _clientB.Connect();

            Assert.That(_wharehouseWait.WaitNew(), Is.True);
            Assert.That(_wharehouseB.GetThing("boat"), Is.Not.Null);
        }
    }
}
