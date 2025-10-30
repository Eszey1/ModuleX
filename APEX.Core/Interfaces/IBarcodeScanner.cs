namespace APEX.Core.Interfaces
{
    public interface IBarcodeScanner
    {
        Task<string?> ScanBarcodeAsync();
    }
}
