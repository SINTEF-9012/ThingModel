using System;
using System.Threading;
using ThingModel.Proto;
using WebSocketSharp;

namespace ThingModel.WebSockets
{
    public class Client
    {
        public string SenderID;
        private WebSocket _ws;
        private Wharehouse _wharehouse;
        private ToProtobuf _toProtobuf;
        private FromProtobuf _fromProtobuf;
        private bool _close = true;
        private int _delayReconnection = 1;
        
        private ProtoModelObserver _thingModelObserver;

        public Client(string senderID, string path, Wharehouse wharehouse)
        {
            SenderID = senderID;

            _wharehouse = wharehouse;

            _thingModelObserver = new ProtoModelObserver();
            _wharehouse.RegisterObserver(_thingModelObserver);

            _fromProtobuf = new FromProtobuf(wharehouse);
            _toProtobuf = new ToProtobuf();

            _ws = new WebSocket(path);

            _ws.OnMessage += WsOnMessage;
            _ws.OnClose += WsOnClose;

            Connect();
        }

        public void Send()
        {
            var transaction = _thingModelObserver.GetTransaction(_toProtobuf, SenderID);
            _ws.Send(_toProtobuf.Convert(transaction));
            _thingModelObserver.Reset();
        }

        private void WsOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            if (!_close)
            {
                Console.WriteLine("Connection lost, try to connect again in "+_delayReconnection+" seconds");
                // ReSharper disable ObjectCreationAsStatement
                new Timer(state =>
                {
                    if (!_ws.IsAlive)
                    {
                        _ws.Connect();
                    }

                    // Increase the connection delay until 16 secondes
                    if (_delayReconnection < 16)
                    {
                        _delayReconnection *= 2;   
                    }
                },
                null, _delayReconnection*1, Timeout.Infinite);
                // ReSharper restore ObjectCreationAsStatement
            }
            
        }

        private void WsOnMessage(object sender, MessageEventArgs args)
        {
            if (args.Type == Opcode.BINARY)
            {
                var senderName = _fromProtobuf.Convert(args.RawData);
                Console.WriteLine(SenderID + " | Binary message from : " + senderName);
				_toProtobuf.ApplyThingSuppressions(_thingModelObserver.Deletions);
                _thingModelObserver.Reset();
            }
        }


        public void Close()
        {
            _close = true;
            _ws.Close();
        }

        public void Connect()
        {
            if (_close)
            {
                _close = false;
                _ws.Connect();
            }
        }

        public bool IsConnected()
        {
            return _ws.IsAlive;
        }

        public void Debug()
        {
            _ws.Log.Level = LogLevel.INFO;
        }
    }
}
