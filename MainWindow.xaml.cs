namespace PUJASM.POS
{
    using System.Windows.Media;
    using FirstFloor.ModernUI.Presentation;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            AppearanceManager.Current.AccentColor = Colors.DarkGray;
            InitializeComponent();
        }
    }
}
