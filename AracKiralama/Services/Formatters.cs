namespace AracKiralama.Services;

/// <summary>Ekranda para ve tarihlerin aynı biçimde gösterilmesi için yardımcılar (kültür: tr-TR).</summary>
public static class Formatters
{
    /// <summary>1450 → "1.450 TL"</summary>
    public static string Money(decimal amount) => $"{amount:#,0.##} TL";

    /// <summary>2026-11-10 → "10 Kasım 2026 Salı"</summary>
    public static string Date(DateTime date) => date.ToString("d MMMM yyyy dddd");
}
