using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PUJASM.POS.Views
{
    using System.Windows.Controls.Primitives;
    using ViewModels;

    /// <summary>
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView
    {
        public SalesView()
        {
            InitializeComponent();
            var vm = new SalesVM();
            DataContext = vm;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void ComboBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) MoveToNextUIElement(e);
        }

        private static void MoveToNextUIElement(KeyEventArgs e)
        {
            // Creating a FocusNavigationDirection object and setting it to a
            // local field that contains the direction selected.
            const FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            // MoveFocus takes a TraveralReqest as its argument.
            var request = new TraversalRequest(focusDirection);

            // Gets the element with keyboard focus.
            var elementWithFocus = Keyboard.FocusedElement as UIElement;

            // Change keyboard focus.
            if (elementWithFocus == null) return;
            if (elementWithFocus.MoveFocus(request)) e.Handled = true;
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var MyTextBox = sender as TextBox;
            if (MyTextBox == null || MyTextBox.Text.Length <= 0) return;
            if (e.Key != Key.Enter) return;
            var binding = MyTextBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}
