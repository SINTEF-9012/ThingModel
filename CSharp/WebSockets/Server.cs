﻿using System;
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

        private class ServerService : WebSocketService
        {
            private readonly ToProtobuf _toProtobuf;
            private readonly FromProtobuf _fromProtobuf;
            private readonly Warehouse _warehouse;
            private readonly ProtoModelObserver _protoModelObserver;
	        private readonly Server _server;
			private readonly object _lock = new Object();

            public ServerService(Warehouse warehouse, Server server)
            {
                _warehouse = warehouse;
	            _server = server;

                _protoModelObserver = new ProtoModelObserver();
                _warehouse.RegisterObserver(_protoModelObserver);

                _toProtobuf = new ToProtobuf();
                _fromProtobuf = new FromProtobuf(warehouse);
            }

            protected override void OnOpen()
            {
				lock (_lock)
				{
					var transaction = _toProtobuf.Convert(_warehouse.Things, new Thing[0], _warehouse.ThingTypes, ServerSenderID);
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
								new TransactionEventArgs(senderID, Context.UserEndPoint.ToString(), e.RawData));
						}

						//                    var analyzedTransaction = ProtoModelObserver.GetTransaction(ToProtobuf, senderID);
						_toProtobuf.ApplyThingsSuppressions(_protoModelObserver.Deletions);

						// Broadcast to other clients
						Transaction transaction = null;
						var somethingChanged = false;
						foreach (var session in Sessions.Sessions)
						{
							if (session != this)
							{
								var s = session as ServerService;
								if (s != null)
								{
									if (transaction == null)
									{
										transaction = _protoModelObserver.GetTransaction(s._toProtobuf, senderID);
										somethingChanged = _protoModelObserver.SomethingChanged();
									}

									if (somethingChanged)
									{
										lock (s._lock)
										{
											s.Send(s._toProtobuf.Convert(transaction));
										}
									}
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
							new TransactionEventArgs(ServerSenderID, Context.ServerEndPoint.ToString(), protoData));
					}
				}
            }
        }

        private readonly WebSocketServer _ws;

        public Server(string path, string endpoint = "/")
        {
            var house = new Warehouse();

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
            _ws.AddWebSocketService(endpoint, () => new ServerService(house, this));
            _ws.Start();


        }

        public void Close()
        {
            _ws.Stop();
        }

        public void Debug()
        {
	        _debug = true;
            _ws.Log.Level = LogLevel.INFO;
        }
    }
}