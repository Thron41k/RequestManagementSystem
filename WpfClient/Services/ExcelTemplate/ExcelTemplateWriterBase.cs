using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services.ExcelTemplate
{
    public abstract class ExcelTemplateWriterBase<T> : IExcelTemplateWriter<T>
    {
        public abstract ExcelTemplateType TemplateType { get; }

        public Type DataType => typeof(T);

        public abstract byte[] FillTemplate(T data);

        byte[] IExcelTemplateWriter.FillTemplateTyped(object data)
        {
            if (data is not T typedData)
                throw new InvalidCastException($"Invalid data type for template '{TemplateType}'. Expected: {typeof(T)}");

            return FillTemplate(typedData);
        }

        protected Stream GetTemplateStream(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException($"Template resource '{fileName}' not found.");

            return assembly.GetManifestResourceStream(resourceName)
                   ?? throw new InvalidOperationException($"Cannot load template resource stream: '{fileName}'.");
        }
    }
}
