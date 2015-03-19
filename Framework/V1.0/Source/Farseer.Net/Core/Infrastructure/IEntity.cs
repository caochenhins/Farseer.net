namespace FS.Core.Infrastructure
{
    public interface IEntity<T>
    {
        T ID { get; set; }
    }

    public interface IEntity : IEntity<int?> { }
}
