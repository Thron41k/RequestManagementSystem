using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Services.ExcelTemplate;

namespace WpfClient.Services.Interfaces
{
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
}
