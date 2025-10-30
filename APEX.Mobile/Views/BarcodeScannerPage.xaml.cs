using BarcodeScanning;
using Microsoft.Maui.Controls;
using System.Linq;

namespace APEX.Mobile.Views
{
    public partial class BarcodeScannerPage : ContentPage
    {
        private TaskCompletionSource<string?> _scanCompletionSource = null!;

        public BarcodeScannerPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Request camera permissions
            await BarcodeScanning.Methods.AskForRequiredPermissionAsync();
            
            // Enable camera
            if (BarcodeScanner != null)
            {
                BarcodeScanner.CameraEnabled = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Disable camera
            if (BarcodeScanner != null)
            {
                BarcodeScanner.CameraEnabled = false;
            }
        }

        private async void CameraView_OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
        {
            if (e.BarcodeResults != null && e.BarcodeResults.Length > 0)
            {
                if (BarcodeScanner != null)
                {
                    BarcodeScanner.PauseScanning = true;
                }
                
                var firstBarcode = e.BarcodeResults.First();
                var barcodeValue = firstBarcode.RawValue;

                if (Application.Current?.Dispatcher != null)
                {
                    await Application.Current.Dispatcher.DispatchAsync(() =>
                    {
                        _scanCompletionSource?.SetResult(barcodeValue);
                    });
                }
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            if (Application.Current?.Dispatcher != null)
            {
                await Application.Current.Dispatcher.DispatchAsync(() =>
                {
                    _scanCompletionSource?.SetResult(null);
                });
            }
        }

        private async void ManualEntryButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync(
                "Manuel Barkod Girişi",
                "Barkod numarasını girin:",
                "Tamam",
                "İptal",
                "Barkod numarası...",
                keyboard: Keyboard.Numeric);

            if (Application.Current?.Dispatcher != null)
            {
                await Application.Current.Dispatcher.DispatchAsync(() =>
                {
                    _scanCompletionSource?.SetResult(result);
                });
            }
        }

        public async Task<string?> ScanAsync()
        {
            _scanCompletionSource = new TaskCompletionSource<string?>();
            
            // Resume scanning if paused
            if (BarcodeScanner != null)
            {
                BarcodeScanner.PauseScanning = false;
            }
            
            try
            {
                var result = await _scanCompletionSource.Task;
                
                // Navigate back
                await Shell.Current.GoToAsync("..");
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Barcode scanning error: {ex.Message}");
                await Shell.Current.GoToAsync("..");
                return null;
            }
        }
    }
}
