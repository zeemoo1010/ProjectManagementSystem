using MediatR;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Hash password (temporary)
            var passwordHash = request.Password;

            // Step 2: Create User entity
            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                passwordHash,
                request.TenantId,
                request.Role
            );

            // Step 3: Simulate saving (we will replace later)
            await Task.CompletedTask;

            // Step 4: Return User Id
            return user.Id;
        }
    }
}
