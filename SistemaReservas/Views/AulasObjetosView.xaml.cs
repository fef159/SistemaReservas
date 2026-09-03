using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class AulasObjetosView : Window
    {

        public AulasObjetosView()
        {
            InitializeComponent();

            DataContext = new AulaViewModel();
        }

    }
}