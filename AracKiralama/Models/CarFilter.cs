namespace AracKiralama.Models;

/// <summary>
/// /cars sayfasındaki filtre seçimleri. Boş metin veya null "filtre yok" anlamına gelir.
/// record olduğu için "filter with { Brand = "Toyota" }" ile değiştirilmiş yeni bir kopya üretilebilir.
/// </summary>
public record CarFilter
{
    public string Brand { get; init; } = "";
    public string Category { get; init; } = "";
    public string FuelType { get; init; } = "";
    public string Transmission { get; init; } = "";
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }

    public bool IsEmpty =>
        Brand == "" && Category == "" && FuelType == "" && Transmission == ""
        && MinPrice is null && MaxPrice is null;

    public bool HasInvalidPriceRange =>
        MinPrice is not null && MaxPrice is not null && MinPrice > MaxPrice;

    /// <summary>Araç bütün seçili filtrelere uyuyorsa true döner.</summary>
    public bool Matches(Car car) =>
        (Brand == "" || car.Brand == Brand)
        && (Category == "" || car.Category == Category)
        && (FuelType == "" || car.FuelType == FuelType)
        && (Transmission == "" || car.Transmission == Transmission)
        && (MinPrice is null || car.DailyPrice >= MinPrice)
        && (MaxPrice is null || car.DailyPrice <= MaxPrice);
}
