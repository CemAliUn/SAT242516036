using Attributes; // Senin yazdığın TitleAttribute burada devreye giriyor

namespace DbModels;

public class Customer
{
    // Veritabanı: Id
    [Title("Kayıt No")]
    public int Id { get; set; }

    // Veritabanı: FirstName
    [Title("Adı")]
    public string FirstName { get; set; }

    // Veritabanı: LastName
    [Title("Soyadı")]
    public string LastName { get; set; }

    // Listede Ad Soyad birleşik görünsün diye ekstra bir property (Veritabanında yok)
    [Title("Adı Soyadı")]
    public string FullName => $"{FirstName} {LastName}";

    // Veritabanı: Email
    [Title("E-Posta")]
    public string Email { get; set; }

    // Veritabanı: Phone
    [Title("Telefon")]
    public string Phone { get; set; }

    // Veritabanı: LoyaltyPoints
    [Title("Puan Bakiyesi")]
    public decimal LoyaltyPoints { get; set; }

    // Veritabanı: IsActive
    // Duruma göre renk vermek için (Madde 21 - Listeleme renklendirme)
    [Title("Durum")]
    public bool IsActive { get; set; }

    // Veritabanı: CreatedAt
    [Title("Kayıt Tarihi")]
    public DateTime CreatedAt { get; set; }
}