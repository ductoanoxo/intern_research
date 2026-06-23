using System;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Parse email using Value Object rules
        var email = Email.Create(request.Email);

        // Check uniqueness
        var emailExists = await _context.Users.AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
        if (emailExists)
        {
            throw new ArgumentException($"Email '{request.Email}' is already in use.");
        }

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Instantiate Aggregate Root
        var user = User.Create(
            Guid.NewGuid(),
            email,
            passwordHash,
            request.FullName,
            request.Role
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
