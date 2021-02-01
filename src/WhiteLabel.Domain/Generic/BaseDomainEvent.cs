using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.Domain.Generic
{
    public abstract class BaseDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
