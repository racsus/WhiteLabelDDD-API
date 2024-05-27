namespace WhiteLabel.Domain.Generic
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }
    }
}
