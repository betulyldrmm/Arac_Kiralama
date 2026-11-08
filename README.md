# Rota39 · Araç Kiralama ve Rezervasyon Uygulaması

> "Rota39", bu ödev için uydurulmuş örnek bir proje adıdır; gerçek bir şirketi temsil etmez.

Blazor WebAssembly ile hazırlanmış bir eğitim prototipi. Uygulamada araçlar listelenir ve filtrelenir. Tarih ve ek hizmet seçilince fiyat anında hesaplanır. Rezervasyonlar tarayıcıda (localStorage) saklanır ve iptal edilebilir. Gerçek ödeme, kullanıcı girişi ve veritabanı **yoktur**.

| | |
|---|---|
| Çatı | **.NET 8 (LTS)**, **Blazor WebAssembly Standalone** (`Microsoft.NET.Sdk.BlazorWebAssembly`) |
| Dil | C# 12, Razor bileşenleri |
| Veri | `wwwroot/data/cars.json` (15 araç, 5 kategori) |
| Kalıcılık | Tarayıcı `localStorage` (anahtar: `arac-kiralama.bookings`) |
| Ek paket | Yok (yalnızca Blazor WebAssembly'nin kendi paketleri) |

---

## 1. Kurulum ve çalıştırma (Windows + Visual Studio)

1. **Visual Studio 2022 (17.8 veya üstü)** kurun. Community sürümü yeterlidir. Kurulumda **"ASP.NET ve web geliştirme"** iş yükünü işaretleyin; bu iş yükü .NET 8 SDK'yı da kurar.
   - Visual Studio 2026 kullanıyorsanız ayrıca **.NET 8 SDK** kurun: https://dotnet.microsoft.com/download/dotnet/8.0
   - Kurulumu kontrol etmek için komut isteminde `dotnet --list-sdks` yazın. Listede `8.0.x` görünmelidir.
2. ZIP dosyasını bir klasöre çıkarın. Yolda Türkçe karakter veya çok uzun klasör adı olmaması sorun riskini azaltır (ör. `C:\Odev\AracKiralamaOdev`).
3. `AracKiralama.sln` dosyasına çift tıklayın.
4. Visual Studio NuGet paketlerini kendiliğinden indirir (ilk açılışta internet gerekir). Alttaki durum çubuğunda "Restore completed" yazısını bekleyin.
5. Üstteki çalıştırma profili listesinden **`http`** profilini seçin ve **F5** (hata ayıklamalı) veya **Ctrl+F5** (hata ayıklamasız) tuşuna basın.
6. Tarayıcı `http://localhost:5080` adresini açar ve uygulama `/cars` sayfasına yönlenir.

**Komut satırından çalıştırma (alternatif):**
```
cd AracKiralamaOdev
dotnet run --project AracKiralama
```
Ardından tarayıcıda `http://localhost:5080` adresini açın.

**Kayıtları sıfırlama (test ve video için):** Tarayıcıda F12 → *Application* (Uygulama) → *Local Storage* → `http://localhost:5080` → `arac-kiralama.bookings` anahtarını silin ve sayfayı yenileyin.

---

## 2. Klasör yapısı

```
AracKiralamaOdev/
├─ AracKiralama.sln
├─ proje.txt                     ← öğrenci bilgileri ve görev dağılımı şablonu
├─ README.md                    ← bu dosya
└─ AracKiralama/
   ├─ AracKiralama.csproj
   ├─ Program.cs                 ← servis kayıtları (HttpClient, CarService, BookingService, RentalState)
   ├─ App.razor                  ← yönlendirme (Router) + "sayfa bulunamadı"
   ├─ _Imports.razor
   ├─ Models/
   │  ├─ Car.cs                  ← araç modeli (PDF'deki yapı)
   │  ├─ CarBooking.cs           ← rezervasyon modeli (PDF'deki yapı)
   │  ├─ CustomerInfo.cs         ← form modeli + doğrulama öznitelikleri
   │  ├─ CarFilter.cs            ← filtre seçimleri ve Matches() kuralı
   │  ├─ RentalExtra.cs          ← ek hizmetler (250 / 75 / 150 TL/gün)
   │  └─ BookingStatus.cs        ← Yaklaşan / Devam ediyor / Tamamlandı / İptal
   ├─ Services/
   │  ├─ CarService.cs           ← cars.json'ı HttpClient ile asenkron yükler
   │  ├─ BookingService.cs       ← localStorage kayıt, çakışma, iptal, kod üretimi
   │  ├─ RentalState.cs          ← sayfalar arası paylaşılan seçimler (scoped)
   │  ├─ RentalRules.cs          ← gün, fiyat, tarih, çakışma ve iptal kuralları
   │  └─ Formatters.cs           ← "1.450 TL", "10 Kasım 2026 Salı"
   ├─ Pages/
   │  ├─ Home.razor              ← "/" → "/cars" yönlendirmesi
   │  ├─ Cars.razor (+ .css)     ← /cars
   │  ├─ Rent.razor (+ .css)     ← /cars/{id}/rent
   │  └─ MyBookings.razor (+ .css) ← /my-bookings
   ├─ Components/                ← her bileşenin yanında kendi .razor.css dosyası
   │  ├─ CarList.razor           ← kart ızgarası + filtre paneli + boş sonuç mesajı
   │  ├─ CarFilterPanel.razor    ← marka, kategori, yakıt, vites, fiyat aralığı
   │  ├─ CarCard.razor           ← tek araç kartı
   │  ├─ CarImage.razor          ← görsel + kırık bağlantıda yedek görünüm
   │  ├─ RentalOptions.razor     ← tarihler, gün sayısı, ek hizmetler
   │  ├─ RentalSummary.razor     ← anlık fiyat özeti
   │  ├─ CustomerForm.razor      ← EditForm + doğrulama
   │  └─ BookingCard.razor       ← rezervasyon özeti + iki adımlı iptal
   ├─ Layout/MainLayout.razor (+ .css)
   ├─ Properties/launchSettings.json
   └─ wwwroot/
      ├─ index.html
      ├─ css/app.css             ← ortak tema (renkler, butonlar, uyarılar, form kontrolleri)
      ├─ data/cars.json
      └─ images/cars/            ← araç görselleri (şimdilik çizimler; bkz. "Araç fotoğraflarını ekleme")
```

---

## 3. Mimari ve veri akışı

```
Pages/Cars ──[Parameter] Cars, Filter──▶ CarList ──▶ CarFilterPanel ──EventCallback FilterChanged──┐
     ▲                                     └──▶ CarCard ──EventCallback OnSelect──┐               │
     └──────────────── EventCallback OnCarSelected / FilterChanged ◀──────────────┴───────────────┘

Pages/Rent ──[Parameter]──▶ RentalOptions  ──EventCallback PickupDateChanged / ReturnDateChanged / OnExtraToggled──▶ Rent
           ──[Parameter]──▶ RentalSummary  (gün, araç, seçili hizmetler → toplam anında yeniden hesaplanır)
           ──[Parameter]──▶ CustomerForm   ──EventCallback<CustomerInfo> OnSubmit──▶ Rent → BookingService.CreateBookingAsync

Pages/MyBookings ──[Parameter]──▶ BookingCard ──EventCallback<CarBooking> OnCancel──▶ MyBookings → BookingService.CancelAsync
```

- **CarService:** `HttpClient.GetFromJsonAsync<List<Car>>("data/cars.json")` ile veriyi asenkron okur. Başarılı sonucu bellekte tutar. Hata olursa Türkçe mesajlı bir istisna fırlatır; sayfa bu mesajı ve "Tekrar dene" butonunu gösterir.
- **RentalState (scoped):** Seçilen araç, tarihler, ek hizmetler ve /cars filtreleri burada tutulur. Kullanıcı listeye dönüp aynı araca geri geldiğinde seçimler korunur. Rezervasyon tamamlanınca kodu `/my-bookings` sayfasına bu servis aktarır. Blazor WebAssembly'de scoped servis bir tarayıcı sekmesi boyunca tek örnektir.
- **BookingService:** Rezervasyonları JSON olarak `localStorage`'a yazar. Okumalar her seferinde `localStorage`'dan yapılır; bu sayede sayfa yenilendiğinde de güncel veri görünür. Kayıt öncesi uygunluk, tarih ve çakışma kurallarını **son kez kontrol eder**; arayüz atlatılsa bile kurallar korunur.
- **localStorage zamanlaması:** Blazor WebAssembly'de kod tarayıcıda çalışır ve sunucu tarafı ön işleme (prerender) yoktur. Bu yüzden `IJSRuntime` çağrıları sayfaların `OnInitializedAsync` metodunda güvenle yapılır. (Blazor Server'da bu çağrıların `OnAfterRenderAsync` içine alınması gerekirdi.)
- **RentalRules:** Kurallar tek bir statik sınıftadır. Hem ekrandaki özet hem kaydedilen tutar aynı formülle hesaplanır.

