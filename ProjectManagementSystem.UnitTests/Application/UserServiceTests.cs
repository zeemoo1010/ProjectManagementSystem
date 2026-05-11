using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Services;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Infrastructure.Persistence;
using ProjectManagementSystem.Infrastructure.Security;
using Xunit;

namespace ProjectManagementSystem.UnitTests.Application
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_rejects_duplicate_email()
        {
            var store = new InMemoryDataStore();
            var repository = new InMemoryUserRepository(store);
            var service = new UserService(repository, new PasswordHasher());
            var tenantId = Guid.NewGuid();
            var request = new CreateUserRequest("Ada", "Lovelace", "ada@example.com", "Password123!", tenantId, UserRole.Admin);

            var first = await service.CreateAsync(request);
            var second = await service.CreateAsync(request);

            Assert.True(first.Succeeded);
            Assert.False(second.Succeeded);
            Assert.Equal("A user with this email already exists.", second.Error);
        }

        [Fact]
        public async Task DeleteAsync_soft_deletes_user()
        {
            var store = new InMemoryDataStore();
            var repository = new InMemoryUserRepository(store);
            var service = new UserService(repository, new PasswordHasher());
            var created = await service.CreateAsync(new CreateUserRequest("Grace", "Hopper", "grace@example.com", "Password123!", Guid.NewGuid(), UserRole.TeamMember));

            var deleted = await service.DeleteAsync(created.Value!.Id, Guid.NewGuid());
            var loaded = await service.GetAsync(created.Value.Id);

            Assert.True(deleted.Succeeded);
            Assert.Null(loaded);
        }
    }
}
