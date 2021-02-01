
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Generic
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        void Handle(T domainEvent);
    }
}