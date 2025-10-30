using APEX.Mobile.Models;
using APEX.Mobile.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace APEX.Mobile.Views;

public partial class LogoErpDashboardPage : ContentPage
{
    private readonly LogoErpApiService _logoErpApiService;
    private readonly ILogger<LogoErpDashboardPage> _logger;
    private readonly LogoErpDashboardModel _dashboardModel;
    private readonly ObservableCollection<SyncLogDisplayModel> _recentLogs;

    public LogoErpDashboardPage(LogoErpApiService logoErpApiService, ILogger<LogoErpDashboardPage> logger)
    {
        InitializeComponent();
        _logoErpApiService = logoErpApiService;
        _logger = logger;
        _dashboardModel = new LogoErpDashboardModel();
        _recentLogs = [];
        
        RecentLogsCollectionView.ItemsSource = _recentLogs;
        
        // Sayfa yüklendiğinde verileri getir
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadDashboardData();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadDashboardData();
        RefreshView.IsRefreshing = false;
    }

    private async Task LoadDashboardData()
    {
        try
        {
            ShowLoading(true, "Dashboard verileri yükleniyor...");

            // Paralel olarak tüm verileri getir
            var tasks = new List<Task>
            {
                LoadStatistics(),
                LoadSyncInfo(),
                LoadRecentLogs()
            };

            await Task.WhenAll(tasks);
            
            UpdateUI();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard verileri yüklenirken hata oluştu");
            await DisplayAlert("Hata", "Dashboard verileri yüklenirken bir hata oluştu: " + ex.Message, "Tamam");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async Task LoadStatistics()
    {
        try
        {
            // Ürün sayısını getir
            var products = await _logoErpApiService.UrunListesiGetirAsync(1, 1);
            _dashboardModel.ToplamUrunSayisi = products?.Count ?? 0;

            // Cari hesap sayısını getir
            var customers = await _logoErpApiService.CariHesapListesiGetirAsync(1, 1);
            _dashboardModel.ToplamCariSayisi = customers?.Count ?? 0;

            // Depo sayısını getir
            var warehouses = await _logoErpApiService.DepoListesiGetirAsync();
            _dashboardModel.ToplamDepoSayisi = warehouses?.Count ?? 0;

            // Bugünkü satış raporunu getir
            var today = DateTime.Today;
            var salesReport = await _logoErpApiService.SatisRaporuGetirAsync(today, today);
            _dashboardModel.BugunkuSatisTutari = salesReport?.Sum(s => s.SatisTutari) ?? 0;
            _dashboardModel.BugunkuFaturaSayisi = salesReport?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İstatistikler yüklenirken hata oluştu");
        }
    }

    private async Task LoadSyncInfo()
    {
        try
        {
            var lastSyncDate = await _logoErpApiService.SonSenkronizasyonTarihiGetirAsync();
            _dashboardModel.SonSenkronizasyonTarihi = lastSyncDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Senkronizasyon bilgileri yüklenirken hata oluştu");
        }
    }

    private Task LoadRecentLogs()
    {
        try
        {
            // Mock data - gerçek implementasyonda API'den gelecek
            _recentLogs.Clear();
            
            var mockLogs = new List<SyncLogDisplayModel>
            {
                new()
                {
                    Tip = "Ürün Senkronizasyonu",
                    BaslangicZamani = DateTime.Now.AddMinutes(-30),
                    Basarili = true,
                    StatusIcon = "✅",
                    StatusText = "Başarılı",
                    StatusColor = Colors.Green,
                    FormattedDate = DateTime.Now.AddMinutes(-30).ToString("HH:mm")
                },
                new()
                {
                    Tip = "Cari Hesap Senkronizasyonu",
                    BaslangicZamani = DateTime.Now.AddHours(-2),
                    Basarili = true,
                    StatusIcon = "✅",
                    StatusText = "Başarılı",
                    StatusColor = Colors.Green,
                    FormattedDate = DateTime.Now.AddHours(-2).ToString("HH:mm")
                },
                new()
                {
                    Tip = "Stok Senkronizasyonu",
                    BaslangicZamani = DateTime.Now.AddHours(-4),
                    Basarili = false,
                    StatusIcon = "❌",
                    StatusText = "Hatalı",
                    StatusColor = Colors.Red,
                    FormattedDate = DateTime.Now.AddHours(-4).ToString("HH:mm")
                }
            };

            foreach (var log in mockLogs)
            {
                _recentLogs.Add(log);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Son işlemler yüklenirken hata oluştu");
        }
        
        return Task.CompletedTask;
    }

    private void UpdateUI()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TotalProductsLabel.Text = _dashboardModel.ToplamUrunSayisi.ToString();
            TotalCustomersLabel.Text = _dashboardModel.ToplamCariSayisi.ToString();
            TotalWarehousesLabel.Text = _dashboardModel.ToplamDepoSayisi.ToString();
            TodaySalesLabel.Text = $"₺{_dashboardModel.BugunkuSatisTutari:N2}";

            if (_dashboardModel.SonSenkronizasyonTarihi.HasValue)
            {
                LastSyncLabel.Text = _dashboardModel.SonSenkronizasyonTarihi.Value.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                LastSyncLabel.Text = "Henüz yapılmadı";
            }

            // Bağlantı durumunu kontrol et
            UpdateConnectionStatus(_logoErpApiService.IsAuthenticated);
        });
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (isConnected)
            {
                ConnectionStatusIcon.TextColor = Colors.LightGreen;
            }
            else
            {
                ConnectionStatusIcon.TextColor = Colors.Red;
            }
        });
    }

    private async void OnProductsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LogoErpProductList");
    }

    private async void OnStockClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LogoErpStock");
    }

    private async void OnCustomersClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LogoErpCustomers");
    }

    private async void OnReportsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LogoErpReports");
    }

    private async void OnSyncClicked(object sender, EventArgs e)
    {
        await PerformSync();
    }

    private async Task PerformSync()
    {
        try
        {
            ShowLoading(true, "Veriler senkronize ediliyor...");

            var result = await _logoErpApiService.VerileriSenkronizeEtAsync();
            
            if (result)
            {
                await DisplayAlert("Başarılı", "Veriler başarıyla senkronize edildi!", "Tamam");
                await LoadDashboardData(); // Dashboard'u yenile
            }
            else
            {
                await DisplayAlert("Hata", "Senkronizasyon işlemi başarısız oldu.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Senkronizasyon işlemi sırasında hata oluştu");
            await DisplayAlert("Hata", "Senkronizasyon işlemi sırasında bir hata oluştu: " + ex.Message, "Tamam");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Çıkış", "Logo ERP oturumunu kapatmak istediğinizden emin misiniz?", "Evet", "Hayır");
        
        if (result)
        {
            _logoErpApiService.CikisYap();
            await Shell.Current.GoToAsync("//LogoErpLogin");
        }
    }

    private void ShowLoading(bool show, string message = "")
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LoadingOverlay.IsVisible = show;
            if (show && !string.IsNullOrEmpty(message))
            {
                LoadingLabel.Text = message;
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Sayfa her göründüğünde bağlantı durumunu kontrol et
        UpdateConnectionStatus(_logoErpApiService.IsAuthenticated);
        
        // Eğer oturum kapalıysa login sayfasına yönlendir
        if (!_logoErpApiService.IsAuthenticated)
        {
            Shell.Current.GoToAsync("//LogoErpLogin");
        }
    }
}

// Display model for sync logs
public class SyncLogDisplayModel
{
    public string Tip { get; set; } = string.Empty;
    public DateTime BaslangicZamani { get; set; }
    public bool Basarili { get; set; }
    public string StatusIcon { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public Color StatusColor { get; set; } = Colors.Black;
    public string FormattedDate { get; set; } = string.Empty;
}