using System.ComponentModel.DataAnnotations;

namespace APEX.Mobile.Models
{
    #region Ürün Modelleri

    public class LogoUrunModel
    {
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string? Barkod { get; set; }
        public string? Birim { get; set; }
        public decimal? SatisFiyati { get; set; }
        public decimal? AlisFiyati { get; set; }
        public decimal? KdvOrani { get; set; }
        public string? KategoriKodu { get; set; }
        public string? KategoriAdi { get; set; }
        public string? Aciklama { get; set; }
        public bool Aktif { get; set; } = true;
        public DateTime? OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public decimal? GuncelStok { get; set; }
        public decimal? MinimumStok { get; set; }
        public decimal? MaksimumStok { get; set; }
    }

    public class LogoUrunKategoriModel
    {
        public string KategoriKodu { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public string? UstKategoriKodu { get; set; }
        public string? Aciklama { get; set; }
        public bool Aktif { get; set; } = true;
    }

    #endregion

    #region Cari Hesap Modelleri

    public class LogoCariHesapModel
    {
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public string? VergiNo { get; set; }
        public string? VergiDairesi { get; set; }
        public string? Adres { get; set; }
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? PostaKodu { get; set; }
        public string? Telefon { get; set; }
        public string? Faks { get; set; }
        public string? Email { get; set; }
        public string? WebSitesi { get; set; }
        public CariTipiModel CariTipi { get; set; }
        public decimal? RiskLimiti { get; set; }
        public decimal? Bakiye { get; set; }
        public bool Aktif { get; set; } = true;
        public DateTime? OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
    }

    public enum CariTipiModel
    {
        Musteri = 1,
        Tedarikci = 2,
        MusteriTedarikci = 3,
        Personel = 4,
        Banka = 5,
        Kasa = 6,
        Diger = 7
    }

    #endregion

    #region Stok Modelleri

    public class LogoStokHareketModel
    {
        public int HareketId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public HareketTipiModel HareketTipi { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public string? BelgeNo { get; set; }
        public DateTime HareketTarihi { get; set; }
        public string? CariKodu { get; set; }
        public string? CariAdi { get; set; }
        public string? Aciklama { get; set; }
    }

    public enum HareketTipiModel
    {
        Giris = 1,
        Cikis = 2,
        Transfer = 3,
        Sayim = 4,
        Uretim = 5,
        Fire = 6
    }

    public class LogoDepoModel
    {
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public DepoTipiModel DepoTipi { get; set; }
        public string? Adres { get; set; }
        public string? Sorumlu { get; set; }
        public string? Telefon { get; set; }
        public bool Aktif { get; set; } = true;
    }

    public enum DepoTipiModel
    {
        Ana = 1,
        Satis = 2,
        Uretim = 3,
        Konsinyasyon = 4,
        Diger = 5
    }

    public class LogoStokRaporuModel
    {
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public decimal GuncelStok { get; set; }
        public decimal? MinimumStok { get; set; }
        public decimal? MaksimumStok { get; set; }
        public decimal? OrtalamaMaliyet { get; set; }
        public decimal? ToplamDeger { get; set; }
        public DateTime? SonHareketTarihi { get; set; }
    }

    #endregion

    #region Belge Modelleri

    public class LogoFaturaModel
    {
        public int FaturaId { get; set; }
        public string FaturaNo { get; set; } = string.Empty;
        public FaturaTipiModel FaturaTipi { get; set; }
        public DateTime FaturaTarihi { get; set; }
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public decimal AraToplam { get; set; }
        public decimal KdvToplami { get; set; }
        public decimal GenelToplam { get; set; }
        public BelgeDurumuModel Durum { get; set; }
        public string? Aciklama { get; set; }
        public List<LogoFaturaDetayModel> Detaylar { get; set; } = new();
    }

    public class LogoFaturaDetayModel
    {
        public int DetayId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal Tutar { get; set; }
        public decimal KdvTutari { get; set; }
        public decimal ToplamTutar { get; set; }
    }

    public enum FaturaTipiModel
    {
        Satis = 1,
        Alis = 2,
        Iade = 3,
        Proforma = 4
    }

