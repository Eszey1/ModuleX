namespace APEX.Core.Entities
{
    public class Urun
    {
        public int Id { get; set; }
        public string? Kod { get; set; }
        public string? Adi { get; set; }
        public string? Barkod { get; set; }
        public string? Barkod2 { get; set; }
        public string? Barkod3 { get; set; }
        public decimal MevcutStok { get; set; }
        public string? Birim { get; set; }
        public decimal Fiyat { get; set; }
    }
}
