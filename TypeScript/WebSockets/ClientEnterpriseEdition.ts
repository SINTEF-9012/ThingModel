 module ThingModel.WebSockets {
	 export class ClientEnterpriseEdition extends Client {
		 private _isLive: boolean;

		 public get IsLive(): boolean {
			 return this._isLive;
		 }

		 private _isPaused: boolean;

		 public get IsPaused(): boolean {
			 return this._isPaused;
		 }

		 constructor(senderID: string, path: string, warehouse: Warehouse) {
			 super(senderID, path, warehouse);

			 this._isLive = true;
			 this._isPaused = false;
		 }

		 public Live(): void {
			 if (this._isLive && !this._isPaused) return;

			 this.SendMessage("live");

			 this._isLive = true;
			 this._isPaused = false;
		 }

		 public Play(): void {
			 if (!this._isPaused) return;

			 this.SendMessage("play");

			 this._isPaused = false;
		 }


		 public Pause(): void {
			 if (this._isPaused) return;

			 this.SendMessage("pause");

			 this._isPaused = true;
		 }

		 public Load(time: Date): void {
			 this.SendMessage("load " + time.getTime());

			 this._isLive = false;
			 this._isPaused = true;
		 }

		 public Send(): void {
			 if (!this._isLive || this._isPaused) {
				 console.debug("ThingModelClientEnterpriseEdition cannot send data while paused or in a past situation");
				 return;
			 }
			 super.Send();
		 }
	 }
 }
