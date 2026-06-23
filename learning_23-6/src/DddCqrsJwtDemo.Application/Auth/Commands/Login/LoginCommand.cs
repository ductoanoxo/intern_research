using MediatR;

namespace DddCqrsJwtDemo.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponse>;
