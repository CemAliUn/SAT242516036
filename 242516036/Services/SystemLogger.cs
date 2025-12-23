using MyDbModels;
using UnitOfWorks;

namespace Services;

public interface ISystemLogger
{
    // Madde 19b: Veritabanına Logla
    Task LogToDb(string operation, string message, string recordId = "0");

    // Madde 19c: Dosyaya Logla
    Task LogToFile(string message);
}

public class SystemLogger : ISystemLogger
{
    private readonly IMyDbModel_UnitOfWork _uow;
    private readonly IWebHostEnvironment _env;

    public SystemLogger(IMyDbModel_UnitOfWork uow, IWebHostEnvironment env)
    {
        _uow = uow;
        _env = env;
    }

    // 19b: DbLogger (Veritabanına yazar)
    public async Task LogToDb(string operation, string message, string recordId = "0")
    {
        try
        {
            // Senin mimarine uygun model oluşturuyoruz
            var model = new MyDbModel<object>();

            // UnitOfWork kullanarak SP çağırıyoruz
            await _uow.Execute(model, "sp_System_Log_Create", false);

            // Parametreleri modele manuel ekleyelim (UnitOfWork içinde Dictionary'den parametreye dönüşüyor)
            // Not: Senin UnitOfWork yapın parametreleri model.Parameters.Params içinden alıyor.

            // Ancak UnitOfWork generic yapıda execute ederken parametreleri Dictionary olarak değil,
            // Extensions_SqlParameter kullanarak çeviriyordu. 
            // Burada basit bir SP çağrısı için UnitOfWork'e ek parametre göndermemiz lazım.
            // Fakat senin UnitOfWork yapın dışarıdan parametre almayı model üzerinden yapıyor.

            // DÜZELTME: Senin UnitOfWork yapın biraz karmaşık olduğu için
            // Buradaki en temiz yöntem, parametreleri modelin Params sözlüğüne eklemektir.

            model.Parameters.Params.Add("TableName", "Application");
            model.Parameters.Params.Add("Operation", operation);
            model.Parameters.Params.Add("RecordId", recordId);
            model.Parameters.Params.Add("NewValue", message); // Mesajı NewValue alanına yazalım

            // Tekrar çağır (Parametreler dolduktan sonra)
            await _uow.Execute(model, "sp_System_Log_Create", false);
        }
        catch
        {
            // Log atarken hata olursa uygulamayı patlatma
        }
    }

    // 19c: FileLogger (Metin dosyasına yazar)
    public async Task LogToFile(string message)
    {
        try
        {
            // Proje klasöründe "Logs" diye bir klasör bulur/oluşturur
            string path = Path.Combine(_env.ContentRootPath, "Logs");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Dosya adı: log_2023-12-23.txt (Her gün için ayrı dosya)
            string fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
            string fullPath = Path.Combine(path, fileName);

            string logLine = $"{DateTime.Now:HH:mm:ss} | {message}{Environment.NewLine}";

            // Dosyaya ekle (Append)
            await File.AppendAllTextAsync(fullPath, logLine);
        }
        catch
        {
            // Dosya hatası olursa yut
        }
    }
}