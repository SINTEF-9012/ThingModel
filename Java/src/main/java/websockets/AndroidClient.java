package websockets;

import com.google.protobuf.InvalidProtocolBufferException;
import com.koushikdutta.async.ByteBufferList;
import com.koushikdutta.async.DataEmitter;
import com.koushikdutta.async.callback.CompletedCallback;
import com.koushikdutta.async.callback.DataCallback;
import com.koushikdutta.async.http.AsyncHttpClient;
import com.koushikdutta.async.http.WebSocket;
import org.thingmodel.Warehouse;
import org.thingmodel.proto.FromProtobuf;
import org.thingmodel.proto.ProtoModelObserver;
import org.thingmodel.proto.ProtoTransaction;
import org.thingmodel.proto.ToProtobuf;

import javax.xml.bind.DatatypeConverter;
import java.util.Timer;
import java.util.TimerTask;

public class AndroidClient {

    public String senderID;
    private String _path;
    private final Warehouse _warehouse;
    private ToProtobuf _toProtobuf;
    private FromProtobuf _fromProtobuf;

    private boolean _closed = true;
    private int _reconnectionDelay = 1;
    private boolean _reconnection = false;

    private final ProtoModelObserver _thingModelObserver;

    private WebSocket _websocket = null;
    private final Object _lock = new Object();

    public AndroidClient(String senderID, String path, Warehouse warehouse) {
        this.senderID = senderID;

        _path = path;
        _warehouse = warehouse;
        _thingModelObserver = new ProtoModelObserver();
        warehouse.RegisterObserver(_thingModelObserver);

        _fromProtobuf = new FromProtobuf(warehouse);
        _toProtobuf = new ToProtobuf();

        connect();
    }

    public void connect() {
        if (!_closed) return;

        final AndroidClient client = this;

        AsyncHttpClient.getDefaultInstance().websocket(_path, null, new AsyncHttpClient.WebSocketConnectCallback() {
            @Override
            public void onCompleted(Exception e, WebSocket webSocket) {
                if (e != null) {
                    e.printStackTrace();
                    _websocket = null;
                    return;
                }

                _websocket = webSocket;
                _fromProtobuf = new FromProtobuf(_warehouse);
                _toProtobuf = new ToProtobuf();
                _reconnectionDelay = 1;
                _closed = false;

                client.send();

                webSocket.setClosedCallback(new CompletedCallback() {
                    @Override
                    public void onCompleted(Exception e) {
                        _websocket = null;
                        if (!_closed) {
                            _closed = true;
                            System.out.println("Connection lost, try to connect again in "+_reconnectionDelay+" seconds");

                            _reconnection = true;

                            Timer timer = new Timer();

                            timer.schedule(new TimerTask() {
                                @Override
                                public void run() {
                                    client.connect();

                                    if (_reconnectionDelay < 16) {
                                        _reconnectionDelay *= 2;
                                    }

                                }
                            }, _reconnectionDelay*1000);
                        }
                    }
                });

                webSocket.setDataCallback(new DataCallback() {
                    @Override
                    public void onDataAvailable(DataEmitter dataEmitter, ByteBufferList byteBufferList) {
                        byte[] buffer = byteBufferList.getAllByteArray();

                        System.out.println(DatatypeConverter.printBase64Binary(buffer));

                        try {
                            ProtoTransaction.Transaction t = ProtoTransaction.Transaction.parseFrom(buffer);
                            String senderName = _fromProtobuf.Convert(t);

                            _toProtobuf.ApplyThingsSuppressions(_thingModelObserver.Deletions
                                    .values());
                            _thingModelObserver.Reset();

                            System.out.println(senderID + " | Binary message from : "
                                    + senderName);

                        } catch (InvalidProtocolBufferException e) {
                            e.printStackTrace();
                        }
                    }
                });
            }
        });
    }

    public void close() {
        _closed = true;
        if (_websocket != null) {
            _websocket.close();
        }
    }

    public void send() {
        synchronized (_lock) {
            if (_closed || _websocket == null) {
                System.out.println("Does not send, waiting for connexion");
                return;
            }

            if (_thingModelObserver.somethingChanged()) {
                ProtoTransaction.Transaction transaction = _thingModelObserver.getTransaction(
                        _toProtobuf, senderID, _reconnection);

                _websocket.send(transaction.toByteArray());

                _thingModelObserver.Reset();
            }
        }
    }
}
