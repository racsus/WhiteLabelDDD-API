using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Users;
using WhiteLabel.UnitTest.Builders;

namespace WhiteLabel.UnitTest.Factory
{
    public static class UsersFactory
    {
        public static List<User> Build(int numElements)
        {
            var res = new List<User>();

            for (int i = 0; i < numElements; i++)
            {
                var newUser = new UserBuilder()
                        .WithTestValues()
                        .Name($"TestName{i:0000}")
                        .Email($"TestName{i:0000}@testdomain.com")
                        .Build();

                res.Add(newUser);
            }

            return res;
        }
    }
}