---

## 4. Kurallar ve varsayımlar

PDF'de açıkça yazmayan noktalarda aşağıdaki varsayımlar kullanıldı.

| Konu | Uygulamadaki davranış |
|---|---|
| **Tarih sınırları** | Saat seçilmez, yalnızca gün kullanılır. Bir kiralama **[teslim günü, iade günü)** aralığını kaplar: teslim günü dahil, iade günü hariçtir. |
| Gün sayısı | `iade − teslim`. Örnek: 10–13 Kasım = 3 gün. |
| Geçmiş tarih | Teslim tarihi bugünden önce olamaz. Bugün teslim almak serbesttir. |
| Aynı gün teslim/iade | Kabul edilmez (0 gün). İade en az 1 gün sonra olmalıdır. |
| **Ardışık rezervasyon** | A kaydının iade günü, B kaydının teslim günü olabilir (ör. 10–13 ve 13–15). Bu durum **çakışma sayılmaz**. |
| Çakışma | Aynı araç için, iptal edilmemiş kayıtlarla: `yeniTeslim < eskiİade` **ve** `eskiTeslim < yeniİade` ise engellenir. İptal edilmiş kayıtlar çakışma oluşturmaz. |
| "Henüz başlamamış" | Teslim tarihi **bugünden sonra** olan, iptal edilmemiş kayıt. Teslim günü gelmiş kayıt "Devam ediyor" durumuna geçer ve iptal edilemez. |
| İptal | Kayıt silinmez, `IsCancelled = true` yapılır. İptal iki adımlı onayla yapılır. |
| Rezervasyon kodu | `CAR-{yıl}-{sıra}`. Sıra, kayıtlı en büyük numaranın bir fazlasıdır ve 101'den başlar. Aynı tarayıcıda iki kayıt aynı kodu alamaz. |
| Telefon | `[Phone]` özniteliği çok serbest olduğu için ek olarak Türkiye cep telefonu biçimi istenir: `05XX XXX XX XX`, `5XXXXXXXXX`, `+90 5XX XXX XX XX`. |
| E-posta | `[EmailAddress]` yalnızca tek bir `@` olup olmadığına bakar. Bu yüzden ek bir desenle alan adında nokta da aranır (`ad@ornek` reddedilir). |
| Ad soyad | 5–60 karakter, en az iki kelime. |
| Uygun olmayan araç | Kartta "Kirala" butonu pasiftir ve uyarı yazısı vardır. Adres elle yazılarak açılırsa kiralama sayfasında da form kapalıdır ve uyarı görünür. |
| Görseller | Görseller `wwwroot/images/cars` klasöründedir. **Bilerek** iki araçta yedek görünüm vardır: C011 Tesla'nın `imageUrl` değeri `null`, C013 Megane ise var olmayan bir dosyayı gösterir (kırık bağlantı örneği). |
| Depolama | Rezervasyonlar tarayıcının `localStorage` alanında `arac-kiralama.bookings` anahtarıyla JSON olarak tutulur. Ekranda kullanıcıya yalnızca "bu tarayıcıda saklanır" denir. |
| Uygun olmayan araçlar | C005 Hyundai i20 ve C010 Mercedes-Benz E 220d (`isAvailable: false`). |

