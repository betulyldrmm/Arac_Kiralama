namespace AracKiralama.Models;

/// <summary>cars.json dosyasındaki bir aracı temsil eder.</summary>
public class Car
{
    public string Id { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public string Category { get; set; } = "";
    public string FuelType { get; set; } = "";
    public string Transmission { get; set; } = "";
    public int Seats { get; set; }
    public decimal DailyPrice { get; set; }
    public bool IsAvailable { get; set; }

    // Görsel bilgileri
    /// <summary>Gösterilecek dosya, ör. images/cars/c001.jpg. Boşsa yedek görünüm gösterilir.</summary>
    public string? ImageUrl { get; set; }
    /// <summary>Fotoğrafın alındığı kaynak sayfa (ör. Wikimedia Commons dosya sayfası).</summary>
    public string? ImageSourceUrl { get; set; }
    /// <summary>Lisansın istediği fotoğrafçı/kaynak adı.</summary>
    public string? ImageCredit { get; set; }
    /// <summary>Lisans adı, ör. CC BY-SA 4.0.</summary>
    public string? ImageLicense { get; set; }
}
