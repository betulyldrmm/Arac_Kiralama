namespace AracKiralama.Models;

/// <summary>Rezervasyonun bugüne göre durumu. RentalRules.GetStatus ile hesaplanır.</summary>
public enum BookingStatus
{
    Upcoming,   // Henüz başlamadı (teslim tarihi bugünden sonra) → iptal edilebilir
    Ongoing,    // Teslim günü geldi, iade günü henüz gelmedi
    Completed,  // İade günü geldi veya geçti
    Cancelled   // Kullanıcı iptal etti
}
