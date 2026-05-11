using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Infrastructure.Persistence
{
    public sealed class InMemoryUserRepository(InMemoryDataStore store) : IUserRepository
    {
        public Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<User>>(store.Users.Values.ToArray());

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.ToLower().Trim();
            return Task.FromResult(store.Users.Values.FirstOrDefault(user => user.Email == normalizedEmail));
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            store.Users[user.Id] = user;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            store.Users[user.Id] = user;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryProjectRepository(InMemoryDataStore store) : IProjectRepository
    {
        public Task<IReadOnlyCollection<Project>> ListAsync(ProjectStatus? status = null, CancellationToken cancellationToken = default)
        {
            var projects = store.Projects.Values.Where(project => !status.HasValue || project.Status == status.Value).ToArray();
            return Task.FromResult<IReadOnlyCollection<Project>>(projects);
        }

        public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Projects.TryGetValue(id, out var project);
            return Task.FromResult(project);
        }

        public Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            store.Projects[project.Id] = project;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            store.Projects[project.Id] = project;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryTaskItemRepository(InMemoryDataStore store) : ITaskItemRepository
    {
        public Task<IReadOnlyCollection<TaskItem>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<TaskItem>>(store.Tasks.Values.ToArray());

        public Task<IReadOnlyCollection<TaskItem>> ListByProjectAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<TaskItem>>(store.Tasks.Values.Where(task => task.ProjectId == projectId).ToArray());

        public Task<IReadOnlyCollection<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<TaskItem>>(store.Tasks.Values.Where(task => task.AssignedToUserId == userId).ToArray());

        public Task<IReadOnlyCollection<TaskItem>> ListByPriorityAsync(TaskPriority priority, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<TaskItem>>(store.Tasks.Values.Where(task => task.Priority == priority).ToArray());

        public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Tasks.TryGetValue(id, out var taskItem);
            return Task.FromResult(taskItem);
        }

        public Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
        {
            store.Tasks[taskItem.Id] = taskItem;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
        {
            store.Tasks[taskItem.Id] = taskItem;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryCommentRepository(InMemoryDataStore store) : ICommentRepository
    {
        public Task<IReadOnlyCollection<Comment>> ListByTaskAsync(Guid taskItemId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<Comment>>(store.Comments.Values.Where(comment => comment.TaskItemId == taskItemId).ToArray());

        public Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Comments.TryGetValue(id, out var comment);
            return Task.FromResult(comment);
        }

        public Task AddAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            store.Comments[comment.Id] = comment;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            store.Comments[comment.Id] = comment;
            return Task.CompletedTask;
        }
    }
}
