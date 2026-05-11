using ProjectManagementSystem.Infrastructure.Security;
using Xunit;

namespace ProjectManagementSystem.UnitTests.Domain
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Verify_accepts_matching_password_and_rejects_wrong_password()
        {
            var hasher = new PasswordHasher();
            var hash = hasher.Hash("Admin123!");

            Assert.True(hasher.Verify("Admin123!", hash));
            Assert.False(hasher.Verify("wrong", hash));
        }
    }
}
