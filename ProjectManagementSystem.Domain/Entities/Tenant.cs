namespace ProjectManagementSystem.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        private readonly List<User> _users = new();
        private readonly List<Project> _projects = new();

        public Guid OwnerId { get; private set; }

        public string Name { get; private set; }

        public string Identifier { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
    }
}
