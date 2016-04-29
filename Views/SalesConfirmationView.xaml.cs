namespace PUJASM.POS.Views
{
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for SalesConfirmationView.xaml
    /// </summary>
    public partial class SalesConfirmationView
    {
        public SalesConfirmationView(SalesVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Cancel_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
