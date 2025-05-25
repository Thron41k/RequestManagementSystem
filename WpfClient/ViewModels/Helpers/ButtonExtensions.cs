using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient.ViewModels.Helpers
{
    public static class ButtonExtensions
    {
        public static readonly DependencyProperty NotificationCountProperty =
            DependencyProperty.RegisterAttached(
                "NotificationCount",
                typeof(int),
                typeof(ButtonExtensions),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnNotificationCountChanged)));
        private static void OnNotificationCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"NotificationCount changed from {e.OldValue} to {e.NewValue}");
            if (d is Button button)
            {
                d.SetValue(NotificationCountProperty, e.NewValue);
                Console.WriteLine($"NotificationCount changed from {e.OldValue} to {e.NewValue}");
            }
        }
        public static int GetNotificationCount(DependencyObject obj)
        {
            return (int)obj.GetValue(NotificationCountProperty);
        }

        public static void SetNotificationCount(DependencyObject obj, int value)
        {
            obj.SetValue(NotificationCountProperty, value);
        }
    }
}
