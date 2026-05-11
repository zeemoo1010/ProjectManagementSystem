using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Infrastructure.Persistence;
using ProjectManagementSystem.Infrastructure.Security;

namespace ProjectManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(options =>
            {
                var section = configuration.GetSection("Jwt");
                options.Issuer = section["Issuer"] ?? options.Issuer;
                options.Audience = section["Audience"] ?? options.Audience;
                options.Secret = section["Secret"] ?? options.Secret;
                if (int.TryParse(section["ExpirationMinutes"], out var expirationMinutes))
                    options.ExpirationMinutes = expirationMinutes;
            });
            services.AddSingleton<InMemoryDataStore>();
            services.AddScoped<IUserRepository, InMemoryUserRepository>();
            services.AddScoped<IProjectRepository, InMemoryProjectRepository>();
            services.AddScoped<ITaskItemRepository, InMemoryTaskItemRepository>();
            services.AddScoped<ICommentRepository, InMemoryCommentRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
