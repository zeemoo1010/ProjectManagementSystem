using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public Guid TenantId { get; private set; }

        public UserRole Role { get; private set; }

        public bool IsActive { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        public User(string firstName, string lastName, string email, string passwordHash, Guid tenantId, UserRole role)
        {
            Validate(firstName, lastName, email, passwordHash, tenantId);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.ToLower().Trim();
            PasswordHash = passwordHash;
            TenantId = tenantId;
            Role = role;
            IsActive = true;
        }

        public void Update(string firstName, string lastName, string email, UserRole role)
        {
            Validate(firstName, lastName, email, PasswordHash, TenantId);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.ToLower().Trim();
            Role = role;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password cannot be empty", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        private static void Validate(string firstName, string lastName, string email, string passwordHash, Guid tenantId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required", nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required", nameof(passwordHash));

            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant id is required", nameof(tenantId));
        }
    }
}
