using Microsoft.EntityFrameworkCore;
using APEX.Core.Entities;
using APEX.Data.Configurations;

namespace APEX.Data.Context
{
    public class ApexDbContext(DbContextOptions<ApexDbContext> options, string companyCode = "001") : DbContext(options)
    {
        private readonly string _companyCode = companyCode;

        // Multi-Tenant Tables
        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<TenantUser> TenantUsers { get; set; } = null!;

        // Logo ERP Integration Tables
        public DbSet<LogoErpConnection> LogoErpConnections { get; set; } = null!;
        
        // Logo ERP Mapped Tables (Gerçek Logo Tablo İsimleri)
        public DbSet<LogoUrun> LogoUrunler { get; set; } = null!;
        public DbSet<LogoCariHesap> LogoCariHesaplar { get; set; } = null!;
        public DbSet<LogoStokHareket> LogoStokHareketleri { get; set; } = null!;
        public DbSet<LogoDepo> LogoDepolar { get; set; } = null!;
        public DbSet<LogoFatura> LogoFaturalar { get; set; } = null!;
        public DbSet<LogoIrsaliye> LogoIrsaliyeler { get; set; } = null!;
        public DbSet<LogoSiparis> LogoSiparisler { get; set; } = null!;

        // APEX Specific Tables
        public DbSet<Sayim> Sayimlar { get; set; } = null!;
        public DbSet<SayimDetay> SayimDetaylari { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tenant Configuration
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.SubDomain).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.SubDomain).IsUnique();
                entity.Property(e => e.DatabaseName).HasMaxLength(100);
            });

            // TenantUser Configuration
            modelBuilder.Entity<TenantUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
                
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Users)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Logo ERP Connection Configuration
            modelBuilder.Entity<LogoErpConnection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConnectionName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ServerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(100);
            });

            // Logo ERP Tablo Konfigürasyonları (Gerçek Logo Tablo Yapısı)
            LogoErpTableConfiguration.ConfigureLogoTables(modelBuilder, _companyCode);

            // APEX Specific Tables
            modelBuilder.Entity<Sayim>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FisNo).HasMaxLength(50);
                entity.HasIndex(e => e.FisNo).IsUnique();
            });

            modelBuilder.Entity<SayimDetay>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MalzemeKodu).HasMaxLength(25);
                entity.Property(e => e.DepoKodu).HasMaxLength(25);
                
                entity.HasOne(e => e.Sayim)
                      .WithMany(s => s.Detaylar)
                      .HasForeignKey(e => e.SayimId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}