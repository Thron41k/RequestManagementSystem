using WpfClient.Models;

namespace WpfClient.Services.Interfaces
{
    public interface IExcelReaderService
    {
        List<MaterialStock> ReadMaterialStock(string filePath);
    }
}