---

## Araç fotoğraflarını ekleme

Her araç kaydında görselle ilgili dört alan vardır (`Models/Car.cs`, `wwwroot/data/cars.json`):

| Alan | Anlamı | Örnek |
|---|---|---|
| `imageUrl` | Gösterilecek dosya | `images/cars/c001-toyota-corolla.jpg` |
| `imageSourceUrl` | Fotoğrafın alındığı sayfa | Wikimedia Commons'taki dosya sayfası |
| `imageCredit` | Lisansın istediği fotoğrafçı/kaynak adı | Commons sayfasındaki "Author" (Yazar) |
| `imageLicense` | Lisans adı | `CC BY-SA 4.0` |

`imageSourceUrl` doluysa görselin sağ alt köşesinde "© yazar · lisans" bilgisi ve yeni sekmede açılan **"Fotoğraf kaynağı ↗"** bağlantısı görünür (`Components/CarImage.razor`). Görselin tamamı bağlantı değildir. Fotoğraflar `object-fit: contain` ile gösterildiği için araç kesilmez; bütün kartlarda yükseklik aynıdır.

Fotoğraf ekleme adımları:
1. https://commons.wikimedia.org adresinde marka ve model adıyla arayın (ör. "Toyota Corolla E210 sedan"). **Aynı model ve kasa** olduğundan emin olun.
2. Dosya sayfasında lisansın CC0, CC BY veya CC BY-SA gibi serbest bir lisans olduğunu kontrol edin.
3. "Download" ile yaklaşık 800 px genişliğinde bir sürüm indirin. Dosyayı `wwwroot/images/cars/` klasörüne kısa bir adla kaydedin (ör. `c001-toyota-corolla.jpg`).
4. `cars.json` içinde o aracın `imageUrl` değerini yeni dosya yolu yapın. `imageSourceUrl` alanına dosya sayfasının adresini, `imageCredit` alanına yazar adını, `imageLicense` alanına lisans adını yazın.
5. Uygulamayı yenileyin ve kaynak bağlantısının doğru sayfayı açtığını kontrol edin.

