using System.Windows.Input;

namespace SistemaReservas.MVVM;

/// <summary>Serializa la ejecución y notifica a los controles cuándo pueden volver a ejecutarse.</summary>
public sealed class AsyncRelayCommand(
    Func<Task> execute,
    Action<Exception> onError,
    Func<bool>? canExecute = null) : ViewModelBase, ICommand
{
    private bool isExecuting;
    public bool IsExecuting
    {
        get => isExecuting;
        private set
        {
            if (SetProperty(ref isExecuting, value)) NotifyCanExecuteChanged();
        }
    }

    public bool CanExecute(object? parameter) => !IsExecuting && (canExecute?.Invoke() ?? true);
    public event EventHandler? CanExecuteChanged;
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public async Task ExecuteAsync()
    {
        if (!CanExecute(null)) return;
        IsExecuting = true;
        try { await execute(); }
        catch (Exception exception) { onError(exception); }
        finally { IsExecuting = false; }
    }

    public async void Execute(object? parameter) => await ExecuteAsync();
}
