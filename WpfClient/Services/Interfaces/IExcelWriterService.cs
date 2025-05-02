using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Services.ExcelTemplate;

namespace WpfClient.Services.Interfaces
{
    public interface IExcelWriterService
    {
        byte[] Export<T>(ExcelTemplateType type, T data);
        void ExportAndSave<T>(ExcelTemplateType type, T data, string suggestedFileName);
        void ExportAndPrint<T>(ExcelTemplateType type, T data);
    }
}
