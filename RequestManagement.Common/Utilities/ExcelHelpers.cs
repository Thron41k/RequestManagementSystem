using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Utilities
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
    }
}