    public class LogoSiparisModel
    {
        public int SiparisId { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public SiparisTipiModel SiparisTipi { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public string CariKodu { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public decimal AraToplam { get; set; }
        public decimal KdvToplami { get; set; }
        public decimal GenelToplam { get; set; }
        public SiparisDurumuModel Durum { get; set; }
        public string? Aciklama { get; set; }
        public List<LogoSiparisDetayModel> Detaylar { get; set; } = new();
    }

    public class LogoSiparisDetayModel
    {
        public int DetayId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public decimal TeslimEdilen { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal Tutar { get; set; }
        public decimal KdvTutari { get; set; }
        public decimal ToplamTutar { get; set; }
    }

    public enum SiparisTipiModel
    {
        Satis = 1,
        Alis = 2
    }

    public enum SiparisDurumuModel
    {
        Beklemede = 1,
        Onaylandi = 2,
        KismiTeslim = 3,
        Tamamlandi = 4,
        Iptal = 5
    }

    public enum BelgeDurumuModel
    {
        Taslak = 1,
        Onaylandi = 2,
        Gonderildi = 3,
        Iptal = 4
    }

    #endregion

    #region Rapor Modelleri

    public class LogoSatisRaporuModel
    {
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public string? CariKodu { get; set; }
        public string? CariAdi { get; set; }
        public decimal SatilanMiktar { get; set; }
        public decimal SatisTutari { get; set; }
        public decimal Kar { get; set; }
        public decimal KarMarji { get; set; }
        public DateTime Tarih { get; set; }
    }

    public class LogoFinansalRaporModel
    {
        public string Hesap { get; set; } = string.Empty;
        public string HesapAdi { get; set; } = string.Empty;
        public decimal Borc { get; set; }
        public decimal Alacak { get; set; }
        public decimal Bakiye { get; set; }
        public DateTime RaporTarihi { get; set; }
    }

    public class LogoAylikOzetModel
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public decimal ToplamSatis { get; set; }
        public decimal ToplamAlis { get; set; }
        public decimal ToplamKar { get; set; }
        public int SatisFaturaSayisi { get; set; }
        public int AlisFaturaSayisi { get; set; }
        public int YeniMusteriSayisi { get; set; }
    }

    #endregion

    #region Senkronizasyon Modelleri

    public class LogoSenkronizasyonLogModel
    {
        public int LogId { get; set; }
        public SenkronizasyonTipiModel Tip { get; set; }
        public DateTime BaslangicZamani { get; set; }
        public DateTime? BitisZamani { get; set; }
        public bool Basarili { get; set; }
        public string? HataMesaji { get; set; }
        public int? IslemSayisi { get; set; }
        public string? Detaylar { get; set; }
    }

    public enum SenkronizasyonTipiModel
    {
        TumVeriler = 1,
        Urunler = 2,
        CariHesaplar = 3,
        StokHareketleri = 4,
        Faturalar = 5,
        Siparisler = 6
    }

    #endregion

    #region UI Helper Modelleri

    public class LogoErpBaglantiModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        public string Sifre { get; set; } = string.Empty;

        public string? SirketKodu { get; set; }
        public bool BeniHatirla { get; set; }
    }

    public class LogoErpDashboardModel
    {
        public bool BaglantiDurumu { get; set; }
        public DateTime? SonSenkronizasyonTarihi { get; set; }
        public int ToplamUrunSayisi { get; set; }
        public int ToplamCariSayisi { get; set; }
        public int ToplamDepoSayisi { get; set; }
        public decimal? BugunkuSatisTutari { get; set; }
        public int BugunkuFaturaSayisi { get; set; }
        public List<LogoSenkronizasyonLogModel> SonLoglar { get; set; } = new();
    }

    public class LogoErpAramaModel
    {
        public string AramaMetni { get; set; } = string.Empty;
        public string AramaTipi { get; set; } = "Tumu"; // Tumu, Urun, Cari, Stok
        public int SayfaNo { get; set; } = 1;
        public int KayitSayisi { get; set; } = 20;
    }

    #endregion
}