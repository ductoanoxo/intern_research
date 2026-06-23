using DddCqrsJwtDemo.Domain.Primitives;
using MediatR;

namespace DddCqrsJwtDemo.Application.Events;

public class DomainEventNotification<TEvent> : INotification
    where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; }

    public DomainEventNotification(TEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
