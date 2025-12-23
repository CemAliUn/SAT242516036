using Attributes;

namespace DbModels;

public class LoyaltyTransaction
{
    [Title("İşlem No")]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    // Earn (Kazanma) veya Redeem (Harcama)
    [Title("İşlem Türü")]
    public string TransactionType { get; set; }

    [Title("İşlem Tutarı")]
    public decimal Amount { get; set; }

    [Title("Açıklama")]
    public string Description { get; set; }

    [Title("İşlem Tarihi")]
    public DateTime TransactionDate { get; set; }
}