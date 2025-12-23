using Attributes;

namespace DbModels;

public class SystemLog
{
    [Title("Log No")]
    public int Id { get; set; }

    [Title("Tablo")]
    public string TableName { get; set; }

    [Title("İşlem")]
    public string Operation { get; set; } // Insert, Update, Delete, Info...

    [Title("Kayıt ID")]
    public string RecordId { get; set; }

    [Title("Eski Veri")]
    public string OldValue { get; set; }

    [Title("Yeni Veri")]
    public string NewValue { get; set; }

    [Title("Tarih")]
    public DateTime LogDate { get; set; }
}