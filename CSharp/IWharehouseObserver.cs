namespace ThingModel
{
    public interface IWharehouseObserver
    {
        void New(Thing thing);
        void Deleted(Thing thing);
        void Updated(Thing thing);
        void Define(ThingType thing);
    }
}