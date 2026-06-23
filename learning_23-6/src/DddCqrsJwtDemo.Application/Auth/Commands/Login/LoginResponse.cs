namespace DddCqrsJwtDemo.Application.Auth.Commands.Login;

public record LoginResponse(
    string Token,
    string Email,
    string FullName,
    string Role);
