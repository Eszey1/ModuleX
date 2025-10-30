using System;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using DevExpress.Xpf.Core;

namespace APEX.Desktop
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // DevExpress teması uygula (örnek: "Office2019Colorful")
                try
                {
                    ThemeManager.SetTheme(Application.Current, "Office2019Colorful");
                    System.Diagnostics.Debug.WriteLine("DevExpress theme applied: Office2019Colorful");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Tema yükleme hatası: {ex.Message}");
                }

                var main = new DevDashboard();
                main.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Uygulama başlatma hatası: {ex}");
                MessageBox.Show($"Uygulama başlatılırken hata oluştu:\n{ex.Message}",
                    "Başlatma Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e.Exception}");
            // production için buraya log ekleyin ve gerektiğinde e.Handled = true;
        }
    }
}