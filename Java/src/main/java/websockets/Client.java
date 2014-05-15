package websockets;

import java.net.URI;
import java.nio.ByteBuffer;
import java.util.Timer;
import java.util.TimerTask;

import org.java_websocket.WebSocket.READYSTATE;
import org.java_websocket.WebSocketImpl;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.drafts.Draft_17;
import org.java_websocket.handshake.ServerHandshake;
import org.thingmodel.Warehouse;
import org.thingmodel.proto.FromProtobuf;
import org.thingmodel.proto.ProtoModelObserver;
import org.thingmodel.proto.ProtoTransaction.Transaction;
import org.thingmodel.proto.ToProtobuf;

import com.google.protobuf.InvalidProtocolBufferException;

public class Client extends WebSocketClient {

	public String senderID;

	private final Warehouse _warehouse;
	private ToProtobuf _toProtobuf;
	private FromProtobuf _fromProtobuf;

	private boolean _closed = true;
	private int _reconnectionDelay = 1;
	private boolean _reconnection = false;

	private final ProtoModelObserver _thingModelObserver;
	private final Object _lock = new Object();

	public Client(String senderID, URI path, Warehouse warehouse) {
		super(path, new Draft_17());
		WebSocketImpl.DEBUG = true;

		this.senderID = senderID;
		_warehouse = warehouse;
		_thingModelObserver = new ProtoModelObserver();
		warehouse.RegisterObserver(_thingModelObserver);

		_fromProtobuf = new FromProtobuf(warehouse);
		_toProtobuf = new ToProtobuf();

		connect();
	}

	public void connect() {
		if (_closed) {
            super.connect();
        }
	}

	public void close() {
		_closed = true;
		super.close();
	}
	
	public void Send() {
		synchronized (_lock) {
			if (_closed) {
				System.out.println("Does not send, waiting for connexion");
				return;
			}

			if (_thingModelObserver.somethingChanged()) {
				Transaction transaction = _thingModelObserver.getTransaction(
						_toProtobuf, senderID, _reconnection);

				send(transaction.toByteArray());

				_thingModelObserver.Reset();
			}
		}
	}

	@Override
	public void onClose(int arg0, String arg1, boolean arg2) {
		if (!_closed) {
			System.out.println("Connection lost, try to connect again in "+_reconnectionDelay+" seconds");
			
			_reconnection = true;
			
			Timer timer = new Timer();
			final Client client = this;
			
			timer.schedule(new TimerTask() {
				@Override
				public void run() {
					client.connect();

					if (_reconnectionDelay < 16) {
						_reconnectionDelay *= 2;
					}
					
				}
			}, _reconnectionDelay*100);
		}
	}

	@Override
	public void onError(Exception arg0) {
		arg0.printStackTrace();
	}

	@Override
	public void onMessage(String arg0) {
	}

	@Override
	public void onMessage(ByteBuffer arg0) {

		try {
			Transaction t = Transaction.parseFrom(arg0.array());
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

	@Override
	public void onOpen(ServerHandshake arg0) {
        _fromProtobuf = new FromProtobuf(_warehouse);
        _toProtobuf = new ToProtobuf();
		_reconnectionDelay = 1;
        _closed = false;
		Send();
	}
}
