using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectManagementSystem.API.Endpoints;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Application.Services;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Infrastructure;
using ProjectManagementSystem.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProjectManagementSystemAPI",
        Version = "v1",
        Description = "RESTful API for managing projects, tasks, users, comments, and delivery reports."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT returned from /auth/login."
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(nameof(UserRole.Admin)));
    options.AddPolicy("ProjectWrite", policy => policy.RequireRole(nameof(UserRole.Admin), nameof(UserRole.ProjectManager)));
    options.AddPolicy("TaskWrite", policy => policy.RequireRole(nameof(UserRole.Admin), nameof(UserRole.ProjectManager), nameof(UserRole.TeamMember)));
    options.AddPolicy("ReportsRead", policy => policy.RequireRole(nameof(UserRole.Admin), nameof(UserRole.ProjectManager), nameof(UserRole.Viewer)));
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = new JwtOptions
{
    Issuer = jwtSection["Issuer"] ?? "ProjectManagementSystem",
    Audience = jwtSection["Audience"] ?? "ProjectManagementSystem.Api",
    Secret = jwtSection["Secret"] ?? "development-secret-key-change-before-production-123456",
    ExpirationMinutes = int.TryParse(jwtSection["ExpirationMinutes"], out var expirationMinutes) ? expirationMinutes : 120
};
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectManagementSystemAPI v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapProjectManagementEndpoints();

await SeedDevelopmentAdminAsync(app.Services);

app.Run();

static async Task SeedDevelopmentAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var users = scope.ServiceProvider.GetRequiredService<IUserService>();
    var existing = await users.ListAsync();
    if (existing.Any())
        return;

    var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    await users.CreateAsync(new CreateUserRequest("System", "Admin", "admin@pms.local", "Admin123!", tenantId, UserRole.Admin));
}

public partial class Program;
