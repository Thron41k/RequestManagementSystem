using System.Windows;

namespace RequestManagement.WpfClient.ViewModels.Helpers;

public static class PlaceholderHelper
{
    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.RegisterAttached(
            "PlaceholderText",
            typeof(string),
            typeof(PlaceholderHelper),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetPlaceholderText(DependencyObject element, string value)
    {
        element.SetValue(PlaceholderTextProperty, value);
    }

    public static string GetPlaceholderText(DependencyObject element)
    {
        return (string)element.GetValue(PlaceholderTextProperty);
    }
}