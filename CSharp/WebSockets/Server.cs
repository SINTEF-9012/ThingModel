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
            protected ProtoObserver ProtoObserver;

            public ServerService(Wharehouse wharehouse)
            {
                Wharehouse = wharehouse;

                ProtoObserver = new ProtoObserver();
                Wharehouse.RegisterObserver(ProtoObserver);

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

                    var senderID = FromProtobuf.Convert(e.RawData);
                    Console.WriteLine("Server | Message from : " + senderID);
                    
                    var analyzedTransaction = ProtoObserver.GetTransaction(ToProtobuf, senderID);
                    
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
            _ws = new WebSocketServer(path);
            _ws.AddWebSocketService("/", () => new ServerService(new Wharehouse()));
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