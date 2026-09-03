using SistemaReservas.MVVM;

namespace SistemaReservas.ViewModels;

public sealed class NuevaReservaViewModel : ViewModelBase
{
    public RelayCommand VerAulasCommand { get; }
    public RelayCommand VerReservasCommand { get; }

    public NuevaReservaViewModel(Action<Pagina> navegar)
    {
        VerAulasCommand = new RelayCommand(_ => navegar(Pagina.Aulas));
        VerReservasCommand = new RelayCommand(_ => navegar(Pagina.Reservas));
    }
}
