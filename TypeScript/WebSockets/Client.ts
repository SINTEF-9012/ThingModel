 module ThingModel.WebSockets {
	 export class Client {
		 public SenderID: string;
		 private ws: WebSocket;
		 private path: string;

		 private _wharehouse: Wharehouse;
		 private _toProtobuf: Proto.ToProtobuf;
		 private _fromProtobuf: Proto.FromProtobuf;
		 private _thingModelObserver: Proto.ProtoModelObserver;

		 constructor(senderID: string, path: string, wharehouse: Wharehouse) {
			 this.SenderID = senderID;
			 this.path = path;

			 this._wharehouse = wharehouse;

			 this._thingModelObserver = new Proto.ProtoModelObserver();
			 wharehouse.RegisterObserver(this._thingModelObserver);

			 this._fromProtobuf = new Proto.FromProtobuf(wharehouse);
			 this._toProtobuf = new Proto.ToProtobuf();

			 this.Connect();
		 }

		 private Connect(): void {
			 this.ws = new WebSocket(this.path);

			 this.ws.onopen = () => {
				 console.log("Open connection");
			 };

			 this.ws.onclose = () => {
				 console.log("Connection lost");
				 this._fromProtobuf = new Proto.FromProtobuf(this._wharehouse);
				 this._toProtobuf = new Proto.ToProtobuf();

				 setTimeout(() => this.Connect(), 2000);
			 };

			 this.ws.onmessage = (message) => {
				 // Convert the Blob message to an ArrayBuffer object
				 var fileReader = new FileReader();

				 fileReader.readAsArrayBuffer(message.data);

				 fileReader.onload = () => {
					 var arrayBuffer = fileReader.result;

					 var senderName = this._fromProtobuf.Convert(arrayBuffer);

					 console.log("Binary message from: " + senderName);
					 this._toProtobuf.ApplyThingsSuppressions(_.values(this._thingModelObserver.Deletions));
					 this._thingModelObserver.Reset();
				 };

			 };
		 }
	 }
 }