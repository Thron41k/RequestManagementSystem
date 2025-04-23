using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace WpfClient.ViewModels.Behaviors;

public class UnfocusOnClearBehavior : Behavior<TextBox>
{
    private TextBox _hiddenTextBox;

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += (s, e) =>
        {
            _hiddenTextBox = new TextBox
            {
                Width = 0,
                Height = 0,
                Opacity = 0,
                IsTabStop = false,
                Focusable = true,
                Visibility = Visibility.Collapsed
            };

            var parentPanel = FindParentPanel(AssociatedObject);
            if (parentPanel != null && !_hiddenTextBox.IsLoaded)
            {
                parentPanel.Children.Add(_hiddenTextBox);
            }

            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        };
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source &&
            FindParent<Button>(source) is Button button &&
            button.Name == "ClearButton")
        {
            AssociatedObject.Clear();
            AssociatedObject.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // от греха

            if (_hiddenTextBox != null)
            {
                _hiddenTextBox.Visibility = Visibility.Visible;
                _hiddenTextBox.Focus();
                _hiddenTextBox.Visibility = Visibility.Collapsed;
            }

            e.Handled = true;
        }
    }

    private T? FindParent<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child != null && child is not T)
        {
            child = VisualTreeHelper.GetParent(child);
        }
        return child as T;
    }

    private Panel? FindParentPanel(DependencyObject? child)
    {
        while (child != null && child is not Panel)
        {
            child = VisualTreeHelper.GetParent(child);
        }
        return child as Panel;
    }
}

public static class ClearTextBehaviorHelper
{
    public static readonly DependencyProperty EnableUnfocusOnClearProperty =
        DependencyProperty.RegisterAttached(
            "EnableUnfocusOnClear",
            typeof(bool),
            typeof(ClearTextBehaviorHelper),
            new PropertyMetadata(false, OnEnableChanged));

    public static void SetEnableUnfocusOnClear(DependencyObject element, bool value) =>
        element.SetValue(EnableUnfocusOnClearProperty, value);

    public static bool GetEnableUnfocusOnClear(DependencyObject element) =>
        (bool)element.GetValue(EnableUnfocusOnClearProperty);

    private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        if ((bool)e.NewValue)
        {
            var behaviors = Interaction.GetBehaviors(textBox);
            if (!behaviors.OfType<UnfocusOnClearBehavior>().Any())
                behaviors.Add(new UnfocusOnClearBehavior());
        }
        else
        {
            var behavior = Interaction.GetBehaviors(textBox).FirstOrDefault(b => b is UnfocusOnClearBehavior);
            if (behavior != null)
                Interaction.GetBehaviors(textBox).Remove(behavior);
        }
    }
}