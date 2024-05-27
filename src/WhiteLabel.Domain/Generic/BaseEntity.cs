namespace WhiteLabel.Domain.Generic
{
    public abstract class BaseEntity<T> : BaseEntityWithEvents, IBaseEntity<T>
    {
        public T Id { get; set; }
    }
}
