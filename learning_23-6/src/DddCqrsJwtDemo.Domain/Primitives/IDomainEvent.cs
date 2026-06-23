using System;

namespace DddCqrsJwtDemo.Domain.Primitives;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
