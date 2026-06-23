using DddCqrsJwtDemo.Domain.Entities;

namespace DddCqrsJwtDemo.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
}
