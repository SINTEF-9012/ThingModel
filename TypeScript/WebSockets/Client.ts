 module ThingModel.WebSockets {
	 export class Client {
		 public SenderID: string;
		 private _ws: WebSocket;
		 private _path: string;
		 private _closed: boolean;
		 private _reconnection: boolean;

		 private _wharehouse: Wharehouse;
		 private _toProtobuf: Proto.ToProtobuf;
		 private _fromProtobuf: Proto.FromProtobuf;
		 private _thingModelObserver: Proto.ProtoModelObserver;
		 private _connexionDelay = 2000;

		 constructor(senderID: string, path: string, wharehouse: Wharehouse) {
			 this.SenderID = senderID;
			 this._path = path;

			 this._wharehouse = wharehouse;

			 this._thingModelObserver = new Proto.ProtoModelObserver();
			 wharehouse.RegisterObserver(this._thingModelObserver);

			 this._fromProtobuf = new Proto.FromProtobuf(this._wharehouse);
			 this._toProtobuf = new Proto.ToProtobuf();

			 this._closed = true;
			 this._reconnection = false;
			 this.Connect();
		 }

		 public Connect(): void {
			 if (!this._closed) {
				 return;
			 }

			 this._ws = new WebSocket(this._path);

			 this._ws.onopen = () => {
				 console.info("ThingModel: Open connection");
				 this._closed = false;
				 this._fromProtobuf = new Proto.FromProtobuf(this._wharehouse);
				 this._toProtobuf = new Proto.ToProtobuf();
				 this._connexionDelay = 2000;

				 // Send changes
				 this.Send();
			 };

			 this._ws.onclose = () => {
				 if (!this._closed) {
					 this._closed = true;
					 console.info("ThingModel: Connection lost");
				 }
				this._reconnection = true;

				 setTimeout(() => this.Connect(), this._connexionDelay);

				 if (this._connexionDelay < 20000) {
					 this._connexionDelay += 3500;
				 }
			 };

			 var useFileReader = typeof FileReader !== "undefined";

			 this._ws.onmessage = (message) => {
				 // Convert the Blob message to an ArrayBuffer object

				 if (useFileReader) {
					 var fileReader = new FileReader();
					 fileReader.readAsArrayBuffer(message.data);
					 fileReader.onload = ()=> this.parseBuffer(fileReader.result);
				 } else {
					 this.parseBuffer(message.data);
				 }
			 };

		 }

		 private parseBuffer(buffer) {
			 var senderName = this._fromProtobuf.Convert(buffer);
			 console.debug("ThingModel: message from: "+senderName);

			 this._toProtobuf.ApplyThingsSuppressions(_.values(this._thingModelObserver.Deletions));
			 this._thingModelObserver.Reset();
		 }

		 public Send(): void {
			 if (this._closed) {
				 console.debug("ThingModel: Does not send, waiting for connexion");
				 return;
			 }
			 if (this._thingModelObserver.SomethingChanged) {
				 var transaction = this._thingModelObserver.GetTransaction(this._toProtobuf, this.SenderID,
					 this._reconnection);

				 this._ws.send(this._toProtobuf.ConvertTransaction(transaction));
				 this._thingModelObserver.Reset();
				 this._reconnection = false;
				 console.debug("ThingModel: transaction sent");
			 }
		 }

		 public Close(): void {
			 if (!this._closed) {
				 this._ws.close();
				 this._closed = true;
			 }
		 }

		 public IsConnected(): boolean {
			 return this._closed;
		 }
	 }
 }