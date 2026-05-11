using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task<IReadOnlyCollection<Comment>> ListByTaskAsync(Guid taskItemId, CancellationToken cancellationToken = default);
        Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Comment comment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Comment comment, CancellationToken cancellationToken = default);
    }
}
