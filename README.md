# Rota39 · Araç Kiralama ve Rezervasyon Uygulaması

Blazor WebAssembly ile geliştirilmiş bir araç kiralama uygulaması. Kullanıcı araçları listeleyip filtreleyebilir, tarih ve ek hizmet seçerek fiyatı anında görebilir, rezervasyon oluşturup iptal edebilir.

> "Rota39" bu proje için kullanılan örnek bir addır; gerçek bir şirketi temsil etmez. Uygulamada gerçek ödeme, kullanıcı girişi ve veritabanı yoktur.

## Özellikler

- `cars.json` dosyasından asenkron yüklenen 15 araç ve 5 kategori
- Marka, kategori, yakıt, vites ve günlük fiyat aralığına göre filtreleme
- Tarih seçimi ve gün sayısı hesabı (geçmiş tarih ve geçersiz aralık kontrolü)
- Ek hizmetler: tam sigorta (250 TL/gün), çocuk koltuğu (75 TL/gün), ek sürücü (150 TL/gün)
- Anında güncellenen fiyat özeti: `gün × (araç ücreti + seçilen hizmetler)`
- Ad soyad, e-posta ve telefon için form doğrulama
- Benzersiz rezervasyon kodu (`CAR-2026-101` biçiminde)
- Aynı araç için çakışan tarihlerde rezervasyon engeli
- Henüz başlamamış rezervasyonları iptal etme
- Rezervasyonların tarayıcıda (`localStorage`) saklanması; sayfa yenilense de korunur
- Mobil uyumlu arayüz ve bileşen bazlı scoped CSS

## Teknolojiler

| | |
|---|---|
| Çatı | .NET 8, Blazor WebAssembly (Standalone) |
| Dil | C#, Razor bileşenleri |
| Veri | `wwwroot/data/cars.json` |
| Kalıcılık | Tarayıcı `localStorage` (`arac-kiralama.bookings` anahtarı) |
| Ek paket | Yok |

## Çalıştırma

Gereksinim: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) veya üstü.

```bash
git clone https://github.com/betulyldrmm/Arac_Kiralama.git
cd Arac_Kiralama
dotnet run --project AracKiralama
```

Ardından tarayıcıda `http://localhost:5080` adresini açın.

Visual Studio 2022 ile: `AracKiralama.sln` dosyasını açın, `http` profilini seçin ve **Ctrl+F5** ile çalıştırın.

## Proje yapısı

```
AracKiralama/
├─ Program.cs              Servis kayıtları (HttpClient, CarService, BookingService, RentalState)
├─ App.razor               Yönlendirme
├─ Models/                 Car, CarBooking, CustomerInfo, CarFilter, RentalExtra, BookingStatus
├─ Services/
│  ├─ CarService.cs        cars.json dosyasını HttpClient ile asenkron yükler
│  ├─ BookingService.cs    Rezervasyon kaydı, çakışma kontrolü, iptal, kod üretimi (localStorage)
│  ├─ RentalState.cs       Sayfalar arasında paylaşılan seçimler (scoped servis)
│  ├─ RentalRules.cs       Gün, fiyat, tarih, çakışma ve iptal kuralları
│  └─ Formatters.cs        Para ve tarih biçimleri
├─ Pages/                  /cars, /cars/{id}/rent, /my-bookings
├─ Components/             CarList, CarFilterPanel, CarCard, CarImage, RentalOptions,
│                          RentalSummary, CustomerForm, BookingCard (+ her birinin .razor.css dosyası)
├─ Layout/                 MainLayout
└─ wwwroot/                index.html, css/app.css, data/cars.json, images/cars/
```

## Bileşenler arası veri akışı

Veri üst bileşenden alt bileşene `[Parameter]` ile gider; kullanıcı işlemleri alt bileşenden üst bileşene `EventCallback` ile bildirilir.

```
Cars ──▶ CarList ──▶ CarFilterPanel   (FilterChanged ▲)
                 └─▶ CarCard          (OnSelect ▲)

Rent ──▶ RentalOptions   (PickupDateChanged, ReturnDateChanged, OnExtraToggled ▲)
     ──▶ RentalSummary   (fiyat özeti)
     ──▶ CustomerForm    (OnSubmit ▲ → BookingService.CreateBookingAsync)

MyBookings ──▶ BookingCard (OnCancel ▲ → BookingService.CancelAsync)
```

## Kurallar

| Konu | Davranış |
|---|---|
| Gün sayısı | İade − teslim tarihi (ör. 10–13 Kasım = 3 gün) |
| Tarih kontrolü | Teslim tarihi geçmişte olamaz; iade en az 1 gün sonra olmalıdır |
| Tarih aralığı | Teslim günü dahil, iade günü hariç. Bir rezervasyonun iade günü, diğerinin teslim günü olabilir |
| Çakışma | Aynı araç için aktif rezervasyonlarla tarihler kesişiyorsa yeni rezervasyon engellenir; iptal edilenler çakışma oluşturmaz |
| İptal | Yalnızca teslim tarihi henüz gelmemiş rezervasyonlar iptal edilebilir; kayıt silinmez, "İptal edildi" olarak işaretlenir |
| Uygun olmayan araç | `isAvailable: false` olan araçlar (Hyundai i20, Mercedes-Benz E 220d) kiralanamaz |
| Telefon | Türkiye cep telefonu biçimi: `05XX XXX XX XX` |
| Görseller | Görsel yüklenemezse kartta "Görsel yok" yedek görünümü gösterilir |

Rezervasyon kayıtlarını sıfırlamak için: tarayıcıda F12 → Application → Local Storage → `arac-kiralama.bookings` anahtarını silin.