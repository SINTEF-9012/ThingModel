module ThingModel.WebSockets {
	export interface IClientObserver {
		OnFirstOpen(): void;
		OnOpen(): void;
		OnClose(): void;
		OnTransaction(senderName: string): void;
		OnSend(): void;
	}
}
