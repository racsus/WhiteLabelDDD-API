using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Events
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(BaseDomainEvent domainEvent);
    }
}
