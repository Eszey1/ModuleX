using System.Text.RegularExpressions;

namespace APEX.Core.Validation
{
    public static class ValidationHelper
    {
        // Barkod validation patterns
        private static readonly Regex BarkodPattern = new Regex(@"^[0-9]{8,13}$", RegexOptions.Compiled);
        private static readonly Regex UrunKodPattern = new Regex(@"^[A-Za-z0-9\-_]{1,20}$", RegexOptions.Compiled);
        
        /// <summary>
        /// Barkod formatını doğrular (8-13 haneli sayısal değer)
        /// </summary>
        public static bool IsValidBarkod(string? barkod)
        {
            if (string.IsNullOrWhiteSpace(barkod))
                return false;

            return BarkodPattern.IsMatch(barkod.Trim());
        }

        /// <summary>
        /// Ürün kodu formatını doğrular
        /// </summary>
        public static bool IsValidUrunKod(string? kod)
        {
            if (string.IsNullOrWhiteSpace(kod))
                return false;

            return UrunKodPattern.IsMatch(kod.Trim());
        }

        /// <summary>
        /// Pozitif sayı kontrolü
        /// </summary>
        public static bool IsPositiveNumber(decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Pozitif veya sıfır sayı kontrolü
        /// </summary>
        public static bool IsNonNegativeNumber(decimal value)
        {
            return value >= 0;
        }

        /// <summary>
        /// String uzunluk kontrolü
        /// </summary>
        public static bool IsValidLength(string? value, int minLength, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return minLength == 0;

            var length = value.Trim().Length;
            return length >= minLength && length <= maxLength;
        }

        /// <summary>
        /// SQL injection riski olan karakterleri kontrol eder
        /// </summary>
        public static bool ContainsSqlInjectionRisk(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var dangerousPatterns = new[]
            {
                "'", "\"", ";", "--", "/*", "*/", "xp_", "sp_", 
                "exec", "execute", "select", "insert", "update", 
                "delete", "drop", "create", "alter", "union"
            };

            var lowerInput = input.ToLowerInvariant();
            return dangerousPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        /// <summary>
        /// Güvenli string temizleme
        /// </summary>
        public static string SanitizeInput(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Tehlikeli karakterleri temizle
            return input.Trim()
                       .Replace("'", "")
                       .Replace("\"", "")
                       .Replace(";", "")
                       .Replace("--", "")
                       .Replace("/*", "")
                       .Replace("*/", "");
        }

        /// <summary>
        /// Limit değeri kontrolü
        /// </summary>
        public static bool IsValidLimit(int limit, int maxAllowed = 10000)
        {
            return limit > 0 && limit <= maxAllowed;
        }

        /// <summary>
        /// Sayım detayı doğrulama
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateSayimDetay(Core.Entities.SayimDetay detay)
        {
            if (detay == null)
                return (false, "Sayım detayı null olamaz");

            if (detay.UrunId <= 0)
                return (false, "Geçersiz ürün ID");

            if (string.IsNullOrWhiteSpace(detay.Barkod) || !IsValidBarkod(detay.Barkod))
                return (false, "Geçersiz barkod formatı");

            if (string.IsNullOrWhiteSpace(detay.UrunAdi) || !IsValidLength(detay.UrunAdi, 1, 100))
                return (false, "Ürün adı 1-100 karakter arasında olmalıdır");

            if (!IsNonNegativeNumber(detay.MevcutStok))
                return (false, "Mevcut stok negatif olamaz");

            if (!IsNonNegativeNumber(detay.SayilanMiktar))
                return (false, "Sayılan miktar negatif olamaz");

            return (true, string.Empty);
        }

        /// <summary>
        /// Sayım doğrulama
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateSayim(Core.Entities.Sayim sayim)
        {
            if (sayim == null)
                return (false, "Sayım objesi null olamaz");

            if (string.IsNullOrWhiteSpace(sayim.KullaniciId))
                return (false, "Geçersiz kullanıcı ID");

            if (sayim.Detaylar == null || !sayim.Detaylar.Any())
                return (false, "Sayım detayları boş olamaz");

            if (sayim.Detaylar.Count > 1000)
                return (false, "Sayım detayları 1000'den fazla olamaz");

            // Her detayı doğrula
            for (int i = 0; i < sayim.Detaylar.Count; i++)
            {
                var (isValid, errorMessage) = ValidateSayimDetay(sayim.Detaylar[i]);
                if (!isValid)
                    return (false, $"Detay {i + 1}: {errorMessage}");
            }

            return (true, string.Empty);
        }
    }
}
