using CleanArchitectureDemo.Application;
using CleanArchitectureDemo.Infrastructure;
using CleanArchitectureDemo.Infrastructure.Persistence;
using CleanArchitectureDemo.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Clean Architecture & MediatR Book Store API",
        Version = "v1",
        Description = "Demonstrating SOLID principles, Clean Architecture, and MediatR / Mediator pattern in .NET 8."
    });
});

// Register Clean Architecture layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Initialize and Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BookStoreDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Enable Swagger UI for development and demos
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API v1");
    c.RoutePrefix = string.Empty; // Swagger at root URL
});

// Register custom global exception handler middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
