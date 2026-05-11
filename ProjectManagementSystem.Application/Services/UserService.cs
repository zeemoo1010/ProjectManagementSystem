using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class UserService(IUserRepository users, IPasswordHasher passwordHasher) : IUserService
    {
        public async Task<IReadOnlyCollection<UserDto>> ListAsync(CancellationToken cancellationToken = default)
        {
            var allUsers = await users.ListAsync(cancellationToken);
            return allUsers.Where(user => !user.IsDeleted).Select(user => user.ToDto()).ToArray();
        }

        public async Task<UserDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await users.GetByIdAsync(id, cancellationToken);
            return user is null || user.IsDeleted ? null : user.ToDto();
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            if (await users.GetByEmailAsync(request.Email, cancellationToken) is not null)
                return Result<UserDto>.Failure("A user with this email already exists.");

            var user = new User(request.FirstName, request.LastName, request.Email, passwordHasher.Hash(request.Password), request.TenantId, request.Role);
            if (actorId.HasValue)
                user.RecordCreatedBy(actorId.Value);

            await users.AddAsync(user, cancellationToken);
            return Result<UserDto>.Success(user.ToDto());
        }

        public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            var user = await users.GetByIdAsync(id, cancellationToken);
            if (user is null || user.IsDeleted)
                return Result<UserDto>.Failure("User was not found.");

            user.Update(request.FirstName, request.LastName, request.Email, request.Role);
            if (request.IsActive)
                user.Activate();
            else
                user.Deactivate();

            if (actorId.HasValue)
                user.RecordModifiedBy(actorId.Value);

            await users.UpdateAsync(user, cancellationToken);
            return Result<UserDto>.Success(user.ToDto());
        }

        public async Task<Result<bool>> DeleteAsync(Guid id, Guid actorId, CancellationToken cancellationToken = default)
        {
            var user = await users.GetByIdAsync(id, cancellationToken);
            if (user is null || user.IsDeleted)
                return Result<bool>.Failure("User was not found.");

            user.SoftDelete(actorId);
            await users.UpdateAsync(user, cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
