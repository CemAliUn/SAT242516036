using DbModels;

namespace _242516036.Services
{
    public interface IReportService
    {
        // Artık Customer sınıfını tanıyor
        byte[] GenerateCustomerReport(List<Customer> customers);
    }
}