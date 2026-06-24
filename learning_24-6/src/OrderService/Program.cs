using InventoryService.Protos;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to run REST API on port 8081
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DB Context with SQLite
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OrderConnection") ?? "Data Source=orders.db"));

// Configure gRPC Client for Inventory Service
builder.Services.AddGrpcClient<InventoryGrpc.InventoryGrpcClient>(o =>
{
    var inventoryGrpcUrl = builder.Configuration["GrpcSettings:InventoryUrl"] ?? "http://localhost:8083";
    o.Address = new Uri(inventoryGrpcUrl);
});

// Configure MassTransit with RabbitMQ (Publisher only)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.EnsureCreated();
}

// Swagger UI configuration
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Order Service is running. REST API on port 8081.");

app.Run();
