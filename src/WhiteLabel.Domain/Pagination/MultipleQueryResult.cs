using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    public class MultipleQueryResult<T> : IMultipleQueryResult<T> where T : class
    {
        /// <summary>
        /// Creates an instance of <see cref="MultipleQueryResult"/>
        /// </summary>
        /// <param name="result"></param>
        public MultipleQueryResult(IEnumerable<T> result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Data resulting from the query
        /// </summary>
        public IEnumerable<T> Result { get; }
    }
}
