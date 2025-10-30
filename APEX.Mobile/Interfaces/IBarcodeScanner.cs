namespace APEX.Mobile.Interfaces
{
    public interface IBarcodeScanner
    {
        Task<string?> ScanBarcodeAsync();
    }
}
