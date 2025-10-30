namespace APEX.Core.Entities
{
    public class SayimDetay
    {
        public int Id { get; set; }
        public int SayimId { get; set; }
        public int UrunId { get; set; }
        public string? UrunKodu { get; set; }
        public string? UrunAdi { get; set; }
        public string? Barkod { get; set; }
        public decimal MevcutStok { get; set; }
        public decimal SayilanMiktar { get; set; }
        public decimal Fark { get; set; }
        public string? Birim { get; set; }
        public DateTime SonGuncelleme { get; set; }
        
        // Logo ERP ile uyumlu alanlar
        public string MalzemeKodu { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
        
        // Navigation Property
        public Sayim Sayim { get; set; } = null!;
    }
}
