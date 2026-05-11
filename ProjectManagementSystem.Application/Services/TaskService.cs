using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class TaskService(ITaskItemRepository tasks, IProjectRepository projects, IUserRepository users) : ITaskService
    {
        public async Task<IReadOnlyCollection<TaskDto>> ListAsync(Guid? projectId = null, Guid? userId = null, TaskPriority? priority = null, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<TaskItem> taskItems;
            if (projectId.HasValue)
                taskItems = await tasks.ListByProjectAsync(projectId.Value, cancellationToken);
            else if (userId.HasValue)
                taskItems = await tasks.ListByUserAsync(userId.Value, cancellationToken);
            else if (priority.HasValue)
                taskItems = await tasks.ListByPriorityAsync(priority.Value, cancellationToken);
            else
                taskItems = await tasks.ListAsync(cancellationToken);

            return taskItems.Where(task => !task.IsDeleted).Select(task => task.ToDto()).ToArray();
        }

        public async Task<TaskDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var task = await tasks.GetByIdAsync(id, cancellationToken);
            return task is null || task.IsDeleted ? null : task.ToDto();
        }

        public async Task<Result<TaskDto>> CreateAsync(CreateTaskRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            if (await projects.GetByIdAsync(request.ProjectId, cancellationToken) is null)
                return Result<TaskDto>.Failure("Project was not found.");

            var assignee = await users.GetByIdAsync(request.AssignedToUserId, cancellationToken);
            if (assignee is null || !assignee.IsActive)
                return Result<TaskDto>.Failure("Assigned user was not found or is inactive.");

            var task = new TaskItem(request.Title, request.Description, request.ProjectId, request.AssignedToUserId, request.DueDate, request.Priority);
            if (actorId.HasValue)
                task.RecordCreatedBy(actorId.Value);

            await tasks.AddAsync(task, cancellationToken);
            return Result<TaskDto>.Success(task.ToDto());
        }

        public async Task<Result<TaskDto>> UpdateAsync(Guid id, UpdateTaskRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            var task = await tasks.GetByIdAsync(id, cancellationToken);
            if (task is null || task.IsDeleted)
                return Result<TaskDto>.Failure("Task was not found.");

            task.Update(request.Title, request.Description, request.AssignedToUserId, request.DueDate, request.Priority);
            task.ChangeStatus(request.Status);

            if (actorId.HasValue)
                task.RecordModifiedBy(actorId.Value);

            await tasks.UpdateAsync(task, cancellationToken);
            return Result<TaskDto>.Success(task.ToDto());
        }
    }
}
