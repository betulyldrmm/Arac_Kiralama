using System.Globalization;
using AracKiralama;
using AracKiralama.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// cars.json, uygulamanın kendi adresinden (wwwroot) okunur.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Scoped servisler: Blazor WebAssembly'de bir tarayıcı sekmesi boyunca tek örnek olarak yaşar.
builder.Services.AddScoped<CarService>();      // Araç verilerini cars.json'dan yükler
builder.Services.AddScoped<BookingService>();  // Rezervasyonları localStorage'da saklar
builder.Services.AddScoped<RentalState>();     // Seçilen araç, tarihler, ek hizmetler ve filtreler

// Tarih ve para birimi Türkçe biçimde gösterilsin (ör. 1.450 TL, 10 Kasım 2026).
var culture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await builder.Build().RunAsync();
