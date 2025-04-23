using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfClient.ViewModels.Behaviors;

public class MouseClickCommandBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(MouseClickCommandBehavior), new PropertyMetadata(null));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source)
        {
            var button = FindParent<Button>(source);
            if (button is { Name: "ClearButton" })
            {
                var cmd = button.Command;
                if (cmd?.CanExecute(null) == true)
                {
                    cmd.Execute(null);
                }
                return;
            }
        }

        if (Command?.CanExecute(null) == true)
        {
            Command.Execute(null);
        }
    }

    private static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);

        while (parent != null)
        {
            if (parent is T typed)
                return typed;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}