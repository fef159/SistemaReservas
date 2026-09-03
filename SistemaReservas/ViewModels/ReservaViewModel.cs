using System.Data;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class ReservaViewModel : LoadableViewModel
{
    private readonly IReservaRepository repository;
    private readonly bool comoTabla;
    private bool datosCargados;
    private string busqueda = "";
    private string resumen = "Consultando…";
    private string mensajeVacio = "Cargando reservas…";
    private bool mostrarMensaje = true;
    private DataTable reservasDataTable = new();
    private List<Reserva> reservas = [];

    public DataTable ReservasDataTable { get => reservasDataTable; private set => SetProperty(ref reservasDataTable, value); }
    public List<Reserva> Reservas { get => reservas; private set => SetProperty(ref reservas, value); }
    public string Resumen { get => resumen; private set => SetProperty(ref resumen, value); }
    public string MensajeVacio { get => mensajeVacio; private set => SetProperty(ref mensajeVacio, value); }
    public bool MostrarMensaje { get => mostrarMensaje; private set => SetProperty(ref mostrarMensaje, value); }

    public string Busqueda
    {
        get => busqueda;
        set { if (SetProperty(ref busqueda, value)) Filtrar(); }
    }

    public ReservaViewModel(IReservaRepository repository, bool comoTabla = true)
    {
        this.repository = repository;
        this.comoTabla = comoTabla;
    }

    protected override async Task CargarAsync()
    {
        Estado = "Actualizando agenda…";
        if (comoTabla)
        {
            var table = await repository.ObtenerReservasDataTableAsync();
            table.DefaultView.Sort = "Fecha DESC, Hora DESC";
            ReservasDataTable = table;
        }
        else Reservas = await repository.ObtenerReservasObjetosAsync();
        datosCargados = true;
        Filtrar();
        Estado = comoTabla ? "Selecciona un encabezado para ordenar la agenda." : $"{Reservas.Count} registros cargados.";
    }

    private void Filtrar()
    {
        if (!datosCargados) return;
        int count;
        int total;
        if (comoTabla)
        {
            TableSearch.Apply(ReservasDataTable, Busqueda, "Aula", "Usuario", "Motivo");
            total = ReservasDataTable.Rows.Count;
            count = ReservasDataTable.DefaultView.Count;
        }
        else { total = count = Reservas.Count; }

        Resumen = $"{count} de {total} reservas";
        MensajeVacio = total == 0 ? "Todavía no hay reservas registradas." : "No hay coincidencias. Prueba con otra búsqueda.";
        MostrarMensaje = count == 0;
    }

    protected override void AlFallarCarga()
    {
        Estado = "No se pudo actualizar. Comprueba la conexión y pulsa Actualizar.";
        if (datosCargados) return;
        Resumen = "Sin conexión";
        MensajeVacio = "No se pudieron cargar las reservas.";
        MostrarMensaje = true;
    }
}
