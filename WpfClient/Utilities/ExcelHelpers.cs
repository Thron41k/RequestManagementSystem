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
        public static string NumberToWords(int number)
        {
            if (number == 0) return "ноль";

            string[] units =
            [
                "", "один", "два", "три", "четыре", "пять",
                "шесть", "семь", "восемь", "девять", "десять",
                "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать",
                "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
            ];

            string[] tens =
            [
                "", "", "двадцать", "тридцать", "сорок",
                "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"
            ];

            string[] hundreds =
            [
                "", "сто", "двести", "триста", "четыреста",
                "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"
            ];

            switch (number)
            {
                case < 20:
                    return units[number];
                case < 100:
                    return tens[number / 10] + (number % 10 > 0 ? " " + units[number % 10] : "");
                case < 1000:
                    return hundreds[number / 100] + (number % 100 > 0 ? " " + NumberToWords(number % 100) : "");
                case < 10000:
                {
                    var thousands = number / 1000;
                    var thousandWord = thousands == 1 ? "тысяча" :
                        thousands < 5 ? NumberToWords(thousands) + " тысячи" :
                        NumberToWords(thousands) + " тысяч";
                    return thousandWord + (number % 1000 > 0 ? " " + NumberToWords(number % 1000) : "");
                }
                default:
                    return number.ToString();
            }
        }
    }
}
