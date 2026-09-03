using System.Data;
using System.Globalization;
using SistemaReservas.Data;
using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class InicioViewModel : LoadableViewModel
{
    private readonly IAulaRepository aulas;
    private readonly IReservaRepository reservas;
    private readonly Func<DateTime> ahora;
    private string totalAulas = "—";
    private string reservasHoy = "—";
    private string capacidadTotal = "—";
    private string mensajeVacio = "Cargando la agenda…";
    private bool mostrarMensaje = true;
    private DataView? agendaHoy;

    public string TotalAulas { get => totalAulas; private set => SetProperty(ref totalAulas, value); }
    public string ReservasHoy { get => reservasHoy; private set => SetProperty(ref reservasHoy, value); }
    public string CapacidadTotal { get => capacidadTotal; private set => SetProperty(ref capacidadTotal, value); }
    public string MensajeVacio { get => mensajeVacio; private set => SetProperty(ref mensajeVacio, value); }
    public bool MostrarMensaje { get => mostrarMensaje; private set => SetProperty(ref mostrarMensaje, value); }
    public DataView? AgendaHoy { get => agendaHoy; private set => SetProperty(ref agendaHoy, value); }
    public RelayCommand VerAulasCommand { get; }
    public RelayCommand VerReservasCommand { get; }

    public InicioViewModel(IAulaRepository aulas, IReservaRepository reservas, Action<Pagina> navegar, Func<DateTime>? ahora = null)
    {
        this.aulas = aulas;
        this.reservas = reservas;
        this.ahora = ahora ?? (() => DateTime.Now);
        VerAulasCommand = new RelayCommand(_ => navegar(Pagina.Aulas));
        VerReservasCommand = new RelayCommand(_ => navegar(Pagina.Reservas));
    }

    protected override async Task CargarAsync()
    {
        Estado = "Actualizando información…";
        var aulasTask = aulas.ObtenerAulasObjetosAsync();
        var reservasTask = reservas.ObtenerReservasDataTableAsync();
        await Task.WhenAll(aulasTask, reservasTask);
        var listaAulas = await aulasTask;
        var tablaReservas = await reservasTask;
        DateTime fecha = ahora();
        var today = new DataView(tablaReservas)
        {
            RowFilter = $"Fecha = #{fecha.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}#",
            Sort = "Hora ASC"
        };

        TotalAulas = listaAulas.Count.ToString();
        CapacidadTotal = listaAulas.Sum(a => a.Capacidad).ToString();
        ReservasHoy = today.Count.ToString();
        AgendaHoy = today;
        MensajeVacio = "Hoy no hay reservas programadas.";
        MostrarMensaje = today.Count == 0;
        Estado = $"Información actualizada a las {fecha:HH:mm}.";
    }

    protected override void AlFallarCarga()
    {
        Estado = "No se pudo actualizar la información. Comprueba la conexión e inténtalo otra vez.";
        if (AgendaHoy == null) MensajeVacio = "La agenda no está disponible en este momento.";
    }
}
