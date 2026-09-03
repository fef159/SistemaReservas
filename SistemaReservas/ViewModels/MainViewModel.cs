using SistemaReservas.MVVM;
using SistemaReservas.Services;

namespace SistemaReservas.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    public RelayCommand AbrirLoginCommand { get; }

    public MainViewModel(IWindowService windows) =>
        AbrirLoginCommand = new RelayCommand(_ => windows.MostrarLogin());
}
