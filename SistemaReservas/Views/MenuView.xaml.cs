using System.Windows;


namespace SistemaReservas.Views
{
    public partial class MenuView : Window
    {


        public MenuView()
        {
            InitializeComponent();
        }



        private void AulasDataTable_Click(object sender, RoutedEventArgs e)
        {

            AulasDataTableView ventana =
                new AulasDataTableView();


            ventana.Show();

        }




        private void AulasObjetos_Click(object sender, RoutedEventArgs e)
        {

            AulasObjetosView ventana =
                new AulasObjetosView();


            ventana.Show();

        }





        private void ReservasDataTable_Click(object sender, RoutedEventArgs e)
        {

            ReservasDataTableView ventana =
                new ReservasDataTableView();


            ventana.Show();

        }





        private void ReservasObjetos_Click(object sender, RoutedEventArgs e)
        {

            ReservasObjetosView ventana =
                new ReservasObjetosView();


            ventana.Show();

        }



    }
}