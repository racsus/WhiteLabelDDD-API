using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.UnitTest.Factory
{
    public static class SortOptionFactory
    {
        public static SortOption Build(string sortMember, SortDirection sortDirection)
        {
            if (!string.IsNullOrEmpty(sortMember))
                return new SortOption { Member = sortMember, Direction = sortDirection };
            return null;
        }
    }
}
