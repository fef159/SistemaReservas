using System.Windows;
using SistemaReservas.Data;
using SistemaReservas.ViewModels;
using SistemaReservas.Views;

namespace SistemaReservas.Services;

/// <summary>Único responsable de crear ventanas y gestionar su ciclo de vida.</summary>
public sealed class WpfWindowService(
    IAulaRepository aulas,
    IReservaRepository reservas,
    IUsuarioRepository usuarios) : IWindowService
{
    private LoginView? login;

    public void MostrarBienvenida()
    {
        var window = new MainWindow { DataContext = new MainViewModel(this) };
        Application.Current.MainWindow = window;
        window.Show();
    }

    public void MostrarLogin()
    {
        if (login != null) { login.Activate(); return; }
        var owner = Application.Current.MainWindow;
        var vm = new LoginViewModel(usuarios);
        var window = new LoginView { Owner = owner, DataContext = vm };
        login = window;

        void AlAutenticar()
        {
            var menu = new MenuView { DataContext = new ShellViewModel(aulas, reservas) };
            Application.Current.MainWindow = menu;
            menu.Show();
            window.Close();
            owner?.Close();
        }

        vm.Autenticado += AlAutenticar;
        window.Closed += (_, _) => vm.Desactivar();
        try { window.ShowDialog(); }
        finally
        {
            vm.Desactivar();
            vm.Autenticado -= AlAutenticar;
            login = null;
        }
    }
}
