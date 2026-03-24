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

        public Tenant(Guid ownerId, string name, string identifier)
        {
            if (ownerId == Guid.Empty)
                throw new ArgumentException("OwnerId cannot be empty");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier is required");

            OwnerId = ownerId;
            Name = name;
            Identifier = identifier;
            IsActive = true;
        }

        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (_users.Any(u => u.Id == user.Id))
                return;

            _users.Add(user);
        }


        public void AddProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (_projects.Any(p => p.Id == project.Id))
                return;

            _projects.Add(project);
        }
    }
}
