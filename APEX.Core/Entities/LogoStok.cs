namespace APEX.Core.Entities
{
    public class LogoStokHareket
    {
        public int HareketId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
        public HareketTipi HareketTipi { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public DateTime HareketTarihi { get; set; }
        public string BelgeNo { get; set; } = string.Empty;
        public string BelgeTipi { get; set; } = string.Empty;
        public string CariKodu { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string KullaniciKodu { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; }
        public string SeriNo { get; set; } = string.Empty;
        public string LotNo { get; set; } = string.Empty;
    }

    public enum HareketTipi
    {
        Giris = 1,
        Cikis = 2,
        Transfer = 3,
        Sayim = 4,
        Uretim = 5,
        Fire = 6,
        Iade = 7
    }

    public class LogoDepo
    {
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string YetkiliKisi { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public bool Aktif { get; set; } = true;
        public DepoTipi DepoTipi { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }

    public enum DepoTipi
    {
        Ana = 1,
        Sube = 2,
        Konsinyasyon = 3,
        Sanal = 4,
        Uretim = 5,
        Kalite = 6
    }

    public class LogoStokRaporu
    {
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public decimal MevcutStok { get; set; }
        public decimal RezerveStok { get; set; }
        public decimal KullanilabilirStok { get; set; }
        public decimal OrtalamaMaliyet { get; set; }
        public decimal ToplamDeger { get; set; }
        public DateTime SonHareketTarihi { get; set; }
        public decimal MinimumStok { get; set; }
        public decimal MaksimumStok { get; set; }
        public bool StokUyarisi { get; set; }
    }
}