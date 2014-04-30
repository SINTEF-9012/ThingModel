namespace ThingModel
{
    public interface IWarehouseObserver
    {
        void New(Thing thing, string sender);
        void Deleted(Thing thing, string sender);
        void Updated(Thing thing, string sender);
        void Define(ThingType thingType, string sender);
    }
}