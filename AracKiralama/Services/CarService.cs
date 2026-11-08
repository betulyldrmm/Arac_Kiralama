using System.Net.Http.Json;
using System.Text.Json;
using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>
/// Araç verilerini wwwroot/data/cars.json dosyasından HttpClient ile asenkron yükler.
/// Başarılı yükleme bellekte saklanır; sayfalar arası geçişte dosya tekrar indirilmez.
/// Hata durumunda kullanıcıya gösterilebilecek Türkçe mesajlı InvalidOperationException fırlatır.
/// </summary>
public class CarService
{
    // ?v=2: dosya değişince tarayıcının eski kopyayı (önbellek) kullanmaması için sürüm numarası.
    private const string DataPath = "data/cars.json?v=3";

    private readonly HttpClient _http;
    private List<Car>? _cars;

    public CarService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<Car>> GetCarsAsync()
    {
        if (_cars is not null)
            return _cars;

        List<Car>? cars;
        try
        {
            cars = await _http.GetFromJsonAsync<List<Car>>(DataPath);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("cars.json dosyasına ulaşılamadı. Dosyanın wwwroot/data klasöründe olduğundan emin olun.", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("cars.json dosyası okunamadı: JSON biçimi hatalı.", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException("cars.json dosyası beklenen JSON biçiminde değil.", ex);
        }

        if (cars is null || cars.Count == 0)
            throw new InvalidOperationException("cars.json dosyasında hiç araç bulunamadı.");

        _cars = cars;
        return _cars;
    }

    /// <summary>Kimliği verilen aracı döndürür; bulunamazsa null.</summary>
    public async Task<Car?> GetCarByIdAsync(string id)
    {
        var cars = await GetCarsAsync();
        return cars.FirstOrDefault(car => string.Equals(car.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}

