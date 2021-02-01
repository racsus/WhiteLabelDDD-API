using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    public class PagedQueryResult<T> : MultipleQueryResult<T>, IPagedQueryResult<T> where T : class
    {
        /// <summary>
        /// Creates an instance of <see cref="PagedQueryResult"/>
        /// </summary>
        /// <param name="result">Data resulting from the query</param>
        /// <param name="take">Take</param>
        /// <param name="skip">Skip</param>
        /// <param name="total">Total results</param>
        public PagedQueryResult(IEnumerable<T> result, int? take, int? skip, int total) : base(result)
        {
            this.Skip = skip;
            this.Take = take;
            this.Total = total;
        }

        /// <summary>
        /// Skip
        /// </summary>
        public int? Skip { get; }
        /// <summary>
        /// Take
        /// </summary>
        public int? Take { get; }
        /// <summary>
        /// Total result
        /// </summary>
        public int Total { get; }
    }
}
