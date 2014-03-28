namespace ThingModel
{
    public interface IWarehouseObserver
    {
        void New(Thing thing);
        void Deleted(Thing thing);
        void Updated(Thing thing);
        void Define(ThingType thingType);
    }
}