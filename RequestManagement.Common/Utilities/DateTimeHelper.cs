using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Utilities
{
    public static class DateTimeHelper
    {
        private static readonly string[] DateFormats =
        [
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-dd HH:mm:ss.fff K",
            "yyyy-MM-dd HH:mm:ss zzz",
            "yyyy-MM-dd HH:mm:ss.fff zzz",
            "dd.MM.yyyy",
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy HH:mm:ss zzz",
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss zzz"
        ];

        public static bool TryParseDto(string? s, out DateTimeOffset dto)
        {
            if (!string.IsNullOrWhiteSpace(s) &&
                DateTimeOffset.TryParseExact(
                    s.Trim(),
                    DateFormats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    out dto))
            {
                return true;
            }
            dto = default;
            return false;
        }
    }
}
