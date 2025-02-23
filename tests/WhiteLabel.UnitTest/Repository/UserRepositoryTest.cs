using System;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Domain.Users;
using WhiteLabel.UnitTest.Builders;
using WhiteLabel.UnitTest.Factory;
using WhiteLabel.UnitTest.Specifications;
using Xunit;

namespace WhiteLabel.UnitTest.Repository
{
    public class UserRepositoryTest : BaseAppDbContextTest<Guid>
    {
        [Fact]
        public void Add_InsertElement_ExistsElement()
        {
            var user = new UserBuilder()
                .WithTestValues()
                //.Name("Test Name") In case you need to modify some value use the specific method of the Builder
                .Build();

            var item = Repository.Add(user);
            SaveChanges();

            var newItem = Repository.FindAll<User>().FirstOrDefault();

            Assert.NotNull(newItem);
            Assert.Equal(item, newItem);

            ClearMemory();
        }

        [Theory]
        [InlineData("Unmodified", "Modified")]
        public void Update_ModifyElement_ElementModifiedCorrectly(
            string originalName,
            string modifiedName
        )
        {
            var user = new UserBuilder().WithTestValues().Name(originalName).Build();

            Repository.Add(user);
            SaveChanges();
            user.Name = modifiedName;
            Repository.Update(user);
            SaveChanges();

            var newItem = Repository.FindById<User>(user.Id);

            Assert.True(newItem.Name == modifiedName);
            ClearMemory();
        }

        [Fact]
        public void Remove_DeleteElement_NoElements()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            SaveChanges();
            Repository.Delete(user);
            SaveChanges();

            var newItem = Repository.FindById<User>(user.Id);

            Assert.True(newItem == null);
            ClearMemory();
        }

        [Fact]
        public void FindById_FindElement_ElementFound()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            SaveChanges();
            var newItem = Repository.FindById<User>(user.Id);

