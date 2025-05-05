using System.IO;
using System.Windows.Controls;
using WpfClient.Services.ExcelTemplate;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services
{
    public class ExcelWriterService : IExcelWriterService
    {
        private readonly Dictionary<ExcelTemplateType, IExcelTemplateWriter> _writers;
        private readonly IFileSaveDialogService _fileDialogService;
        private readonly IExcelPrintService _printService;
        public ExcelWriterService(IEnumerable<IExcelTemplateWriter> writers, IFileSaveDialogService fileDialogService, IExcelPrintService printService)
        {
            _fileDialogService = fileDialogService;
            _printService = printService;
            _writers = writers.ToDictionary(w => w.TemplateType);
        }
        public byte[] Export<T>(ExcelTemplateType type, T data)
        {
            if (!_writers.TryGetValue(type, out var writer))
                throw new InvalidOperationException($"Template '{type}' not registered.");

            if (writer is not IExcelTemplateWriter<T> typedWriter)
                throw new InvalidOperationException(
                    $"Template '{type}' expects data of type '{writer.DataType.Name}', but received '{typeof(T).Name}'");

            return typedWriter.FillTemplate(data);
        }
        public void ExportAndSave<T>(ExcelTemplateType type, T data, string suggestedFileName)
        {
            var bytes = Export(type, data);

            var path = _fileDialogService.ShowSaveFileDialog(suggestedFileName, "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*");

            if (!string.IsNullOrWhiteSpace(path))
            {
                File.WriteAllBytes(path, bytes);
            }
        }
        public void ExportAndPrint<T>(ExcelTemplateType type, T data)
        {
            var bytes = Export(type, data);

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            var printerName = printDialog.PrintQueue.FullName;
            var copies = printDialog.PrintTicket.CopyCount ?? 1;

            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
            File.WriteAllBytes(tempFile, bytes);

            try
            {
                _printService.Print(tempFile, printerName, copies);
            }
            finally
            {
                try { File.Delete(tempFile); } catch { /* можно залогировать */ }
            }
        }
    }
}
