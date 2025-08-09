using System.Drawing;
using System.Drawing.Text;
using System.IO;
using Font = System.Drawing.Font;

namespace RequestManagement.WpfClient.Utilities
{
    public static class ExcelHelpers
    {
        public static string GetSafeSheetName(string name)
        {
            // Удаляем недопустимые символы и обрезаем до 31 символа
            var invalidChars = Path.GetInvalidFileNameChars().Concat(['[', ']', '*', '?', '/', '\\']);
            name = invalidChars.Aggregate(name, (current, c) => current.Replace(c, '_'));
            return name.Length > 31 ? name[..31] : name;
        }

        public static double GetRowHeight(double columnWidth, string fontName, float fontSize, string text,float rowHeight = 13.25f)
        {
            const float pixelsPerExcelColumn = 6.11f;
            var columnWidthInPixels = (float)(columnWidth * pixelsPerExcelColumn)/1.01f;
            var lineCount =  GetLineCount(columnWidthInPixels, fontName, fontSize, text, rowHeight);
            return lineCount * rowHeight;
        }
        private static int GetLineCount(double columnWidth, string fontName, float fontSize, string text, float rowHeight)
        {
            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            var font = new Font(fontName, fontSize);
            var maxSize = new SizeF((float)columnWidth, 1000f);
            var format = new StringFormat(StringFormatFlags.LineLimit);
            format.Trimming = StringTrimming.Word;
            var textSize = g.MeasureString(text, font, maxSize, format);
            var lineCount = (int)Math.Ceiling(textSize.Height / rowHeight);
            return lineCount;
        }
    }
}
