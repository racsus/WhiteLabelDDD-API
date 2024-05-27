using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data.Pagination;

namespace WhiteLabel.UnitTest.Factory
{
    public static class PagedOptionFactory
    {
        public static PageOption Build(
            int take,
            int skip,
            FilterOption filterOption,
            SortOption sortOption
        )
        {
            var res = new PageOption();
            res.Take = take;
            res.Skip = skip;
            if (sortOption != null)
                res.Sorts.Add(sortOption);
            if (filterOption != null)
                res.Filters.Add(filterOption);
            return res;
        }
    }
}
