namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    ///     Core Domain Entity interface
    /// </summary>
    public interface IEntity : IObject
    {
    }

    /// <summary>
    ///     Keyed Core Domain Entity Interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey> : IEntity, IObject<TKey>
    {
    }
}