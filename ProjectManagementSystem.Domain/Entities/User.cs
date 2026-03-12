namespace ProjectManagementSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        public Guid TenantId { get; private set; }
    }
}
