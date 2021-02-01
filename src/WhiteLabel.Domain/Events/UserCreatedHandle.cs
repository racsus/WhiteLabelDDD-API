using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Events
{
    public class UserCreatedHandle: IHandle<UserCreatedEvent>
    {

        public UserCreatedHandle()
        {

        }

        public void Handle(UserCreatedEvent userCreatedEvent)
        {
            //TODO
        }
    }
}
