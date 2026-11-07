namespace AracKiralama.Models;

/// <summary>Tamamlanmış bir rezervasyon kaydı. localStorage'a JSON olarak yazılır.</summary>
public class CarBooking
{
    /// <summary>Benzersiz rezervasyon kodu, ör. CAR-2026-101.</summary>
    public string Id { get; set; } = "";
    public string CarId { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public List<string> ExtraIds { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}
