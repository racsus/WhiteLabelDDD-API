using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.UnitTest.Factory
{
    public static class FilterOptionFactory
    {
        public static FilterOption Build(
            string filterMember,
            string filterValue,
            FilterOperator filterOperator
        )
        {
            if (!string.IsNullOrEmpty(filterMember) && !string.IsNullOrEmpty(filterValue))
                return new FilterOption
                {
                    Member = filterMember,
                    Value = filterValue,
                    Operator = filterOperator,
                };
            return null;
        }
    }
}
