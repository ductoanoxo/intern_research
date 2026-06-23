using System;
using System.Security.Claims;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DddCqrsJwtDemo.Api.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                              ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var guid))
            {
                return guid;
            }
            return null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public UserRole? Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim, true, out var role))
            {
                return role;
            }
            return null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
