using APEX.Core.Entities;

namespace APEX.Core.Interfaces
{
    public interface ILogoErpService
    {
        #region Bağlantı ve Kimlik Doğrulama
        Task<bool> BaglantiTestiYapAsync();
        Task<string> GirisYapAsync(string kullaniciAdi, string sifre, string sirketKodu);
        #endregion

        #region Ürün Yönetimi
        Task<List<LogoUrun>> UrunListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100);
        Task<LogoUrun?> UrunDetayGetirAsync(string malzemeKodu);
        Task<bool> UrunEkleAsync(LogoUrun urun);
        Task<bool> UrunGuncelleAsync(LogoUrun urun);
        Task<bool> UrunSilAsync(string malzemeKodu);
        Task<List<LogoUrunKategori>> UrunKategorileriGetirAsync();
        #endregion

        #region Cari Hesap Yönetimi
        Task<List<LogoCariHesap>> CariHesapListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100);
        Task<LogoCariHesap?> CariHesapDetayGetirAsync(string cariKodu);
        Task<bool> CariHesapEkleAsync(LogoCariHesap cariHesap);
        Task<bool> CariHesapGuncelleAsync(LogoCariHesap cariHesap);
        Task<decimal> CariHesapBakiyeGetirAsync(string cariKodu);
        #endregion

        #region Stok Yönetimi
        Task<List<LogoStokHareket>> StokHareketleriGetirAsync(string malzemeKodu, DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<decimal> GuncelStokGetirAsync(string malzemeKodu, string depoKodu = "");
        Task<bool> StokHareketEkleAsync(LogoStokHareket stokHareket);
        Task<List<LogoDepo>> DepoListesiGetirAsync();
        #endregion

        #region Fatura ve Belge İşlemleri
        Task<List<LogoFatura>> FaturaListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<LogoFatura?> FaturaDetayGetirAsync(string faturaNo);
        Task<bool> FaturaOlusturAsync(LogoFatura fatura);
        Task<bool> IrsaliyeOlusturAsync(LogoIrsaliye irsaliye);
        #endregion

        #region Sipariş Yönetimi
        Task<List<LogoSiparis>> SiparisListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<LogoSiparis?> SiparisDetayGetirAsync(string siparisNo);
        Task<bool> SiparisOlusturAsync(LogoSiparis siparis);
        Task<bool> SiparisGuncelleAsync(LogoSiparis siparis);
        #endregion

        #region Raporlama ve Analiz
        Task<List<LogoSatisRaporu>> SatisRaporuGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi);
        Task<List<LogoStokRaporu>> StokRaporuGetirAsync(string depoKodu = "");
        Task<List<LogoCariRaporu>> CariRaporuGetirAsync();
        Task<LogoFinansalRapor> FinansalRaporGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi);
        #endregion

        #region Senkronizasyon İşlemleri
        Task<bool> VerileriSenkronizeEtAsync();
        Task<DateTime?> SonSenkronizasyonTarihiGetirAsync();
        Task<List<LogoSenkronizasyonLog>> SenkronizasyonLoglariniGetirAsync(int gunSayisi = 7);
        #endregion
    }
}