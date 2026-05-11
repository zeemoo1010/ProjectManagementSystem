using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class AuthService(IUserRepository users, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator) : IAuthService
    {
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await users.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null || user.IsDeleted || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure("Invalid email or password.");

            var userDto = user.ToDto();
            return Result<AuthResponse>.Success(new AuthResponse(tokenGenerator.Generate(userDto), userDto));
        }
    }
}
