namespace GenericApi
{
    public interface IEntityWithState
    {
        EntityState EntityState { get; set; }
    }

    public enum EntityState
    {
        Unchanged,
        Added,
        Modified,
        Deleted,
        Detached
    }
}
