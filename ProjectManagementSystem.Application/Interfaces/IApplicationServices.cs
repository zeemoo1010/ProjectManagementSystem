using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }

    public interface IUserService
    {
        Task<IReadOnlyCollection<UserDto>> ListAsync(CancellationToken cancellationToken = default);
        Task<UserDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> CreateAsync(CreateUserRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(Guid id, Guid actorId, CancellationToken cancellationToken = default);
    }

    public interface IProjectService
    {
        Task<IReadOnlyCollection<ProjectDto>> ListAsync(ProjectStatus? status = null, CancellationToken cancellationToken = default);
        Task<ProjectDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
        Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
    }

    public interface ITaskService
    {
        Task<IReadOnlyCollection<TaskDto>> ListAsync(Guid? projectId = null, Guid? userId = null, TaskPriority? priority = null, CancellationToken cancellationToken = default);
        Task<TaskDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<TaskDto>> CreateAsync(CreateTaskRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
        Task<Result<TaskDto>> UpdateAsync(Guid id, UpdateTaskRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
    }

    public interface ICommentService
    {
        Task<IReadOnlyCollection<CommentDto>> ListByTaskAsync(Guid taskItemId, CancellationToken cancellationToken = default);
        Task<CommentDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<CommentDto>> CreateAsync(CreateCommentRequest request, Guid? actorId = null, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(Guid id, Guid actorId, CancellationToken cancellationToken = default);
    }

    public interface IReportService
    {
        Task<ReportSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
    }

    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }

    public interface IJwtTokenGenerator
    {
        string Generate(UserDto user);
    }
}