            Assert.True(newItem != null);
            ClearMemory();
        }

        [Fact]
        public async Task FindByIdAsync_FindElement_ElementFound()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            await SaveChangesAsync();
            var newItem = await Repository.FindByIdAsync<User>(user.Id);

            Assert.True(newItem != null);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData("Test")]
        public void Find_FindElementsBySpec_ListContainsOneElement(string name)
        {
            var user = new UserBuilder().WithTestValues().Name(name).Build();

            Repository.Add(user);
            SaveChanges();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = Repository.Find(userNameSpec);

            Assert.True(newItem.Count() == 1);
            ClearMemory();
        }

        [Theory]
        [InlineData("TestAsync")]
        public async Task Find_FindElementsBySpecAsync_ListContainsOneElement(string name)
        {
            var user = new UserBuilder().WithTestValues().Name(name).Build();

            Repository.Add(user);
            await SaveChangesAsync();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = await Repository.FindAsync(userNameSpec);

            Assert.True(newItem.Count() == 1);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData("Test_One")]
        public void FindOne_FindOneElementBySpec_ElementFound(string name)
        {
            var user = new UserBuilder().WithTestValues().Name(name).Build();

            Repository.Add(user);
            SaveChanges();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = Repository.FindOne(userNameSpec);

            Assert.True(newItem != null);
            ClearMemory();
        }

        [Theory]
        [InlineData("TestAsync_One")]
        public async Task FindOne_FindOneElementBySpecAsync_ElementFound(string name)
        {
            var user = new UserBuilder().WithTestValues().Name(name).Build();

            Repository.Add(user);
            await SaveChangesAsync();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = await Repository.FindOneAsync(userNameSpec);

            Assert.True(newItem != null);
            await ClearMemoryAsync();
        }

        [Fact]
        public void FindAll_GetAllElements_ListContainsElements()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            SaveChanges();

            var items = Repository.FindAll<User>();

            Assert.True(items.Any());
            ClearMemory();
        }

        [Fact]
        public async Task FindAllAsync_GetAllElements_ListContainsElements()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            await SaveChangesAsync();

            var items = await Repository.FindAllAsync<User>();

            Assert.True(items.Any());
            await ClearMemoryAsync();
        }

        [Fact]
        public void Count_CountElements_NumberElementsAbove0()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            SaveChanges();

            var numberOfElements = Repository.Count<User>();

            Assert.True(numberOfElements > 0);
            ClearMemory();
        }

        [Fact]
        public async Task CountAsync_CountElements_NumberElementsAbove0()
        {
            var user = new UserBuilder().WithTestValues().Build();

            Repository.Add(user);
            await SaveChangesAsync();

            var numberOfElements = await Repository.CountAsync<User>();

            Assert.True(numberOfElements > 0);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData(2, 5)]
        public void FindPaged_TakeNElements_CheckNumElements(int pageSize, int numberOfElements)
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0, null, null);

            var result = Repository.FindPaged<User>(pageOptions, null);

            Assert.True(result.Result.Count() == pageSize);
            ClearMemory();
        }

        [Theory]
        [InlineData(2, 5)]
        public async Task FindPagedAsync_TakeNElements_CheckNumElements(
            int pageSize,
            int numberOfElements
        )
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0, null, null);

            var result = await Repository.FindPagedAsync<User>(pageOptions, null);

            Assert.True(result.Result.Count() == pageSize);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData(2, 5, "Name", "1", FilterOperator.Contains, 1)]
        [InlineData(2, 8, "Email", "1", FilterOperator.Contains, 1)]
        [InlineData(2, 10, "Name", "1", FilterOperator.EndsWith, 1)]
        [InlineData(2, 12, "Name", "1", FilterOperator.IsEqualTo, 0)]
        [InlineData(2, 12, "Name", "1", FilterOperator.DoesNotContain, 2)]
        public void FindPaged_Filter_CheckNumElements(
            int pageSize,
            int numberOfElements,
            string filterMember,
            string filterValue,
            FilterOperator filterOperator,
            int numElementsResult
        )
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                0,
                FilterOptionFactory.Build(filterMember, filterValue, filterOperator),
                null
            );

            var result = Repository.FindPaged<User>(pageOptions, null);

            Assert.True(result.Result.Count() == numElementsResult);
            ClearMemory();
        }

        [Theory]
        [InlineData(2, 5, "Name", "1", FilterOperator.Contains, 1)]
        [InlineData(2, 8, "Email", "1", FilterOperator.Contains, 1)]
        [InlineData(2, 10, "Name", "1", FilterOperator.EndsWith, 1)]
        [InlineData(2, 12, "Name", "1", FilterOperator.IsEqualTo, 0)]
        [InlineData(2, 12, "Name", "1", FilterOperator.DoesNotContain, 2)]
        public async Task FindPagedAsync_Filter_CheckNumElements(
            int pageSize,
            int numberOfElements,
            string filterMember,
            string filterValue,
            FilterOperator filterOperator,
            int numElementsResult
        )
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                0,
                FilterOptionFactory.Build(filterMember, filterValue, filterOperator),
                null
            );

            var result = await Repository.FindPagedAsync<User>(pageOptions, null);

            Assert.True(result.Result.Count() == numElementsResult);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData(2, 12, "Name", SortDirection.Ascending, "TestName0000")]
        [InlineData(4, 5, "Name", SortDirection.Descending, "TestName0004")]
        public void FindPaged_Sort_CheckFirstElement(
            int pageSize,
            int numberOfElements,
            string sortMember,
            SortDirection sortDirection,
            string firstName
        )
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                0,
                null,
                SortOptionFactory.Build(sortMember, sortDirection)
            );

            var result = Repository.FindPaged<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            ClearMemory();
        }

        [Theory]
        [InlineData(2, 12, "Name", SortDirection.Ascending, "TestName0000")]
        [InlineData(4, 5, "Name", SortDirection.Descending, "TestName0004")]
        public async Task FindPagedAsync_Sort_CheckFirstElement(
            int pageSize,
            int numberOfElements,
            string sortMember,
            SortDirection sortDirection,
            string firstName
        )
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                0,
                null,
                SortOptionFactory.Build(sortMember, sortDirection)
            );

            var result = await Repository.FindPagedAsync<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            await ClearMemoryAsync();
        }

        [Theory]
        [InlineData(2, 12, 2, "TestName0002")]
        [InlineData(2, 12, 4, "TestName0004")]
        public void FindPaged_Skip_CheckFirstElement(
            int pageSize,
            int numberOfElements,
            int skip,
            string firstName
        )
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                skip,
                null,
                SortOptionFactory.Build("Name", SortDirection.Ascending)
            );

            var result = Repository.FindPaged<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            ClearMemory();
        }

        [Theory]
        [InlineData(2, 12, 2, "TestName0002")]
        [InlineData(2, 12, 4, "TestName0004")]
        public async Task FindPagedAsync_Skip_CheckFirstElement(
            int pageSize,
            int numberOfElements,
            int skip,
            string firstName
        )
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(
                pageSize,
                skip,
                null,
                SortOptionFactory.Build("Name", SortDirection.Ascending)
            );

            var result = await Repository.FindPagedAsync<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            await ClearMemoryAsync();
        }

        private void PopulateUsers(int numberOfElements)
        {
            var users = UsersFactory.Build(numberOfElements);
            foreach (var user in users)
                Repository.Add(user);
            SaveChanges();
        }

        private async Task PopulateUsersAsync(int numberOfElements)
        {
            var users = UsersFactory.Build(numberOfElements);
            foreach (var user in users)
                Repository.Add(user);
            await SaveChangesAsync();
        }
    }
}
