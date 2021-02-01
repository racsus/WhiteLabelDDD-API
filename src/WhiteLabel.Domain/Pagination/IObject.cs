namespace WhiteLabel.Domain.Pagination
{
    public interface IObject
    {
    }

    public interface IObject<TKey> : IObject
    {
        /// <summary>
        ///     Primary Key
        /// </summary>
        TKey Id { get; set; }
    }
}