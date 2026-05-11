using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class ReportService(IProjectRepository projects, ITaskItemRepository tasks, IUserRepository users) : IReportService
    {
        public async Task<ReportSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            var allProjects = await projects.ListAsync(null, cancellationToken);
            var allTasks = await tasks.ListAsync(cancellationToken);
            var allUsers = await users.ListAsync(cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var projectsByStatus = Enum.GetValues<ProjectStatus>()
                .ToDictionary(status => status, status => allProjects.Count(project => !project.IsDeleted && project.Status == status));

            var tasksByPriority = Enum.GetValues<TaskPriority>()
                .ToDictionary(priority => priority, priority => allTasks.Count(task => !task.IsDeleted && task.Priority == priority));

            var workloads = allUsers
                .Where(user => !user.IsDeleted)
                .Select(user => new UserWorkloadDto(
                    user.Id,
                    user.FullName,
                    allTasks.Count(task => !task.IsDeleted && task.AssignedToUserId == user.Id && task.Status != ProjectManagementSystem.Domain.Enums.TaskStatus.Completed),
                    allTasks.Count(task => !task.IsDeleted && task.AssignedToUserId == user.Id && task.Status == ProjectManagementSystem.Domain.Enums.TaskStatus.Completed)))
                .ToArray();

            var completedByMonth = allTasks
                .Where(task => !task.IsDeleted && task.CompletedAt.HasValue)
                .GroupBy(task => new { task.CompletedAt!.Value.Year, task.CompletedAt.Value.Month })
                .Select(group => new MonthlyCompletedTasksDto(group.Key.Year, group.Key.Month, group.Count()))
                .OrderBy(item => item.Year)
                .ThenBy(item => item.Month)
                .ToArray();

            return new ReportSummary(
                projectsByStatus,
                tasksByPriority,
                allTasks.Count(task => !task.IsDeleted && task.IsOverdue(today)),
                workloads,
                completedByMonth);
        }
    }
}