Fotoğraflar eklenene kadar projede kendi çizdiğimiz SVG çizimler kullanılır; bunlar için kaynak gösterilmesi gerekmez.

## 5. Değerlendirme kontrol tablosu

| No | Kriter (puan) | Karşılayan dosyalar | Videoda gösterilecek |
|---|---|---|---|
| 1 | JSON'dan veri okuma ve servis mimarisi (10) | `wwwroot/data/cars.json` (15 araç, 5 kategori), `Services/CarService.cs`, `Program.cs` (HttpClient + servis kaydı), `Pages/Cars.razor` | `GetFromJsonAsync`, `async/await`, yükleniyor ve hata mesajı |
| 2 | Bileşen yapısı ve veri akışı (15) | `Components/*.razor`. `[Parameter]`: `CarList`, `CarCard`, `CarFilterPanel`, `RentalOptions`, `RentalSummary`, `CustomerForm`, `BookingCard`, `CarImage`. `EventCallback`: `FilterChanged`, `OnCarSelected`, `OnSelect`, `PickupDateChanged`, `ReturnDateChanged`, `OnExtraToggled`, `OnSubmit`, `OnCancel` | Bölüm 3'teki akış şeması |
| 3 | Kiralama ve dinamik fiyat hesaplama (15) | `Models/CarFilter.cs`, `Services/RentalRules.cs`, `Services/BookingService.cs`, `Components/RentalOptions.razor`, `Components/RentalSummary.razor`, `Pages/Rent.razor`, `Pages/MyBookings.razor` | Filtreler, tarih hataları, anlık toplam, uygun olmayan araç, çakışma, iptal |
| 4 | Form doğrulama (15) | `Models/CustomerInfo.cs` (`[Required]`, `[StringLength]`, `[EmailAddress]`, `[Phone]`, `[RegularExpression]`), `Components/CustomerForm.razor` (`EditForm`, `DataAnnotationsValidator`, `ValidationMessage`) | Boş, hatalı e-posta ve hatalı telefonla gönderme |
| 5 | Durum yönetimi (10) | `Services/RentalState.cs` (scoped), `Services/BookingService.cs` (localStorage), `Program.cs` (`AddScoped`) | Listeye dönüp gelince seçimlerin korunması; F5 sonrası kayıtların ve iptallerin korunması; F12 → Local Storage |
| 6 | Tasarım ve scoped CSS (20) | Her bileşenin ve sayfanın `.razor.css` dosyası, `wwwroot/css/app.css`, `Components/CarImage.razor` | Mobil görünüm (F12 → cihaz modu), klavye ile gezinme, hata/başarı mesajları, yedek görsel |
| 7 | Dokümantasyon ve teslim (15) | `proje.txt`, `README.md`, ZIP, video | — |

