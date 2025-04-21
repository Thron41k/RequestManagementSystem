using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WpfClient.ViewModels.Behaviors
{
    public class FocusOnClearBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ClearFocusProperty =
            DependencyProperty.Register(nameof(ClearFocus), typeof(bool), typeof(FocusOnClearBehavior), new PropertyMetadata(false));

        public bool ClearFocus
        {
            get => (bool)GetValue(ClearFocusProperty);
            set => SetValue(ClearFocusProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text) && ClearFocus)
            {
                AssociatedObject.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= OnTextChanged;
        }
    }
}
