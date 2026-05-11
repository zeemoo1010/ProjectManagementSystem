using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class CommentService(ICommentRepository comments, ITaskItemRepository tasks, IUserRepository users) : ICommentService
    {
        public async Task<IReadOnlyCollection<CommentDto>> ListByTaskAsync(Guid taskItemId, CancellationToken cancellationToken = default)
        {
            var taskComments = await comments.ListByTaskAsync(taskItemId, cancellationToken);
            return taskComments.Where(comment => !comment.IsDeleted).Select(comment => comment.ToDto()).ToArray();
        }

        public async Task<CommentDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var comment = await comments.GetByIdAsync(id, cancellationToken);
            return comment is null || comment.IsDeleted ? null : comment.ToDto();
        }

        public async Task<Result<CommentDto>> CreateAsync(CreateCommentRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            if (await tasks.GetByIdAsync(request.TaskItemId, cancellationToken) is null)
                return Result<CommentDto>.Failure("Task was not found.");

            if (await users.GetByIdAsync(request.AuthorId, cancellationToken) is null)
                return Result<CommentDto>.Failure("Author was not found.");

            var comment = new Comment(request.TaskItemId, request.AuthorId, request.Body);
            if (actorId.HasValue)
                comment.RecordCreatedBy(actorId.Value);

            await comments.AddAsync(comment, cancellationToken);
            return Result<CommentDto>.Success(comment.ToDto());
        }

        public async Task<Result<bool>> DeleteAsync(Guid id, Guid actorId, CancellationToken cancellationToken = default)
        {
            var comment = await comments.GetByIdAsync(id, cancellationToken);
            if (comment is null || comment.IsDeleted)
                return Result<bool>.Failure("Comment was not found.");

            comment.SoftDelete(actorId);
            await comments.UpdateAsync(comment, cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
