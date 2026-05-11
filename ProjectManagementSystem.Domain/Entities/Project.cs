using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Project : BaseEntity
    {
        private readonly HashSet<TaskItem> _tasks = new();

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public Guid TenantId { get; private set; }

        public Guid ManagerId { get; private set; }

        public DateOnly StartDate { get; private set; }

        public DateOnly EndDate { get; private set; }

        public ProjectStatus Status { get; private set; }

        public IReadOnlyCollection<TaskItem> Tasks => _tasks;

        public Project(string name, string? description, Guid tenantId, Guid managerId, DateOnly startDate, DateOnly endDate)
        {
            Validate(name, tenantId, managerId, startDate, endDate);

            Name = name.Trim();
            Description = description?.Trim();
            TenantId = tenantId;
            ManagerId = managerId;
            StartDate = startDate;
            EndDate = endDate;
            Status = ProjectStatus.Planning;
        }

        public void Update(string name, string? description, Guid managerId, DateOnly startDate, DateOnly endDate)
        {
            Validate(name, TenantId, managerId, startDate, endDate);

            Name = name.Trim();
            Description = description?.Trim();
            ManagerId = managerId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Activate() => Status = ProjectStatus.Active;

        public void Complete() => Status = ProjectStatus.Completed;

        public void Archive() => Status = ProjectStatus.Archived;

        public void AddTask(TaskItem task)
        {
            if (task.ProjectId != Id)
                throw new InvalidOperationException("Task belongs to a different project");

            _tasks.Add(task);
        }

        private static void Validate(string name, Guid tenantId, Guid managerId, DateOnly startDate, DateOnly endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required", nameof(name));

            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant id is required", nameof(tenantId));

            if (managerId == Guid.Empty)
                throw new ArgumentException("Manager id is required", nameof(managerId));

            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date", nameof(endDate));
        }
    }
}
