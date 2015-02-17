using System;
using System.Collections.Generic;
using System.Globalization;
using ThingModel.Proto;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ThingModel.WebSockets
{
    public class Server
    {
        public const string ServerSenderID = "ThingModel C# Broadcaster";

	    public delegate void TransactionEventHandler(object sender, TransactionEventArgs e);

	    public event TransactionEventHandler Transaction;

	    private bool _debug = false;

	    public class TransactionEventArgs : EventArgs
	    {
			public byte[] Message { private set; get; }
			public string SenderID { private set; get; }
			public string EndPoint { private set; get; }

		    public TransactionEventArgs(string senderID, string endPoint, byte[] message)
		    {
			    SenderID = senderID;
			    EndPoint = endPoint;
			    Message = message;
		    }
	    }

        private class ServerService : WebSocketBehavior
        {
            private readonly ToProtobuf _toProtobuf;
            private readonly FromProtobuf _fromProtobuf;
            private readonly Warehouse _warehouse;
            private readonly ProtoModelObserver _protoModelObserver;
	        private readonly Server _server;
	        private readonly object _lock;
	        private bool _strictServer; 

            public ServerService(Warehouse warehouse, ProtoModelObserver observer, object uglyLock, bool strictServer, Server server)
            {
                _warehouse = warehouse;
	            _server = server;
	            _strictServer = strictServer;

	            _protoModelObserver = observer;

                _toProtobuf = new ToProtobuf();
                _fromProtobuf = new FromProtobuf(warehouse);
	            _lock = uglyLock;
            }

            protected override void OnOpen()
            {
	            Transaction transaction;
				lock (_lock)
				{
					transaction = _toProtobuf.Convert(_warehouse.Things, new Thing[0], _warehouse.ThingTypes, ServerSenderID);
				}
				Send(transaction);
            }

            protected override void OnMessage(MessageEventArgs e)
            {
				lock (_lock)
				{
					if (e.Type == Opcode.Binary)
					{
						_protoModelObserver.Reset();

						string senderID;
						try
						{
							senderID = _fromProtobuf.Convert(e.RawData, _strictServer);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							return;
						}

						if (senderID == "undefined")
						{
							Console.WriteLine("Undefined senderIDs are not allowed");
							return;
						}

						_toProtobuf.ApplyThingsSuppressions(_protoModelObserver.Deletions);

						// Broadcast to other clients
						if (_protoModelObserver.SomethingChanged())
						{
							foreach (var session in Sessions.Sessions)
							{
								if (session != this)
								{
									var s = session as ServerService;
									if (s != null)
									{
										var transaction = _protoModelObserver.GetTransaction(s._toProtobuf, senderID);
										s.Send(s._toProtobuf.Convert(transaction));
									}
								}
							}
						}

                        // Send the events
						if (_server.Transaction != null)
						{
							try
							{
								_server.Transaction(this,
									new TransactionEventArgs(senderID, Context.UserEndPoint.ToString(), e.RawData));
							}
							catch (Exception)
							{ }
						}
					}
				}
            }

            private void Send(Transaction transaction)
            {
				lock (_lock)
				{
					var protoData = _toProtobuf.Convert(transaction);
					Send(protoData);

					if (_server.Transaction != null)
					{
						_server.Transaction(this,
							new TransactionEventArgs(ServerSenderID, Context.ServerEndPoint.ToString(), protoData));
					}
				}
            }
        }

        private readonly WebSocketServer _ws;

        public Server(string path, string endpoint = "/", bool strictServer = true)
        {
            var house = new Warehouse();
            var observer = new ProtoModelObserver();
			house.RegisterObserver(observer);
			var uglyLock = new object();

	        Transaction += (sender, args) =>
	        {
		        Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " | Server | Message from : " +
		                          args.SenderID +
		                          " | " + args.EndPoint.ToString(CultureInfo.InvariantCulture) + " | " + args.Message.Length +
		                          " bytes");
		        if (_debug)
		        {
			        Console.WriteLine("DEBUG: " + Convert.ToBase64String(args.Message));
		        }
	        };

            _ws = new WebSocketServer(path);
            _ws.AddWebSocketService(endpoint, () => new ServerService(house, observer, uglyLock, strictServer, this));
            _ws.Start();


        }

        public void Close()
        {
            _ws.Stop();
        }

        public void Debug()
        {
	        _debug = true;
            _ws.Log.Level = LogLevel.Info;
        }
    }
}