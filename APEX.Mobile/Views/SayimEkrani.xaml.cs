using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using APEX.Mobile.Interfaces;

namespace APEX.Mobile.Views
{
    public partial class SayimEkrani : ContentPage
    {
        private readonly IBarcodeScanner _barcodeScanner;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        public ObservableCollection<SayimUrun> Urunler { get; set; }
        private DateTime sayimBaslangic;

        public SayimEkrani(IBarcodeScanner barcodeScanner)
        {
            InitializeComponent();

            _barcodeScanner = barcodeScanner;
            
            // API URL'yi konfigürasyondan al veya varsayılan değer kullan
            _apiBaseUrl = Preferences.Get("ApiBaseUrl", "http://localhost:5105");
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            Urunler = new ObservableCollection<SayimUrun>();
            UrunListesi.ItemsSource = Urunler;

            sayimBaslangic = DateTime.Now;
            SayimTarihLabel.Text = sayimBaslangic.ToString("dd MMMM yyyy - HH:mm");

            ToplamGuncelle();
        }

        private async void BarkodOkutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var barkod = await _barcodeScanner.ScanBarcodeAsync();

                if (!string.IsNullOrWhiteSpace(barkod))
                {
                    await UrunEkle(barkod);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Kamera açılamadı: {ex.Message}\nManuel giriş deneyin.",
                    "Tamam");
                BarkodGir();
            }
        }

        private void ManuelGirisButton_Clicked(object sender, EventArgs e)
        {
            BarkodGir();
        }

        private async void BarkodGir()
        {
            string barkod = await DisplayPromptAsync(
                "Barkod Giriş",
                "Barkod numarasını girin:",
                "Ekle",
                "İptal",
                placeholder: "Örn: 8690123456789",
                keyboard: Keyboard.Text,
                maxLength: 20);

            if (!string.IsNullOrWhiteSpace(barkod))
            {
                await UrunEkle(barkod);
            }
        }

        private async Task UrunEkle(string barkod)
        {
            var mevcutUrun = Urunler.FirstOrDefault(u => u.Barkod == barkod);

            if (mevcutUrun != null)
            {
                mevcutUrun.SayilanMiktar++;
                ToplamGuncelle();

                await DisplayAlert("Güncellendi",
                    $"{mevcutUrun.Adi}\nYeni miktar: {mevcutUrun.SayilanMiktar} {mevcutUrun.Birim}",
                    "Tamam");
            }
            else
            {
                var yeniUrun = new SayimUrun
                {
                    Id = Urunler.Count + 1,
                    Barkod = barkod,
                    Kod = $"URN{barkod.Substring(Math.Max(0, barkod.Length - 4))}",
                    Adi = $"Test Ürün {barkod}",
                    MevcutStok = new Random().Next(10, 100),
                    SayilanMiktar = 1,
                    Birim = "Adet",
                    Fiyat = new Random().Next(10, 500)
                };

                Urunler.Add(yeniUrun);
                ToplamGuncelle();

                await DisplayAlert("Eklendi",
                    $"{yeniUrun.Adi}\nBarkod: {yeniUrun.Barkod}\nMiktar: 1 {yeniUrun.Birim}",
                    "Tamam");
            }
        }

        private void ArtirButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is SayimUrun urun)
            {
                urun.SayilanMiktar++;
                ToplamGuncelle();
            }
        }

        private void AzaltButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is SayimUrun urun)
            {
                if (urun.SayilanMiktar > 0)
                {
                    urun.SayilanMiktar--;
                    ToplamGuncelle();
                }
            }
        }

        private async void SilButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is SayimUrun urun)
            {
                bool onay = await DisplayAlert(
                    "Ürün Sil",
                    $"{urun.Adi}\nBu ürünü listeden çıkarmak istediğinize emin misiniz?",
                    "Evet, Sil",
                    "Hayır");

                if (onay)
                {
                    Urunler.Remove(urun);
                    ToplamGuncelle();

                    await DisplayAlert("Silindi",
                        $"{urun.Adi} listeden çıkarıldı.",
                        "Tamam");
                }
            }
        }

        private async void TemizleButton_Clicked(object sender, EventArgs e)
        {
            if (Urunler.Count == 0) return;

            bool onay = await DisplayAlert(
                "Tümünü Temizle",
                $"Listedeki {Urunler.Count} ürünün tamamı silinecek!\nEmin misiniz?",
                "Evet, Temizle",
                "Hayır");

            if (onay)
            {
                Urunler.Clear();
                ToplamGuncelle();

                await DisplayAlert("Temizlendi",
                    "Tüm ürünler listeden çıkarıldı.",
                    "Tamam");
            }
        }

        private void ToplamGuncelle()
        {
            int toplamAdet = Urunler.Sum(u => u.SayilanMiktar);
            ToplamUrunLabel.Text = $"{toplamAdet} adet";

            TemizleButonu.IsVisible = Urunler.Count > 0;
        }

        private void MiktarEntry_Completed(object sender, EventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is SayimUrun urun)
            {
                if (int.TryParse(entry.Text, out int yeniMiktar) && yeniMiktar >= 0)
                {
                    urun.SayilanMiktar = yeniMiktar;
                    ToplamGuncelle();
                }
                else
                {
                    entry.Text = urun.SayilanMiktar.ToString();
                }
            }
        }

        private async void TamamlaButton_Clicked(object sender, EventArgs e)
        {
            if (Urunler.Count == 0)
            {
                await DisplayAlert("Uyarı",
                    "Sayım listesi boş!\nEn az bir ürün ekleyin.",
                    "Tamam");
                return;
            }

            int toplamFark = Urunler.Sum(u => u.Fark);
            string farkMesaj = toplamFark == 0 ? "Fark yok" :
                              toplamFark > 0 ? $"+{toplamFark} fazla" :
                              $"{toplamFark} eksik";

            bool onay = await DisplayAlert(
                "Sayımı Tamamla",
                $"{Urunler.Count} ürün sayıldı\n" +
                $"Toplam miktar: {Urunler.Sum(u => u.SayilanMiktar)} adet\n" +
                $"Toplam fark: {farkMesaj}\n\n" +
                $"Sayım kaydedilip Logo'ya gönderilsin mi?",
                "Evet, Tamamla",
                "Hayır");

            if (onay)
            {
                await SayimKaydet();
            }
        }

        private async Task SayimKaydet()
        {
            // Önce yerel olarak kaydet
            var sayimId = await YerelKaydet();
            
            try
            {
                // API'ye gönderilecek veriyi hazırla
                var sayimData = new
                {
                    Tarih = sayimBaslangic,
                    KullaniciId = "1", // Şimdilik sabit kullanıcı ID
                    Urunler = Urunler.Select(u => new
                    {
                        Id = u.Id,
                        Kod = u.Kod ?? "",
                        Adi = u.Adi ?? "",
                        Barkod = u.Barkod ?? "",
                        MevcutStok = u.MevcutStok,
                        SayilanMiktar = (decimal)u.SayilanMiktar,
                        Fark = (decimal)u.Fark,
                        Birim = u.Birim ?? ""
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(sayimData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Sayim/kaydet", content);

                if (response.IsSuccessStatusCode)
                {
                    // Logo'ya başarıyla gönderildi, yerel kaydı gönderildi olarak işaretle
                    await YerelKaydiGonderildiOlarakIsaretle(sayimId);
                    
                    await DisplayAlert("Başarılı",
                        $"Sayım kaydedildi!\n\n" +
                        $"Tarih: {sayimBaslangic:dd/MM/yyyy HH:mm}\n" +
                        $"Ürün: {Urunler.Count} adet\n" +
                        $"Logo'ya gönderildi",
                        "Tamam");

                    await Navigation.PopAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Uyarı",
                        $"Sayım yerel olarak kaydedildi ancak Logo'ya gönderilemedi!\n\n" +
                        $"Hata: {response.StatusCode}\n" +
                        $"Detay: {errorContent}\n\n" +
                        $"Sayım daha sonra tekrar gönderilebilir.",
                        "Tamam");
                    
                    await Navigation.PopAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Uyarı",
                    $"Sayım yerel olarak kaydedildi ancak Logo'ya gönderilemedi!\n\n" +
                    $"URL: {_apiBaseUrl}\n" +
                    $"Hata: {ex.Message}\n\n" +
                    $"Sayım daha sonra tekrar gönderilebilir.",
                    "Tamam");
                
                await Navigation.PopAsync();
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                await DisplayAlert("Uyarı",
                    $"Sayım yerel olarak kaydedildi ancak Logo'ya gönderilemedi!\n\n" +
                    $"API yanıt vermiyor.\n\n" +
                    $"Sayım daha sonra tekrar gönderilebilir.",
                    "Tamam");
                
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Uyarı",
                    $"Sayım yerel olarak kaydedildi ancak Logo'ya gönderilemedi!\n\n" +
                    $"Hata: {ex.Message}\n\n" +
                    $"Sayım daha sonra tekrar gönderilebilir.",
                    "Tamam");
                
                await Navigation.PopAsync();
            }
        }

        private async Task<string> YerelKaydet()
        {
            try
            {
                var sayimId = Guid.NewGuid().ToString();
                var sayimData = new
                {
                    Id = sayimId,
                    Tarih = sayimBaslangic,
                    KullaniciId = "1",
                    Gonderildi = false,
                    Urunler = Urunler.Select(u => new
                    {
                        Id = u.Id,
                        Kod = u.Kod ?? "",
                        Adi = u.Adi ?? "",
                        Barkod = u.Barkod ?? "",
                        MevcutStok = u.MevcutStok,
                        SayilanMiktar = (decimal)u.SayilanMiktar,
                        Fark = (decimal)u.Fark,
                        Birim = u.Birim ?? ""
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(sayimData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                // Yerel dosya sistemine kaydet
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var sayimlarPath = Path.Combine(documentsPath, "APEX", "Sayimlar");
                Directory.CreateDirectory(sayimlarPath);
                
                var fileName = $"sayim_{sayimBaslangic:yyyyMMdd_HHmmss}_{sayimId}.json";
                var filePath = Path.Combine(sayimlarPath, fileName);
                
                await File.WriteAllTextAsync(filePath, json);
                
                return sayimId;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", 
                    $"Yerel kayıt sırasında hata oluştu: {ex.Message}", 
                    "Tamam");
                throw;
            }
        }

        private async Task YerelKaydiGonderildiOlarakIsaretle(string sayimId)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var sayimlarPath = Path.Combine(documentsPath, "APEX", "Sayimlar");
                
                var files = Directory.GetFiles(sayimlarPath, $"*{sayimId}.json");
                if (files.Length > 0)
                {
                    var filePath = files[0];
                    var json = await File.ReadAllTextAsync(filePath);
                    var sayimData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (sayimData != null)
                    {
                        sayimData["gonderildi"] = true;
                        var updatedJson = JsonSerializer.Serialize(sayimData, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        });
                        
                        await File.WriteAllTextAsync(filePath, updatedJson);
                    }
                }
            }
            catch (Exception ex)
            {
                // Yerel kayıt güncellemesi başarısız olsa da devam et
                System.Diagnostics.Debug.WriteLine($"Yerel kayıt güncelleme hatası: {ex.Message}");
            }
        }
    }

    public class SayimUrun : INotifyPropertyChanged
    {
        private int _sayilanMiktar;
        private decimal _mevcutStok;

        public int Id { get; set; }
        public string? Kod { get; set; }
        public string? Adi { get; set; }
        public string? Barkod { get; set; }
        public string? Birim { get; set; }
        public decimal Fiyat { get; set; }

        public decimal MevcutStok
        {
            get => _mevcutStok;
            set
            {
                _mevcutStok = value;
                OnPropertyChanged(nameof(MevcutStok));
                OnPropertyChanged(nameof(Fark));
                OnPropertyChanged(nameof(FarkText));
                OnPropertyChanged(nameof(FarkDurumu));
            }
        }

        public int SayilanMiktar
        {
            get => _sayilanMiktar;
            set
            {
                _sayilanMiktar = value;
                OnPropertyChanged(nameof(SayilanMiktar));
                OnPropertyChanged(nameof(Fark));
                OnPropertyChanged(nameof(FarkText));
                OnPropertyChanged(nameof(FarkDurumu));
            }
        }

        public int Fark => SayilanMiktar - (int)MevcutStok;

        public string FarkText
        {
            get
            {
                if (Fark > 0) return $"+{Fark}";
                if (Fark < 0) return Fark.ToString();
                return "0";
            }
        }

        public string FarkDurumu
        {
            get
            {
                if (Fark > 0) return "Pozitif";
                if (Fark < 0) return "Negatif";
                return "Esit";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
