using WpfClient.Models;

namespace WpfClient.Services.Interfaces
{
    public interface IExcelReaderService
    {
        (List<MaterialStock> materialStocks, string? date, string? warehouse) ReadMaterialStock(string filePath);
    }
}
