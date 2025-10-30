using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Text.Json;
using APEX.Mobile.Models;

namespace APEX.Mobile.Views
{
    public partial class RaporlarEkrani : ContentPage
    {
        public ObservableCollection<SayimRapor> Sayimlar { get; set; } = new();
        private ObservableCollection<SayimRapor> TumSayimlar { get; set; } = new();

        public RaporlarEkrani()
        {
            InitializeComponent();
            BindingContext = this;

            // Tarih picker'ları bugünün tarihine ayarla
            BaslangicTarihi.Date = DateTime.Today.AddDays(-30);
            BitisTarihi.Date = DateTime.Today;

            // Sayım verilerini yükle
            _ = SayimVerileriniYukleAsync();
        }

        private async Task SayimVerileriniYukleAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("API'den sayım verileri yükleniyor...");
                
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5105/");
                
                var response = await client.GetAsync("api/Sayim/liste");
                
                System.Diagnostics.Debug.WriteLine($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Response JSON: {json}");
                    
                    var sayimlar = JsonSerializer.Deserialize<List<SayimApiResponse>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    System.Diagnostics.Debug.WriteLine($"Deserialize edilen sayım sayısı: {sayimlar?.Count ?? 0}");

                    if (sayimlar != null)
                    {
                        // Önce mevcut verileri temizle
                        TumSayimlar.Clear();
                        Sayimlar.Clear();
                        
                        foreach (var sayim in sayimlar)
                        {
                            System.Diagnostics.Debug.WriteLine($"Sayım işleniyor - FisNo: {sayim.FisNo}, Ürün sayısı: {sayim.Urunler?.Count ?? 0}");
                            
                            // Hem Urunler hem de Detaylar alanlarını kontrol et
                            var urunler = sayim.Urunler ?? sayim.Detaylar ?? new List<SayimDetayApiResponse>();
                            
                            var rapor = new SayimRapor
                            {
                                Id = sayim.Id,
                                Tarih = sayim.Tarih,
                                Kullanici = sayim.KullaniciId ?? "Bilinmeyen",
                                ToplamUrun = urunler.Count,
                                ToplamFark = (int)(urunler.Sum(u => u.Fark)),
                                Durum = sayim.Durum ?? "Tamamlandı"
                            };

                            System.Diagnostics.Debug.WriteLine($"Rapor oluşturuldu - ToplamUrun: {rapor.ToplamUrun}, ToplamFark: {rapor.ToplamFark}");

                            TumSayimlar.Add(rapor);
                            Sayimlar.Add(rapor);
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Toplam {Sayimlar.Count} sayım raporu yüklendi");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API'den veri alınamadı. Status: {response.StatusCode}");
                    // API'den veri alınamazsa test verileri yükle
                    TestVerileriYukle();
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda test verileri yükle
                System.Diagnostics.Debug.WriteLine($"Sayım verileri yüklenirken hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                TestVerileriYukle();
            }
        }

        private void TestVerileriYukle()
        {
            var testVeriler = new List<SayimRapor>
            {
                new SayimRapor
                {
                    Id = 1,
                    Tarih = DateTime.Now.AddDays(-1),
                    Kullanici = "Admin",
                    ToplamUrun = 45,
                    ToplamFark = -3,
                    Durum = "Tamamlandı"
                },
                new SayimRapor
                {
                    Id = 2,
                    Tarih = DateTime.Now.AddDays(-3),
                    Kullanici = "Kullanıcı 1",
                    ToplamUrun = 120,
                    ToplamFark = 8,
                    Durum = "Tamamlandı"
                },
                new SayimRapor
                {
                    Id = 3,
                    Tarih = DateTime.Now.AddDays(-7),
                    Kullanici = "Admin",
                    ToplamUrun = 89,
                    ToplamFark = -12,
                    Durum = "Tamamlandı"
                },
                new SayimRapor
                {
                    Id = 4,
                    Tarih = DateTime.Now.AddDays(-15),
                    Kullanici = "Kullanıcı 2",
                    ToplamUrun = 67,
                    ToplamFark = 5,
                    Durum = "Tamamlandı"
                },
                new SayimRapor
                {
                    Id = 5,
                    Tarih = DateTime.Now.AddDays(-20),
                    Kullanici = "Admin",
                    ToplamUrun = 134,
                    ToplamFark = -7,
                    Durum = "Tamamlandı"
                }
            };

            foreach (var veri in testVeriler)
            {
                TumSayimlar.Add(veri);
                Sayimlar.Add(veri);
            }
        }

        private void TarihFiltresi_Changed(object sender, EventArgs e)
        {
            if (TarihFiltresi.SelectedIndex < 0) return;

            var secim = TarihFiltresi.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(secim)) return;

            // Özel tarih alanını göster/gizle
            if (secim == "Özel Tarih Aralığı")
            {
                OzelTarihAlani.IsVisible = true;
                TemizleButonu.IsVisible = true;
                // Özel tarih seçildiğinde henüz filtreleme yapma, kullanıcı Filtrele'ye bassın
                return;
            }
            else
            {
                OzelTarihAlani.IsVisible = false;
                TemizleButonu.IsVisible = true;
            }

            FiltrelemeYap(secim);
        }

        private void OzelTarih_Selected(object sender, DateChangedEventArgs e)
        {
            // DatePicker değiştiğinde otomatik filtreleme yapma
            // Kullanıcı Filtrele butonuna bassın
        }

        private void Filtrele_Clicked(object sender, EventArgs e)
        {
            FiltrelemeYap("Özel Tarih Aralığı");
        }

