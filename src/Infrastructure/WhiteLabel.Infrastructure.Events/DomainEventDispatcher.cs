﻿using Autofac;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Events
{
    // https://gist.github.com/jbogard/54d6569e883f63afebc7
    // http://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IComponentContext container;

        public DomainEventDispatcher(IComponentContext container)
        {
            this.container = container;
        }

        public void Dispatch(BaseDomainEvent domainEvent)
        {
            Type handlerType = typeof(IHandle<>).MakeGenericType(domainEvent.GetType());
            Type wrapperType = typeof(DomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            IEnumerable handlers = (IEnumerable)this.container.Resolve(typeof(IEnumerable<>).MakeGenericType(handlerType));
            IEnumerable<DomainEventHandler> wrappedHandlers = handlers.Cast<object>()
                .Select(handler => (DomainEventHandler)Activator.CreateInstance(wrapperType, handler));

            foreach (DomainEventHandler handler in wrappedHandlers)
            {
                handler.Handle(domainEvent);
            }
        }

        private abstract class DomainEventHandler
        {
            public abstract void Handle(BaseDomainEvent domainEvent);
        }

        private class DomainEventHandler<T> : DomainEventHandler
            where T : BaseDomainEvent
        {
            private readonly IHandle<T> _handler;

            public DomainEventHandler(IHandle<T> handler)
            {
                _handler = handler;
            }

            public override void Handle(BaseDomainEvent domainEvent)
            {
                _handler.Handle((T)domainEvent);
            }
        }
    }
}