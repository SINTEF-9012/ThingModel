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
        private readonly Warehouse _warehouse;
        private ToProtobuf _toProtobuf;
        private FromProtobuf _fromProtobuf;
        private bool _closed = true;
        private int _reconnectionDelay = 1;
	    private bool _reconnection = false;
        
        private readonly ProtoModelObserver _thingModelObserver;
		private readonly object _lock = new Object();

        public Client(string senderID, string path, Warehouse warehouse)
        {
            SenderID = senderID;

            _warehouse = warehouse;

            _thingModelObserver = new ProtoModelObserver();
            _warehouse.RegisterObserver(_thingModelObserver);

            _fromProtobuf = new FromProtobuf(warehouse);
            _toProtobuf = new ToProtobuf();

            _ws = new WebSocket(path);

            _ws.OnMessage += WsOnMessage;
            _ws.OnClose += WsOnClose;
			_ws.OnOpen += WsOnOpen;

            Connect();
        }

	    private void WsOnOpen(object sender, EventArgs eventArgs)
	    {
            _fromProtobuf = new FromProtobuf(_warehouse);
            _toProtobuf = new ToProtobuf();

		    _reconnectionDelay = 1;
		    Send();
	    }

	    public virtual void Send()
        {
			lock (_lock) {
				if (_closed)
				{
					Console.WriteLine("Does not send, waiting for connexion");
					return;
				}

				if (_thingModelObserver.SomethingChanged())
				{
					var transaction = _thingModelObserver.GetTransaction(_toProtobuf, SenderID,
						_reconnection);
					_ws.Send(_toProtobuf.Convert(transaction));
					_thingModelObserver.Reset();
					_reconnection = false;
				}
			}
        }

        protected void Send(string message)
        {
            lock (_lock)
            {
                _ws.Send(message);
            }
        }

        private void WsOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            if (!_closed)
            {
                Console.WriteLine("Connection lost, try to connect again in "+_reconnectionDelay+" seconds");
	            
				_reconnection = true;

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
			lock (_lock)
			{
				if (args.Type == Opcode.Binary)
				{
					try
					{
						var senderName = _fromProtobuf.Convert(args.RawData);
						if (senderName == "undefined")
						{
							Console.WriteLine("Something went wrong : received a undefined senderID");
						}
						else
						{
							_toProtobuf.ApplyThingsSuppressions(_thingModelObserver.Deletions);
							Console.WriteLine(SenderID + " | Binary message from : " + senderName);
						}
						_thingModelObserver.Reset();
					}
					catch (Exception e)
					{
						Console.WriteLine(SenderID + " | Big exception when receiving the message : " + e.Message);
					}
                }
                else if (args.Type == Opcode.Text)
                {
                    WsOnStringMessage(args.Data);
                }
			}
        }

        protected virtual void WsOnStringMessage(string message)
        {
            Console.WriteLine("ThingModelClient has received a string: "+message);
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
            _ws.Log.Level = LogLevel.Info;
        }
    }
}
