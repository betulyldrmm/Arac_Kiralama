using System.Text.Json;
using AracKiralama.Models;
using Microsoft.JSInterop;

namespace AracKiralama.Services;

/// <summary>
/// Rezervasyonları tarayıcının localStorage alanında JSON olarak saklar.
///
/// Blazor WebAssembly'de kod doğrudan tarayıcıda çalışır ve sunucu tarafı ön işleme (prerender) yoktur;
/// bu yüzden IJSRuntime ile localStorage'a bileşenlerin OnInitializedAsync metodunda güvenle erişilebilir.
///
/// Her okuma localStorage'dan yapılır (önbellek yok). Böylece sayfa yenilense veya ikinci bir sekmede
/// değişiklik yapılsa bile en güncel kayıtlarla çalışılır.
/// </summary>
public class BookingService
{
    private const string StorageKey = "arac-kiralama.bookings";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;

    public BookingService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>Son okumada bir sorun olduysa kullanıcıya gösterilecek uyarı; sorun yoksa null.</summary>
    public string? StorageWarning { get; private set; }

    public async Task<List<CarBooking>> GetBookingsAsync()
    {
        StorageWarning = null;

        string? json;
        try
        {
            json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch (JSException)
        {
            StorageWarning = "Tarayıcı depolamasına erişilemedi. Rezervasyonlar kaydedilemeyebilir.";
            return new List<CarBooking>();
        }

        if (string.IsNullOrWhiteSpace(json))
            return new List<CarBooking>();

        try
        {
            return JsonSerializer.Deserialize<List<CarBooking>>(json, JsonOptions) ?? new List<CarBooking>();
        }
        catch (JsonException)
        {
            StorageWarning = "Kayıtlı rezervasyon verisi okunamadı (bozuk veri). Liste boş gösteriliyor.";
            return new List<CarBooking>();
        }
    }

    /// <summary>Aynı araç için seçilen tarihlerle çakışan, iptal edilmemiş ilk rezervasyonu döndürür; yoksa null.</summary>
    public async Task<CarBooking?> FindConflictAsync(string carId, DateTime pickupDate, DateTime returnDate)
    {
        var bookings = await GetBookingsAsync();
        return FindConflict(bookings, carId, pickupDate, returnDate);
    }

    /// <summary>
    /// Bütün kuralları (uygunluk, tarih, çakışma) son kez kontrol eder, toplam tutarı hesaplar,
    /// benzersiz kod üretir ve kaydı localStorage'a yazar. Kural bozulursa InvalidOperationException fırlatır.
    /// </summary>
    public async Task<CarBooking> CreateBookingAsync(
        Car car, DateTime? pickupDate, DateTime? returnDate, IEnumerable<string> extraIds, CustomerInfo customer)
    {
        if (!car.IsAvailable)
            throw new InvalidOperationException("Bu araç şu anda kiralamaya uygun değil.");

        var dateError = RentalRules.ValidateDates(pickupDate, returnDate, DateTime.Today);
        if (dateError is not null)
            throw new InvalidOperationException(dateError);

        var pickup = DateOnlyValue(pickupDate!.Value);
        var dropOff = DateOnlyValue(returnDate!.Value);

        var bookings = await GetBookingsAsync();
        var conflict = FindConflict(bookings, car.Id, pickup, dropOff);
        if (conflict is not null)
            throw new InvalidOperationException(
                $"Seçilen tarihler, bu araç için {conflict.Id} kodlu aktif rezervasyonla çakışıyor.");

        var extras = RentalExtraCatalog.GetByIds(extraIds);
        var days = RentalRules.CalculateDays(pickup, dropOff);

        var booking = new CarBooking
        {
            Id = GenerateCode(bookings, DateTime.Now),
            CarId = car.Id,
            CustomerName = customer.CustomerName.Trim(),
            Email = customer.Email.Trim(),
            Phone = customer.Phone.Trim(),
            PickupDate = pickup,
            ReturnDate = dropOff,
            ExtraIds = extras.Select(extra => extra.Id).ToList(),
            TotalAmount = RentalRules.CalculateTotal(car.DailyPrice, extras, days),
            IsCancelled = false
        };

        bookings.Add(booking);
        await SaveAsync(bookings);
        return booking;
    }

    /// <summary>Rezervasyonu iptal edildi olarak işaretler (silmez). Kurala uymuyorsa InvalidOperationException fırlatır.</summary>
    public async Task CancelAsync(string bookingId)
    {
        var bookings = await GetBookingsAsync();
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId)
            ?? throw new InvalidOperationException("Rezervasyon bulunamadı.");

        if (!RentalRules.CanCancel(booking, DateTime.Today))
            throw new InvalidOperationException("Yalnızca henüz başlamamış, aktif rezervasyonlar iptal edilebilir.");

        booking.IsCancelled = true;
        await SaveAsync(bookings);
    }

    private static CarBooking? FindConflict(IEnumerable<CarBooking> bookings, string carId, DateTime pickupDate, DateTime returnDate) =>
        bookings.FirstOrDefault(b =>
            !b.IsCancelled // İptal edilmiş kayıtlar çakışma oluşturmaz.
            && string.Equals(b.CarId, carId, StringComparison.OrdinalIgnoreCase)
            && RentalRules.Overlaps(pickupDate, returnDate, b.PickupDate, b.ReturnDate));

    private async Task SaveAsync(List<CarBooking> bookings)
    {
        var json = JsonSerializer.Serialize(bookings, JsonOptions);
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
        catch (JSException ex)
        {
            throw new InvalidOperationException("Rezervasyon tarayıcıya kaydedilemedi. Tarayıcının depolama izinlerini kontrol edin.", ex);
        }
    }

    /// <summary>
    /// Kod biçimi: CAR-{yıl}-{sıra no}. Sıra numarası kayıtlı en büyük numaranın bir fazlasıdır (ilk kod 101),
    /// bu yüzden aynı tarayıcıda iki kayıt asla aynı kodu almaz. Örnek: CAR-2026-101, CAR-2026-102...
    /// </summary>
    private static string GenerateCode(IEnumerable<CarBooking> existing, DateTime now)
    {
        var lastNumber = 100;
        foreach (var booking in existing)
        {
            var dashIndex = booking.Id.LastIndexOf('-');
            if (dashIndex >= 0 && int.TryParse(booking.Id[(dashIndex + 1)..], out var number) && number > lastNumber)
                lastNumber = number;
        }
        return $"CAR-{now.Year}-{lastNumber + 1}";
    }

    /// <summary>Saat ve saat dilimi bilgisini atar; localStorage'a yalnızca gün yazılır (ör. 2026-11-10T00:00:00).</summary>
    private static DateTime DateOnlyValue(DateTime value) =>
        DateTime.SpecifyKind(value.Date, DateTimeKind.Unspecified);
}
