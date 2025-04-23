using System.Windows.Controls;
using System.Windows;

namespace WpfClient.ViewModels.Helpers;

public static class TextBoxFocusHelper
{
    public static readonly DependencyProperty UnfocusOnClearProperty =
        DependencyProperty.RegisterAttached(
            "UnfocusOnClear",
            typeof(bool),
            typeof(TextBoxFocusHelper),
            new PropertyMetadata(false, OnUnfocusOnClearChanged));

    public static bool GetUnfocusOnClear(DependencyObject obj) => (bool)obj.GetValue(UnfocusOnClearProperty);
    public static void SetUnfocusOnClear(DependencyObject obj, bool value) => obj.SetValue(UnfocusOnClearProperty, value);

    private static void OnUnfocusOnClearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && e.NewValue is true)
        {
            textBox.TextChanged += (sender, args) =>
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    Unfocus(textBox);
                }
            };
        }
    }

    private static void Unfocus(TextBox textBox)
    {
        var dummy = new TextBox
        {
            Width = 0,
            Height = 0,
            Opacity = 0,
            Focusable = true,
            IsTabStop = false,
            Visibility = Visibility.Collapsed
        };

        var parent = FindRootVisual(textBox);
        if (parent is Panel panel)
        {
            panel.Children.Add(dummy);
            dummy.Focus();
            dummy.Dispatcher.BeginInvoke(() =>
            {
                panel.Children.Remove(dummy);
            });
        }
    }

    private static DependencyObject FindRootVisual(DependencyObject current)
    {
        var parent = current;
        while (parent is FrameworkElement { Parent: FrameworkElement p })
            parent = p;
        return parent;
    }
}