using System.Collections.Concurrent;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Infrastructure.Persistence
{
    public sealed class InMemoryDataStore
    {
        public ConcurrentDictionary<Guid, User> Users { get; } = new();

        public ConcurrentDictionary<Guid, Project> Projects { get; } = new();

        public ConcurrentDictionary<Guid, TaskItem> Tasks { get; } = new();

        public ConcurrentDictionary<Guid, Comment> Comments { get; } = new();
    }
}
