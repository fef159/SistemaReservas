using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

        }

    }
}