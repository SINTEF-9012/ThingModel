using System;
using System.Threading;
using ThingModel.Proto;
using WebSocketSharp;

namespace ThingModel.WebSockets
{
    public class Client
    {
        public string SenderID;
        private readonly WebSocket _ws;
		
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Wharehouse _wharehouse;
        private readonly ToProtobuf _toProtobuf;
        private readonly FromProtobuf _fromProtobuf;
        private bool _closed = true;
        private int _reconnectionDelay = 1;
        
        private readonly ProtoModelObserver _thingModelObserver;

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
	        if (_thingModelObserver.SomethingChanged())
	        {
				var transaction = _thingModelObserver.GetTransaction(_toProtobuf, SenderID);
				_ws.Send(_toProtobuf.Convert(transaction));
				_thingModelObserver.Reset();
	        }
        }

        private void WsOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            if (!_closed)
            {
                Console.WriteLine("Connection lost, try to connect again in "+_reconnectionDelay+" seconds");
                // ReSharper disable ObjectCreationAsStatement
                new Timer(state =>
                {
                    if (!_ws.IsAlive)
                    {
                        _ws.Connect();
                    }

                    // Increase the connection delay until 16 secondes
                    if (_reconnectionDelay < 16)
                    {
                        _reconnectionDelay *= 2;   
                    }
                },
                null, _reconnectionDelay*1, Timeout.Infinite);
                // ReSharper restore ObjectCreationAsStatement
            }
            
        }

        private void WsOnMessage(object sender, MessageEventArgs args)
        {
            if (args.Type == Opcode.BINARY)
            {
                var senderName = _fromProtobuf.Convert(args.RawData);
                Console.WriteLine(SenderID + " | Binary message from : " + senderName);
				_toProtobuf.ApplyThingsSuppressions(_thingModelObserver.Deletions);
                _thingModelObserver.Reset();
            }
        }


        public void Close()
        {
            _closed = true;
            _ws.Close();
        }

        public void Connect()
        {
            if (_closed)
            {
                _closed = false;
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
