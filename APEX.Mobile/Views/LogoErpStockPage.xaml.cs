using APEX.Mobile.Models;
using APEX.Mobile.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace APEX.Mobile.Views;

public partial class LogoErpStockPage : ContentPage
{
    private readonly LogoErpApiService _logoErpApiService;
    private readonly ILogger<LogoErpStockPage> _logger;
    private readonly ObservableCollection<StockDisplayModel> _stockItems;
    private List<LogoUrunModel> _allProducts;
    private string _currentSearchText = string.Empty;
    private string _currentFilter = string.Empty;
    private bool _isLoading = false;

    public LogoErpStockPage(LogoErpApiService logoErpApiService, ILogger<LogoErpStockPage> logger)
    {
        InitializeComponent();
        _logoErpApiService = logoErpApiService;
        _logger = logger;
        _stockItems = [];
        _allProducts = [];
        
        StockCollectionView.ItemsSource = _stockItems;
        
        // Sayfa yüklendiğinde stok bilgilerini getir
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadStockData();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadStockData(true);
        RefreshView.IsRefreshing = false;
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadStockData(true);
    }

    private async Task LoadStockData(bool forceRefresh = false)
    {
        if (_isLoading) return;

        try
        {
            _isLoading = true;
            ShowLoading(true, "Stok bilgileri yükleniyor...");

            if (forceRefresh)
            {
                _allProducts.Clear();
                _stockItems.Clear();
            }

            // Ürün listesini ve stok bilgilerini getir
            var products = await _logoErpApiService.UrunListesiGetirAsync(1, 1000); // Tüm ürünleri getir
            
            if (products != null && products.Count > 0)
            {
                _allProducts = [.. products];
                await UpdateStockDisplay();
                UpdateStockSummary();
            }
            else
            {
                EmptyViewLabel.Text = "Stok bilgisi bulunamadı";
                RetryButton.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stok bilgileri yüklenirken hata oluştu");
            
            if (_stockItems.Count == 0)
            {
                EmptyViewLabel.Text = "Stok bilgileri yüklenirken hata oluştu";
                RetryButton.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Hata", "Stok bilgileri yüklenirken bir hata oluştu: " + ex.Message, "Tamam");
            }
        }
        finally
        {
            _isLoading = false;
            ShowLoading(false);
        }
    }

    private async Task UpdateStockDisplay()
    {
        var filteredProducts = _allProducts.AsEnumerable();

        // Arama filtresi uygula
        if (!string.IsNullOrWhiteSpace(_currentSearchText))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.MalzemeAdi.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) ||
                p.MalzemeKodu.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(p.Barkod) && p.Barkod.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase)));
        }

        // Stok durumu filtresi uygula
        if (!string.IsNullOrWhiteSpace(_currentFilter))
        {
            filteredProducts = _currentFilter switch
            {
                "Düşük Stok" => filteredProducts.Where(p => p.GuncelStok.HasValue && p.MinimumStok.HasValue && p.GuncelStok <= p.MinimumStok),
                "Stokta Yok" => filteredProducts.Where(p => !p.GuncelStok.HasValue || p.GuncelStok <= 0),
                "Normal Stok" => filteredProducts.Where(p => p.GuncelStok.HasValue && p.GuncelStok > 0 && (!p.MinimumStok.HasValue || p.GuncelStok > p.MinimumStok)),
                _ => filteredProducts
            };
        }

        // Display modellerine dönüştür
        var displayModels = filteredProducts.Select(p => new StockDisplayModel
        {
            MalzemeKodu = p.MalzemeKodu,
            MalzemeAdi = p.MalzemeAdi,
            Barkod = p.Barkod,
            GuncelStok = p.GuncelStok,
            MinimumStok = p.MinimumStok,
            Birim = p.Birim ?? "Adet",
            HasBarcode = !string.IsNullOrEmpty(p.Barkod),
            HasMinimumStock = p.MinimumStok.HasValue,
            FormattedCurrentStock = $"{p.GuncelStok?.ToString("N2") ?? "0"} {p.Birim ?? "Adet"}",
            FormattedMinimumStock = p.MinimumStok?.ToString("N2") ?? "",
            StockColor = GetStockColor(p.GuncelStok, p.MinimumStok),
            StockStatusIcon = GetStockStatusIcon(p.GuncelStok, p.MinimumStok),
            HasLastMovement = false, // Bu bilgi stok hareket tablosundan gelecek
            LastMovementDate = "",
            OriginalProduct = p
        }).OrderBy(p => p.StockColor == Colors.Red ? 0 : p.StockColor == Colors.Orange ? 1 : 2)
          .ThenBy(p => p.MalzemeAdi)
          .ToList();

        // Son hareket tarihlerini getir (opsiyonel)
        await LoadLastMovementDates(displayModels);

        // UI'ı güncelle
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _stockItems.Clear();
            foreach (var item in displayModels)
            {
                _stockItems.Add(item);
            }
        });
    }

    private async Task LoadLastMovementDates(List<StockDisplayModel> stockItems)
    {
        try
        {
            // Her ürün için son hareket tarihini getir (performans için sadece ilk 50 ürün)
            var itemsToProcess = stockItems.Take(50).ToList();
            
            foreach (var item in itemsToProcess)
            {
                try
                {
                    var movements = await _logoErpApiService.StokHareketleriGetirAsync(
                        item.MalzemeKodu, 
                        DateTime.Now.AddMonths(-1), 
                        DateTime.Now);
                    
                    if (movements != null && movements.Count > 0)
                    {
                        var lastMovement = movements.First();
                        item.HasLastMovement = true;
                        item.LastMovementDate = lastMovement.HareketTarihi.ToString("dd.MM.yyyy");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ürün {ProductCode} için son hareket tarihi alınamadı", item.MalzemeKodu);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Son hareket tarihleri yüklenirken hata oluştu");
        }
    }

    private void UpdateStockSummary()
    {
        try
        {
            var totalProducts = _allProducts.Count;
            var lowStockCount = _allProducts.Count(p => p.GuncelStok.HasValue && p.MinimumStok.HasValue && p.GuncelStok <= p.MinimumStok);
            var outOfStockCount = _allProducts.Count(p => !p.GuncelStok.HasValue || p.GuncelStok <= 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TotalProductsLabel.Text = totalProducts.ToString();
                LowStockLabel.Text = lowStockCount.ToString();
                OutOfStockLabel.Text = outOfStockCount.ToString();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stok özeti güncellenirken hata oluştu");
        }
    }

    private static Color GetStockColor(decimal? currentStock, decimal? minimumStock)
    {
        if (!currentStock.HasValue || currentStock <= 0)
            return Colors.Red;
        
        if (minimumStock.HasValue && currentStock <= minimumStock)
            return Colors.Red;
        
        if (minimumStock.HasValue && currentStock <= minimumStock * 1.5m)
            return Colors.Orange;
        
        return Colors.Green;
    }

    private static string GetStockStatusIcon(decimal? currentStock, decimal? minimumStock)
    {
        if (!currentStock.HasValue || currentStock <= 0)
            return "❌"; // Stokta yok
        
        if (minimumStock.HasValue && currentStock <= minimumStock)
            return "⚠️"; // Düşük stok
        
        if (minimumStock.HasValue && currentStock <= minimumStock * 1.5m)
            return "⚡"; // Dikkat
        
        return "✅"; // Normal stok
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _currentSearchText = e.NewTextValue ?? string.Empty;
        
        // Debounce search
        await Task.Delay(500);
        if (_currentSearchText == (e.NewTextValue ?? string.Empty))
        {
            await UpdateStockDisplay();
        }
    }

    private async void OnStockItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is StockDisplayModel selectedItem)
        {
            // Seçimi temizle
            StockCollectionView.SelectedItem = null;
            
            // Stok hareket detaylarına git
            await Shell.Current.GoToAsync($"LogoErpStockMovements?productCode={Uri.EscapeDataString(selectedItem.MalzemeKodu)}");
        }
    }

    private async void OnFilterClicked(object sender, EventArgs e)
    {
        var filterOptions = new[] { "Tümü", "Düşük Stok", "Stokta Yok", "Normal Stok" };
        var selectedFilter = await DisplayActionSheet("Stok Durumu Filtresi", "İptal", null, filterOptions);
        
        if (selectedFilter != null && selectedFilter != "İptal")
        {
            if (selectedFilter == "Tümü")
            {
                _currentFilter = string.Empty;
                FilterInfoLayout.IsVisible = false;
            }
            else
            {
                _currentFilter = selectedFilter;
                FilterInfoLabel.Text = selectedFilter;
                FilterInfoLayout.IsVisible = true;
            }
            
            await UpdateStockDisplay();
        }
    }

    private async void OnClearFilterClicked(object sender, EventArgs e)
    {
        _currentFilter = string.Empty;
        FilterInfoLayout.IsVisible = false;
        await UpdateStockDisplay();
    }

    private async void OnRetryClicked(object sender, EventArgs e)
    {
        RetryButton.IsVisible = false;
        await LoadStockData(true);
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
        
        // Eğer oturum kapalıysa login sayfasına yönlendir
        if (!_logoErpApiService.IsAuthenticated)
        {
            Shell.Current.GoToAsync("//LogoErpLogin");
        }
    }
}

// Display model for stock items
public class StockDisplayModel
{
    public string MalzemeKodu { get; set; } = string.Empty;
    public string MalzemeAdi { get; set; } = string.Empty;
    public string? Barkod { get; set; }
    public decimal? GuncelStok { get; set; }
    public decimal? MinimumStok { get; set; }
    public string Birim { get; set; } = string.Empty;
    
    // Display properties
    public bool HasBarcode { get; set; }
    public bool HasMinimumStock { get; set; }
    public bool HasLastMovement { get; set; }
    public string FormattedCurrentStock { get; set; } = string.Empty;
    public string FormattedMinimumStock { get; set; } = string.Empty;
    public Color StockColor { get; set; } = Colors.Black;
    public string StockStatusIcon { get; set; } = string.Empty;
    public string LastMovementDate { get; set; } = string.Empty;
    
    // Original model reference
    public LogoUrunModel? OriginalProduct { get; set; }
}