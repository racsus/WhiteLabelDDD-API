
namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// Paged Value Query interface. Inherits from IQuery
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public interface IPagedValueQuery<TEntity, TResult> : IQuery<TEntity, IPagedQueryResult<TResult>> 
        where TEntity : class 
        where TResult : class
    {
        int? Take { get; }
        int? Skip { get; }
    }
}