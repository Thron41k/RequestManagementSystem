using System.Globalization;
using System.Windows.Data;

namespace RequestManagement.WpfClient.ViewModels.Helpers;

public class AspectRatioConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double height && parameter is string ratioString && double.TryParse(ratioString, out double ratio))
        {
            return height * ratio;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}