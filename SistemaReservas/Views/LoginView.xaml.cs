using System.Windows;
using SistemaReservas.ViewModels;


namespace SistemaReservas.Views
{
    public partial class LoginView : Window
    {

        public LoginView()
        {
            InitializeComponent();

            DataContext = new LoginViewModel();
        }



        private void Ingresar_Click(object sender, RoutedEventArgs e)
        {

            LoginViewModel vm =
                (LoginViewModel)DataContext;



            bool resultado =
                vm.Login(
                    txtUsuario.Text,
                    txtPassword.Password
                );



            if (resultado)
            {

                MessageBox.Show(
                    "Ingreso correcto"
                );


                MenuView menu =
                    new MenuView();


                menu.Show();


                this.Close();

            }
            else
            {

                MessageBox.Show(
                    "Usuario o contraseña incorrectos"
                );

            }


        }

    }
}