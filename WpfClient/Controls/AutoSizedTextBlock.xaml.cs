using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RequestManagement.WpfClient.Controls
{
    public partial class AutoSizedTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(TextValue), typeof(string), typeof(AutoSizedTextBlock),
                new PropertyMetadata(string.Empty, OnTextChanged));

        public string TextValue
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double MinFontSize { get; set; } = 20;
        public double MaxFontSize { get; set; } = 100;

        public AutoSizedTextBlock()
        {
            SizeChanged += (_, _) => AdjustFontSize();
            DataContextChanged += (_, _) => AdjustFontSize();
            InitializeComponent();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoSizedTextBlock { IsLoaded: true } control)
                control.AdjustFontSize();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustFontSize();
        }


        private void AdjustFontSize()
        {
            if (string.IsNullOrWhiteSpace(TextValue)) return;
            var low = MinFontSize;
            var high = MaxFontSize;
            var bestSize = MinFontSize;
            while (high - low > 0.5)
            {
                var mid = (low + high) / 2;
                Text.FontSize = mid;
                Text.Measure(new Size(ActualWidth, double.PositiveInfinity));
                if (Text.DesiredSize.Height <= Height)
                {
                    bestSize = mid;
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }
            Text.FontSize = bestSize;
        }
    }
}
