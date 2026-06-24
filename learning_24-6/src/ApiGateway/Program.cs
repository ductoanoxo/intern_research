var builder = WebApplication.CreateBuilder(args);

// Add YARP reverse proxy services
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();

// Map YARP routes
app.MapReverseProxy();

app.MapGet("/", () => "API Gateway is running. Route to /api/orders or /api/inventory.");

app.Run();
