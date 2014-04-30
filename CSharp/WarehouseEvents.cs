using System;

namespace ThingModel
{
	public class WarehouseEvents : IWarehouseObserver
	{
		public class ThingEventArgs : EventArgs
		{
			public Thing Thing { private set; get; }
			public string Sender { private set; get; }

			public ThingEventArgs(Thing thing, string sender)
			{
				Thing = thing;
				Sender = sender;
			}
		}

		public class ThingTypeEventArgs : EventArgs
		{
			public ThingType ThingType { private set; get; }
			public string Sender { private set; get; }

			public ThingTypeEventArgs(ThingType thingType, string sender)
			{
				ThingType = thingType;
				Sender = sender;
			}
		}

		public delegate void ThingEventHandler(object sender, ThingEventArgs e);
		public delegate void ThingTypeEventHandler(object sender, ThingTypeEventArgs e);

		public event ThingEventHandler OnNew;
		public event ThingEventHandler OnDelete;
		public event ThingEventHandler OnUpdate;
		public event ThingTypeEventHandler OnDefine;

		public event ThingEventHandler OnReceivedNew;
		public event ThingEventHandler OnReceivedDelete;
		public event ThingEventHandler OnReceivedUpdate;
		public event ThingTypeEventHandler OnReceivedDefine;

		public void New(Thing thing, string sender)
		{
			ThingEventArgs args = null;
			if (OnNew != null)
			{
				args = new ThingEventArgs(thing, sender);
				OnNew(this, args);
			}
			if (OnReceivedNew != null && sender != null)
			{
				if (args == null)
				{
					args = new ThingEventArgs(thing, sender);	
				}
				OnReceivedNew(this, args);
			}
		}

		public void Deleted(Thing thing, string sender)
		{
			ThingEventArgs args = null;
			if (OnDelete != null)
			{
				args = new ThingEventArgs(thing, sender);
				OnDelete(this, args);
			}

			if (OnReceivedDelete != null && sender != null)
			{
				if (args == null)
				{
					args = new ThingEventArgs(thing, sender);	
				}
				OnReceivedDelete(this, args);
			}
		}

		public void Updated(Thing thing, string sender)
		{
			ThingEventArgs args = null;
			if (OnUpdate != null)
			{
				args = new ThingEventArgs(thing, sender);
				OnUpdate(this, args);
			}

			if (OnReceivedUpdate != null && sender != null)
			{
				if (args == null)
				{
					args = new ThingEventArgs(thing, sender);	
				}
				OnReceivedUpdate(this, args);
			}
		}

		public void Define(ThingType thingType, string sender)
		{
			ThingTypeEventArgs args = null;
			if (OnDefine != null)
			{
				args = new ThingTypeEventArgs(thingType, sender);
				OnDefine(this, args);
			}

			if (OnReceivedDefine != null && sender != null)
			{
				if (args == null)
				{
					args = new ThingTypeEventArgs(thingType, sender);	
				}
				OnReceivedDefine(this, args);
			}
		}
	}
}