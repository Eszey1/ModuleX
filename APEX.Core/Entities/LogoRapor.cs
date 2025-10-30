namespace APEX.Core.Entities
{
    public class LogoSatisRaporu
    {
        public DateTime Tarih { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public decimal SatisMiktari { get; set; }
        public decimal SatisTutari { get; set; }
        public decimal MaliyetTutari { get; set; }
        public decimal KarTutari { get; set; }
        public decimal KarOrani { get; set; }
        public string FaturaNo { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
    }

    public class LogoFinansalRapor
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ToplamSatis { get; set; }
        public decimal ToplamAlis { get; set; }
        public decimal ToplamKar { get; set; }
        public decimal ToplamGider { get; set; }
        public decimal NetKar { get; set; }
        public decimal ToplamAlacak { get; set; }
        public decimal ToplamBorc { get; set; }
        public decimal NetBakiye { get; set; }
        public int ToplamFaturaSayisi { get; set; }
        public int ToplamMusteriSayisi { get; set; }
        public List<LogoAylikOzet> AylikOzetler { get; set; } = new List<LogoAylikOzet>();
    }

    public class LogoAylikOzet
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public decimal AylikSatis { get; set; }
        public decimal AylikAlis { get; set; }
        public decimal AylikKar { get; set; }
        public int FaturaSayisi { get; set; }
    }

    public class LogoSenkronizasyonLog
    {
        public int LogId { get; set; }
        public DateTime SenkronizasyonTarihi { get; set; }
        public SenkronizasyonTipi Tipi { get; set; }
        public string TabloAdi { get; set; } = string.Empty;
        public int EtkilenenKayitSayisi { get; set; }
        public bool Basarili { get; set; }
        public string HataMesaji { get; set; } = string.Empty;
        public TimeSpan Sure { get; set; }
        public string KullaniciKodu { get; set; } = string.Empty;
    }

    public enum SenkronizasyonTipi
    {
        Manuel = 1,
        Otomatik = 2,
        Zamanli = 3,
        Acil = 4
    }

    public class LogoBaglantiAyarlari
    {
        public string ServerAdresi { get; set; } = string.Empty;
        public string VeritabaniAdi { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public int BaglantiZamanasimiSaniye { get; set; } = 30;
        public bool SifreliMi { get; set; } = true;
        public string SirketKodu { get; set; } = string.Empty;
        public string DonemKodu { get; set; } = string.Empty;
        public bool AktifMi { get; set; } = true;
        public DateTime SonBaglantiTarihi { get; set; }
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public DateTime TokenGecerlilikTarihi { get; set; }
    }

    public class LogoApiYanit<T>
    {
        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public T? Veri { get; set; }
        public int ToplamKayit { get; set; }
        public int SayfaNo { get; set; }
        public int SayfaBoyutu { get; set; }
        public string HataKodu { get; set; } = string.Empty;
        public DateTime IslemTarihi { get; set; }
    }
}