namespace Communication.Domain
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }
    }
}