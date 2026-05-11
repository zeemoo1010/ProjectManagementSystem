using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ProjectManagementSystem.IntegrationTests.Api
{
    public class AuthFlowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthFlowTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureLogging(logging => logging.ClearProviders());
            });
        }

        [Fact]
        public async Task Login_with_seeded_admin_returns_token()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/auth/login", new { email = "admin@pms.local", password = "Admin123!" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", content, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Admin_can_create_project_task_and_comment()
        {
            var client = _factory.CreateClient();
            var token = await LoginAsync(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var managerId = await PostAndReadIdAsync(client, "/users", new
            {
                firstName = "Project",
                lastName = "Manager",
                email = $"manager-{Guid.NewGuid():N}@pms.local",
                password = "Password123!",
                tenantId,
                role = "ProjectManager"
            });

            var memberId = await PostAndReadIdAsync(client, "/users", new
            {
                firstName = "Team",
                lastName = "Member",
                email = $"member-{Guid.NewGuid():N}@pms.local",
                password = "Password123!",
                tenantId,
                role = "TeamMember"
            });

            var projectId = await PostAndReadIdAsync(client, "/projects", new
            {
                name = "Delivery Portal",
                description = "Internal delivery tracking",
                tenantId,
                managerId,
                startDate = "2026-05-01",
                endDate = "2026-05-31"
            });

            var taskId = await PostAndReadIdAsync(client, "/tasks", new
            {
                title = "Create API flow",
                description = "Wire project task flow",
                projectId,
                assignedToUserId = memberId,
                dueDate = "2026-05-15",
                priority = "High"
            });

            var commentId = await PostAndReadIdAsync(client, "/comments", new
            {
                taskItemId = taskId,
                authorId = memberId,
                body = "Initial progress recorded."
            });

            var commentsResponse = await client.GetAsync($"/comments/task/{taskId}");

            Assert.Equal(HttpStatusCode.OK, commentsResponse.StatusCode);
            Assert.NotEqual(Guid.Empty, projectId);
            Assert.NotEqual(Guid.Empty, taskId);
            Assert.NotEqual(Guid.Empty, commentId);
        }

        [Fact]
        public async Task Projects_endpoint_requires_authentication()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/projects");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private static async Task<string> LoginAsync(HttpClient client)
        {
            var response = await client.PostAsJsonAsync("/auth/login", new { email = "admin@pms.local", password = "Admin123!" });
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("data").GetProperty("token").GetString()
                ?? throw new InvalidOperationException("Login response did not include a token.");
        }

        private static async Task<Guid> PostAndReadIdAsync(HttpClient client, string url, object body)
        {
            var response = await client.PostAsJsonAsync(url, body);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var id = document.RootElement.GetProperty("data").GetProperty("id").GetGuid();
            Assert.NotEqual(Guid.Empty, id);
            return id;
        }
    }
}
