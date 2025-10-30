namespace APEX.Core.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public TenantStatus Status { get; set; } = TenantStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Basic;
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public bool IsTrialActive { get; set; }
        public int MaxUsers { get; set; } = 5;
        public int MaxLogoConnections { get; set; } = 1;
        public string DatabaseName { get; set; } = string.Empty;
        public string? CustomDomain { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
        
        // Navigation Properties
        public List<TenantUser> Users { get; set; } = new();
        public List<LogoErpConnection> LogoConnections { get; set; } = new();
    }

    public enum TenantStatus
    {
        Active = 1,
        Suspended = 2,
        Inactive = 3,
        Trial = 4,
        Expired = 5
    }

    public enum SubscriptionPlan
    {
        Trial = 0,
        Basic = 1,
        Professional = 2,
        Enterprise = 3,
        Custom = 4
    }
}