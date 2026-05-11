namespace ProjectManagementSystem.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid TaskItemId { get; private set; }

        public Guid AuthorId { get; private set; }

        public string Body { get; private set; }

        public Comment(Guid taskItemId, Guid authorId, string body)
        {
            if (taskItemId == Guid.Empty)
                throw new ArgumentException("Task id is required", nameof(taskItemId));

            if (authorId == Guid.Empty)
                throw new ArgumentException("Author id is required", nameof(authorId));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Comment body is required", nameof(body));

            TaskItemId = taskItemId;
            AuthorId = authorId;
            Body = body.Trim();
        }
    }
}
