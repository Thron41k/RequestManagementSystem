using System.Drawing;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services.ExcelTemplate;

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
    protected double MeasureTextHeight(string text, ExcelFont font, double width)
    {
        if (string.IsNullOrEmpty(text)) return 0.0;
        var bitmap  = new Bitmap(1, 1);
        var graphics = Graphics.FromImage(bitmap);

        var pixelWidth = Convert.ToInt32(width * 7);  //7 pixels per excel column width
        var fontSize = font.Size * 1.01f;
        var drawingFont = new Font(font.Name, fontSize);
        var size = graphics.MeasureString(text, drawingFont, pixelWidth, new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

        //72 DPI and 96 points per inch.  Excel height in points with max of 409 per Excel requirements.
        return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
    }
    public static void AdjustRowHeightForText(ExcelWorksheet worksheet, int row, int column)
    {
        var cell = worksheet.Cells[row, column];
        cell.Style.WrapText = true;
        var textLength = cell.Text.Length;
        var width = worksheet.Column(column).Width;
        var height = Math.Ceiling(textLength / width) * 7;
        worksheet.Row(row).Height = height;
    }
}