using System;
using System.Text;
using DddCqrsJwtDemo.Api.Middleware;
using DddCqrsJwtDemo.Api.Services;
using DddCqrsJwtDemo.Application;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Infrastructure;
using DddCqrsJwtDemo.Infrastructure.Authentication;
using DddCqrsJwtDemo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Layers DI (Application & Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Add API services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. Configure JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection.GetValue<string>("SecretKey") ?? "super_secret_key_that_is_long_enough_for_hmac_256";
var issuer = jwtSection.GetValue<string>("Issuer") ?? "BookStoreApi";
var audience = jwtSection.GetValue<string>("Audience") ?? "BookStoreClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove standard 5 mins tolerance for local testing
    };
});

builder.Services.AddAuthorization();

// 4. Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DddCqrsJwtDemo.Api", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field (e.g. 'Bearer <token>')",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 5. Seed Database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DbInitializer.InitializeAsync(context, passwordHasher);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// 6. HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
