using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Application.DTOs.Generic
{
    public class PagedQueryResultDTO<T> 
        where T : class

    {
        public int? Take { get; }

        public int? Skip { get; }

        public int Total { get; }

        public IEnumerable<T> Result { get; }

        public PagedQueryResultDTO(int? take, int? skip, int total, IEnumerable<T> result)
        {
            this.Take = take;
            this.Skip = skip;
            this.Total = total;
            this.Result = result;
        }
    }
}
