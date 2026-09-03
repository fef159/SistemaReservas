using System.Data;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class AulaViewModel : LoadableViewModel
{
    private readonly IAulaRepository repository;
    private readonly bool comoTabla;
    private bool datosCargados;
    private string busqueda = "";
    private string resumen = "Consultando…";
    private string mensajeVacio = "Cargando aulas…";
    private bool mostrarMensaje = true;
    private DataTable aulasDataTable = new();
    private List<Aula> aulas = [];

    public DataTable AulasDataTable { get => aulasDataTable; private set => SetProperty(ref aulasDataTable, value); }
    public List<Aula> Aulas { get => aulas; private set => SetProperty(ref aulas, value); }
    public string Resumen { get => resumen; private set => SetProperty(ref resumen, value); }
    public string MensajeVacio { get => mensajeVacio; private set => SetProperty(ref mensajeVacio, value); }
    public bool MostrarMensaje { get => mostrarMensaje; private set => SetProperty(ref mostrarMensaje, value); }
    public RelayCommand BuscarCommand { get; }

    public string Busqueda
    {
        get => busqueda;
        set { if (SetProperty(ref busqueda, value)) Filtrar(); }
    }

    public AulaViewModel(IAulaRepository repository, bool comoTabla = true)
    {
        this.repository = repository;
        this.comoTabla = comoTabla;
        BuscarCommand = new RelayCommand(_ => Filtrar());
    }

    protected override async Task CargarAsync()
    {
        Estado = "Actualizando directorio…";
        if (comoTabla) AulasDataTable = await repository.ObtenerAulasDataTableAsync();
        else Aulas = await repository.ObtenerAulasObjetosAsync();
        datosCargados = true;
        Filtrar();
        Estado = comoTabla ? "Consulta la capacidad antes de planificar tu actividad." : $"{Aulas.Count} registros cargados.";
    }

    private void Filtrar()
    {
        if (!datosCargados) return;
        int count;
        int total;
        if (comoTabla)
        {
            TableSearch.Apply(AulasDataTable, Busqueda, "Nombre");
            total = AulasDataTable.Rows.Count;
            count = AulasDataTable.DefaultView.Count;
        }
        else { total = count = Aulas.Count; }

        Resumen = $"{count} de {total} aulas";
        MensajeVacio = total == 0 ? "Todavía no hay aulas registradas." : "No se encontraron aulas. Prueba con otro nombre.";
        MostrarMensaje = count == 0;
    }

    protected override void AlFallarCarga()
    {
        Estado = "No se pudo actualizar. Comprueba la conexión y pulsa Actualizar.";
        if (datosCargados) return;
        Resumen = "Sin conexión";
        MensajeVacio = "No se pudieron cargar las aulas.";
        MostrarMensaje = true;
    }
}
