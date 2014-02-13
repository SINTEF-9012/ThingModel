package websockets;

import java.net.URI;
import java.nio.ByteBuffer;

import org.java_websocket.WebSocketImpl;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.drafts.Draft_10;
import org.java_websocket.drafts.Draft_17;
import org.java_websocket.handshake.ServerHandshake;
import org.thingmodel.Wharehouse;
import org.thingmodel.proto.FromProtobuf;
import org.thingmodel.proto.ProtoModelObserver;
import org.thingmodel.proto.ProtoTransaction.Transaction;
import org.thingmodel.proto.ToProtobuf;

import com.google.protobuf.InvalidProtocolBufferException;

public class Client extends WebSocketClient {
	
	public String senderID;
	private Wharehouse _wharehouse;
	private ToProtobuf _toProtobuf;
	private FromProtobuf _fromProtobuf;
	private boolean _closed = true;
	private int _reconnectionDelay = 1;
	private ProtoModelObserver _thingModelObserver;

	public Client(String senderID, URI path, Wharehouse wharehouse)
	{
		super(path, new Draft_17());
        WebSocketImpl.DEBUG = true;
		
		this.senderID = senderID;
		_wharehouse = wharehouse;
		_thingModelObserver = new ProtoModelObserver();
		wharehouse.RegisterObserver(_thingModelObserver);
	
		_fromProtobuf = new FromProtobuf(wharehouse);
		_toProtobuf = new ToProtobuf();

		connect();
	}
	
	public void connect() {
		if (_closed) {
			_closed = false;
		
			super.connect();
		}
	}
	
	/*public boolean isConnected() {
		return 
	}*/
	
	public void Send() {
		if (_thingModelObserver.somethingChanged()) {
			Transaction transaction = _thingModelObserver.getTransaction(
							_toProtobuf, senderID);
			
			send(transaction.toByteArray());
			
			_thingModelObserver.Reset();
		}
	}

	@Override
	public void onClose(int arg0, String arg1, boolean arg2) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onError(Exception arg0) {
		// TODO Auto-generated method stub
		arg0.printStackTrace();
	}

	@Override
	public void onMessage(String arg0) {
		// TODO Auto-generated method stub
		
	}
	
	@Override
	public void onMessage(ByteBuffer arg0) {
		
		try {
			Transaction t = Transaction.parseFrom(arg0.array());
			String senderName = _fromProtobuf.Convert(t);
			
			System.out.println(senderID+" | Binary message from : "+senderName);
			
			_toProtobuf.ApplyThingsSuppressions(_thingModelObserver.Deletions.values());
			_thingModelObserver.Reset();
			
		} catch (InvalidProtocolBufferException e) {
			e.printStackTrace();
		}
	}
	

	@Override
	public void onOpen(ServerHandshake arg0) {
		// TODO Auto-generated method stub
		
	}
}
