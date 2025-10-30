# Hata Tarama Raporu

Bu rapor, mevcut kod tabanında yapılan statik inceleme sonucunda tespit edilen önemli risk ve tutarsızlıkları özetler. Amaç, üretim verilerini etkilemeden önce giderilmesi gereken noktaları belirlemektir.

## 1. Doğrulama hatalarının API'de 500 olarak dönmesi
- `SayimService.SayimKaydetAsync`, sayım doğrulaması başarısız olduğunda `InvalidOperationException` fırlatıyor. 【F:APEX.Business/Services/SayimService.cs†L148-L209】
- `SayimController.SayimKaydet` ise yalnızca `ArgumentException` tipini 400 ile döndürüyor; `InvalidOperationException` yakalanmadığı için bu durumlar 500 sunucu hatasına dönüşüyor. 【F:APEX.API/Controllers/SayimController.cs†L99-L126】
- Sonuç: İstemci tarafı, kullanıcı kaynaklı veri hatalarında bile 500 hatası alıyor; bu hem yanlış HTTP kodu hem de gözlemlenebilirlik problemleri yaratıyor.

## 2. Ürün kimliği eksik olduğunda sabit `1` atanması
- API katmanı, `SayimKaydet` isteğinde ürün kimliği 0 veya negatif gelirse otomatik olarak `1` değerini yazıyor. 【F:APEX.API/Controllers/SayimController.cs†L82-L96】
- Aynı sayım içinde birden fazla ürün bu koşulu tetiklediğinde tüm detaylar aynı `UrunId` ile kaydediliyor. Bu, raporlama ve stok güncellemelerinde tutarsızlıklara yol açabilir.

## 3. Firma numarasının SQL sorgularına doğrudan gömülmesi
- Repository sınıfı, tablo adlarını oluştururken `_firmaNo` değerini doğrudan sorgu dizesine ekliyor. Örnek: `LG_" + _firmaNo + "_ITEMS`. 【F:APEX.Data/Repositories/LogoRepository.cs†L23-L106】
- `_firmaNo` dış kaynaktan geliyor ve yalnızca null kontrolünden geçiyor; sayı dışında karakterler içerirse SQL enjeksiyonu riski oluşur. Tablo adları parametrelenemediği için en azından beyaz liste veya regex doğrulaması yapılmalı.

## 4. Mock sayımların API uyumluluğu için kopyalanması sırasında referans paylaşımı
- `SayimListesiGetirAsync`, mock ve örnek sayım kayıtlarını birleştirirken `sayim.Urunler = sayim.Detaylar;` ataması yapıyor. 【F:APEX.Data/Repositories/LogoRepository.cs†L360-L412】
- `Urunler` ve `Detaylar` aynı liste örneğine işaret ettiği için dış katmanlar koleksiyonu değiştirdiğinde mock veri kaynağı etkilenebiliyor. Yeni kopya oluşturulmadığından veri yarışları ortaya çıkabilir.

## Önerilen Adımlar
1. `SayimController` içinde `InvalidOperationException` yakalanıp 400 veya 422 olarak dönmeli.
2. `SayimKaydet` isteğinde `UrunId` eksikse hataya düşülmeli ya da benzersiz geçici bir kimlik kullanılmalı.
3. `_firmaNo` için yalnızca beklenen formatı (örn. `Regex("^[0-9]{1,3}$")`) kabul eden bir doğrulama eklenmeli.
4. Mock sayım verileri döndürülürken `Detaylar` listesinin derin kopyası `Urunler` için yeniden oluşturulmalı.

Bu maddeler, hem güvenlik hem de veri bütünlüğü açısından öncelikli olarak ele alınmalıdır.
