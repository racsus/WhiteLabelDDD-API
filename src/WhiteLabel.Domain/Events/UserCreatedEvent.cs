using System;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Domain.Events
{
    public class UserCreatedEvent(Guid id, User user) : BaseDomainEvent
    {
        public Guid Id { get; } = id;
        public User User { get; } = user;
    }
}
