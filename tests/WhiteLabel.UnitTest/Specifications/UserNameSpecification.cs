using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.UnitTest.Specifications
{
    public class UserNameSpecification : SpecificationBase<User>
    {
        public UserNameSpecification(string name)
            : base(x => x.Name == name) { }
    }
}
