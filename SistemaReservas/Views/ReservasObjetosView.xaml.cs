using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class ReservasObjetosView : Window
    {

        public ReservasObjetosView()
        {

            InitializeComponent();


            DataContext =
                new ReservaViewModel();

        }

    }
}