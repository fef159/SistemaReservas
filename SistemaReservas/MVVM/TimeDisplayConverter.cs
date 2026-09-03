using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SistemaReservas.MVVM;

public sealed class TimeDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is TimeSpan time ? time.ToString(@"hh\:mm", culture) : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        DependencyProperty.UnsetValue;
}
