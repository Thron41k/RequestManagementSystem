using RequestManagement.Common.Models;
using WpfClient.Models;

namespace WpfClient.Services.Interfaces
{
    public interface IExcelReaderService
    {
        (List<MaterialExpense> materialStocks, string? warehouse) ReadExpenses(string filePath);
        (List<MaterialStock> materialStocks, string? date, string? warehouse) ReadMaterialStock(string filePath);
    }
}
