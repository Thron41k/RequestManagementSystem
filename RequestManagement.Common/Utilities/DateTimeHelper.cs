using System.Globalization;

namespace RequestManagement.Common.Utilities;

public static class DateTimeHelper
{
    public static bool TryParseDto(string? s, out DateTime dto)
    {
        dto = default;
        if (string.IsNullOrWhiteSpace(s)) throw new InvalidDataException("Дата имеет неверный формат");
        if (!DateTime.TryParseExact(
                s.Trim(),
                [
                    "yyyy-MM-dd",
                    "yyyy-MM-dd H:mm:ss",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss.fff",
                    "dd.MM.yyyy",
                    "dd.MM.yyyy H:mm:ss", 
                    "dd.MM.yyyy HH:mm:ss",
                    "dd/MM/yyyy",
                    "dd/MM/yyyy H:mm:ss",
                    "dd/MM/yyyy HH:mm:ss"
                ],
                new CultureInfo("ru-RU"),
                DateTimeStyles.None,
                out var dt)) throw new InvalidDataException("Дата имеет неверный формат");
        dto = new DateTimeOffset(dt).DateTime; 
        return true;
    }
}