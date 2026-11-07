using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>
/// Sayfalar arasında paylaşılan seçimler (scoped servis).
/// /cars → /cars/{id}/rent → /my-bookings geçişlerinde aynı örnek kullanılır:
///   • /cars sayfasındaki filtreler, araç sayfasından geri dönüldüğünde korunur.
///   • Seçilen araç, tarihler ve ek hizmetler; liste sayfasına gidip aynı araca dönüldüğünde korunur.
///   • Son oluşturulan rezervasyon kodu /my-bookings sayfasına özet göstermek için aktarılır.
/// Bu bilgiler yalnızca bellekte tutulur, sayfa yenilenince sıfırlanır. Kalıcı olan rezervasyonlardır (localStorage).
/// </summary>
public class RentalState
{
    private readonly List<string> _extraIds = new();

    public CarFilter Filter { get; set; } = new();

    public string? SelectedCarId { get; private set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public IReadOnlyList<string> ExtraIds => _extraIds;

    /// <summary>Az önce tamamlanan rezervasyonun kodu; /my-bookings bir kez okuyup temizler.</summary>
    public string? LastBookingId { get; set; }

    /// <summary>Farklı bir araç seçildiyse varsayılan tarihlerle (yarın, +3 gün) yeni bir seçim başlatır.</summary>
    public void SelectCar(string carId)
    {
        if (SelectedCarId == carId)
            return;

        SelectedCarId = carId;
        PickupDate = DateTime.Today.AddDays(1);
        ReturnDate = DateTime.Today.AddDays(4);
        _extraIds.Clear();
    }

    public void ToggleExtra(string extraId)
    {
        if (!_extraIds.Remove(extraId))
            _extraIds.Add(extraId);
    }

    public void ClearSelection()
    {
        SelectedCarId = null;
        PickupDate = null;
        ReturnDate = null;
        _extraIds.Clear();
    }
}
