using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Infrastructure.Authentication;
using DddCqrsJwtDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DddCqrsJwtDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Configure Password Hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // 2. Configure JWT options and Provider
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtProvider, JwtProvider>();

        // 3. Configure Database connection (SQLite)
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? "Data Source=bookstore.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // 4. Map DbContext interface
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
