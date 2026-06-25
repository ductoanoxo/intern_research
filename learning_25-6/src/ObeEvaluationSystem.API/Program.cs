using Microsoft.EntityFrameworkCore;
using ObeEvaluationSystem.API.Data;
using ObeEvaluationSystem.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure SQLite Database
builder.Services.AddDbContext<ObeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=obe.db"));

// Register OBE Evaluation Service
builder.Services.AddScoped<IObeEvaluationService, ObeEvaluationService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "OBE Evaluation Engine API", Version = "v1" });
});

var app = builder.Build();

// Automatically ensure SQLite database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ObeDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OBE Evaluation Engine API v1");
    c.RoutePrefix = "swagger"; // Expose Swagger at /swagger
});

app.UseAuthorization();

app.MapControllers();

// Redirect root URL to swagger
app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger");
    await Task.CompletedTask;
});

app.Run();
