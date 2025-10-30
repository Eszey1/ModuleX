namespace APEX.Core.Entities
{
    public class LogoCariHesap
    {
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public string VergiNo { get; set; } = string.Empty;
        public string VergiDairesi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string PostaKodu { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Faks { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WebSitesi { get; set; } = string.Empty;
        public CariTipi CariTipi { get; set; }
        public decimal RiskLimiti { get; set; }
        public decimal VadeGunSayisi { get; set; }
        public decimal IskontoOrani { get; set; }
        public bool Aktif { get; set; } = true;
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string YetkiliKisi { get; set; } = string.Empty;
        public string YetkiliTelefon { get; set; } = string.Empty;
        public string Notlar { get; set; } = string.Empty;
        public decimal GuncelBakiye { get; set; }
        public string ParaBirimi { get; set; } = "TRY";
    }

    public enum CariTipi
    {
        Musteri = 1,
        Tedarikci = 2,
        MusteriTedarikci = 3,
        Personel = 4,
        Banka = 5,
        Diger = 6
    }

    public class LogoCariRaporu
    {
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public decimal ToplamBorc { get; set; }
        public decimal ToplamAlacak { get; set; }
        public decimal NetBakiye { get; set; }
        public DateTime SonHareketTarihi { get; set; }
        public int HareketSayisi { get; set; }
    }
}