using System.Diagnostics;
using SistemaReservas.Data;
using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IUsuarioRepository repository;
    private string username = "";
    private string password = "";
    private string error = "";
    private bool activa = true;

    public string Usuario { get => username; set => SetProperty(ref username, value); }
    public string Contrasena { get => password; set => SetProperty(ref password, value); }
    public string Error { get => error; private set => SetProperty(ref error, value); }
    public string TextoIngreso => IngresarCommand.IsExecuting ? "Ingresando…" : "Ingresar    →";
    public AsyncRelayCommand IngresarCommand { get; }
    public event Action? Autenticado;

    public LoginViewModel(IUsuarioRepository repository)
    {
        this.repository = repository;
        IngresarCommand = new AsyncRelayCommand(IngresarAsync, exception =>
        {
            Trace.TraceError("Error de autenticación: {0}", exception.GetType().Name);
            if (activa) Error = "No se pudo conectar con la base de datos. Comprueba la conexión e inténtalo otra vez.";
        }, () => activa);
        IngresarCommand.PropertyChanged += (_, _) => OnPropertyChanged(nameof(TextoIngreso));
    }

    private async Task IngresarAsync()
    {
        Error = "";
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrEmpty(Contrasena))
        {
            Error = "Completa el usuario y la contraseña.";
            return;
        }

        try
        {
            var usuario = await repository.ValidarUsuarioAsync(Usuario.Trim(), Contrasena);
            if (!activa) return;
            if (usuario == null)
            {
                Error = "El usuario o la contraseña no son correctos.";
                return;
            }

            // Impide un segundo ingreso, aunque el consumidor aún no haya cerrado la ventana.
            activa = false;
            Autenticado?.Invoke();
        }
        finally { Contrasena = ""; }
    }

    public void Desactivar()
    {
        activa = false;
        Contrasena = "";
        IngresarCommand.NotifyCanExecuteChanged();
    }
}
