using System;

namespace DddCqrsJwtDemo.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
