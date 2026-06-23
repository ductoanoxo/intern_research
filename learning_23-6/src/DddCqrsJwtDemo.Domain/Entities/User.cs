using System;
using DddCqrsJwtDemo.Domain.Enums;
using DddCqrsJwtDemo.Domain.Primitives;
using DddCqrsJwtDemo.Domain.ValueObjects;

namespace DddCqrsJwtDemo.Domain.Entities;

public class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }

    private User(Guid id, Email email, string passwordHash, string fullName, UserRole role) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
    }

    // EF Core constructor
    private User()
    {
    }

    public static User Create(Guid id, Email email, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        return new User(id, email, passwordHash, fullName, role);
    }
}
