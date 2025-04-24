using OfficeOpenXml;
using System.IO;
using WpfClient.Models;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services
{
    public class ExcelReaderService : IExcelReaderService
    {
        public ExcelReaderService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        }
        public List<MaterialStock> ReadMaterialStock(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found.", filePath);
            var materialStocks = new List<MaterialStock>();
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows+2 ?? 0;
            const int startRow = 11;
            var date = worksheet.Cells[4, 3].Value.ToString()?.Split('-')[1].Trim();
            var warehouse = worksheet.Cells[10, 1].Value.ToString()?.Trim();
            for (var row = startRow; row < rowCount; row++)
            {
                if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null)
                    continue;
                try
                {
                    var material = new MaterialStock
                    {
                        ItemName = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty,
                        Code = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty,
                        Article = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty,
                        Unit = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty,
                        FinalBalance = double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var balance) ? balance : 0
                    };
                    materialStocks.Add(material);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row {row}: {ex.Message}");
                }
            }
            return materialStocks;
        }
    }
}
