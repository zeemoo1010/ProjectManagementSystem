using MediatR;
using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        Guid TenantId,
        UserRole Role
    ) : IRequest<Guid>;
}