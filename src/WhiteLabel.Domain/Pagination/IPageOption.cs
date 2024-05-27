using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// PageDescriptor interface
    /// </summary>
    public interface IPageOption
    {
        /// <summary>
        /// Number of elements to skip in a section
        /// </summary>
        int? Skip { get; set; }

        /// <summary>
        /// Number of elements to select
        /// </summary>
        int? Take { get; set; }

        ICollection<FilterOption> Filters { get; }

        ICollection<SortOption> Sorts { get; }
    }
}
