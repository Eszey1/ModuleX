using APEX.Core.Entities;

namespace APEX.Core.Interfaces
{
    public interface ILogoRepository
    {
        // Mevcut Depo Sayım İşlemleri
        Task<Urun?> UrunAraAsync(string barkod);
        Task<List<Urun>> TumUrunleriGetirAsync(int limit = 1000);
        Task<bool> SayimFisiOlusturAsync(Sayim sayim);
        Task<List<Sayim>> SayimListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);

        // Logo ERP Bağlantı ve Kimlik Doğrulama
        Task<bool> LogoBaglantiTestiAsync();
        Task<string> LogoTokenAlAsync(string kullaniciAdi, string sifre, string sirketKodu);
        Task<bool> LogoBaglantiKurAsync(string connectionString, string kullaniciAdi, string sifre);

        // Malzeme/Ürün Yönetimi
        Task<List<LogoUrun>> LogoUrunListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100);
        Task<LogoUrun?> LogoUrunDetayGetirAsync(string malzemeKodu);
        Task<bool> LogoUrunEkleAsync(LogoUrun urun);
        Task<bool> LogoUrunGuncelleAsync(LogoUrun urun);
        Task<bool> LogoUrunSilAsync(string malzemeKodu);
        Task<List<LogoUrunKategori>> LogoUrunKategorileriGetirAsync();

        // Cari Hesap Yönetimi
        Task<List<LogoCariHesap>> LogoCariHesapListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100);
        Task<LogoCariHesap?> LogoCariHesapDetayGetirAsync(string cariKodu);
        Task<bool> LogoCariHesapEkleAsync(LogoCariHesap cariHesap);
        Task<bool> LogoCariHesapGuncelleAsync(LogoCariHesap cariHesap);
        Task<decimal> LogoCariHesapBakiyeGetirAsync(string cariKodu);

        // Stok Yönetimi
        Task<List<LogoStokHareket>> LogoStokHareketleriGetirAsync(string malzemeKodu, DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<decimal> LogoGuncelStokGetirAsync(string malzemeKodu, string depoKodu = "");
        Task<bool> LogoStokHareketEkleAsync(LogoStokHareket stokHareket);
        Task<List<LogoDepo>> LogoDepoListesiGetirAsync();

        // Fatura ve Belge İşlemleri
        Task<List<LogoFatura>> LogoFaturaListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<LogoFatura?> LogoFaturaDetayGetirAsync(string faturaNo);
        Task<bool> LogoFaturaOlusturAsync(LogoFatura fatura);
        Task<bool> LogoIrsaliyeOlusturAsync(LogoIrsaliye irsaliye);

        // Sipariş Yönetimi
        Task<List<LogoSiparis>> LogoSiparisListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null);
        Task<LogoSiparis?> LogoSiparisDetayGetirAsync(string siparisNo);
        Task<bool> LogoSiparisOlusturAsync(LogoSiparis siparis);
        Task<bool> LogoSiparisGuncelleAsync(LogoSiparis siparis);

        // Raporlama ve Analiz
        Task<List<LogoSatisRaporu>> LogoSatisRaporuGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi);
        Task<List<LogoStokRaporu>> LogoStokRaporuGetirAsync(string depoKodu = "");
        Task<List<LogoCariRaporu>> LogoCariRaporuGetirAsync();
        Task<LogoFinansalRapor> LogoFinansalRaporGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi);

        // Senkronizasyon İşlemleri
        Task<bool> LogoVerileriSenkronizeEtAsync();
        Task<DateTime?> LogoSonSenkronizasyonTarihiGetirAsync();
        Task<List<LogoSenkronizasyonLog>> LogoSenkronizasyonLoglariniGetirAsync(int gunSayisi = 7);
    }
}
