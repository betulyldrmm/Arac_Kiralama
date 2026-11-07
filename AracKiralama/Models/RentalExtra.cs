namespace AracKiralama.Models;

/// <summary>Kiralamaya eklenebilen, günlük ücretli isteğe bağlı hizmet.</summary>
public class RentalExtra
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal DailyPrice { get; init; }
}

/// <summary>PDF'de tanımlanan üç ek hizmetin sabit listesi.</summary>
public static class RentalExtraCatalog
{
    public static IReadOnlyList<RentalExtra> All { get; } = new List<RentalExtra>
    {
        new() { Id = "full-insurance", Name = "Tam sigorta", Description = "Kaza ve hasar durumları için kapsamlı güvence", DailyPrice = 250m },
        new() { Id = "child-seat", Name = "Çocuk koltuğu", Description = "Yaşa uygun, araca takılmış çocuk koltuğu", DailyPrice = 75m },
        new() { Id = "extra-driver", Name = "Ek sürücü", Description = "Aracı ikinci bir kişinin de kullanabilmesi", DailyPrice = 150m },
    };

    /// <summary>Verilen kimliklere karşılık gelen hizmetleri katalog sırasıyla döndürür; bilinmeyen kimlikler yok sayılır.</summary>
    public static IReadOnlyList<RentalExtra> GetByIds(IEnumerable<string> ids)
    {
        var selected = ids.ToHashSet();
        return All.Where(extra => selected.Contains(extra.Id)).ToList();
    }
}
