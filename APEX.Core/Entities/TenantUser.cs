namespace APEX.Core.Entities
{
    public class TenantUser
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public List<string> Permissions { get; set; } = new();
        
        // Navigation Properties
        public Tenant Tenant { get; set; } = null!;
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public enum UserRole
    {
        SuperAdmin = 0,  // Platform yöneticisi
        TenantAdmin = 1, // Tenant yöneticisi
        Manager = 2,     // Yönetici
        User = 3,        // Kullanıcı
        ReadOnly = 4     // Sadece okuma yetkisi
    }
}