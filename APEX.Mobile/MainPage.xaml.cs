using System.Text.Json;
using APEX.Core.Entities;
using APEX.Mobile.Interfaces;

namespace APEX.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IBarcodeScanner _barcodeScanner;

        // JsonSerializerOptions için statik alanlar (CA1869 düzeltmesi)
        private static readonly JsonSerializerOptions CamelCaseOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions CamelCaseIndentedOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public MainPage(IBarcodeScanner barcodeScanner)
        {
            InitializeComponent();
            
            _barcodeScanner = barcodeScanner;
            
            // API URL'yi konfigürasyondan al veya varsayılan değer kullan
            _apiBaseUrl = Preferences.Get("ApiBaseUrl", "https://localhost:7001");
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await BekleyenSayimlariKontrolEt();
        }

        private async void YeniSayimButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.SayimEkrani(_barcodeScanner));
        }

        private async void BarkodAraButton_Clicked(object sender, EventArgs e)
        {
            // Önceki sonuçları temizle
            UrunBulunduBorder.IsVisible = false;
            HataMesajiBorder.IsVisible = false;

            var barkod = BarkodEntry.Text?.Trim();
            if (string.IsNullOrEmpty(barkod))
            {
                HataMesajiLabel.Text = "Lütfen barkod girin!";
                HataMesajiBorder.IsVisible = true;
                return;
            }

            try
            {
                var response = await _httpClient.GetAsync($"/api/Urun/barkod/{barkod}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var urun = JsonSerializer.Deserialize<Urun>(jsonResponse, CamelCaseOptions);

                    if (urun != null)
                    {
                        // Ürün bulundu mesajını göster
                        UrunBilgiLabel.Text = $"{urun.Adi}\nKod: {urun.Kod}\nStok: {urun.MevcutStok} {urun.Birim}\nFiyat: {urun.Fiyat:C}";
                        
                        UrunBulunduBorder.IsVisible = true;
                        HataMesajiBorder.IsVisible = false;
                    }
                    else
                    {
                        HataMesajiLabel.Text = "Ürün verisi okunamadı!";
                        HataMesajiBorder.IsVisible = true;
                        UrunBulunduBorder.IsVisible = false;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    HataMesajiLabel.Text = "Ürün bulunamadı!";
                    HataMesajiBorder.IsVisible = true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    HataMesajiLabel.Text = $"API Hatası: {response.StatusCode}";
                    HataMesajiBorder.IsVisible = true;
                }
            }
            catch (HttpRequestException ex)
            {
                HataMesajiLabel.Text = $"Bağlantı Hatası: API'ye ulaşılamıyor.\n\nAPI URL: {_apiBaseUrl}\nHata: {ex.Message}";
                HataMesajiBorder.IsVisible = true;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                HataMesajiLabel.Text = "Zaman Aşımı: API yanıt vermiyor.";
                HataMesajiBorder.IsVisible = true;
            }
            catch (Exception ex)
            {
                HataMesajiLabel.Text = $"Beklenmeyen Hata: {ex.Message}";
                HataMesajiBorder.IsVisible = true;
            }
        }

        private async void TumUrunlerButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Urun/liste?limit=10");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    
                    // API'den gelen yeni format için deserialize
                    var apiResponse = JsonSerializer.Deserialize<ApiListResponse>(json, CaseInsensitiveOptions);

                    if (apiResponse?.Urunler != null && apiResponse.Urunler.Count > 0)
                    {
                        var mesaj = string.Join("\n", apiResponse.Urunler.Take(5).Select(u => $"• {u.Adi} - {u.MevcutStok} {u.Birim}"));
                        await DisplayAlert($"İlk 5 Ürün (Toplam: {apiResponse.Toplam})", mesaj, "Tamam");
                    }
                    else
                    {
                        await DisplayAlert("Bilgi", "Hiç ürün bulunamadı", "Tamam");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Hata", $"Ürünler getirilemedi: {response.StatusCode}", "Tamam");
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Bağlantı Hatası", $"API'ye ulaşılamadı:\n{ex.Message}", "Tamam");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                await DisplayAlert("Zaman Aşımı", "API yanıt vermiyor", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Beklenmeyen hata:\n{ex.Message}", "Tamam");
            }
        }

        private async void RaporlarButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.RaporlarEkrani());
        }

        private async void InsanKaynaklariButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("İnsan Kaynakları", "İnsan Kaynakları modülü yakında eklenecek.", "Tamam");
        }

        private async void SiparislerButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Siparişler", "Siparişler modülü yakında eklenecek.", "Tamam");
        }

        private async void HizliErisimButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Hızlı Erişim", "Hızlı erişim menüsü yakında eklenecek.", "Tamam");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _httpClient?.Dispose();
        }

        private async Task BekleyenSayimlariKontrolEt()
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var sayimlarPath = Path.Combine(documentsPath, "APEX", "Sayimlar");
                
                if (!Directory.Exists(sayimlarPath))
                {
                    BekleyenSayimlariGonderButton.IsVisible = false;
                    return;
                }

                var files = Directory.GetFiles(sayimlarPath, "*.json");
                var bekleyenSayimlar = new List<string>();

                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var sayimData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        
                        if (sayimData != null && sayimData.TryGetValue("gonderildi", out var gonderildiValue))
                        {
                            var gonderildi = gonderildiValue.ToString()?.ToLower() == "true";
                            if (!gonderildi)
                            {
                                bekleyenSayimlar.Add(file);
                            }
                        }
                        else
                        {
                            // Eski format, gönderilmemiş kabul et
                            bekleyenSayimlar.Add(file);
                        }
                    }
                    catch
                    {
                        // Dosya okuma hatası, atla
                        continue;
                    }
                }

                if (bekleyenSayimlar.Count > 0)
                {
                    BekleyenSayimlariGonderButton.IsVisible = true;
                    BekleyenSayimLabel.Text = $"{bekleyenSayimlar.Count} adet sayım Logo'ya gönderilmeyi bekliyor.";
                }
                else
                {
                    BekleyenSayimlariGonderButton.IsVisible = false;
                    BekleyenSayimLabel.Text = "Bekleyen sayım yok";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bekleyen sayımlar kontrol hatası: {ex.Message}");
                BekleyenSayimlariGonderButton.IsVisible = false;
            }
        }

        private async void BekleyenSayimlariGonderButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var sayimlarPath = Path.Combine(documentsPath, "APEX", "Sayimlar");
                
                if (!Directory.Exists(sayimlarPath))
                {
                    await DisplayAlert("Bilgi", "Bekleyen sayım bulunamadı.", "Tamam");
                    return;
                }

                var files = Directory.GetFiles(sayimlarPath, "*.json");
                var bekleyenSayimlar = new List<string>();

                // Bekleyen sayımları bul
                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var sayimData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        
                        if (sayimData != null && sayimData.TryGetValue("gonderildi", out var gonderildiValue))
                        {
                            var gonderildi = gonderildiValue.ToString()?.ToLower() == "true";
                            if (!gonderildi)
                            {
                                bekleyenSayimlar.Add(file);
                            }
                        }
                        else
                        {
                            bekleyenSayimlar.Add(file);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (bekleyenSayimlar.Count == 0)
                {
                    await DisplayAlert("Bilgi", "Bekleyen sayım bulunamadı.", "Tamam");
                    await BekleyenSayimlariKontrolEt();
                    return;
                }

                int basarili = 0;
                int basarisiz = 0;

                foreach (var file in bekleyenSayimlar)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var sayimData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        
                        if (sayimData != null)
                        {
                            // API formatına dönüştür
                            var apiData = new
                            {
                                Tarih = DateTime.Parse(sayimData["tarih"].ToString() ?? DateTime.Now.ToString()),
                                KullaniciId = sayimData["kullaniciId"].ToString() ?? "1",
                                Urunler = JsonSerializer.Deserialize<List<object>>(sayimData["urunler"].ToString() ?? "[]")
                            };

                            var apiJson = JsonSerializer.Serialize(apiData, CamelCaseOptions);

                            var content = new StringContent(apiJson, System.Text.Encoding.UTF8, "application/json");
                            var response = await _httpClient.PostAsync("/api/Sayim/kaydet", content);

                            if (response.IsSuccessStatusCode)
                            {
                                // Başarılı, dosyayı gönderildi olarak işaretle
                                sayimData["gonderildi"] = true;
                                var updatedJson = JsonSerializer.Serialize(sayimData, CamelCaseIndentedOptions);
                                await File.WriteAllTextAsync(file, updatedJson);
                                basarili++;
                            }
                            else
                            {
                                basarisiz++;
                            }
                        }
                    }
                    catch
                    {
                        basarisiz++;
                    }
                }

                await DisplayAlert("Sonuç", 
                    $"Gönderim tamamlandı!\n\n" +
                    $"Başarılı: {basarili}\n" +
                    $"Başarısız: {basarisiz}", 
                    "Tamam");

                await BekleyenSayimlariKontrolEt();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", 
                    $"Bekleyen sayımlar gönderilirken hata oluştu:\n\n{ex.Message}", 
                    "Tamam");
            }
        }
    }

    // API'den gelen liste response formatı için model
    public class ApiListResponse
    {
        public List<Urun>? Urunler { get; set; }
        public int Toplam { get; set; }
        public int Limit { get; set; }
    }
}
