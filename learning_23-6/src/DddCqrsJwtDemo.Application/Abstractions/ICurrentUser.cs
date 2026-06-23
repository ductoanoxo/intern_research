using System;
using DddCqrsJwtDemo.Domain.Enums;

namespace DddCqrsJwtDemo.Application.Abstractions;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}
