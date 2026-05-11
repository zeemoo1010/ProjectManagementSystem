using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Application.DTOs
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed record AuthResponse(string Token, UserDto User);

    public sealed record CreateUserRequest(string FirstName, string LastName, string Email, string Password, Guid TenantId, UserRole Role);

    public sealed record UpdateUserRequest(string FirstName, string LastName, string Email, UserRole Role, bool IsActive);

    public sealed record UserDto(Guid Id, string FirstName, string LastName, string FullName, string Email, Guid TenantId, UserRole Role, bool IsActive);

    public sealed record CreateProjectRequest(string Name, string? Description, Guid TenantId, Guid ManagerId, DateOnly StartDate, DateOnly EndDate);

    public sealed record UpdateProjectRequest(string Name, string? Description, Guid ManagerId, DateOnly StartDate, DateOnly EndDate, ProjectStatus Status);

    public sealed record ProjectDto(Guid Id, string Name, string? Description, Guid TenantId, Guid ManagerId, DateOnly StartDate, DateOnly EndDate, ProjectStatus Status);

    public sealed record CreateTaskRequest(string Title, string? Description, Guid ProjectId, Guid AssignedToUserId, DateOnly DueDate, TaskPriority Priority);

    public sealed record UpdateTaskRequest(string Title, string? Description, Guid AssignedToUserId, DateOnly DueDate, TaskPriority Priority, ProjectManagementSystem.Domain.Enums.TaskStatus Status);

    public sealed record TaskDto(Guid Id, string Title, string? Description, Guid ProjectId, Guid AssignedToUserId, DateOnly DueDate, TaskPriority Priority, ProjectManagementSystem.Domain.Enums.TaskStatus Status, bool IsOverdue);

    public sealed record CreateCommentRequest(Guid TaskItemId, Guid AuthorId, string Body);

    public sealed record CommentDto(Guid Id, Guid TaskItemId, Guid AuthorId, string Body, DateTime CreatedAt);

    public sealed record ReportSummary(
        IReadOnlyDictionary<ProjectStatus, int> ProjectsByStatus,
        IReadOnlyDictionary<TaskPriority, int> TasksByPriority,
        int OverdueTasks,
        IReadOnlyCollection<UserWorkloadDto> UserWorkloads,
        IReadOnlyCollection<MonthlyCompletedTasksDto> CompletedTasksPerMonth);

    public sealed record UserWorkloadDto(Guid UserId, string FullName, int OpenTasks, int CompletedTasks);

    public sealed record MonthlyCompletedTasksDto(int Year, int Month, int CompletedTasks);
}
