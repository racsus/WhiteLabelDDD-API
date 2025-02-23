using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    public interface IPageOption
    {
        int? Skip { get; set; }

        int? Take { get; set; }

        ICollection<FilterOption> Filters { get; }

        ICollection<SortOption> Sorts { get; }
    }
}
