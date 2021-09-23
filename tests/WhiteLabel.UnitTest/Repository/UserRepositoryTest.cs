using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Domain.Users;
using WhiteLabel.UnitTest.Builders;
using WhiteLabel.UnitTest.Factory;
using WhiteLabel.UnitTest.Specifications;

namespace WhiteLabel.UnitTest.Repository
{
    public class UserRepositoryTest: BaseAppDbContextTest<Guid>
    {
        [Test]
        public void Add_InsertElement_ExistsElement()
        {
            var user = new UserBuilder()
                .WithTestValues()
                //.Name("Test Name") In case you need to modify some value use the specific method of the Builder
                .Build();

            var item = this.Repository.Add(user);
            SaveChanges();

            var newItem = this.Repository.FindAll<User>().FirstOrDefault();

            Assert.AreEqual(item, newItem);
            Assert.True(newItem?.Id != null);
            ClearMemory();
        }

        [TestCase("Unmodified", "Modified")]
        public void Update_ModifyElement_ElementModifiedCorrectly(string originalName, string modifiedName)
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Name(originalName)
                .Build();

            this.Repository.Add(user);
            SaveChanges();
            user.Name = modifiedName;
            this.Repository.Update(user);
            SaveChanges();

            var newItem = this.Repository.FindById<User>(user.Id);

            Assert.True(newItem.Name == modifiedName);
            ClearMemory();
        }

        [Test]
        public void Remove_DeleteElement_NoElements()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            SaveChanges();
            this.Repository.Delete(user);
            SaveChanges();

            var newItem = this.Repository.FindById<User>(user.Id);

