using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThingModel.Proto;
using WebSocketSharp;

namespace ThingModel.WebSockets
{
    public class Client
    {
        private readonly Warehouse _warehouse;
        private ToProtobuf _toProtobuf;
        private FromProtobuf _fromProtobuf;
        private readonly ProtoModelObserver _thingModelObserver;

        private readonly AutoResetEvent _sendEvent = new AutoResetEvent(false);
        private readonly object _connexionEvent = new object();
        private WebSocket _ws = null;
        
        public string SenderID;
        private readonly string _path;
		
        private readonly object _lockSocketEvents = new Object();
        private readonly object _lockWsObject = new Object();
        private readonly object _lockCloseSocket = new Object();
        private readonly object _lockWarehouse = new Object();
        private readonly object _lockSend = new Object();

        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan _pingFrequency = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan _restartTimeout = TimeSpan.FromSeconds(16);
        private static readonly TimeSpan _defaultWaitConnectionDelay = TimeSpan.FromSeconds(10);

        private Timer _connexionCheckTimer = null;
        private int _nbConnexionCheck = 0;
        private DateTime _lastConnectedDate = DateTime.Now;

        public bool DebugMode { get; set; }

        private bool _running = false;
	    private bool _reconnection = false;
        
        private readonly IList<string> _sendMessageWaitingList = new List<string>();
        private bool _sendRequired = false;
        private int _threadRunning = 0;

        public Client(string senderID, string path, Warehouse warehouse)
        {
            SenderID = senderID;
            _path = path;

            _warehouse = warehouse;

            _thingModelObserver = new ProtoModelObserver();
            _warehouse.RegisterObserver(_thingModelObserver);

            Connect();
        }

