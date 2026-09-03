using System.Windows;
using System.Windows.Input;

namespace SistemaReservas.Behaviors;

/// <summary>Adapta el evento visual Loaded a un comando, sin conocer el ViewModel.</summary>
public static class ViewCommands
{
    public static readonly DependencyProperty LoadedCommandProperty = DependencyProperty.RegisterAttached(
        "LoadedCommand", typeof(ICommand), typeof(ViewCommands), new PropertyMetadata(null, OnCommandChanged));

    public static ICommand? GetLoadedCommand(DependencyObject element) => (ICommand?)element.GetValue(LoadedCommandProperty);
    public static void SetLoadedCommand(DependencyObject element, ICommand? value) => element.SetValue(LoadedCommandProperty, value);

    private static void OnCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
        if (target is not FrameworkElement element) return;
        element.Loaded -= OnLoaded;
        if (args.NewValue is not ICommand) return;
        element.Loaded += OnLoaded;
        if (element.IsLoaded) Execute(element);
    }

    private static void OnLoaded(object sender, RoutedEventArgs args) => Execute((FrameworkElement)sender);

    private static void Execute(FrameworkElement element)
    {
        var command = GetLoadedCommand(element);
        if (command?.CanExecute(null) == true) command.Execute(null);
    }
}
