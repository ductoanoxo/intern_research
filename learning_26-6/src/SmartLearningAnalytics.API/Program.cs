using Microsoft.EntityFrameworkCore;
using SmartLearningAnalytics.API.Data;
using SmartLearningAnalytics.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure SQLite Database
builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=data/analytics.db"));

// Register Analytics and Modeling Engines
builder.Services.AddScoped<IBktEngine, BktEngine>();
builder.Services.AddScoped<IIrtEngine, IrtEngine>();
builder.Services.AddScoped<IEdmEngine, EdmEngine>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Smart Learning Analytics Engine API", Version = "v1" });
});

var app = builder.Build();

// Automatically ensure SQLite database directory and schema are created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
    // Ensure the data directory exists for SQLite inside docker volume mapping
    var dbPath = Path.GetDirectoryName(dbContext.Database.GetDbConnection().ConnectionString.Split('=')[1]);
    if (!string.IsNullOrEmpty(dbPath) && !Directory.Exists(dbPath))
    {
        Directory.CreateDirectory(dbPath);
    }
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Learning Analytics API v1");
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
