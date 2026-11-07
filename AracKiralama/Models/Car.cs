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
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
}
