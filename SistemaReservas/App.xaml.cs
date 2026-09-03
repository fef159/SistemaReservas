using System.Windows;
using SistemaReservas.Data;
using SistemaReservas.Services;

namespace SistemaReservas;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Raíz de composición: las dependencias concretas se conectan una sola vez.
        var windows = new WpfWindowService(new AulaRepository(), new ReservaRepository(), new UsuarioRepository());
        windows.MostrarBienvenida();
    }
}
