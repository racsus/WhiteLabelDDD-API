using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Domain.Events
{
    public class UserCreatedEvent : BaseDomainEvent
    {
        public Guid Id { get; }
        public User User { get; }

        public UserCreatedEvent(Guid id, User user)
        {
            Id = id;
            User = user;
        }
    }
}
