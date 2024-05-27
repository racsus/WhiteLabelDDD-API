using System.Collections.Generic;
using WhiteLabel.Domain.Users;
using WhiteLabel.UnitTest.Builders;

namespace WhiteLabel.UnitTest.Factory
{
    public static class UsersFactory
    {
        public static List<User> Build(int numElements)
        {
            var res = new List<User>();

            for (var i = 0; i < numElements; i++)
            {
                var newUser = new UserBuilder()
                    .WithTestValues()
                    .Name($"TestName{i:0000}")
                    .Email($"testname{i:0000}@testdomain.com")
                    .Build();

                res.Add(newUser);
            }

            return res;
        }
    }
}
