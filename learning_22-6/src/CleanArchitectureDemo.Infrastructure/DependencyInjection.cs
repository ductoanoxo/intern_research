using CleanArchitectureDemo.Application.Common.Interfaces;
using CleanArchitectureDemo.Infrastructure.Persistence;
using CleanArchitectureDemo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bookstore.db";

        // Register SQLite Database Context
        services.AddDbContext<BookStoreDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register Repository implementation
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
}
