using System;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Domain.Specification.Users
{
    public class UserByIdSpec(Guid userId) : SpecificationBase<User>(x => x.Id == userId);
}
