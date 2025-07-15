using System.Globalization;
using System.Windows.Controls;

namespace RequestManagement.WpfClient.ViewModels.ValidationRules;

public class SimpleDateValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if(ReferenceEquals(value, null) || string.IsNullOrWhiteSpace(value.ToString()))
            return new ValidationResult(false, "Неверная дата.");
        var dateString = value.ToString();
        if (!string.IsNullOrWhiteSpace(dateString))
        {
            var splitDateString = dateString.Split(' ');
            if(splitDateString.Length > 1)
                dateString = splitDateString[0];
        }
        if(dateString is not { Length: 10 } || dateString[2] != '.' || dateString[5] != '.')
            return new ValidationResult(false, "Неверная дата.");
        var result = DateTime.TryParseExact(
            dateString,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
        return !result ? new ValidationResult(false, "Неверная дата.") : ValidationResult.ValidResult;
    }

    public static bool ValidateDateDetailed(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        if (dateString.Length != 10)
            return false;

        if (dateString[2] != '.' || dateString[5] != '.')
            return false;

        // Пробуем разделить строку на компоненты
        var parts = dateString.Split('.');
        if (parts.Length != 3)
            return false;

        // Проверяем, что все компоненты - числа
        if (!int.TryParse(parts[0], out var day) ||
            !int.TryParse(parts[1], out var month) ||
            !int.TryParse(parts[2], out var year))
            return false;

        // Проверяем допустимые диапазоны
        if (year is < 1 or > 9999)
            return false;

        if (month is < 1 or > 12)
            return false;

        // Проверяем допустимое количество дней в месяце
        return day >= 1 && day <= DateTime.DaysInMonth(year, month);
    }
}