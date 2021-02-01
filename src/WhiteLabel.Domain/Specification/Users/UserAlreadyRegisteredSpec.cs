using System;
using System.Linq.Expressions;
using WhiteLabel.Domain.Users;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Specification.Users
{
    public class UserAlreadyRegisteredSpec : SpecificationBase<User>
    {
        public UserAlreadyRegisteredSpec(string email)
            : base(x => x.Email == email)
        {
        }
    }
}
