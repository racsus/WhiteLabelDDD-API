using System;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Domain.Specification.Users
{
    public class UserRegisteredSpec : SpecificationBase<User>
    {
        public UserRegisteredSpec(Guid userId)
            : base(x => x.Id == userId) { }
    }
}
