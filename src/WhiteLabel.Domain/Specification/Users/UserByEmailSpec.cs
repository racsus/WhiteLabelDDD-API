using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Domain.Specification.Users
{
    public class UserByEmailSpec(string email) : SpecificationBase<User>(x => x.Email == email);
}