            Assert.True(newItem == null);
            ClearMemory();
        }

        [Test]
        public void FindById_FindElement_ElementFound()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            SaveChanges();
            var newItem = this.Repository.FindById<User>(user.Id);

            Assert.True(newItem != null);
            ClearMemory();
        }

        [Test]
        public async Task FindByIdAsync_FindElement_ElementFound()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            await SaveChangesAsync();
            var newItem = await this.Repository.FindByIdAsync<User>(user.Id);

            Assert.True(newItem != null);
            await ClearMemoryAsync();
        }

        [TestCase("Test")]
        public void Find_FindElementsBySpec_ListContainsOneElement(string name)
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Name(name)
                .Build();

            this.Repository.Add(user);
            SaveChanges();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = this.Repository.Find<User>(userNameSpec);

            Assert.True(newItem.Count() == 1);
            ClearMemory();
        }

        [TestCase("TestAsync")]
        public async Task Find_FindElementsBySpecAsync_ListContainsOneElement(string name)
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Name(name)
                .Build();

            this.Repository.Add(user);
            await SaveChangesAsync();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = await this.Repository.FindAsync<User>(userNameSpec);

            Assert.True(newItem.Count() == 1);
            await ClearMemoryAsync();
        }

        [TestCase("Test_One")]
        public void FindOne_FindOneElementBySpec_ElementFound(string name)
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Name(name)
                .Build();

            this.Repository.Add(user);
            SaveChanges();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = this.Repository.FindOne<User>(userNameSpec);

            Assert.True(newItem != null);
            ClearMemory();
        }

        [TestCase("TestAsync_One")]
        public async Task FindOne_FindOneElementBySpecAsync_ElementFound(string name)
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Name(name)
                .Build();

            this.Repository.Add(user);
            await SaveChangesAsync();

            ISpecification<User> userNameSpec = new UserNameSpecification(name);

            var newItem = await this.Repository.FindOneAsync<User>(userNameSpec);

            Assert.True(newItem != null);
            await ClearMemoryAsync();
        }

        [Test]
        public void FindAll_GetAllElements_ListContainsElements()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            SaveChanges();

            var items = this.Repository.FindAll<User>();

            Assert.True(items.Any());
            ClearMemory();
        }

        [Test]
        public async Task FindAllAsync_GetAllElements_ListContainsElements()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            await SaveChangesAsync();

            var items = await this.Repository.FindAllAsync<User>();

            Assert.True(items.Any());
            await ClearMemoryAsync();
        }

        [Test]
        public void Count_CountElements_NumberElementsAbove0()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            SaveChanges();

            var numberOfElements = this.Repository.Count<User>();

            Assert.True(numberOfElements > 0);
            ClearMemory();
        }

        [Test]
        public async Task CountAsync_CountElements_NumberElementsAbove0()
        {
            var user = new UserBuilder()
                .WithTestValues()
                .Build();

            this.Repository.Add(user);
            await SaveChangesAsync();

            var numberOfElements = await this.Repository.CountAsync<User>();

            Assert.True(numberOfElements > 0);
            await ClearMemoryAsync();
        }

        [TestCase(2, 5)]
        public void FindPaged_TakeNElements_CheckNumElements(int pageSize, int numberOfElements)
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0, null, null);

            var result = this.Repository.FindPaged<User>(pageOptions, null);

            Assert.True(result.Result.Count() == pageSize);
            ClearMemory();
        }

        [TestCase(2, 5)]
        public async Task FindPagedAsync_TakeNElements_CheckNumElements(int pageSize, int numberOfElements)
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0, null, null);

            var result = await this.Repository.FindPagedAsync<User>(pageOptions, null);

            Assert.True(result.Result.Count() == pageSize);
            await ClearMemoryAsync();
        }

        [TestCase(2, 5, "Name", "1", FilterOperator.Contains, 1)]
        [TestCase(2, 8, "Email", "1", FilterOperator.Contains, 1)]
        [TestCase(2, 10, "Name", "1", FilterOperator.EndsWith, 1)]
        [TestCase(2, 12, "Name", "1", FilterOperator.IsEqualTo, 0)]
        [TestCase(2, 12, "Name", "1", FilterOperator.DoesNotContain, 2)]
        public void FindPaged_Filter_CheckNumElements(int pageSize, int numberOfElements, 
            string filterMember, string filterValue, FilterOperator filterOperator, int numElementsResult)
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0, 
                FilterOptionFactory.Build(filterMember, filterValue, filterOperator), null);

            var result = this.Repository.FindPaged<User>(pageOptions, null);

            Assert.True(result.Result.Count() == numElementsResult);
            ClearMemory();
        }

        [TestCase(2, 5, "Name", "1", FilterOperator.Contains, 1)]
        [TestCase(2, 8, "Email", "1", FilterOperator.Contains, 1)]
        [TestCase(2, 10, "Name", "1", FilterOperator.EndsWith, 1)]
        [TestCase(2, 12, "Name", "1", FilterOperator.IsEqualTo, 0)]
        [TestCase(2, 12, "Name", "1", FilterOperator.DoesNotContain, 2)]
        public async Task FindPagedAsync_Filter_CheckNumElements(int pageSize, int numberOfElements,
            string filterMember, string filterValue, FilterOperator filterOperator, int numElementsResult)
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0,
                FilterOptionFactory.Build(filterMember, filterValue, filterOperator), null);

            var result = await this.Repository.FindPagedAsync<User>(pageOptions, null);

            Assert.True(result.Result.Count() == numElementsResult);
            await ClearMemoryAsync();
        }

        [TestCase(2, 12, "Name", SortDirection.Ascending, "TestName0000")]
        [TestCase(4, 5, "Name", SortDirection.Descending, "TestName0004")]
        public void FindPaged_Sort_CheckFirstElement(int pageSize, int numberOfElements,
            string sortMember, SortDirection sortDirection, string firstName)
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0,
                null, SortOptionFactory.Build(sortMember, sortDirection));

            var result = this.Repository.FindPaged<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            ClearMemory();
        }

        [TestCase(2, 12, "Name", SortDirection.Ascending, "TestName0000")]
        [TestCase(4, 5, "Name", SortDirection.Descending, "TestName0004")]
        public async Task FindPagedAsync_Sort_CheckFirstElement(int pageSize, int numberOfElements,
    string sortMember, SortDirection sortDirection, string firstName)
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, 0,
                null, SortOptionFactory.Build(sortMember, sortDirection));

            var result = await this.Repository.FindPagedAsync<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            await ClearMemoryAsync();
        }

        [TestCase(2, 12, 2, "TestName0002")]
        [TestCase(2, 12, 4, "TestName0004")]
        public void FindPaged_Skip_CheckFirstElement(int pageSize, int numberOfElements, int skip, string firstName)
        {
            PopulateUsers(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, skip,
                null, SortOptionFactory.Build("Name", SortDirection.Ascending));

            var result = this.Repository.FindPaged<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            ClearMemory();
        }

        [TestCase(2, 12, 2, "TestName0002")]
        [TestCase(2, 12, 4, "TestName0004")]
        public async Task FindPagedAsync_Skip_CheckFirstElement(int pageSize, int numberOfElements, int skip, string firstName)
        {
            await PopulateUsersAsync(numberOfElements);
            var pageOptions = PagedOptionFactory.Build(pageSize, skip,
                null, SortOptionFactory.Build("Name", SortDirection.Ascending));

            var result = await this.Repository.FindPagedAsync<User>(pageOptions, null);
            var firstValue = result.Result.FirstOrDefault()?.Name;
            Assert.True(firstValue == firstName);
            await ClearMemoryAsync();
        }

        private void PopulateUsers(int numberOfElements)
        {
            var users = UsersFactory.Build(numberOfElements);
            foreach (var user in users)
            {
                this.Repository.Add(user);
            }
            SaveChanges();
        }

        private async Task PopulateUsersAsync(int numberOfElements)
        {
            var users = UsersFactory.Build(numberOfElements);
            foreach (var user in users)
            {
                this.Repository.Add(user);
            }
            await SaveChangesAsync();
        }
    }
}
