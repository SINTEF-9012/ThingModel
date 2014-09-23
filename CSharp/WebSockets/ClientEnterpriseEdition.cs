using System;

namespace ThingModel.WebSockets
{
    class ClientEnterpriseEdition : Client
    {
        public bool IsLive { get; private set; }
        public bool IsPaused { get; private set; }

        public ClientEnterpriseEdition(string senderID, string path, Warehouse warehouse) : base(senderID, path, warehouse)
        {
            IsLive = true;
            IsPaused = false;
        }


        public void Live()
        {
            if (IsLive && !IsPaused) return;

            Send("live");
            IsLive = true;
            IsPaused = false;
        }

        public void Play()
        {
            if (!IsPaused) return;

            Send("play");
            IsPaused = false;
        }

        public void Pause()
        {
            if (IsPaused) return;

            Send("pause");
            IsPaused = true;
        }

		private static readonly TimeSpan DateTimeEpoch = new TimeSpan(
			new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc).Ticks);

        public void Load(DateTime time)
        {
            IsLive = false;
            IsPaused = true;

	        Send("load "+time.Subtract(DateTimeEpoch).Ticks/10000);
        }

        protected override void WsOnStringMessage(string message)
        {
            if (message.Contains("error"))
            {
                Console.WriteLine("ThingModelClientEnterpriseEdition "+message);
            }
        }

        public override void Send()
        {
            if (!IsLive || IsPaused)
            {
                throw new Exception("ThingModelClientEnterpriseEdition cannot send data while paused or in a past situation");
            }

            base.Send();
        }
    }
}
