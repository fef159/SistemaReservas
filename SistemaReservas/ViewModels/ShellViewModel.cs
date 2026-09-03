using System.Globalization;
using SistemaReservas.Data;
using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class ShellViewModel : ViewModelBase
{
    private readonly IAulaRepository aulas;
    private readonly IReservaRepository reservas;
    private ViewModelBase contenido;
    private Pagina paginaActual;

    public ViewModelBase Contenido { get => contenido; private set => SetProperty(ref contenido, value); }
    public string TituloPagina => paginaActual == Pagina.NuevaReserva ? "Nueva reserva" : paginaActual.ToString();
    public bool EsInicio => paginaActual == Pagina.Inicio;
    public bool EsAulas => paginaActual == Pagina.Aulas;
    public bool EsReservas => paginaActual == Pagina.Reservas;
    public string FechaActual => DateTime.Today.ToString("dd MMM yyyy", CultureInfo.GetCultureInfo("es-PE"));
    public RelayCommand NavegarCommand { get; }

    public ShellViewModel(IAulaRepository aulas, IReservaRepository reservas)
    {
        this.aulas = aulas;
        this.reservas = reservas;
        NavegarCommand = new RelayCommand(value => Navegar((Pagina)value!), value => value is Pagina page && Enum.IsDefined(page));
        contenido = new InicioViewModel(aulas, reservas, Navegar);
    }

    public void Navegar(Pagina pagina)
    {
        ViewModelBase next = pagina switch
        {
            Pagina.Inicio => new InicioViewModel(aulas, reservas, Navegar),
            Pagina.Aulas => new AulaViewModel(aulas),
            Pagina.Reservas => new ReservaViewModel(reservas),
            Pagina.NuevaReserva => new NuevaReservaViewModel(Navegar),
            _ => throw new ArgumentOutOfRangeException(nameof(pagina))
        };

        paginaActual = pagina;
        Contenido = next;
        OnPropertyChanged(nameof(TituloPagina));
        OnPropertyChanged(nameof(EsInicio));
        OnPropertyChanged(nameof(EsAulas));
        OnPropertyChanged(nameof(EsReservas));
        OnPropertyChanged(nameof(FechaActual));
    }
}
