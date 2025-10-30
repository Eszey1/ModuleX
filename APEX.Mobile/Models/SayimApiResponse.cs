namespace APEX.Mobile.Models
{
    public class SayimApiResponse
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string? KullaniciId { get; set; }
        public string? FisNo { get; set; }
        public string? Durum { get; set; }
        public List<SayimDetayApiResponse>? Detaylar { get; set; }
        public List<SayimDetayApiResponse>? Urunler { get; set; }
    }

    public class SayimDetayApiResponse
    {
        public int Id { get; set; }
        public int SayimId { get; set; }
        public int UrunId { get; set; }
        public string UrunKodu { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public string Barkod { get; set; } = string.Empty;
        public decimal MevcutStok { get; set; }
        public decimal SayilanMiktar { get; set; }
        public decimal Fark { get; set; }
        public string Birim { get; set; } = string.Empty;
        public DateTime SonGuncelleme { get; set; }
    }
}
