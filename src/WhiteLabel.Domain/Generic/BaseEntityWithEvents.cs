using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.Domain.Generic
{
    public abstract class BaseEntityWithEvents
    {
        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
    }
}
