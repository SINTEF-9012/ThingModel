using System;
using System.Collections.Generic;
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

	    public class TransactionEventArgs : EventArgs
	    {
			public int Size { private set; get; }
			public string SenderID { private set; get; }
			public string EndPoint { private set; get; }

		    public TransactionEventArgs(string senderID, string endPoint, int size)
		    {
			    SenderID = senderID;
			    EndPoint = endPoint;
			    Size = size;
		    }
	    }

        private class ServerService : WebSocketService
        {
            private readonly ToProtobuf _toProtobuf;
            private readonly FromProtobuf _fromProtobuf;
            private readonly Wharehouse _wharehouse;
            private readonly ProtoModelObserver _protoModelObserver;
	        private readonly Server _server;
			private readonly object _lock = new Object();

            public ServerService(Wharehouse wharehouse, Server server)
            {
                _wharehouse = wharehouse;
	            _server = server;

                _protoModelObserver = new ProtoModelObserver();
                _wharehouse.RegisterObserver(_protoModelObserver);

                _toProtobuf = new ToProtobuf();
                _fromProtobuf = new FromProtobuf(wharehouse);
            }

            protected override void OnOpen()
            {
				lock (_lock)
				{
					var transaction = _toProtobuf.Convert(_wharehouse.Things, new Thing[0], _wharehouse.ThingTypes, ServerSenderID);
					Send(transaction);
				}
            }

            protected override void OnMessage(MessageEventArgs e)
            {
				lock (_lock)
				{
					if (e.Type == Opcode.BINARY)
					{
						_protoModelObserver.Reset();

						var senderID = _fromProtobuf.Convert(e.RawData, true);

						if (_server.Transaction != null)
						{
							_server.Transaction(this,
								new TransactionEventArgs(senderID, Context.UserEndPoint.ToString(), e.RawData.Length));
						}

						//                    var analyzedTransaction = ProtoModelObserver.GetTransaction(ToProtobuf, senderID);
						_toProtobuf.ApplyThingsSuppressions(_protoModelObserver.Deletions);

						// Broadcast to other clients
						foreach (var session in Sessions.Sessions)
						{
							if (session != this)
							{
								var s = session as ServerService;
								if (s != null)
								{
									var analyzedTransaction = s._toProtobuf.Convert(
										new List<Thing>(_protoModelObserver.Updates),
										new List<Thing>(_protoModelObserver.Deletions),
										new List<ThingType>(_protoModelObserver.Definitions),
										senderID);
									s.Send(analyzedTransaction);
								}
							}
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
							new TransactionEventArgs(ServerSenderID, Context.ServerEndPoint.ToString(), protoData.Length));
					}
				}
            }
        }

        private readonly WebSocketServer _ws;

        public Server(string path)
        {
            var house = new Wharehouse();

			Transaction += (sender, args) => 
				Console.WriteLine("Server | Message from : " + args.SenderID +
				" | "+ args.Size + " bytes");

            _ws = new WebSocketServer(path);
            _ws.AddWebSocketService("/", () => new ServerService(house, this));
            _ws.Start();


        }

        public void Close()
        {
            _ws.Stop();
        }

        public void Debug()
        {
            _ws.Log.Level = LogLevel.INFO;
        }
    }
}