---

## 6. Test senaryoları (elle kontrol listesi)

Her senaryodan önce gerekirse kayıtları sıfırlayın (Bölüm 1). "Sonuç" sütununu kendi testinizde doldurun.

| # | Senaryo | Adımlar | Beklenen | Sonuç |
|---|---|---|---|---|
| 1 | Veri yükleme | `/cars` sayfasını açın | 15 / 15 araç; kartlarda marka, model, kategori, görsel, yakıt, vites, günlük ücret | |
| 2 | Filtreler | Marka = Toyota; sonra Kategori = SUV | 2 araç, sonra 1 araç (C-HR) | |
| 3 | Fiyat aralığı | En az 2000, en çok 3000 | Yalnızca 2.000–3.000 TL arası araçlar | |
| 4 | Boş sonuç | Kategori = Lüks, En çok = 1000 | "Filtrelere uygun araç bulunamadı" + "Filtreleri temizle" | |
| 5 | Hatalı fiyat aralığı | En az 3000, En çok 1000 | Kırmızı uyarı, sonuç boş | |
| 6 | Uygun olmayan araç | Hyundai i20 kartı; sonra `/cars/C005/rent` adresi | Kartta "Kirala" pasif ve uyarı var; sayfada uyarı, alanlar kapalı, buton pasif | |
| 7 | Geçersiz araç kimliği | `/cars/XYZ/rent` adresi | "Araç bulunamadı" mesajı ve listeye dönüş bağlantısı | |
| 8 | Geçmiş tarih | Teslim alma kutusuna dünün tarihini klavyeyle yazın | "Teslim alma tarihi geçmişte olamaz." Buton pasif | |
| 9 | Aynı gün teslim/iade | Teslim = İade = aynı gün | "İade tarihi … en az 1 gün sonra olmalıdır." | |
| 10 | Gün sayısı | 10 Kasım → 13 Kasım | "Kiralama süresi: 3 gün" | |
| 11 | Ek hizmet fiyatları | Corolla (1.450 TL), 3 gün, üç hizmet de seçili | Günlük 1.450+250+75+150 = 1.925 TL; toplam **5.775 TL** | |
| 12 | Anlık güncelleme | Hizmetleri tek tek açıp kapatın, iade tarihini değiştirin | Toplam her değişiklikte hemen güncellenir | |
| 13 | Form doğrulama: boş | Formu boş gönderin | Üç alanda da "zorunludur" mesajı | |
| 14 | Hatalı e-posta | `ayse`, sonra `ayse@ornek` | "Geçerli bir e-posta…", sonra "alan adı kısmı eksik" | |
| 15 | Hatalı telefon | `12345`, `0212 123 45 67` | "Telefon 05XX XXX XX XX biçiminde olmalıdır." | |
| 16 | Başarılı rezervasyon | Geçerli bilgilerle onaylayın | `/my-bookings` açılır; "Rezervasyonunuz oluşturuldu", kod `CAR-2026-101`, araç, tarihler, hizmetler, toplam | |
| 17 | Çakışma | Aynı araç, çakışan tarihler (ör. 11–12 Kasım) | Kırmızı "Bu tarihlerde araç dolu" uyarısı; buton pasif | |
| 18 | Sınır günü | Aynı araç, önceki kaydın iade günü teslim olacak şekilde (ör. 13–15 Kasım) | Çakışma uyarısı **çıkmaz**, rezervasyon oluşturulur | |
| 19 | İptal | Yaklaşan bir kayıtta "İptal et" → "Evet, iptal et" | Durum "İptal edildi" olur, başarı mesajı görünür | |
| 20 | İptal sonrası yeniden rezervasyon | İptal edilen kaydın tarihleriyle aynı aracı tekrar kiralayın | Çakışma uyarısı yok, yeni kod alınır | |
| 21 | Başlamış kayıt | Teslim = bugün olan bir rezervasyon oluşturun | Durum "Devam ediyor"; iptal butonu yok | |
| 22 | Yenileme sonrası kalıcılık | `/my-bookings` sayfasında F5 | Bütün kayıtlar ve iptal durumları aynen görünür | |
| 23 | Seçimlerin korunması | Kiralama sayfasında tarih/hizmet seçin → "Araç listesine dön" → aynı aracı tekrar açın | Seçimler korunur; filtreler de korunur | |
| 24 | Veri yükleme hatası | Uygulamayı durdurun, `wwwroot/data/cars.json` adını geçici değiştirin, çalıştırın (gerekirse Ctrl+F5 ile önbelleği temizleyin) | "Araçlar yüklenemedi" mesajı ve "Tekrar dene" butonu. **Testten sonra adı geri alın!** | |
| 25 | Kırık görsel | Renault Megane ve Tesla kartları | "Görsel yok" yedek görünümü | |
| 26 | Mobil görünüm | F12 → cihaz modu (ör. 390 px genişlik) | Tek sütun, yatay kaydırma yok, butonlar tıklanabilir boyutta | |
| 27 | Klavye | Yalnızca Tab / Shift+Tab / Space / Enter ile bir rezervasyon yapın | Her öğede odak çerçevesi görünür; form açılınca odak "Ad soyad" alanına gelir | |

