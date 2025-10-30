using System.ComponentModel.DataAnnotations;

namespace APEX.Core.Entities
{
    public class LogoErpConnection
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int TenantId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ConnectionName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ServerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string DatabaseName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string? CompanyCode { get; set; }
        
        [StringLength(20)]
        public string? LogoVersion { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsDefault { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? LastConnectionTest { get; set; }
        
        public bool? LastConnectionResult { get; set; }
        
        [StringLength(500)]
        public string? ConnectionString { get; set; }
        
        // Navigation Properties
        public Tenant Tenant { get; set; } = null!;
    }
}