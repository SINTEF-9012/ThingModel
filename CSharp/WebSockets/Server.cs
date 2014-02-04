using System;
using ThingModel.Client;
using ThingModel.Proto;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ThingModel.WebSockets
{
    public class Server
    {
        public const string ServerSenderID = "ThingModel C# Broadcaster";
        protected Wharehouse Wharehouse;

        protected class ServerService : WebSocketService
        {
            protected ToProtobuf ToProtobuf;
            protected FromProtobuf FromProtobuf;
            protected Wharehouse Wharehouse;
            protected ProtoModelObserver ProtoModelObserver;

            public ServerService(Wharehouse wharehouse)
            {
                Wharehouse = wharehouse;

                ProtoModelObserver = new ProtoModelObserver();
                Wharehouse.RegisterObserver(ProtoModelObserver);

                ToProtobuf = new ToProtobuf();
                FromProtobuf = new FromProtobuf(wharehouse);
            }

            protected override void OnOpen()
            {
                var transaction = ToProtobuf.Convert(Wharehouse.Things, new Thing[0], new ThingType[0], ServerSenderID);
                Send(transaction);
            }

            protected override void OnMessage(MessageEventArgs e)
            { 
                if (e.Type == Opcode.BINARY)
                {
                    ProtoModelObserver.Reset();

                    var senderID = FromProtobuf.Convert(e.RawData, true);
                    Console.WriteLine("Server | Message from : " + senderID);
                    
                    var analyzedTransaction = ProtoModelObserver.GetTransaction(ToProtobuf, senderID);
                    
                    

                    // Broadcast to other clients
                    foreach (var session in Sessions.Sessions)
                    {
                        if (session != this)
                        {
                            var s = session as ServerService;
                            if (s != null)
                            {
                                s.Send(analyzedTransaction);
                            }
                        }
                    }   
                }
                
            }

            public void Send(Transaction transaction)
            {
                var protoData = ToProtobuf.Convert(transaction);
                Send(protoData);
            }
        }

        private WebSocketServer _ws;

        public Server(string path)
        {
            var house = new Wharehouse();
            _ws = new WebSocketServer(path);
            _ws.AddWebSocketService("/", () => new ServerService(house));
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