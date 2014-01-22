namespace ThingModel
{
    public interface IThingObserver
    {
        void New(Thing thing);
        void Deleted(Thing thing);
        void Updated(Thing thing);
    }
}