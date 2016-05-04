using System.Windows;
using System.Windows.Interactivity;

namespace SpeechControls
{
    public class FsRichTextSelectionBehavior : Behavior<FsRichTextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextBox.SelectionChanged += RichTextBoxSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextBox.SelectionChanged -= RichTextBoxSelectionChanged;
        }

        void RichTextBoxSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedText = AssociatedObject.TextBox.Selection.Text;
        }

        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register(
                "SelectedText",
                typeof(string),
                typeof(FsRichTextSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTextChanged));

        private static void OnSelectedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as FsRichTextSelectionBehavior;
            if (behavior == null) return;
            behavior.AssociatedObject.TextBox.Selection.Text = behavior.SelectedText;
        }
    }
}
