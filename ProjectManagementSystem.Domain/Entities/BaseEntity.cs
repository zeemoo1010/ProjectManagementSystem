namespace ProjectManagementSystem.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected init; } = Guid.NewGuid();

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public Guid? ModifiedBy { get; private set; }

        public bool IsDeleted { get; private set; }

        public void RecordCreatedBy(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required", nameof(userId));

            CreatedBy = userId;
        }

        public void RecordModifiedBy(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required", nameof(userId));

            ModifiedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete(Guid userId)
        {
            IsDeleted = true;
            RecordModifiedBy(userId);
        }
    }
}
