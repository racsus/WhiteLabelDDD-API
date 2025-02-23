using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    public interface IPagedQueryResult<T> : IQueryResult<T>
        where T : class
    {
        int? Take { get; }
        int? Skip { get; }
        int Total { get; }
        IEnumerable<T> Result { get; }
    }
}
