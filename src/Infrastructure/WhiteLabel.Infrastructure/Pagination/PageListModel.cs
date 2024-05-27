using System.Collections.Generic;
using System.Collections.ObjectModel;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public class PagedListModel : IPageOption
    {
        public const string SkipKey = "skip";
        public const string TakeKey = "take";
        public const string EnvelopeKey = "envelope";
        public const string FilterKey = "filters";
        public const string SortKey = "sorts";

        public int? Skip { get; set; }

        public int? Take { get; set; }

        public bool? Envelope { get; set; }

        public ICollection<FilterOption> Filters { get; set; } = new Collection<FilterOption>();

        public ICollection<SortOption> Sorts { get; set; } = new Collection<SortOption>();
    }
}
