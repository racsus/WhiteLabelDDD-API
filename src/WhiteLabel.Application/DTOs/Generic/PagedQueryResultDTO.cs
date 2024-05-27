using System.Collections.Generic;

namespace WhiteLabel.Application.DTOs.Generic
{
    public class PagedQueryResultDto<T>
        where T : class
    {
        public int? Take { get; }

        public int? Skip { get; }

        public int Total { get; }

        public IEnumerable<T> Result { get; }

        public PagedQueryResultDto(int? take, int? skip, int total, IEnumerable<T> result)
        {
            Take = take;
            Skip = skip;
            Total = total;
            Result = result;
        }
    }
}
