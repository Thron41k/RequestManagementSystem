using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace OneCOverlayClient.ViewModels.Behaviors;

public class MouseClickCommandBehavior : Behavior<TextBox>
{
    private static readonly ICommand ClearCommand = new RoutedCommand();
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
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        AssociatedObject.Loaded -= OnLoaded;
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AssociatedObject.Tag != null)
            {
                var button = FindChild<Button>(AssociatedObject, "PART_ClearButton");
                if(button != null)
                    FindChild<Button>(AssociatedObject, "PART_ClearButton").Command = AssociatedObject.Tag as ICommand;
            }
        }
        catch { }
    }
    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source)
        {
            var button = FindParent<Button>(source);
            if (button is { Name: "PART_ClearButton" })
            {
                return;
            }
        }

        if (Command?.CanExecute(null) == true)
        {
            Command.Execute(null);
        }
    }
    private static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
    {
        if (parent == null)
            return null;

        T foundChild = null;

        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (!(child is T childType))
            {
                foundChild = FindChild<T>(child, childName);

                if (foundChild != null) break;
            }
            else if (!string.IsNullOrEmpty(childName))
            {
                if (childType is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    foundChild = childType;
                    break;
                }
            }
            else
            {
                foundChild = childType;
                break;
            }
        }

        return foundChild;
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