---

## 7. İki kişilik görev dağılımı (öneri)

Önce **ortak sözleşme** birlikte yazılır, sonra iki kişi paralel çalışabilir.

**Ortak, 1. gün:** `Models/*` sınıfları, `RentalRules` metot imzaları ve aşağıdaki servis imzaları:
```csharp
// CarService
Task<IReadOnlyList<Car>> GetCarsAsync();
Task<Car?> GetCarByIdAsync(string id);
// BookingService
Task<List<CarBooking>> GetBookingsAsync();
Task<CarBooking?> FindConflictAsync(string carId, DateTime pickup, DateTime ret);
Task<CarBooking> CreateBookingAsync(Car car, DateTime? pickup, DateTime? ret, IEnumerable<string> extraIds, CustomerInfo customer);
Task CancelAsync(string bookingId);
// RentalState: Filter, SelectedCarId, PickupDate, ReturnDate, ExtraIds, LastBookingId, SelectCar(), ToggleExtra(), ClearSelection()
```

| | Üye 1 – Araçlar ve fiyat | Üye 2 – Rezervasyon ve kalıcılık |
|---|---|---|
| C# mantığı | `CarService`, `CarFilter.Matches`, `RentalRules` fiyat metotları | `BookingService` (localStorage, çakışma, iptal, kod), `RentalState`, `RentalRules` tarih/iptal metotları |
| Razor bileşenleri | `CarList`, `CarFilterPanel`, `CarCard`, `CarImage`, `RentalSummary` | `RentalOptions`, `CustomerForm`, `BookingCard` |
| Sayfalar | `/cars` | `/cars/{id}/rent`, `/my-bookings` |
| Arayüz | Kartlar, filtre paneli, fiyat özeti CSS'i; `MainLayout`, `app.css` teması | Kiralama sayfası düzeni, form ve rezervasyon kartı CSS'i |
| Test | Senaryo 1–7, 10–12, 25–26 | Senaryo 8–9, 13–24, 27 |

