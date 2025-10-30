using Microsoft.Extensions.Logging;
using APEX.Mobile.Interfaces;
using APEX.Mobile.Views;
using APEX.Mobile.Services;
using Microsoft.Extensions.Http;
using BarcodeScanning;

#if ANDROID
using APEX.Mobile.Platforms.Android;
#elif WINDOWS
using APEX.Mobile.Platforms.Windows;
#endif

namespace APEX.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeScanning()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

#if DEBUG
            builder.Services.AddLogging(logging =>
            {
                logging.AddDebug();
            });
#endif

            // Register App
            builder.Services.AddSingleton<App>();
            
            // Register Logo ERP API Service
            builder.Services.AddHttpClient<LogoErpApiService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5105/"); // API base URL
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddSingleton<LogoErpApiService>();
            
            // Register pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Views.SayimEkrani>();
            builder.Services.AddTransient<Views.BarcodeScannerPage>();
            
            // Register Logo ERP pages
            builder.Services.AddTransient<LogoErpLoginPage>();
            builder.Services.AddTransient<LogoErpDashboardPage>();
            builder.Services.AddTransient<LogoErpProductListPage>();
            builder.Services.AddTransient<LogoErpStockPage>();

            // Register platform-specific services
#if ANDROID
            builder.Services.AddSingleton<IBarcodeScanner, BarcodeScanner>();
#elif WINDOWS
            builder.Services.AddSingleton<IBarcodeScanner, BarcodeScanner>();
#endif

            return builder.Build();
        }
    }
}
