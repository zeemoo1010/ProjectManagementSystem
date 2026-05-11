using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Domain.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<IReadOnlyCollection<TaskItem>> ListAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<TaskItem>> ListByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<TaskItem>> ListByPriorityAsync(TaskPriority priority, CancellationToken cancellationToken = default);
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    }
}
