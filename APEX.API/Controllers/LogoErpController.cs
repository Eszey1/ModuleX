using APEX.Core.Entities;
using APEX.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APEX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogoErpController : ControllerBase
    {
        private readonly ILogoErpService _logoErpService;
        private readonly ILogger<LogoErpController> _logger;

        public LogoErpController(ILogoErpService logoErpService, ILogger<LogoErpController> logger)
        {
            _logoErpService = logoErpService ?? throw new ArgumentNullException(nameof(logoErpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Bağlantı ve Kimlik Doğrulama

        [HttpGet("baglanti-testi")]
        public async Task<IActionResult> BaglantiTesti()
        {
            try
            {
                var sonuc = await _logoErpService.BaglantiTestiYapAsync();
                return Ok(new { Basarili = sonuc, Mesaj = sonuc ? "Bağlantı başarılı" : "Bağlantı başarısız" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logo ERP bağlantı testi sırasında hata oluştu");
                return StatusCode(500, new { Hata = "Bağlantı testi sırasında hata oluştu", Detay = ex.Message });
            }
        }

        [HttpPost("giris")]
        public async Task<IActionResult> Giris([FromBody] GirisRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.KullaniciAdi) || string.IsNullOrWhiteSpace(request.Sifre))
                {
                    return BadRequest(new { Hata = "Kullanıcı adı ve şifre gereklidir" });
                }

                var token = await _logoErpService.GirisYapAsync(request.KullaniciAdi, request.Sifre, request.SirketKodu ?? "");
                
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { Hata = "Geçersiz kullanıcı bilgileri" });
                }

                return Ok(new { Token = token, Mesaj = "Giriş başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logo ERP giriş işlemi sırasında hata oluştu");
                return StatusCode(500, new { Hata = "Giriş işlemi sırasında hata oluştu", Detay = ex.Message });
            }
        }

        #endregion

        #region Ürün Yönetimi

        [HttpGet("urunler")]
        public async Task<IActionResult> UrunListesi([FromQuery] int sayfa = 1, [FromQuery] int kayitSayisi = 100)
        {
            try
            {
                var urunler = await _logoErpService.UrunListesiGetirAsync(sayfa, kayitSayisi);
                return Ok(new { Veri = urunler, Sayfa = sayfa, KayitSayisi = kayitSayisi });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün listesi alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Ürün listesi alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("urunler/{malzemeKodu}")]
        public async Task<IActionResult> UrunDetay(string malzemeKodu)
        {
            try
            {
                var urun = await _logoErpService.UrunDetayGetirAsync(malzemeKodu);
                
                if (urun == null)
                {
                    return NotFound(new { Hata = "Ürün bulunamadı" });
                }

                return Ok(new { Veri = urun });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün detayı alınırken hata oluştu: {MalzemeKodu}", malzemeKodu);
                return StatusCode(500, new { Hata = "Ürün detayı alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpPost("urunler")]
        public async Task<IActionResult> UrunEkle([FromBody] LogoUrun urun)
        {
            try
            {
                var sonuc = await _logoErpService.UrunEkleAsync(urun);
                
                if (sonuc)
                {
                    return CreatedAtAction(nameof(UrunDetay), new { malzemeKodu = urun.MalzemeKodu }, new { Mesaj = "Ürün başarıyla eklendi" });
                }

                return BadRequest(new { Hata = "Ürün eklenemedi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün eklenirken hata oluştu");
                return StatusCode(500, new { Hata = "Ürün eklenirken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpPut("urunler/{malzemeKodu}")]
        public async Task<IActionResult> UrunGuncelle(string malzemeKodu, [FromBody] LogoUrun urun)
        {
            try
            {
                if (malzemeKodu != urun.MalzemeKodu)
                {
                    return BadRequest(new { Hata = "Malzeme kodu uyuşmuyor" });
                }

                var sonuc = await _logoErpService.UrunGuncelleAsync(urun);
                
                if (sonuc)
                {
                    return Ok(new { Mesaj = "Ürün başarıyla güncellendi" });
                }

                return BadRequest(new { Hata = "Ürün güncellenemedi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün güncellenirken hata oluştu: {MalzemeKodu}", malzemeKodu);
                return StatusCode(500, new { Hata = "Ürün güncellenirken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpDelete("urunler/{malzemeKodu}")]
        public async Task<IActionResult> UrunSil(string malzemeKodu)
        {
            try
            {
                var sonuc = await _logoErpService.UrunSilAsync(malzemeKodu);
                
                if (sonuc)
                {
                    return Ok(new { Mesaj = "Ürün başarıyla silindi" });
                }

                return BadRequest(new { Hata = "Ürün silinemedi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün silinirken hata oluştu: {MalzemeKodu}", malzemeKodu);
                return StatusCode(500, new { Hata = "Ürün silinirken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("urun-kategorileri")]
        public async Task<IActionResult> UrunKategorileri()
        {
            try
            {
                var kategoriler = await _logoErpService.UrunKategorileriGetirAsync();
                return Ok(new { Veri = kategoriler });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün kategorileri alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Ürün kategorileri alınırken hata oluştu", Detay = ex.Message });
            }
        }

        #endregion

        #region Cari Hesap Yönetimi

        [HttpGet("cari-hesaplar")]
        public async Task<IActionResult> CariHesapListesi([FromQuery] int sayfa = 1, [FromQuery] int kayitSayisi = 100)
        {
            try
            {
                var cariHesaplar = await _logoErpService.CariHesapListesiGetirAsync(sayfa, kayitSayisi);
                return Ok(new { Veri = cariHesaplar, Sayfa = sayfa, KayitSayisi = kayitSayisi });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cari hesap listesi alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Cari hesap listesi alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("cari-hesaplar/{cariKodu}")]
        public async Task<IActionResult> CariHesapDetay(string cariKodu)
        {
            try
            {
                var cariHesap = await _logoErpService.CariHesapDetayGetirAsync(cariKodu);
                
                if (cariHesap == null)
                {
                    return NotFound(new { Hata = "Cari hesap bulunamadı" });
                }

                return Ok(new { Veri = cariHesap });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cari hesap detayı alınırken hata oluştu: {CariKodu}", cariKodu);
                return StatusCode(500, new { Hata = "Cari hesap detayı alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("cari-hesaplar/{cariKodu}/bakiye")]
        public async Task<IActionResult> CariHesapBakiye(string cariKodu)
        {
            try
            {
                var bakiye = await _logoErpService.CariHesapBakiyeGetirAsync(cariKodu);
                return Ok(new { CariKodu = cariKodu, Bakiye = bakiye });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cari hesap bakiyesi alınırken hata oluştu: {CariKodu}", cariKodu);
                return StatusCode(500, new { Hata = "Cari hesap bakiyesi alınırken hata oluştu", Detay = ex.Message });
            }
        }

        #endregion

        #region Stok Yönetimi

        [HttpGet("stok/{malzemeKodu}/hareketler")]
        public async Task<IActionResult> StokHareketleri(string malzemeKodu, [FromQuery] DateTime? baslangicTarihi = null, [FromQuery] DateTime? bitisTarihi = null)
        {
            try
            {
                var hareketler = await _logoErpService.StokHareketleriGetirAsync(malzemeKodu, baslangicTarihi, bitisTarihi);
                return Ok(new { Veri = hareketler, MalzemeKodu = malzemeKodu });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stok hareketleri alınırken hata oluştu: {MalzemeKodu}", malzemeKodu);
                return StatusCode(500, new { Hata = "Stok hareketleri alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("stok/{malzemeKodu}/guncel")]
        public async Task<IActionResult> GuncelStok(string malzemeKodu, [FromQuery] string depoKodu = "")
        {
            try
            {
                var stok = await _logoErpService.GuncelStokGetirAsync(malzemeKodu, depoKodu);
                return Ok(new { MalzemeKodu = malzemeKodu, DepoKodu = depoKodu, GuncelStok = stok });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Güncel stok alınırken hata oluştu: {MalzemeKodu}", malzemeKodu);
                return StatusCode(500, new { Hata = "Güncel stok alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("depolar")]
        public async Task<IActionResult> DepoListesi()
        {
            try
            {
                var depolar = await _logoErpService.DepoListesiGetirAsync();
                return Ok(new { Veri = depolar });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Depo listesi alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Depo listesi alınırken hata oluştu", Detay = ex.Message });
            }
        }

        #endregion

        #region Raporlama

        [HttpGet("raporlar/satis")]
        public async Task<IActionResult> SatisRaporu([FromQuery] DateTime baslangicTarihi, [FromQuery] DateTime bitisTarihi)
        {
            try
            {
                var rapor = await _logoErpService.SatisRaporuGetirAsync(baslangicTarihi, bitisTarihi);
                return Ok(new { Veri = rapor, BaslangicTarihi = baslangicTarihi, BitisTarihi = bitisTarihi });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Satış raporu alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Satış raporu alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("raporlar/stok")]
        public async Task<IActionResult> StokRaporu([FromQuery] string depoKodu = "")
        {
            try
            {
                var rapor = await _logoErpService.StokRaporuGetirAsync(depoKodu);
                return Ok(new { Veri = rapor, DepoKodu = depoKodu });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stok raporu alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Stok raporu alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("raporlar/finansal")]
        public async Task<IActionResult> FinansalRapor([FromQuery] DateTime baslangicTarihi, [FromQuery] DateTime bitisTarihi)
        {
            try
            {
                var rapor = await _logoErpService.FinansalRaporGetirAsync(baslangicTarihi, bitisTarihi);
                return Ok(new { Veri = rapor, BaslangicTarihi = baslangicTarihi, BitisTarihi = bitisTarihi });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finansal rapor alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Finansal rapor alınırken hata oluştu", Detay = ex.Message });
            }
        }

        #endregion

        #region Senkronizasyon

        [HttpPost("senkronizasyon")]
        public async Task<IActionResult> VerileriSenkronizeEt()
        {
            try
            {
                var sonuc = await _logoErpService.VerileriSenkronizeEtAsync();
                
                if (sonuc)
                {
                    return Ok(new { Mesaj = "Veriler başarıyla senkronize edildi" });
                }

                return BadRequest(new { Hata = "Senkronizasyon başarısız" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Senkronizasyon sırasında hata oluştu");
                return StatusCode(500, new { Hata = "Senkronizasyon sırasında hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("senkronizasyon/son-tarih")]
        public async Task<IActionResult> SonSenkronizasyonTarihi()
        {
            try
            {
                var tarih = await _logoErpService.SonSenkronizasyonTarihiGetirAsync();
                return Ok(new { SonSenkronizasyonTarihi = tarih });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Son senkronizasyon tarihi alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Son senkronizasyon tarihi alınırken hata oluştu", Detay = ex.Message });
            }
        }

        [HttpGet("senkronizasyon/loglar")]
        public async Task<IActionResult> SenkronizasyonLoglari([FromQuery] int gunSayisi = 7)
        {
            try
            {
                var loglar = await _logoErpService.SenkronizasyonLoglariniGetirAsync(gunSayisi);
                return Ok(new { Veri = loglar, GunSayisi = gunSayisi });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Senkronizasyon logları alınırken hata oluştu");
                return StatusCode(500, new { Hata = "Senkronizasyon logları alınırken hata oluştu", Detay = ex.Message });
            }
        }

        #endregion
    }

    #region Request Models

    public class GirisRequest
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string? SirketKodu { get; set; }
    }

    #endregion
}