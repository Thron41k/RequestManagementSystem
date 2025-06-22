using System.Globalization;
using System.Windows.Controls;

namespace WpfClient.ViewModels.ValidationRules;

public class NotEmptyDriverNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var input = (value ?? "").ToString();
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ValidationResult(false, "");
        }
        var words = input.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        return words.Length is >= 2 and <= 4 ? ValidationResult.ValidResult : new ValidationResult(false, "");
    }
}