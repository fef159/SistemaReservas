using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class AulasDataTableView : Window
    {


        public AulasDataTableView()
        {
            InitializeComponent();


            DataContext =
                new AulaViewModel();

        }


    }
}