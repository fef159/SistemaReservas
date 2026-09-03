using SistemaReservas.Data;
using SistemaReservas.MVVM;


namespace SistemaReservas.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        private readonly UsuarioRepository repo;


        public LoginViewModel()
        {
            repo = new UsuarioRepository();
        }



        public bool Login(string username, string password)
        {

            var usuario =
                repo.ValidarUsuario(
                    username,
                    password);



            if (usuario != null)
            {
                return true;
            }


            return false;

        }


    }
}