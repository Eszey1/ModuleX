using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APEX.Core.Entities;

namespace APEX.Data.Configurations
{
    /// <summary>
    /// Logo ERP Tablo Yapısı Konfigürasyonu
    /// Bu sınıf Logo ERP'nin gerçek tablo yapısını APEX entity'leriyle eşleştirir
    /// </summary>
    public static class LogoErpTableConfiguration
    {
        /// <summary>
        /// Logo ERP Firma Kodu (XXX) - Her firma için farklı
        /// Örnek: LG_001_ITEMS, LG_002_ITEMS
        /// </summary>
        public static string GetLogoTableName(string baseTableName, string companyCode = "001")
        {
            return $"LG_{companyCode}_{baseTableName}";
        }

        public static void ConfigureLogoTables(ModelBuilder modelBuilder, string companyCode = "001")
        {
            // LG_XXX_ITEMS - Malzeme Kartları
            modelBuilder.Entity<LogoUrun>(entity =>
            {
                entity.ToTable(GetLogoTableName("ITEMS", companyCode));
                entity.HasKey(e => e.MalzemeKodu);
                entity.Property(e => e.MalzemeKodu).HasColumnName("CODE").HasMaxLength(25);
                entity.Property(e => e.MalzemeAdi).HasColumnName("NAME").HasMaxLength(200);
                entity.Property(e => e.Barkod).HasColumnName("BARCODE").HasMaxLength(50);
                entity.Property(e => e.Birim).HasColumnName("UNIT1").HasMaxLength(10);
                entity.Property(e => e.BirimFiyat).HasColumnName("PRICE1").HasColumnType("decimal(18,4)");
                entity.Property(e => e.KdvOrani).HasColumnName("VATRATE").HasColumnType("decimal(5,2)");
                entity.Property(e => e.Aktif).HasColumnName("ACTIVE").HasColumnType("bit");
            });

            // LG_XXX_CLCARD - Cari Hesap Kartları
            modelBuilder.Entity<LogoCariHesap>(entity =>
            {
                entity.ToTable(GetLogoTableName("CLCARD", companyCode));
                entity.HasKey(e => e.CariKodu);
                entity.Property(e => e.CariKodu).HasColumnName("CODE").HasMaxLength(25);
                entity.Property(e => e.CariAdi).HasColumnName("DEFINITION_").HasMaxLength(200);
                entity.Property(e => e.VergiNo).HasColumnName("TAXNR").HasMaxLength(20);
                entity.Property(e => e.VergiDairesi).HasColumnName("TAXOFFICE").HasMaxLength(50);
                entity.Property(e => e.Adres).HasColumnName("ADDR1").HasMaxLength(200);
                entity.Property(e => e.Il).HasColumnName("CITY").HasMaxLength(50);
                entity.Property(e => e.Telefon).HasColumnName("TELNRS1").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("EMAILADDR").HasMaxLength(100);
                entity.Property(e => e.Aktif).HasColumnName("ACTIVE").HasColumnType("bit");
            });

            // LG_XXX_STLINE - Stok Hareketleri
            modelBuilder.Entity<LogoStokHareket>(entity =>
            {
                entity.ToTable(GetLogoTableName("STLINE", companyCode));
                entity.HasKey(e => e.HareketId);
                entity.Property(e => e.HareketId).HasColumnName("LOGICALREF").ValueGeneratedOnAdd();
                entity.Property(e => e.MalzemeKodu).HasColumnName("STOCKREF").HasMaxLength(25);
                entity.Property(e => e.DepoKodu).HasColumnName("SOURCEINDEX").HasMaxLength(25);
                entity.Property(e => e.Miktar).HasColumnName("AMOUNT").HasColumnType("decimal(18,4)");
                entity.Property(e => e.BirimFiyat).HasColumnName("PRICE").HasColumnType("decimal(18,4)");
                entity.Property(e => e.HareketTarihi).HasColumnName("DATE_").HasColumnType("datetime");
                entity.Property(e => e.BelgeNo).HasColumnName("FICHENO").HasMaxLength(50);
                entity.Property(e => e.CariKodu).HasColumnName("CLIENTREF").HasMaxLength(25);
            });

            // LG_XXX_WAREHOUSE - Depo Kartları
            modelBuilder.Entity<LogoDepo>(entity =>
            {
                entity.ToTable(GetLogoTableName("WAREHOUSE", companyCode));
                entity.HasKey(e => e.DepoKodu);
                entity.Property(e => e.DepoKodu).HasColumnName("NR").HasMaxLength(25);
                entity.Property(e => e.DepoAdi).HasColumnName("NAME").HasMaxLength(200);
                entity.Property(e => e.Adres).HasColumnName("ADDR1").HasMaxLength(200);
                entity.Property(e => e.Aktif).HasColumnName("ACTIVE").HasColumnType("bit");
            });

            // LG_XXX_INVOICE - Fatura Başlıkları
            modelBuilder.Entity<LogoFatura>(entity =>
            {
                entity.ToTable(GetLogoTableName("INVOICE", companyCode));
                entity.HasKey(e => e.FaturaNo);
                entity.Property(e => e.FaturaNo).HasColumnName("FICHENO").HasMaxLength(50);
                entity.Property(e => e.FaturaTarihi).HasColumnName("DATE_").HasColumnType("datetime");
                entity.Property(e => e.CariKodu).HasColumnName("CLIENTREF").HasMaxLength(25);
                entity.Property(e => e.AraToplam).HasColumnName("NETTOTAL").HasColumnType("decimal(18,4)");
                entity.Property(e => e.KdvTutari).HasColumnName("TOTALTAX").HasColumnType("decimal(18,4)");
                entity.Property(e => e.GenelToplam).HasColumnName("GROSSTOTAL").HasColumnType("decimal(18,4)");
            });

            // LG_XXX_STFICHE - Stok Fişi Başlıkları (İrsaliye)
            modelBuilder.Entity<LogoIrsaliye>(entity =>
            {
                entity.ToTable(GetLogoTableName("STFICHE", companyCode));
                entity.HasKey(e => e.IrsaliyeNo);
                entity.Property(e => e.IrsaliyeNo).HasColumnName("FICHENO").HasMaxLength(50);
                entity.Property(e => e.IrsaliyeTarihi).HasColumnName("DATE_").HasColumnType("datetime");
                entity.Property(e => e.CariKodu).HasColumnName("CLIENTREF").HasMaxLength(25);
                entity.Property(e => e.CikisDepoKodu).HasColumnName("SOURCEINDEX").HasMaxLength(25);
                entity.Property(e => e.GirisDepoKodu).HasColumnName("DESTINDEX").HasMaxLength(25);
            });

            // LG_XXX_ORFICHE - Sipariş Fişi Başlıkları
            modelBuilder.Entity<LogoSiparis>(entity =>
            {
                entity.ToTable(GetLogoTableName("ORFICHE", companyCode));
                entity.HasKey(e => e.SiparisNo);
                entity.Property(e => e.SiparisNo).HasColumnName("FICHENO").HasMaxLength(50);
                entity.Property(e => e.SiparisTarihi).HasColumnName("DATE_").HasColumnType("datetime");
                entity.Property(e => e.TeslimTarihi).HasColumnName("DELIVDATE").HasColumnType("datetime");
                entity.Property(e => e.CariKodu).HasColumnName("CLIENTREF").HasMaxLength(25);
                entity.Property(e => e.AraToplam).HasColumnName("NETTOTAL").HasColumnType("decimal(18,4)");
                entity.Property(e => e.GenelToplam).HasColumnName("GROSSTOTAL").HasColumnType("decimal(18,4)");
            });
        }
    }

    /// <summary>
    /// Logo ERP Tablo İsimleri Referansı
    /// </summary>
    public static class LogoTableNames
    {
        public const string ITEMS = "ITEMS";           // Malzeme Kartları
        public const string CLCARD = "CLCARD";         // Cari Hesap Kartları
        public const string STLINE = "STLINE";         // Stok Hareketleri
        public const string WAREHOUSE = "WAREHOUSE";   // Depo Kartları
        public const string INVOICE = "INVOICE";       // Fatura Başlıkları
        public const string STFICHE = "STFICHE";       // Stok Fişi Başlıkları
        public const string ORFICHE = "ORFICHE";       // Sipariş Fişi Başlıkları
        public const string STLINE_TRANS = "STLINE";   // Stok Hareket Detayları
        public const string INVOICE_TRANS = "STLINE";  // Fatura Detayları
        public const string ORFLINE = "ORFLINE";       // Sipariş Detayları
    }
}