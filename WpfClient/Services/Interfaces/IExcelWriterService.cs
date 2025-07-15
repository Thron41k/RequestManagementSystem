using RequestManagement.WpfClient.Services.ExcelTemplate;

namespace RequestManagement.WpfClient.Services.Interfaces;

public interface IExcelWriterService
{
    byte[] Export<T>(ExcelTemplateType type, T data);
    void ExportAndSave<T>(ExcelTemplateType type, T data, string suggestedFileName);
    void ExportAndPrint<T>(ExcelTemplateType type, T data);
}