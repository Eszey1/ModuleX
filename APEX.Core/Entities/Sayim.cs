namespace APEX.Core.Entities
{
    public class Sayim
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string? KullaniciId { get; set; }
        public string? FisNo { get; set; }
        public string? Durum { get; set; } // "Aktif", "Tamamlandi"
        public List<SayimDetay>? Detaylar { get; set; }
        public List<SayimDetay>? Urunler { get; set; } // API uyumluluğu için
    }
}
