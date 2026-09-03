using System.Diagnostics;

namespace SistemaReservas.MVVM;

public abstract class LoadableViewModel : ViewModelBase
{
    private string estado = "Consultando información…";
    public string Estado { get => estado; protected set => SetProperty(ref estado, value); }
    public AsyncRelayCommand ActualizarCommand { get; }

    protected LoadableViewModel()
    {
        ActualizarCommand = new AsyncRelayCommand(CargarAsync, exception =>
        {
            Trace.TraceError("Error al cargar {0}: {1}", GetType().Name, exception.GetType().Name);
            AlFallarCarga();
        });
    }

    protected abstract Task CargarAsync();
    protected abstract void AlFallarCarga();
}