        private void Temizle_Clicked(object sender, EventArgs e)
        {
            TarihFiltresi.SelectedIndex = 4; // "Tüm Zamanlar"
            TemizleButonu.IsVisible = false;
            OzelTarihAlani.IsVisible = false;
            FiltrelemeYap("Tüm Zamanlar");
        }

        private void FiltrelemeYap(string secim)
        {
            Sayimlar.Clear();
            IEnumerable<SayimRapor> filtreliListe = TumSayimlar;

            switch (secim)
            {
                case "Bugün":
                    filtreliListe = TumSayimlar.Where(s => s.Tarih.Date == DateTime.Today);
                    break;

                case "Son 7 Gün":
                    filtreliListe = TumSayimlar.Where(s => s.Tarih >= DateTime.Now.AddDays(-7));
                    break;

                case "Son 30 Gün":
                    filtreliListe = TumSayimlar.Where(s => s.Tarih >= DateTime.Now.AddDays(-30));
                    break;

                case "Bu Ay":
                    filtreliListe = TumSayimlar.Where(s =>
                        s.Tarih.Month == DateTime.Now.Month &&
                        s.Tarih.Year == DateTime.Now.Year);
                    break;

                case "Özel Tarih Aralığı":
                    var baslangic = BaslangicTarihi.Date.Date;
                    var bitis = BitisTarihi.Date.Date.AddDays(1).AddSeconds(-1);
                    filtreliListe = TumSayimlar.Where(s =>
                        s.Tarih >= baslangic &&
                        s.Tarih <= bitis);
                    break;

                case "Tüm Zamanlar":
                default:
                    filtreliListe = TumSayimlar;
                    TemizleButonu.IsVisible = false;
                    break;
            }

            foreach (var item in filtreliListe.OrderByDescending(x => x.Tarih))
            {
                Sayimlar.Add(item);
            }
        }

        private async void Sayim_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not SayimRapor rapor) return;

            var detayContent = new StackLayout
            {
                Padding = 20,
                Spacing = 15,
                BackgroundColor = Colors.White
            };

            detayContent.Children.Add(new Label
            {
                Text = $"Sayım Tarihi: {rapor.Tarih:dd MMMM yyyy - HH:mm}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black
            });

            detayContent.Children.Add(new Label
            {
                Text = $"Kullanıcı: {rapor.Kullanici}",
                FontSize = 15,
                TextColor = Color.FromArgb("#666")
            });

            detayContent.Children.Add(new Label
            {
                Text = $"Toplam Ürün: {rapor.ToplamUrun}",
                FontSize = 15,
                TextColor = Color.FromArgb("#666")
            });

            detayContent.Children.Add(new Label
            {
                Text = $"Toplam Fark: {rapor.ToplamFark}",
                FontSize = 15,
                TextColor = rapor.ToplamFark < 0 ? Colors.Red : Colors.Green,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            });

            detayContent.Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#E0E0E0")
            });

            detayContent.Children.Add(new Label
            {
                Text = "Sayılan Ürünler:",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 10, 0, 10)
            });

            for (int i = 1; i <= 5; i++)
            {
                var border = new Border
                {
                    Stroke = Color.FromArgb("#E0E0E0"),
                    StrokeThickness = 1,
                    Padding = 12,
                    Margin = new Thickness(0, 0, 0, 10),
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 6 }
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };

                var urunAdi = new Label
                {
                    Text = $"Ürün {i} - Test Ürünü",
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black
                };
                Grid.SetColumn(urunAdi, 0);
                Grid.SetRow(urunAdi, 0);
                grid.Children.Add(urunAdi);

                var stokBilgi = new Label
                {
                    Text = $"Sistem: 50 • Sayılan: {48 + i}",
                    FontSize = 13,
                    TextColor = Color.FromArgb("#666"),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                Grid.SetColumn(stokBilgi, 0);
                Grid.SetRow(stokBilgi, 1);
                grid.Children.Add(stokBilgi);

                var farkDegeri = i % 2 == 0 ? i : -i;
                var fark = new Label
                {
                    Text = $"Fark: {(farkDegeri > 0 ? "+" : "")}{farkDegeri}",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = farkDegeri < 0 ? Colors.Red : Colors.Green,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(fark, 1);
                Grid.SetRowSpan(fark, 2);
                grid.Children.Add(fark);

                border.Content = grid;
                detayContent.Children.Add(border);
            }

            var scrollView = new ScrollView { Content = detayContent };

            await Navigation.PushAsync(new ContentPage
            {
                Title = "Sayım Detayı",
                Content = scrollView,
                BackgroundColor = Color.FromArgb("#F5F5F5")
            });
        }

        private async void ExcelIndir_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is SayimRapor rapor)
            {
                await DisplayAlert(
                    "Excel İndir",
                    $"📊 {rapor.Tarih:dd/MM/yyyy HH:mm} tarihli sayım raporu Excel formatında indirilecek.\n\n" +
                    $"Toplam Ürün: {rapor.ToplamUrun}\n" +
                    $"Toplam Fark: {rapor.ToplamFark}",
                    "Tamam");
            }
        }

        private async void PdfIndir_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is SayimRapor rapor)
            {
                await DisplayAlert(
                    "PDF İndir",
                    $"📄 {rapor.Tarih:dd/MM/yyyy HH:mm} tarihli sayım raporu PDF formatında indirilecek.\n\n" +
                    $"Toplam Ürün: {rapor.ToplamUrun}\n" +
                    $"Toplam Fark: {rapor.ToplamFark}",
                    "Tamam");
            }
        }
    }

    public class SayimRapor
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string? Kullanici { get; set; }
        public int ToplamUrun { get; set; }
        public int ToplamFark { get; set; }
        public string? Durum { get; set; }
    }
}
