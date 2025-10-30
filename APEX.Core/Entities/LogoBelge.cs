namespace APEX.Core.Entities
{
    public class LogoFatura
    {
        public string FaturaNo { get; set; } = string.Empty;
        public DateTime FaturaTarihi { get; set; }
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public FaturaTipi FaturaTipi { get; set; }
        public decimal AraToplam { get; set; }
        public decimal KdvTutari { get; set; }
        public decimal GenelToplam { get; set; }
        public decimal IskontoTutari { get; set; }
        public string ParaBirimi { get; set; } = "TRY";
        public decimal DovizKuru { get; set; } = 1;
        public string Aciklama { get; set; } = string.Empty;
        public BelgeDurumu Durumu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string OlusturanKullanici { get; set; } = string.Empty;
        public List<LogoFaturaDetay> FaturaDetaylari { get; set; } = new List<LogoFaturaDetay>();
    }

    public class LogoFaturaDetay
    {
        public int SiraNo { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal IskontoOrani { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal Tutar { get; set; }
        public string DepoKodu { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
    }

    public enum FaturaTipi
    {
        Satis = 1,
        Alis = 2,
        Iade = 3,
        Konsinye = 4
    }

    public class LogoIrsaliye
    {
        public string IrsaliyeNo { get; set; } = string.Empty;
        public DateTime IrsaliyeTarihi { get; set; }
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public IrsaliyeTipi IrsaliyeTipi { get; set; }
        public string CikisDepoKodu { get; set; } = string.Empty;
        public string GirisDepoKodu { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public BelgeDurumu Durumu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string OlusturanKullanici { get; set; } = string.Empty;
        public List<LogoIrsaliyeDetay> IrsaliyeDetaylari { get; set; } = new List<LogoIrsaliyeDetay>();
    }

    public class LogoIrsaliyeDetay
    {
        public int SiraNo { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public string SeriNo { get; set; } = string.Empty;
        public string LotNo { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
    }

    public enum IrsaliyeTipi
    {
        Sevkiyat = 1,
        Teslim = 2,
        Transfer = 3,
        Iade = 4
    }

    public class LogoSiparis
    {
        public string SiparisNo { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public DateTime TeslimTarihi { get; set; }
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public SiparisTipi SiparisTipi { get; set; }
        public decimal AraToplam { get; set; }
        public decimal KdvTutari { get; set; }
        public decimal GenelToplam { get; set; }
        public string ParaBirimi { get; set; } = "TRY";
        public decimal DovizKuru { get; set; } = 1;
        public string Aciklama { get; set; } = string.Empty;
        public SiparisDurumu Durumu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string OlusturanKullanici { get; set; } = string.Empty;
        public List<LogoSiparisDetay> SiparisDetaylari { get; set; } = new List<LogoSiparisDetay>();
    }

    public class LogoSiparisDetay
    {
        public int SiraNo { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal SiparisMiktari { get; set; }
        public decimal TeslimMiktari { get; set; }
        public decimal KalanMiktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal Tutar { get; set; }
        public string DepoKodu { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
    }

    public enum SiparisTipi
    {
        Satis = 1,
        Alis = 2
    }

    public enum SiparisDurumu
    {
        Beklemede = 1,
        Onaylandi = 2,
        KismiTeslim = 3,
        TamTeslim = 4,
        Iptal = 5
    }

    public enum BelgeDurumu
    {
        Taslak = 1,
        Onaylandi = 2,
        Gonderildi = 3,
        Tamamlandi = 4,
        Iptal = 5
    }
}