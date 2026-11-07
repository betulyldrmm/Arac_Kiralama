using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>
/// Kiralama kurallarının tek bir yerde toplandığı yardımcı sınıf (fiyat, tarih, çakışma, iptal).
/// Hem sayfalar hem de BookingService aynı kuralları kullanır; böylece arayüzde gösterilen
/// sonuç ile kaydedilen sonuç her zaman aynı olur.
///
/// TARİH SINIRLARI: Saat seçilmez, yalnızca gün kullanılır. Bir kiralama [teslim günü, iade günü)
/// yarı açık aralığını kaplar: teslim günü dahil, iade günü hariç.
///   • Gün sayısı = iade − teslim (10–13 Kasım = 3 gün).
///   • Aynı gün teslim ve iade (0 gün) kabul edilmez; iade en az 1 gün sonra olmalıdır.
///   • A rezervasyonunun iade günü, B rezervasyonunun teslim günü olabilir (araç o gün geri gelir
///     ve yeniden verilir). Bu durum çakışma SAYILMAZ.
/// </summary>
public static class RentalRules
{
    public static int CalculateDays(DateTime pickupDate, DateTime returnDate) =>
        (returnDate.Date - pickupDate.Date).Days;

    /// <summary>Tarihler geçerliyse null, değilse kullanıcıya gösterilecek hata mesajını döndürür.</summary>
    public static string? ValidateDates(DateTime? pickupDate, DateTime? returnDate, DateTime today)
    {
        if (pickupDate is null)
            return "Lütfen teslim alma tarihini seçin.";
        if (pickupDate.Value.Date < today.Date)
            return "Teslim alma tarihi geçmişte olamaz.";
        if (returnDate is null)
            return "Lütfen iade tarihini seçin.";
        if (returnDate.Value.Date <= pickupDate.Value.Date)
            return "İade tarihi, teslim alma tarihinden en az 1 gün sonra olmalıdır.";
        return null;
    }

    /// <summary>Araç günlük ücreti + seçilen hizmetlerin günlük ücretleri.</summary>
    public static decimal CalculateDailyTotal(decimal carDailyPrice, IEnumerable<RentalExtra> extras) =>
        carDailyPrice + extras.Sum(extra => extra.DailyPrice);

    /// <summary>Toplam tutar = gün sayısı × (araç günlük ücreti + hizmetlerin günlük ücretleri).</summary>
    public static decimal CalculateTotal(decimal carDailyPrice, IEnumerable<RentalExtra> extras, int days) =>
        days <= 0 ? 0m : days * CalculateDailyTotal(carDailyPrice, extras);

    /// <summary>
    /// İki [başlangıç, bitiş) aralığı çakışıyor mu? Biri diğeri bitmeden başlıyorsa çakışır.
    /// Örnek: 10–13 ile 13–15 çakışmaz; 10–13 ile 12–14 çakışır.
    /// </summary>
    public static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB) =>
        startA.Date < endB.Date && startB.Date < endA.Date;

    public static BookingStatus GetStatus(CarBooking booking, DateTime today)
    {
        if (booking.IsCancelled) return BookingStatus.Cancelled;
        if (booking.PickupDate.Date > today.Date) return BookingStatus.Upcoming;
        if (booking.ReturnDate.Date > today.Date) return BookingStatus.Ongoing;
        return BookingStatus.Completed;
    }

    /// <summary>Yalnızca henüz başlamamış (teslim günü gelmemiş) ve iptal edilmemiş kayıtlar iptal edilebilir.</summary>
    public static bool CanCancel(CarBooking booking, DateTime today) =>
        GetStatus(booking, today) == BookingStatus.Upcoming;
}
