using System.Collections.Generic;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public class MultipleQueryResult<T> : IMultipleQueryResult<T>
        where T : class
    {
        /// <summary>
        /// Creates an instance of <see>
        ///     <cref>MultipleQueryResult</cref>
        /// </see>
        /// </summary>
        /// <param name="result"></param>
        protected MultipleQueryResult(IEnumerable<T> result)
        {
            Result = result;
        }

        /// <summary>
        /// Data resulting from the query
        /// </summary>
        public IEnumerable<T> Result { get; }
    }
}
