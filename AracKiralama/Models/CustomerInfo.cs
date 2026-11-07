using System.ComponentModel.DataAnnotations;

namespace AracKiralama.Models;

/// <summary>
/// Müşteri formunun modeli. EditForm + DataAnnotationsValidator bu özniteliklere göre doğrulama yapar.
/// Bir alanda birden fazla kural bozulursa CustomerForm yalnızca ilk mesajı gösterir.
/// </summary>
public class CustomerInfo
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(60, MinimumLength = 5, ErrorMessage = "Ad soyad 5 ile 60 karakter arasında olmalıdır.")]
    [RegularExpression(@"^\s*\S+(\s+\S+)+\s*$", ErrorMessage = "Lütfen adınızı ve soyadınızı aralarında boşluk bırakarak yazın.")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin (ör. ad@ornek.com).")]
    // [EmailAddress] yalnızca tek bir @ olup olmadığına bakar; "ad@ornek" gibi noktasız alan adlarını da reddetmek için:
    [RegularExpression(@"^\s*[^@\s]+@[^@\s]+\.[^@\s]+\s*$", ErrorMessage = "E-posta adresinin alan adı kısmı eksik (ör. ad@ornek.com).")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    // [Phone] çok serbesttir; Türkiye cep telefonu biçimini zorunlu kılar: 05XX XXX XX XX, 5XX..., +90 5XX...
    [RegularExpression(@"^\s*(\+90\s?|0)?5\d{2}\s?\d{3}\s?\d{2}\s?\d{2}\s*$", ErrorMessage = "Telefon 05XX XXX XX XX biçiminde olmalıdır.")]
    public string Phone { get; set; } = "";
}
