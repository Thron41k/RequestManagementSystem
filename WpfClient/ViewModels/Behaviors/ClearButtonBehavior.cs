using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace RequestManagement.WpfClient.ViewModels.Behaviors;

public class ClearButtonBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (AssociatedObject.Template.FindName("ClearButton", AssociatedObject) is Button button)
        {
            button.Visibility = string.IsNullOrEmpty(AssociatedObject.Text)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    protected override void OnDetaching()
    {
        AssociatedObject.TextChanged -= OnTextChanged;
        base.OnDetaching();
    }
}