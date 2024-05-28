namespace WhiteLabel.Domain.Pagination
{
    public interface IQueryResult;

    public interface IQueryResult<T> : IQueryResult
        where T : class;
}
