using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DbModels;

namespace _242516036.Services
{
    public class ReportService : IReportService
    {
        public byte[] GenerateCustomerReport(List<Customer> customers)
        {
            // PDF Dokümanı Oluşturuluyor
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // --- BAŞLIK ---
                    page.Header()
                        .Text("Müşteri Listesi Raporu")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    // --- İÇERİK (TABLO) ---
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            // Kolon Genişlikleri
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // ID
                                columns.RelativeColumn();   // Ad Soyad
                                columns.RelativeColumn();   // Email
                                columns.ConstantColumn(80); // Telefon
                                columns.ConstantColumn(60); // Puan
                            });

                            // Tablo Başlıkları
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Ad Soyad");
                                header.Cell().Element(CellStyle).Text("E-Posta");
                                header.Cell().Element(CellStyle).Text("Telefon");
                                header.Cell().Element(CellStyle).Text("Puan");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold())
                                                    .PaddingVertical(5)
                                                    .BorderBottom(1)
                                                    .BorderColor(Colors.Black);
                                }
                            });

                            // Tablo Satırları
                            foreach (var customer in customers)
                            {
                                table.Cell().Element(CellStyle).Text(customer.Id.ToString());
                                table.Cell().Element(CellStyle).Text(customer.FullName);
                                table.Cell().Element(CellStyle).Text(customer.Email);
                                table.Cell().Element(CellStyle).Text(customer.Phone ?? "-");
                                table.Cell().Element(CellStyle).Text(customer.LoyaltyPoints.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1)
                                                    .BorderColor(Colors.Grey.Lighten2)
                                                    .PaddingVertical(5);
                                }
                            }
                        });

                    // --- ALT BİLGİ ---
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}