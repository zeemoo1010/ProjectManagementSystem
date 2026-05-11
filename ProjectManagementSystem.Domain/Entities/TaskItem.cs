using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; private set; }

        public string? Description { get; private set; }

        public Guid ProjectId { get; private set; }

        public Guid AssignedToUserId { get; private set; }

        public DateOnly DueDate { get; private set; }

        public TaskPriority Priority { get; private set; }

        public Enums.TaskStatus Status { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public TaskItem(string title, string? description, Guid projectId, Guid assignedToUserId, DateOnly dueDate, TaskPriority priority)
        {
            Validate(title, projectId, assignedToUserId);

            Title = title.Trim();
            Description = description?.Trim();
            ProjectId = projectId;
            AssignedToUserId = assignedToUserId;
            DueDate = dueDate;
            Priority = priority;
            Status = Enums.TaskStatus.Todo;
        }

        public bool IsOverdue(DateOnly today) => Status != Enums.TaskStatus.Completed && DueDate < today;

        public void Update(string title, string? description, Guid assignedToUserId, DateOnly dueDate, TaskPriority priority)
        {
            Validate(title, ProjectId, assignedToUserId);

            Title = title.Trim();
            Description = description?.Trim();
            AssignedToUserId = assignedToUserId;
            DueDate = dueDate;
            Priority = priority;
        }

        public void ChangeStatus(Enums.TaskStatus status)
        {
            Status = status;
            CompletedAt = status == Enums.TaskStatus.Completed ? DateTime.UtcNow : null;
        }

        public void AssignTo(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required", nameof(userId));

            AssignedToUserId = userId;
        }

        private static void Validate(string title, Guid projectId, Guid assignedToUserId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title is required", nameof(title));

            if (projectId == Guid.Empty)
                throw new ArgumentException("Project id is required", nameof(projectId));

            if (assignedToUserId == Guid.Empty)
                throw new ArgumentException("Assigned user id is required", nameof(assignedToUserId));
        }
    }
}