        private void ConnexionCheckTimer(object state)
        {
            Interlocked.Increment(ref _nbConnexionCheck);
            if (_nbConnexionCheck > 2)
            {
                Console.WriteLine("ThingModel: dead lock ???");
            }

            try
            {
                WebSocket w;
                lock (_lockWsObject)
                {
                    w = _ws;
                }

                if (w != null && w.ReadyState == WebSocketState.Open)
                {
                    var now = DateTime.Now;
                    if (w.IsAlive)
                    {
                        Console.WriteLine("ThingModel: ping: "+(DateTime.Now-now).Milliseconds+"ms");
                        _lastConnectedDate = now;
                    }
                    else if (now - _lastConnectedDate > _restartTimeout)
                    {
                        lock (_lockCloseSocket)
                        {
                            if (w.ReadyState != WebSocketState.Closed || w.ReadyState != WebSocketState.Closing)
                            {
                                Console.WriteLine("ThingModel: closing unresponding websocket");
                                lock (_lockWsObject)
                                {
                                    if (w == _ws)
                                    {
                                        _ws = null;
					                    _reconnection = true;
                                    }
                                }

                                w.CloseAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ThingModel: "+e.Message); 
            }

            Interlocked.Decrement(ref _nbConnexionCheck);
        }

        private void ThreadConnection(object state)
        {
            Interlocked.Increment(ref _threadRunning);
            while (_running)
            {
                try
                {
                    WebSocketState s;
                    lock (_lockWsObject)
                    {
                        s = _ws == null ? WebSocketState.Closed : _ws.ReadyState;
                    }

                    WebSocket w;
                    switch (s)
                    {
                        case WebSocketState.Closing:
                        case WebSocketState.Closed:
                            Console.WriteLine("ThingModel: creating new connection");

                            w = new WebSocket(_path);
                            w.OnMessage += WsOnMessage;
                            w.OnClose += WsOnClose;
                            w.OnOpen += WsOnOpen;

                            if (DebugMode)
                            {
                                //w.Log.Level = LogLevel.Debug;
                            }

                            lock (_lockWsObject)
                            {
                                if (_ws != null)
                                {
					                _reconnection = true;
                                }
                                _ws = w;
                            }

                            w.Connect();

                            lock (_connexionEvent)
                            {
                                if (w.ReadyState != WebSocketState.Open)
                                {
                                    Monitor.Wait(_connexionEvent, _timeout);
                                }
                            }
                            break;
                        case WebSocketState.Connecting:
                            Console.WriteLine("ThingModel: ...connecting...");
                            lock (_connexionEvent)
                            {
                                Monitor.Wait(_connexionEvent, _timeout);
                            }
                            break;
                        default:

                            bool immediateSend;
                            lock (_lockSend)
                            {
                                immediateSend = _sendRequired || _sendMessageWaitingList.Count > 0;
                            }

                            if (immediateSend || _sendEvent.WaitOne(1000))
                            {
                                lock (_lockSend)
                                {
                                    lock (_lockWsObject)
                                    {
                                        w = _ws;
                                    }

                                    if (w == null)
                                    {
                                        break;
                                    }

                                    if (_sendRequired)
                                    {
                                        lock (_lockWarehouse)
                                        {
                                            if (_thingModelObserver.SomethingChanged())
                                            {
                                                var transaction = _thingModelObserver.GetTransaction(_toProtobuf,
                                                    SenderID, _reconnection);
                                                byte[] output = _toProtobuf.Convert(transaction);
                                                _thingModelObserver.Reset();

                                                lock (_lockWsObject)
                                                {
                                                    _reconnection = false;
                                                }
                                                Console.WriteLine("ThingModel: sending: " +
                                                                  Convert.ToBase64String(output));
                                                _ws.Send(output);
                                            }
                                            else
                                            {
                                                Console.WriteLine("ThingModel: nothing to send");
                                            }
                                            _sendRequired = false;
                                        }
                                    }

                                    if (_sendMessageWaitingList.Count > 0)
                                    {
                                        foreach (var message in _sendMessageWaitingList)
                                        {
                                            Console.WriteLine("ThingModel: sending text: "+message);
                                            w.Send(message);
                                        }
                                        _sendMessageWaitingList.Clear();
                                    }
                                }
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ThingModel: communication exception: "+e.Message);
                }
            }

            lock (_lockWsObject)
            {
                if (_ws != null && _ws.ReadyState != WebSocketState.Closed && _ws.ReadyState != WebSocketState.Closing)
                {
                    Console.WriteLine("ThingModel: shutting down connexion");
                    _ws.Close();
                }
            }
            Interlocked.Decrement(ref _threadRunning);
        }

        private void WsOnOpen(object sender, EventArgs e)
        {
            if (sender != _ws)
            {
                var ws = sender as WebSocket;
                if (ws != null)
                {
                    Console.WriteLine("ThingModel: connected to an old websocket: will close");
                    ws.CloseAsync();
                    return;
                }
            }

            Console.WriteLine("ThingModel: connected");

            lock (_lockWarehouse)
            {
                _fromProtobuf = new FromProtobuf(_warehouse);
                _toProtobuf = new ToProtobuf();
            }

            lock (_lockSocketEvents)
            {
                lock (_connexionEvent)
                {
                    Monitor.PulseAll(_connexionEvent);
                }
            }
        }

        private void WsOnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("ThingModel: closed");
        }

        private void WsOnMessage(object sender, MessageEventArgs args)
        {
            if (sender != _ws)
            {
                var ws = sender as WebSocket;
                if (ws != null)
                {
                    Console.WriteLine("ThingModel: message received on an old websocket: ignored");
                    ws.CloseAsync();
                    return;
                }
            }

            lock (_lockSocketEvents)
            {
				if (args.Type == Opcode.Binary)
				{
                    WsOnBinaryMessage(args.RawData);
				    //Task.Factory.StartNew(() => WsOnBinaryMessage(args.RawData));
                }
                else if (args.Type == Opcode.Text)
                {
                    WsOnStringMessage(args.Data);
				    //Task.Factory.StartNew(() => WsOnStringMessage(args.Data));
                }
            }
        }

        protected virtual void WsOnStringMessage(string message)
        {
            Console.WriteLine("ThingModel: Client has received an ignored string: "+message);
        }

        private void WsOnBinaryMessage(byte[] message)
        {
            try
            {
                lock (_lockWarehouse)
                {
                    var transaction = _fromProtobuf.Deserialize(message);
                    var senderId = _fromProtobuf.GetStringId(transaction);
                    _thingModelObserver.IgnoreSenderId(senderId);
                    var senderName = _fromProtobuf.Convert(transaction);
                    if (senderName == "undefined")
                    {
                        Console.WriteLine("ThingModel: Something went wrong : has received an undefined senderID");
                    }
                    else
                    {
                        _toProtobuf.ApplyThingsSuppressions(_thingModelObserver.IgnoredDeletions);
                        Console.WriteLine("ThingModel: "+senderId + " : message : " + Convert.ToBase64String(message));
                    }
                    _thingModelObserver.ResetIgnore();
                    // TODO WTF ?_thingModelObserver.Reset();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ThingModel:" +SenderID + " : Exception when receiving the message : " + e.Message);
            }
        }

        public virtual void Send()
        {
            lock (_lockSend)
            {
                _sendRequired = true;
            }

            _sendEvent.Set();
        }
        
        protected void Send(string message)
        {
            lock (_lockSend)
            {
                _sendMessageWaitingList.Add(message);
            }
            _sendEvent.Set();
        }

        public bool IsConnected()
        {
            lock (_lockWsObject)
            {
                return _ws != null && _ws.ReadyState == WebSocketState.Open;
            }
        }

        public bool WaitConnection(TimeSpan delay)
        {
            if (IsConnected()) return true;
            lock (_connexionEvent)
            {
                return Monitor.Wait(_connexionEvent, delay);
            }
        }


        public bool WaitConnection()
        {
            return WaitConnection(_defaultWaitConnectionDelay);
        }

        public void Close()
        {
            _connexionCheckTimer.Dispose();
            _running = false;
        }

        public void Connect()
        {
            if (_running)
            {
                Console.WriteLine("ThingModel: already connected");
                return;
            }

            _running = true;
            if (_threadRunning == 0)
            {
                ThreadPool.QueueUserWorkItem(ThreadConnection);
            }
            _connexionCheckTimer = new Timer(ConnexionCheckTimer, null, _pingFrequency, _pingFrequency);
        }
    }
}
