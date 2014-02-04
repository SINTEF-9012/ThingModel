namespace ThingModel
{
    public interface IThingModelObserver
    {
        void New(Thing thing);
        void Deleted(Thing thing);
        void Updated(Thing thing);
        void Define(ThingType thing);
    }
}