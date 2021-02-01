using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WhiteLabel.Domain.Pagination
{
    public class PageOption : IPageOption
    {
        public PageOption()
        {
            this.Filters = new Collection<FilterOption>();
            this.Sorts = new Collection<SortOption>();
        }

        public PageOption(ICollection<FilterOption> filters, ICollection<SortOption> sorts)
        {
            this.Filters = filters ?? new Collection<FilterOption>();
            this.Sorts = sorts ?? new Collection<SortOption>();
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
