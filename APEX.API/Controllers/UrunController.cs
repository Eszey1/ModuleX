using APEX.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace APEX.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunController : ControllerBase
    {
        private readonly SayimService _sayimService;
        private readonly ILogger<UrunController> _logger;

        public UrunController(SayimService sayimService, ILogger<UrunController> logger)
        {
            _sayimService = sayimService ?? throw new ArgumentNullException(nameof(sayimService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/Urun/ara/8691234567890
        [HttpGet("ara/{barkod}")]
        public async Task<IActionResult> BarkodIleAra(string barkod)
        {
            try
            {
                _logger.LogInformation("Barkod arama isteği: {Barkod}", barkod);
                var urun = await _sayimService.BarkodOkutAsync(barkod);
                _logger.LogInformation("Barkod arama başarılı: {Barkod}", barkod);
                return Ok(urun);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Geçersiz barkod parametresi: {Barkod}, Hata: {Message}", barkod, ex.Message);
                return BadRequest(new { mesaj = ex.Message, hataKodu = "GECERSIZ_PARAMETRE" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Ürün bulunamadı: {Barkod}, Hata: {Message}", barkod, ex.Message);
                return NotFound(new { mesaj = ex.Message, hataKodu = "URUN_BULUNAMADI" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Barkod arama sırasında beklenmeyen hata: {Barkod}", barkod);
                return StatusCode(500, new { mesaj = "İç sunucu hatası oluştu", hataKodu = "SUNUCU_HATASI" });
            }
        }

        // GET: api/Urun/liste?limit=100
        [HttpGet("liste")]
        public async Task<IActionResult> TumUrunler([FromQuery] int limit = 1000)
        {
            try
            {
                _logger.LogInformation("Ürün listesi isteği: Limit {Limit}", limit);
                var urunler = await _sayimService.TumUrunleriGetirAsync(limit);
                _logger.LogInformation("Ürün listesi başarıyla getirildi: {Count} adet", urunler?.Count ?? 0);
                return Ok(new { 
                    urunler = urunler, 
                    toplam = urunler?.Count ?? 0,
                    limit = limit
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Geçersiz limit parametresi: {Limit}, Hata: {Message}", limit, ex.Message);
                return BadRequest(new { mesaj = ex.Message, hataKodu = "GECERSIZ_LIMIT" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün listesi getirme sırasında beklenmeyen hata: Limit {Limit}", limit);
                return StatusCode(500, new { mesaj = "İç sunucu hatası oluştu", hataKodu = "SUNUCU_HATASI" });
            }
        }
    }
}
