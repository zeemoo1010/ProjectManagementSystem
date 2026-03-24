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

        public User(string firstName, string lastName, string email, string passwordHash, Guid tenantId, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required");

            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLower().Trim();
            PasswordHash = passwordHash;
            TenantId = tenantId;
            Role = role;
            IsActive = true;
        }
        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password cannot be empty");

            PasswordHash = newPasswordHash;
        }
    }
}