
namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// SortDescriptor class to implement sorting in the Query
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// Member
        /// </summary>
        public string Member { get; set; }
        /// <summary>
        /// Enumerable SortDirection
        /// </summary>
        public SortDirection Direction { get; set; }
    }
}
