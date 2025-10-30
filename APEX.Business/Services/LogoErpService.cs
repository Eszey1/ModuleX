using APEX.Core.Entities;
using APEX.Core.Interfaces;

namespace APEX.Business.Services
{
    public class LogoErpService : ILogoErpService
    {
        private readonly ILogoRepository _logoRepository;

        public LogoErpService(ILogoRepository logoRepository)
        {
            _logoRepository = logoRepository ?? throw new ArgumentNullException(nameof(logoRepository));
        }

        #region Bağlantı ve Kimlik Doğrulama

        public async Task<bool> BaglantiTestiYapAsync()
        {
            return await _logoRepository.LogoBaglantiTestiAsync();
        }

        public async Task<string> GirisYapAsync(string kullaniciAdi, string sifre, string sirketKodu)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi))
                throw new ArgumentException("Kullanıcı adı boş olamaz", nameof(kullaniciAdi));
            
            if (string.IsNullOrWhiteSpace(sifre))
                throw new ArgumentException("Şifre boş olamaz", nameof(sifre));

            return await _logoRepository.LogoTokenAlAsync(kullaniciAdi, sifre, sirketKodu);
        }

        #endregion

        #region Ürün Yönetimi

        public async Task<List<LogoUrun>> UrunListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100)
        {
            if (sayfa < 1)
                throw new ArgumentException("Sayfa numarası 1'den küçük olamaz", nameof(sayfa));
            
            if (kayitSayisi < 1 || kayitSayisi > 1000)
                throw new ArgumentException("Kayıt sayısı 1-1000 arasında olmalıdır", nameof(kayitSayisi));

            return await _logoRepository.LogoUrunListesiGetirAsync(sayfa, kayitSayisi);
        }

        public async Task<LogoUrun?> UrunDetayGetirAsync(string malzemeKodu)
        {
            if (string.IsNullOrWhiteSpace(malzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz", nameof(malzemeKodu));

            return await _logoRepository.LogoUrunDetayGetirAsync(malzemeKodu);
        }

        public async Task<bool> UrunEkleAsync(LogoUrun urun)
        {
            ValidateUrun(urun);
            return await _logoRepository.LogoUrunEkleAsync(urun);
        }

        public async Task<bool> UrunGuncelleAsync(LogoUrun urun)
        {
            ValidateUrun(urun);
            return await _logoRepository.LogoUrunGuncelleAsync(urun);
        }

        public async Task<bool> UrunSilAsync(string malzemeKodu)
        {
            if (string.IsNullOrWhiteSpace(malzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz", nameof(malzemeKodu));

            return await _logoRepository.LogoUrunSilAsync(malzemeKodu);
        }

        public async Task<List<LogoUrunKategori>> UrunKategorileriGetirAsync()
        {
            return await _logoRepository.LogoUrunKategorileriGetirAsync();
        }

        #endregion

        #region Cari Hesap Yönetimi

        public async Task<List<LogoCariHesap>> CariHesapListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100)
        {
            if (sayfa < 1)
                throw new ArgumentException("Sayfa numarası 1'den küçük olamaz", nameof(sayfa));
            
            if (kayitSayisi < 1 || kayitSayisi > 1000)
                throw new ArgumentException("Kayıt sayısı 1-1000 arasında olmalıdır", nameof(kayitSayisi));

            return await _logoRepository.LogoCariHesapListesiGetirAsync(sayfa, kayitSayisi);
        }

        public async Task<LogoCariHesap?> CariHesapDetayGetirAsync(string cariKodu)
        {
            if (string.IsNullOrWhiteSpace(cariKodu))
                throw new ArgumentException("Cari kodu boş olamaz", nameof(cariKodu));

            return await _logoRepository.LogoCariHesapDetayGetirAsync(cariKodu);
        }

        public async Task<bool> CariHesapEkleAsync(LogoCariHesap cariHesap)
        {
            ValidateCariHesap(cariHesap);
            return await _logoRepository.LogoCariHesapEkleAsync(cariHesap);
        }

        public async Task<bool> CariHesapGuncelleAsync(LogoCariHesap cariHesap)
        {
            ValidateCariHesap(cariHesap);
            return await _logoRepository.LogoCariHesapGuncelleAsync(cariHesap);
        }

        public async Task<decimal> CariHesapBakiyeGetirAsync(string cariKodu)
        {
            if (string.IsNullOrWhiteSpace(cariKodu))
                throw new ArgumentException("Cari kodu boş olamaz", nameof(cariKodu));

            return await _logoRepository.LogoCariHesapBakiyeGetirAsync(cariKodu);
        }

        #endregion

        #region Stok Yönetimi

        public async Task<List<LogoStokHareket>> StokHareketleriGetirAsync(string malzemeKodu, DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null)
        {
            if (string.IsNullOrWhiteSpace(malzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz", nameof(malzemeKodu));

            if (baslangicTarihi.HasValue && bitisTarihi.HasValue && baslangicTarihi > bitisTarihi)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            return await _logoRepository.LogoStokHareketleriGetirAsync(malzemeKodu, baslangicTarihi, bitisTarihi);
        }

        public async Task<decimal> GuncelStokGetirAsync(string malzemeKodu, string depoKodu = "")
        {
            if (string.IsNullOrWhiteSpace(malzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz", nameof(malzemeKodu));

            return await _logoRepository.LogoGuncelStokGetirAsync(malzemeKodu, depoKodu);
        }

        public async Task<bool> StokHareketEkleAsync(LogoStokHareket stokHareket)
        {
            ValidateStokHareket(stokHareket);
            return await _logoRepository.LogoStokHareketEkleAsync(stokHareket);
        }

        public async Task<List<LogoDepo>> DepoListesiGetirAsync()
        {
            return await _logoRepository.LogoDepoListesiGetirAsync();
        }

        #endregion

        #region Fatura ve Belge İşlemleri

        public async Task<List<LogoFatura>> FaturaListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null)
        {
            if (baslangicTarihi.HasValue && bitisTarihi.HasValue && baslangicTarihi > bitisTarihi)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            return await _logoRepository.LogoFaturaListesiGetirAsync(baslangicTarihi, bitisTarihi);
        }

        public async Task<LogoFatura?> FaturaDetayGetirAsync(string faturaNo)
        {
            if (string.IsNullOrWhiteSpace(faturaNo))
                throw new ArgumentException("Fatura numarası boş olamaz", nameof(faturaNo));

            return await _logoRepository.LogoFaturaDetayGetirAsync(faturaNo);
        }

        public async Task<bool> FaturaOlusturAsync(LogoFatura fatura)
        {
            ValidateFatura(fatura);
            return await _logoRepository.LogoFaturaOlusturAsync(fatura);
        }

        public async Task<bool> IrsaliyeOlusturAsync(LogoIrsaliye irsaliye)
        {
            ValidateIrsaliye(irsaliye);
            return await _logoRepository.LogoIrsaliyeOlusturAsync(irsaliye);
        }

        #endregion

        #region Sipariş Yönetimi

        public async Task<List<LogoSiparis>> SiparisListesiGetirAsync(DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null)
        {
            if (baslangicTarihi.HasValue && bitisTarihi.HasValue && baslangicTarihi > bitisTarihi)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            return await _logoRepository.LogoSiparisListesiGetirAsync(baslangicTarihi, bitisTarihi);
        }

        public async Task<LogoSiparis?> SiparisDetayGetirAsync(string siparisNo)
        {
            if (string.IsNullOrWhiteSpace(siparisNo))
                throw new ArgumentException("Sipariş numarası boş olamaz", nameof(siparisNo));

            return await _logoRepository.LogoSiparisDetayGetirAsync(siparisNo);
        }

        public async Task<bool> SiparisOlusturAsync(LogoSiparis siparis)
        {
            ValidateSiparis(siparis);
            return await _logoRepository.LogoSiparisOlusturAsync(siparis);
        }

        public async Task<bool> SiparisGuncelleAsync(LogoSiparis siparis)
        {
            ValidateSiparis(siparis);
            return await _logoRepository.LogoSiparisGuncelleAsync(siparis);
        }

        #endregion

        #region Raporlama ve Analiz

        public async Task<List<LogoSatisRaporu>> SatisRaporuGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            if (baslangicTarihi > bitisTarihi)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            return await _logoRepository.LogoSatisRaporuGetirAsync(baslangicTarihi, bitisTarihi);
        }

        public async Task<List<LogoStokRaporu>> StokRaporuGetirAsync(string depoKodu = "")
        {
            return await _logoRepository.LogoStokRaporuGetirAsync(depoKodu);
        }

        public async Task<List<LogoCariRaporu>> CariRaporuGetirAsync()
        {
            return await _logoRepository.LogoCariRaporuGetirAsync();
        }

        public async Task<LogoFinansalRapor> FinansalRaporGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            if (baslangicTarihi > bitisTarihi)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            return await _logoRepository.LogoFinansalRaporGetirAsync(baslangicTarihi, bitisTarihi);
        }

        #endregion

        #region Senkronizasyon İşlemleri

        public async Task<bool> VerileriSenkronizeEtAsync()
        {
            return await _logoRepository.LogoVerileriSenkronizeEtAsync();
        }

        public async Task<DateTime?> SonSenkronizasyonTarihiGetirAsync()
        {
            return await _logoRepository.LogoSonSenkronizasyonTarihiGetirAsync();
        }

        public async Task<List<LogoSenkronizasyonLog>> SenkronizasyonLoglariniGetirAsync(int gunSayisi = 7)
        {
            if (gunSayisi < 1 || gunSayisi > 365)
                throw new ArgumentException("Gün sayısı 1-365 arasında olmalıdır", nameof(gunSayisi));

            return await _logoRepository.LogoSenkronizasyonLoglariniGetirAsync(gunSayisi);
        }

        #endregion

        #region Validation Methods

        private static void ValidateUrun(LogoUrun urun)
        {
            if (urun == null)
                throw new ArgumentNullException(nameof(urun));

            if (string.IsNullOrWhiteSpace(urun.MalzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz");

            if (string.IsNullOrWhiteSpace(urun.MalzemeAdi))
                throw new ArgumentException("Malzeme adı boş olamaz");

            if (urun.BirimFiyat < 0)
                throw new ArgumentException("Birim fiyat negatif olamaz");
        }

        private static void ValidateCariHesap(LogoCariHesap cariHesap)
        {
            if (cariHesap == null)
                throw new ArgumentNullException(nameof(cariHesap));

            if (string.IsNullOrWhiteSpace(cariHesap.CariKodu))
                throw new ArgumentException("Cari kodu boş olamaz");

            if (string.IsNullOrWhiteSpace(cariHesap.CariAdi))
                throw new ArgumentException("Cari adı boş olamaz");
        }

        private static void ValidateStokHareket(LogoStokHareket stokHareket)
        {
            if (stokHareket == null)
                throw new ArgumentNullException(nameof(stokHareket));

            if (string.IsNullOrWhiteSpace(stokHareket.MalzemeKodu))
                throw new ArgumentException("Malzeme kodu boş olamaz");

            if (stokHareket.Miktar == 0)
                throw new ArgumentException("Miktar sıfır olamaz");
        }

        private static void ValidateFatura(LogoFatura fatura)
        {
            if (fatura == null)
                throw new ArgumentNullException(nameof(fatura));

            if (string.IsNullOrWhiteSpace(fatura.CariKodu))
                throw new ArgumentException("Cari kodu boş olamaz");

            if (fatura.FaturaDetaylari == null || !fatura.FaturaDetaylari.Any())
                throw new ArgumentException("Fatura detayları boş olamaz");
        }

        private static void ValidateIrsaliye(LogoIrsaliye irsaliye)
        {
            if (irsaliye == null)
                throw new ArgumentNullException(nameof(irsaliye));

            if (string.IsNullOrWhiteSpace(irsaliye.CariKodu))
                throw new ArgumentException("Cari kodu boş olamaz");

            if (irsaliye.IrsaliyeDetaylari == null || !irsaliye.IrsaliyeDetaylari.Any())
                throw new ArgumentException("İrsaliye detayları boş olamaz");
        }

        private static void ValidateSiparis(LogoSiparis siparis)
        {
            if (siparis == null)
                throw new ArgumentNullException(nameof(siparis));

            if (string.IsNullOrWhiteSpace(siparis.CariKodu))
                throw new ArgumentException("Cari kodu boş olamaz");

            if (siparis.SiparisDetaylari == null || !siparis.SiparisDetaylari.Any())
                throw new ArgumentException("Sipariş detayları boş olamaz");
        }

        #endregion
    }
}