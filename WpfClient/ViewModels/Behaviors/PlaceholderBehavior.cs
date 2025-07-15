using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace RequestManagement.WpfClient.ViewModels.Behaviors;

public class PlaceholderBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty PlaceholderTargetNameProperty =
        DependencyProperty.Register(nameof(PlaceholderTargetName), typeof(string), typeof(PlaceholderBehavior));

    public string PlaceholderTargetName
    {
        get => (string)GetValue(PlaceholderTargetNameProperty);
        set => SetValue(PlaceholderTargetNameProperty, value);
    }

    private FrameworkElement _placeholderElement;

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.TextChanged += OnStateChanged;
        AssociatedObject.GotFocus += OnStateChanged;
        AssociatedObject.LostFocus += OnStateChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_placeholderElement == null && !string.IsNullOrEmpty(PlaceholderTargetName))
        {
            _placeholderElement = AssociatedObject.Template.FindName(PlaceholderTargetName, AssociatedObject) as FrameworkElement;
        }

        UpdatePlaceholderVisibility();
    }

    private void OnStateChanged(object sender, RoutedEventArgs e)
    {
        UpdatePlaceholderVisibility();
    }

    private void UpdatePlaceholderVisibility()
    {
        if (_placeholderElement == null) return;

        bool hasText = !string.IsNullOrEmpty(AssociatedObject.Text);
        bool hasFocus = AssociatedObject.IsKeyboardFocused;

        _placeholderElement.Visibility = (!hasText && !hasFocus)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.TextChanged -= OnStateChanged;
        AssociatedObject.GotFocus -= OnStateChanged;
        AssociatedObject.LostFocus -= OnStateChanged;
    }
}