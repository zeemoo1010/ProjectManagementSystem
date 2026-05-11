using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.Services
{
    internal static class MappingExtensions
    {
        internal static UserDto ToDto(this User user) =>
            new(user.Id, user.FirstName, user.LastName, user.FullName, user.Email, user.TenantId, user.Role, user.IsActive);

        internal static ProjectDto ToDto(this Project project) =>
            new(project.Id, project.Name, project.Description, project.TenantId, project.ManagerId, project.StartDate, project.EndDate, project.Status);

        internal static TaskDto ToDto(this TaskItem taskItem) =>
            new(taskItem.Id, taskItem.Title, taskItem.Description, taskItem.ProjectId, taskItem.AssignedToUserId, taskItem.DueDate, taskItem.Priority, taskItem.Status, taskItem.IsOverdue(DateOnly.FromDateTime(DateTime.UtcNow)));

        internal static CommentDto ToDto(this Comment comment) =>
            new(comment.Id, comment.TaskItemId, comment.AuthorId, comment.Body, comment.CreatedAt);
    }
}
