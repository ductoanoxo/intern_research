var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = TimeProvider.System.GetUtcNow(),
    Environment = app.Environment.EnvironmentName
}));

// Demo C# 12 Collection Expression
app.MapGet("/demo/collections", () =>
{
    int[] numbers = [1, 2, 3, 4, 5];
    List<string> fruits = ["Apple", "Banana", "Cherry"];
    int[] combined = [.. numbers, 6, 7, 8];

    return Results.Ok(new { numbers, fruits, combined });
});

// Demo Primary Constructor (C# 12)
app.MapGet("/demo/user/{name}", (string name) =>
{
    var user = new UserDto(name, "intern@company.com");
    return Results.Ok(user);
});

app.Run();

// Primary Constructor — C# 12
public class UserDto(string name, string email)
{
    public string Name { get; } = name;
    public string Email { get; } = email;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
