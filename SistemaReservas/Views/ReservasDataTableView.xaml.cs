using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class ReservasDataTableView : Window
    {


        public ReservasDataTableView()
        {

            InitializeComponent();


            DataContext =
                new ReservaViewModel();

        }


    }
}