namespace ProjectManagementSystem.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Guid TenantId { get; private set; }
    }
}
