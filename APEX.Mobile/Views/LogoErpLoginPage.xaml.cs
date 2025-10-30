using APEX.Mobile.Models;
using APEX.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace APEX.Mobile.Views;

public partial class LogoErpLoginPage : ContentPage
{
    private readonly LogoErpApiService _logoErpApiService;
    private readonly ILogger<LogoErpLoginPage> _logger;
    private readonly LogoErpBaglantiModel _baglantiModel;

    public LogoErpLoginPage(LogoErpApiService logoErpApiService, ILogger<LogoErpLoginPage> logger)
    {
        InitializeComponent();
        _logoErpApiService = logoErpApiService;
        _logger = logger;
        _baglantiModel = new LogoErpBaglantiModel();
        BindingContext = _baglantiModel;
        
        // Sayfa yüklendiğinde bağlantı durumunu kontrol et
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await CheckConnectionStatus();
        LoadSavedCredentials();
    }

    private async Task CheckConnectionStatus()
    {
        try
        {
            var isConnected = await _logoErpApiService.BaglantiTestiAsync();
            UpdateConnectionStatus(isConnected);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bağlantı durumu kontrol edilirken hata oluştu");
            UpdateConnectionStatus(false);
        }
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (isConnected)
            {
                ConnectionStatusIcon.TextColor = Colors.Green;
                ConnectionStatusLabel.Text = "Logo ERP bağlantısı başarılı";
            }
            else
            {
                ConnectionStatusIcon.TextColor = Colors.Red;
                ConnectionStatusLabel.Text = "Logo ERP bağlantısı kurulamadı";
            }
        });
    }

    private void LoadSavedCredentials()
    {
        try
        {
            // Kaydedilmiş kullanıcı bilgilerini yükle
            var savedUsername = Preferences.Get("LogoErp_Username", string.Empty);
            var savedCompanyCode = Preferences.Get("LogoErp_CompanyCode", string.Empty);
            var rememberMe = Preferences.Get("LogoErp_RememberMe", false);

            if (rememberMe && !string.IsNullOrEmpty(savedUsername))
            {
                _baglantiModel.KullaniciAdi = savedUsername;
                _baglantiModel.SirketKodu = savedCompanyCode;
                _baglantiModel.BeniHatirla = rememberMe;
                
                UsernameEntry.Text = savedUsername;
                CompanyCodeEntry.Text = savedCompanyCode;
                RememberMeCheckBox.IsChecked = rememberMe;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kaydedilmiş kullanıcı bilgileri yüklenirken hata oluştu");
        }
    }

    private async void OnTestConnectionClicked(object sender, EventArgs e)
    {
        await TestConnection();
    }

    private async Task TestConnection()
    {
        try
        {
            ShowLoading(true, "Bağlantı test ediliyor...");
            HideError();

            var isConnected = await _logoErpApiService.BaglantiTestiAsync();
            UpdateConnectionStatus(isConnected);

            if (isConnected)
            {
                await DisplayAlert("Başarılı", "Logo ERP bağlantısı başarılı!", "Tamam");
            }
            else
            {
                await DisplayAlert("Hata", "Logo ERP bağlantısı kurulamadı. Lütfen API servisinin çalıştığından emin olun.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bağlantı testi sırasında hata oluştu");
            await DisplayAlert("Hata", "Bağlantı testi sırasında bir hata oluştu: " + ex.Message, "Tamam");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await PerformLogin();
    }

    private async Task PerformLogin()
    {
        try
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(_baglantiModel.KullaniciAdi))
            {
                ShowError("Kullanıcı adı gereklidir.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_baglantiModel.Sifre))
            {
                ShowError("Şifre gereklidir.");
                return;
            }

            ShowLoading(true, "Giriş yapılıyor...");
            HideError();

            // Giriş işlemi
            var token = await _logoErpApiService.GirisYapAsync(
                _baglantiModel.KullaniciAdi, 
                _baglantiModel.Sifre, 
                _baglantiModel.SirketKodu);

            if (!string.IsNullOrEmpty(token))
            {
                // Başarılı giriş
                SaveCredentials();
                await DisplayAlert("Başarılı", "Logo ERP'ye başarıyla giriş yapıldı!", "Tamam");
                
                // Dashboard sayfasına yönlendir
                await Shell.Current.GoToAsync("//LogoErpDashboard");
            }
            else
            {
                ShowError("Giriş başarısız. Kullanıcı adı ve şifrenizi kontrol ediniz.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Giriş işlemi sırasında hata oluştu");
            ShowError("Giriş işlemi sırasında bir hata oluştu: " + ex.Message);
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void SaveCredentials()
    {
        try
        {
            if (_baglantiModel.BeniHatirla)
            {
                Preferences.Set("LogoErp_Username", _baglantiModel.KullaniciAdi);
                Preferences.Set("LogoErp_CompanyCode", _baglantiModel.SirketKodu ?? string.Empty);
                Preferences.Set("LogoErp_RememberMe", true);
            }
            else
            {
                // Beni hatırla seçili değilse kaydedilmiş bilgileri temizle
                Preferences.Remove("LogoErp_Username");
                Preferences.Remove("LogoErp_CompanyCode");
                Preferences.Remove("LogoErp_RememberMe");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı bilgileri kaydedilirken hata oluştu");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
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
            
            // Form kontrollerini devre dışı bırak
            UsernameEntry.IsEnabled = !show;
            PasswordEntry.IsEnabled = !show;
            CompanyCodeEntry.IsEnabled = !show;
            RememberMeCheckBox.IsEnabled = !show;
            TestConnectionButton.IsEnabled = !show;
            LoginButton.IsEnabled = !show;
            CancelButton.IsEnabled = !show;
        });
    }

    private void ShowError(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.IsVisible = true;
        });
    }

    private void HideError()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ErrorMessageLabel.IsVisible = false;
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Şifreyi temizle (güvenlik için)
        if (!_baglantiModel.BeniHatirla)
        {
            PasswordEntry.Text = string.Empty;
            _baglantiModel.Sifre = string.Empty;
        }
    }
}