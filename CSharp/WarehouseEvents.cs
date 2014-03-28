using System;

namespace ThingModel
{
	public class WarehouseEvents : IWarehouseObserver
	{
		public class ThingEventArgs : EventArgs
		{
			public Thing Thing { private set; get; }

			public ThingEventArgs(Thing thing)
			{
				Thing = thing;
			}
		}

		public class ThingTypeEventArgs : EventArgs
		{
			public ThingType ThingType { private set; get; }

			public ThingTypeEventArgs(ThingType thingType)
			{
				ThingType = thingType;
			}
		}

		public delegate void ThingEventHandler(object sender, ThingEventArgs e);
		public delegate void ThingTypeEventHandler(object sender, ThingTypeEventArgs e);

		public event ThingEventHandler OnNew;
		public event ThingEventHandler OnDelete;
		public event ThingEventHandler OnUpdate;
		public event ThingTypeEventHandler OnDefine;

		public void New(Thing thing)
		{
			if (OnNew != null)
			{
				OnNew(this, new ThingEventArgs(thing));
			}
		}

		public void Deleted(Thing thing)
		{
			if (OnDelete != null)
			{
				OnDelete(this, new ThingEventArgs(thing));
			}
		}

		public void Updated(Thing thing)
		{
			if (OnUpdate != null)
			{
				OnUpdate(this, new ThingEventArgs(thing));
			}
		}

		public void Define(ThingType thingType)
		{
			if (OnDefine != null)
			{
				OnDefine(this, new ThingTypeEventArgs(thingType));
			}
		}
	}
}