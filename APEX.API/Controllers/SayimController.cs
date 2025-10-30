using APEX.Business.Services;
using APEX.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APEX.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SayimController : ControllerBase
    {
        private readonly SayimService _sayimService;
        private readonly ILogger<SayimController> _logger;

        public SayimController(SayimService sayimService, ILogger<SayimController> logger)
        {
            _sayimService = sayimService ?? throw new ArgumentNullException(nameof(sayimService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST: api/Sayim/test
        [HttpPost("test")]
        public IActionResult TestEndpoint([FromBody] object data)
        {
            _logger.LogInformation("Test endpoint çağrıldı: {Data}", data?.ToString());
            return Ok(new { mesaj = "Test başarılı", data = data });
        }

        // POST: api/Sayim/kaydet
        [HttpPost("kaydet")]
        public async Task<IActionResult> SayimKaydet([FromBody] JsonElement rawRequest)
        {
            try
            {
                _logger.LogInformation("=== Sayım kaydetme endpoint'i çağrıldı ===");
                _logger.LogInformation("Raw request alındı");
                
                // JsonElement'i string'e çevir
                var jsonString = rawRequest.GetRawText();
                _logger.LogInformation("JSON string: {JsonString}", jsonString);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                SayimKaydetRequest? request;
                try
                {
                    request = JsonSerializer.Deserialize<SayimKaydetRequest>(jsonString, options);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON deserializasyon hatası");
                    return BadRequest(new { mesaj = "JSON format hatası", hataKodu = "JSON_ERROR" });
                }
                
                if (request == null)
                {
                    _logger.LogWarning("Request deserialize edilemedi");
                    return BadRequest(new { mesaj = "Geçersiz istek verisi", hataKodu = "NULL_REQUEST" });
                }
                
                _logger.LogInformation("Request detayları - Tarih: {Tarih}, KullaniciId: {KullaniciId}, Urun sayısı: {UrunSayisi}", 
                    request.Tarih, request.KullaniciId, request.Urunler?.Count ?? 0);
                
                if (request.Urunler == null || !request.Urunler.Any())
                {
                    _logger.LogWarning("Boş ürün listesi ile sayım kaydetme isteği");
                    return BadRequest(new { mesaj = "Sayım listesi boş olamaz", hataKodu = "BOS_LISTE" });
                }

                // Her ürün için detaylı log
                foreach (var urun in request.Urunler)
                {
                    _logger.LogInformation("Ürün: Id={Id}, Kod={Kod}, Adi={Adi}, Barkod={Barkod}", 
                        urun.Id, urun.Kod, urun.Adi, urun.Barkod);
                }

                var sayim = new Sayim
                {
                    Tarih = request.Tarih,
                    KullaniciId = request.KullaniciId ?? "1",
                    Durum = "Tamamlandi",
                    Detaylar = request.Urunler.Select(u => new SayimDetay
                    {
                        UrunId = u.Id > 0 ? u.Id : 1, // UrunId 0 ise 1 yap
                        UrunAdi = u.Adi,
                        Barkod = u.Barkod,
                        MevcutStok = u.MevcutStok,
                        SayilanMiktar = u.SayilanMiktar,
                        Fark = u.Fark,
                        SonGuncelleme = DateTime.Now
                    }).ToList()
                };

                var sonuc = await _sayimService.SayimKaydetAsync(sayim);
                
                if (sonuc)
                {
                    _logger.LogInformation("Sayım başarıyla kaydedildi: {UrunSayisi} ürün", request.Urunler.Count);
                    return Ok(new { 
                        mesaj = "Sayım başarıyla kaydedildi ve Logo'ya gönderildi",
                        tarih = request.Tarih,
                        urunSayisi = request.Urunler.Count,
                        toplamFark = request.Urunler.Sum(u => u.Fark)
                    });
                }
                else
                {
                    _logger.LogWarning("Sayım kaydedilemedi");
                    return StatusCode(500, new { mesaj = "Sayım kaydedilemedi", hataKodu = "KAYIT_HATASI" });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Geçersiz sayım verisi: {Message}", ex.Message);
                return BadRequest(new { mesaj = ex.Message, hataKodu = "GECERSIZ_VERI" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayım kaydetme sırasında beklenmeyen hata");
                return StatusCode(500, new { mesaj = "İç sunucu hatası oluştu", hataKodu = "SUNUCU_HATASI" });
            }
        }

        [HttpGet("liste")]
        public async Task<IActionResult> SayimListesiGetir([FromQuery] DateTime? baslangicTarihi = null, [FromQuery] DateTime? bitisTarihi = null)
        {
            try
            {
                var sayimlar = await _sayimService.SayimListesiGetirAsync(baslangicTarihi, bitisTarihi);
                return Ok(sayimlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayım listesi getirme hatası");
                return StatusCode(500, new { message = "Sayım listesi getirme işlemi sırasında bir hata oluştu" });
            }
        }
    }

    public class SayimKaydetRequest
    {
        public DateTime Tarih { get; set; }
        public string? KullaniciId { get; set; }
        public List<SayimUrunDto>? Urunler { get; set; }
    }

    public class SayimUrunDto
    {
        public int Id { get; set; }
        public string Kod { get; set; } = string.Empty;
        public string Adi { get; set; } = string.Empty;
        public string Barkod { get; set; } = string.Empty;
        public decimal MevcutStok { get; set; }
        public decimal SayilanMiktar { get; set; }
        public decimal Fark { get; set; }
        public string Birim { get; set; } = string.Empty;
    }
}
