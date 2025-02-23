using System;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.UnitTest.Builders
{
    /// <summary>
    /// https://ardalis.com/improve-tests-with-the-builder-pattern-for-test-data/
    /// </summary>
    public class UserBuilder
    {
        private User entity = new();

        public UserBuilder Id(Guid id)
        {
            entity.Id = id;
            return this;
        }

        public UserBuilder Name(string name)
        {
            entity.Name = name;
            return this;
        }

        public UserBuilder Email(string email)
        {
            entity.Email = email;
            return this;
        }

        // more methods omitted

        public User Build()
        {
            return entity;
        }

        // This approach allows easy modification of test values
        // Another approach would just have a static method returning AddressDTO
        public UserBuilder WithTestValues()
        {
            entity = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test Name",
                Email = "testname@testdomain.com",
            };
            return this;
        }
    }
}
