using APEX.Mobile.Models;
using APEX.Mobile.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace APEX.Mobile.Views;

public partial class LogoErpProductListPage : ContentPage
{
    private readonly LogoErpApiService _logoErpApiService;
    private readonly ILogger<LogoErpProductListPage> _logger;
    private readonly ObservableCollection<ProductDisplayModel> _products;
    private readonly List<LogoUrunModel> _allProducts;
    private string _currentSearchText = string.Empty;
    private string _currentFilter = string.Empty;
    private int _currentPage = 1;
    private const int PageSize = 50;
    private bool _isLoading = false;

    public LogoErpProductListPage(LogoErpApiService logoErpApiService, ILogger<LogoErpProductListPage> logger)
    {
        InitializeComponent();
        _logoErpApiService = logoErpApiService;
        _logger = logger;
        _products = [];
        _allProducts = [];
        
        ProductsCollectionView.ItemsSource = _products;
        
        // Sayfa yüklendiğinde ürünleri getir
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadProducts();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadProducts(true);
        RefreshView.IsRefreshing = false;
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadProducts(true);
    }

    private async Task LoadProducts(bool forceRefresh = false)
    {
        if (_isLoading) return;

        try
        {
            _isLoading = true;
            ShowLoading(true, "Ürünler yükleniyor...");

            if (forceRefresh)
            {
                _currentPage = 1;
                _allProducts.Clear();
                _products.Clear();
            }

            var products = await _logoErpApiService.UrunListesiGetirAsync(_currentPage, PageSize);
            
            if (products != null && products.Count > 0)
            {
                _allProducts.AddRange(products);
                await UpdateProductDisplay();
                _currentPage++;
            }
            else if (_allProducts.Count == 0)
            {
                EmptyViewLabel.Text = "Henüz ürün bulunamadı";
                RetryButton.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ürünler yüklenirken hata oluştu");
            
            if (_allProducts.Count == 0)
            {
                EmptyViewLabel.Text = "Ürünler yüklenirken hata oluştu";
                RetryButton.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Hata", "Ürünler yüklenirken bir hata oluştu: " + ex.Message, "Tamam");
            }
        }
        finally
        {
            _isLoading = false;
            ShowLoading(false);
        }
    }

    private Task UpdateProductDisplay()
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

        // Kategori filtresi uygula
        if (!string.IsNullOrWhiteSpace(_currentFilter))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.KategoriAdi?.Contains(_currentFilter, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Display modellerine dönüştür
        var displayModels = filteredProducts.Select(p => new ProductDisplayModel
        {
            MalzemeKodu = p.MalzemeKodu,
            MalzemeAdi = p.MalzemeAdi,
            Barkod = p.Barkod,
            Birim = p.Birim ?? "Adet",
            SatisFiyati = p.SatisFiyati,
            KategoriAdi = p.KategoriAdi,
            GuncelStok = p.GuncelStok,
            Aktif = p.Aktif,
            HasBarcode = !string.IsNullOrEmpty(p.Barkod),
            HasCategory = !string.IsNullOrEmpty(p.KategoriAdi),
            HasSalesPrice = p.SatisFiyati.HasValue && p.SatisFiyati > 0,
            HasStock = p.GuncelStok.HasValue,
            FormattedSalesPrice = p.SatisFiyati.HasValue ? $"₺{p.SatisFiyati:N2}" : "",
            FormattedStock = p.GuncelStok?.ToString("N2") ?? "0",
            StockColor = GetStockColor(p.GuncelStok, p.MinimumStok),
            StatusText = p.Aktif ? "Aktif" : "Pasif",
            StatusColor = p.Aktif ? Colors.Green : Colors.Red,
            OriginalProduct = p
        }).ToList();

        // UI'ı güncelle
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _products.Clear();
            foreach (var product in displayModels)
            {
                _products.Add(product);
            }
        });
        
        return Task.CompletedTask;
    }

    private static Color GetStockColor(decimal? currentStock, decimal? minimumStock)
    {
        if (!currentStock.HasValue) return Colors.Gray;
        
        if (minimumStock.HasValue && currentStock <= minimumStock)
            return Colors.Red;
        
        if (currentStock <= 0)
            return Colors.Red;
        
        if (minimumStock.HasValue && currentStock <= minimumStock * 1.5m)
            return Colors.Orange;
        
        return Colors.Green;
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _currentSearchText = e.NewTextValue ?? string.Empty;
        
        // Debounce search
        await Task.Delay(500);
        if (_currentSearchText == (e.NewTextValue ?? string.Empty))
        {
            await UpdateProductDisplay();
        }
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is ProductDisplayModel selectedProduct)
        {
            // Seçimi temizle
            ProductsCollectionView.SelectedItem = null;
            
            // Ürün detay sayfasına git
            await Shell.Current.GoToAsync($"LogoErpProductDetail?productCode={Uri.EscapeDataString(selectedProduct.MalzemeKodu)}");
        }
    }

    private async void OnFilterClicked(object sender, EventArgs e)
    {
        try
        {
            // Kategori listesini getir
            var categories = await _logoErpApiService.UrunKategorileriGetirAsync();
            
            if (categories != null && categories.Count > 0)
            {
                var categoryNames = categories.Select(c => c.KategoriAdi).ToArray();
                var selectedCategory = await DisplayActionSheet("Kategori Seçin", "İptal", "Tümü", categoryNames);
                
                if (selectedCategory != null && selectedCategory != "İptal")
                {
                    if (selectedCategory == "Tümü")
                    {
                        _currentFilter = string.Empty;
                        FilterInfoLayout.IsVisible = false;
                    }
                    else
                    {
                        _currentFilter = selectedCategory;
                        FilterInfoLabel.Text = selectedCategory;
                        FilterInfoLayout.IsVisible = true;
                    }
                    
                    await UpdateProductDisplay();
                }
            }
            else
            {
                await DisplayAlert("Bilgi", "Kategori bulunamadı", "Tamam");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori filtresi yüklenirken hata oluştu");
            await DisplayAlert("Hata", "Kategori filtresi yüklenirken hata oluştu", "Tamam");
        }
    }

    private async void OnClearFilterClicked(object sender, EventArgs e)
    {
        _currentFilter = string.Empty;
        FilterInfoLayout.IsVisible = false;
        await UpdateProductDisplay();
    }

    private async void OnRetryClicked(object sender, EventArgs e)
    {
        RetryButton.IsVisible = false;
        await LoadProducts(true);
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

// Display model for products
public class ProductDisplayModel
{
    public string MalzemeKodu { get; set; } = string.Empty;
    public string MalzemeAdi { get; set; } = string.Empty;
    public string? Barkod { get; set; }
    public string Birim { get; set; } = string.Empty;
    public decimal? SatisFiyati { get; set; }
    public string? KategoriAdi { get; set; }
    public decimal? GuncelStok { get; set; }
    public bool Aktif { get; set; }
    
    // Display properties
    public bool HasBarcode { get; set; }
    public bool HasCategory { get; set; }
    public bool HasSalesPrice { get; set; }
    public bool HasStock { get; set; }
    public string FormattedSalesPrice { get; set; } = string.Empty;
    public string FormattedStock { get; set; } = string.Empty;
    public Color StockColor { get; set; } = Colors.Black;
    public string StatusText { get; set; } = string.Empty;
    public Color StatusColor { get; set; } = Colors.Black;
    
    // Original model reference
    public LogoUrunModel? OriginalProduct { get; set; }
}