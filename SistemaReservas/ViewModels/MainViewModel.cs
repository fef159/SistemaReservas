using System.Windows.Input;
using SistemaReservas.MVVM;
using SistemaReservas.Views;


namespace SistemaReservas.ViewModels
{

    public class MainViewModel : ViewModelBase
    {


        public ICommand AbrirLoginCommand { get; }



        public MainViewModel()
        {

            AbrirLoginCommand =
                new RelayCommand(AbrirLogin);

        }



        private void AbrirLogin(object obj)
        {

            LoginView ventana =
                new LoginView();

            ventana.Show();

        }


    }

}