`proje.txt` dosyasına bu tabloyu değil, **gerçekte** yaptıklarınızı yazın.

---

## 8. Anlatım videosu planı (≈ 10–12 dk)

| Süre | Kim | İçerik |
|---|---|---|
| 0:00–0:45 | Üye 1 | Proje ve teknoloji (.NET 8, Blazor WebAssembly), klasör yapısı |
| 0:45–2:00 | Üye 1 | `cars.json` → `CarService` (`HttpClient`, `GetFromJsonAsync`, async) → `Program.cs` kayıtları |
| 2:00–3:30 | Üye 1 | `/cars`: kartlar, beş filtre, boş sonuç; kodda `CarList` → `CarFilterPanel` ve `CarCard` arasındaki `[Parameter]` ve `EventCallback` akışı |
| 3:30–4:00 | Üye 1 | Uygun olmayan araç, kırık görsel yedeği, mobil görünüm |
| 4:00–5:30 | Üye 2 | `/cars/{id}/rent`: geçmiş tarih, aynı gün, 10–13 Kasım = 3 gün, ek hizmetlerle anlık toplam; `RentalOptions` → Rent → `RentalSummary` akışı, `RentalRules` formülü |
| 5:30–6:45 | Üye 2 | Form doğrulama: boş, hatalı e-posta ve telefon; `CustomerInfo` öznitelikleri, `EditForm`, `DataAnnotationsValidator`, `ValidationMessage` |
| 6:45–7:30 | Üye 2 | Başarılı rezervasyon → kod ve özet (`/my-bookings`) |
| 7:30–9:00 | Üye 2 | Çakışma, sınır günü (13–15), iptal, iptal sonrası yeniden rezervasyon; `BookingService.FindConflict`, `RentalRules.Overlaps` |
| 9:00–10:00 | Üye 1 | F5 ile kalıcılık; F12 → Local Storage'daki JSON; `RentalState` ile seçimlerin korunması |
| 10:00–10:45 | Üye 1 | Scoped CSS (`.razor.css` dosyaları, `::deep` kullanımı), klavye ile gezinme |
| 10:45–11:30 | İkisi | Varsayımlar (tarih sınırları, iptal kuralı) ve **çalışmayan veya eksik işlevler** (varsa açıkça söyleyin) |

İpucu: Videodan önce kayıtları sıfırlayın. Böylece ilk kod `CAR-2026-101` olur ve anlatım düzenli görünür.

---

## 9. Teslim ZIP içeriği

```
AracKiralamaOdev.zip
├─ AracKiralama.sln
├─ proje.txt                (adlar ve numaralar doldurulmuş olarak)
├─ README.md
└─ AracKiralama/            (bütün kaynak kod; bin/ ve obj/ klasörleri HARİÇ)
```
ZIP oluşturmadan önce Visual Studio'yu kapatın ve `AracKiralama/bin`, `AracKiralama/obj`, `.vs` klasörlerini silin. Bu klasörler derleme çıktısıdır ve dosya boyutunu çok büyütür. Videoyu ZIP'e koymak yerine bağlantısını `proje.txt` içine yazın (yönerge farklı bir şey istiyorsa ona uyun).

---

## 10. Notlar

**.NET 10 / Visual Studio 2026 notu:** Yalnızca .NET 10 kuruluysa `AracKiralama.csproj` içinde `net8.0` değerini `net10.0` yapın, iki paketin sürümünü de `10.0.x` yapın. Ayrıca `index.html` dosyasını .NET 10 Blazor WebAssembly şablonuyla karşılaştırın. Bu geçiş denenmedi; en kolay yol .NET 8 SDK'yı kurmaktır.
