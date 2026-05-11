namespace ProjectManagementSystem.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = "ProjectManagementSystem";

        public string Audience { get; set; } = "ProjectManagementSystem.Api";

        public string Secret { get; set; } = "development-secret-key-change-before-production-123456";

        public int ExpirationMinutes { get; set; } = 120;
    }
}
