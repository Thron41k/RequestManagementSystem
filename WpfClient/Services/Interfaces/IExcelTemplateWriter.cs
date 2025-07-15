using RequestManagement.WpfClient.Services.ExcelTemplate;

namespace RequestManagement.WpfClient.Services.Interfaces;

public interface IExcelTemplateWriter
{
    ExcelTemplateType TemplateType { get; }
    Type DataType { get; }
    byte[] FillTemplateTyped(object data);
}
public interface IExcelTemplateWriter<in T> : IExcelTemplateWriter
{
    byte[] FillTemplate(T data);
}