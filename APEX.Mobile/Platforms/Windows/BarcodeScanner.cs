using APEX.Mobile.Interfaces;

namespace APEX.Mobile.Platforms.Windows
{
    public class BarcodeScanner : IBarcodeScanner
    {
        public async Task<string?> ScanBarcodeAsync()
        {
            try
            {
                // Windows handheld terminals - manual barcode input
                var tcs = new TaskCompletionSource<string?>();

                if (Application.Current?.Dispatcher != null)
                {
                    await Application.Current.Dispatcher.DispatchAsync(async () =>
                    {
                        try
                        {
                            if (Application.Current?.MainPage != null)
                            {
                                var result = await Application.Current.MainPage.DisplayPromptAsync(
                                    "Barkod Okut",
                                    "Windows handheld terminal için barkod numarasını girin:",
                                    "Tamam",
                                    "İptal",
                                    "Barkod numarası...",
                                    keyboard: Keyboard.Numeric);

                                tcs.SetResult(result);
                            }
                            else
                            {
                                tcs.SetResult(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    });
                }
                else
                {
                    tcs.SetResult(null);
                }

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Barcode scanning error: {ex.Message}");
                return null;
            }
        }
    }
}
