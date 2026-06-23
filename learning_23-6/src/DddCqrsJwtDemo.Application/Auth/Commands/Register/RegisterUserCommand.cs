using System;
using DddCqrsJwtDemo.Domain.Enums;
using MediatR;

namespace DddCqrsJwtDemo.Application.Auth.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FullName,
    UserRole Role = UserRole.Customer) : IRequest<Guid>;
