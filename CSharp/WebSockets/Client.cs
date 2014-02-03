﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ProtoBuf;
using ThingModel.Client;
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
        
        private ProtoObserver _thingObserver;

        public Client(string senderID, string path, Wharehouse wharehouse)
        {
            SenderID = senderID;

            _wharehouse = wharehouse;

            _thingObserver = new ProtoObserver();
            _wharehouse.RegisterObserver(_thingObserver);

            _fromProtobuf = new FromProtobuf(wharehouse);
            _toProtobuf = new ToProtobuf();

            _ws = new WebSocket(path);

            _ws.OnMessage += WsOnMessage;
            _ws.OnClose += WsOnClose;

            Connect();
        }

        public void Send()
        {
            var transaction = _thingObserver.GetTransaction(_toProtobuf, SenderID);
            _ws.Send(_toProtobuf.Convert(transaction));
            _thingObserver.Reset();
        }

        private void WsOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            if (!_close)
            {
                Console.WriteLine("Connection lost, try to connect again in 5 seconds");
                // ReSharper disable ObjectCreationAsStatement
                new Timer(state =>
                {
                    if (_ws.IsAlive)
                    {
                        _ws.Connect();
                    }
                },
                null, 5000, Timeout.Infinite);
                // ReSharper restore ObjectCreationAsStatement
            }
            
        }

        private void WsOnMessage(object sender, MessageEventArgs args)
        {
            if (args.Type == Opcode.BINARY)
            {
                var senderName = _fromProtobuf.Convert(args.RawData);
                Console.WriteLine(SenderID + " | Binary message from : " + senderName);
            }
            else
            {
                Console.WriteLine(SenderID + " | String message (ignored) : " + args.Data);
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
