using System.Text;
using System.Text.Json;
using APEX.Mobile.Models;

namespace APEX.Mobile.Services
{
    public class LogoErpApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string? _authToken;

        public LogoErpApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = "http://localhost:5105/api/LogoErp"; // API base URL - düzeltildi
        }

        #region Authentication

        public async Task<bool> BaglantiTestiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/baglanti-testi");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Basarili ?? false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GirisYapAsync(string kullaniciAdi, string sifre, string? sirketKodu = null)
        {
            try
            {
                var loginRequest = new
                {
                    KullaniciAdi = kullaniciAdi,
                    Sifre = sifre,
                    SirketKodu = sirketKodu
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/giris", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (!string.IsNullOrEmpty(result?.Token))
                    {
                        _authToken = result.Token;
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
                        return result.Token;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void CikisYap()
        {
            _authToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        #endregion

        #region Ürün İşlemleri

        public async Task<List<LogoUrunModel>?> UrunListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/urunler?sayfa={sayfa}&kayitSayisi={kayitSayisi}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoUrunModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LogoUrunModel?> UrunDetayGetirAsync(string malzemeKodu)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/urunler/{Uri.EscapeDataString(malzemeKodu)}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<LogoUrunModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<LogoUrunKategoriModel>?> UrunKategorileriGetirAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/urun-kategorileri");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoUrunKategoriModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Cari Hesap İşlemleri

        public async Task<List<LogoCariHesapModel>?> CariHesapListesiGetirAsync(int sayfa = 1, int kayitSayisi = 100)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/cari-hesaplar?sayfa={sayfa}&kayitSayisi={kayitSayisi}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoCariHesapModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LogoCariHesapModel?> CariHesapDetayGetirAsync(string cariKodu)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/cari-hesaplar/{Uri.EscapeDataString(cariKodu)}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<LogoCariHesapModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<decimal?> CariHesapBakiyeGetirAsync(string cariKodu)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/cari-hesaplar/{Uri.EscapeDataString(cariKodu)}/bakiye");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<CariHesapBakiyeResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Bakiye;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Stok İşlemleri

        public async Task<decimal?> GuncelStokGetirAsync(string malzemeKodu, string depoKodu = "")
        {
            try
            {
                var url = $"{_baseUrl}/stok/{Uri.EscapeDataString(malzemeKodu)}/guncel";
                if (!string.IsNullOrEmpty(depoKodu))
                {
                    url += $"?depoKodu={Uri.EscapeDataString(depoKodu)}";
                }

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StokResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.GuncelStok;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<LogoStokHareketModel>?> StokHareketleriGetirAsync(string malzemeKodu, DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null)
        {
            try
            {
                var url = $"{_baseUrl}/stok/{Uri.EscapeDataString(malzemeKodu)}/hareketler";
                var queryParams = new List<string>();

                if (baslangicTarihi.HasValue)
                    queryParams.Add($"baslangicTarihi={baslangicTarihi.Value:yyyy-MM-dd}");
                
                if (bitisTarihi.HasValue)
                    queryParams.Add($"bitisTarihi={bitisTarihi.Value:yyyy-MM-dd}");

                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoStokHareketModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<LogoDepoModel>?> DepoListesiGetirAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/depolar");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoDepoModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Raporlama

        public async Task<List<LogoSatisRaporuModel>?> SatisRaporuGetirAsync(DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            try
            {
                var url = $"{_baseUrl}/raporlar/satis?baslangicTarihi={baslangicTarihi:yyyy-MM-dd}&bitisTarihi={bitisTarihi:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoSatisRaporuModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<LogoStokRaporuModel>?> StokRaporuGetirAsync(string depoKodu = "")
        {
            try
            {
                var url = $"{_baseUrl}/raporlar/stok";
                if (!string.IsNullOrEmpty(depoKodu))
                    url += $"?depoKodu={Uri.EscapeDataString(depoKodu)}";

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LogoStokRaporuModel>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Veri;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Senkronizasyon

        public async Task<bool> VerileriSenkronizeEtAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/senkronizasyon", null);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result != null && string.IsNullOrEmpty(result.Hata);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DateTime?> SonSenkronizasyonTarihiGetirAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/senkronizasyon/son-tarih");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SenkronizasyonTarihResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.SonSenkronizasyonTarihi;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Helper Properties

        public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

        #endregion
    }

    #region Response Models

    public class ApiResponse<T>
    {
        public T? Veri { get; set; }
        public string? Hata { get; set; }
        public string? Mesaj { get; set; }
        public bool Basarili { get; set; }
    }

    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Mesaj { get; set; }
    }

    public class CariHesapBakiyeResponse
    {
        public string? CariKodu { get; set; }
        public decimal Bakiye { get; set; }
    }

    public class StokResponse
    {
        public string? MalzemeKodu { get; set; }
        public string? DepoKodu { get; set; }
        public decimal GuncelStok { get; set; }
    }

    public class SenkronizasyonTarihResponse
    {
        public DateTime? SonSenkronizasyonTarihi { get; set; }
    }

    #endregion
}