using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.UnitTest.Builders
{
    /// <summary>
    /// https://ardalis.com/improve-tests-with-the-builder-pattern-for-test-data/
    /// </summary>
    public class UserBuilder
    {
        private User _entity = new User();
        public UserBuilder Id(Guid id)
        {
            _entity.Id = id;
            return this;
        }

        public UserBuilder Name(string name)
        {
            _entity.Name = name;
            return this;
        }

        public UserBuilder Email(string email)
        {
            _entity.Email = email;
            return this;
        }


        // more methods omitted

        public User Build()
        {
            return _entity;
        }

        // This approach allows easy modification of test values
        // Another approach would just have a static method returning AddressDTO
        public UserBuilder WithTestValues()
        {
            _entity = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test Name",
                Email = "testname@testdomain.com"
            };
            return this;
        }
    }
}
