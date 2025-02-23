using System.Collections.Generic;

namespace WhiteLabel.Application.DTOs.Generic
{
    public class PagedQueryResultDto<T>(int? take, int? skip, int total, IEnumerable<T> result)
        where T : class
    {
        public int? Take { get; } = take;

        public int? Skip { get; } = skip;

        public int Total { get; } = total;

        public IEnumerable<T> Result { get; } = result;
    }
}
