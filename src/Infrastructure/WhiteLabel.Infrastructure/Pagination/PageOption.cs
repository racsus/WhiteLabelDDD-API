using System.Collections.Generic;
using System.Collections.ObjectModel;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public class PageOption : IPageOption
    {
        public PageOption()
        {
            Filters = new Collection<FilterOption>();
            Sorts = new Collection<SortOption>();
        }

        public PageOption(ICollection<FilterOption> filters, ICollection<SortOption> sorts)
        {
            Filters = filters ?? new Collection<FilterOption>();
            Sorts = sorts ?? new Collection<SortOption>();
        }

        /// <summary>
        /// Number of elements to skip in a section
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Number of elements to select
        /// </summary>
        public int? Take { get; set; }

        public ICollection<FilterOption> Filters { get; }

        public ICollection<SortOption> Sorts { get; }
    }
}
