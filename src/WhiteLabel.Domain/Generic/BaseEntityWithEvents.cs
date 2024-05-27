using System.Collections.Generic;

namespace WhiteLabel.Domain.Generic
{
    public abstract class BaseEntityWithEvents
    {
        public readonly List<BaseDomainEvent> Events = new();
    }
}
