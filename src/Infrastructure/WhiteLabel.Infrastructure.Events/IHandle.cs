
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Events
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        void Handle(T domainEvent);
    }
}